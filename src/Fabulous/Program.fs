// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Diagnostics
open FSharp.Data.Adaptive

/// Representation of the host framework with access to the root view to update (e.g. Xamarin.Forms.Application)
type IHost =
    /// Gets a reference to the root view item (e.g. Xamarin.Forms.Application.MainPage)
    abstract member GetRootView : unit -> obj
    /// Sets a new instance of the root view item (e.g. Xamarin.Forms.Application.MainPage)
    abstract member SetRootView : obj -> unit

/// We store the current dispatch function for the running Elmish program as a 
/// static-global thunk because we want old view elements stored in the `dependsOn` global table
/// to be recyclable on resumption (when a new ProgramRunner gets created).
type internal ProgramDispatch<'msg>()  = 
    static let mutable dispatchImpl = (fun (_msg: 'msg) -> failwith "do not call dispatch during initialization" : unit)

    static let dispatch = 
        id (fun msg -> 
            dispatchImpl msg)

    static member DispatchViaThunk = dispatch 
    static member SetDispatchThunk v = dispatchImpl <- v

/// Program type captures various aspects of program behavior
type Program<'arg, 'model, 'amodel, 'msg> = 
    { init : 'arg -> 'model * Cmd<'msg>
      update : 'msg -> 'model -> 'model * Cmd<'msg>
      ainit : 'model -> 'amodel 
      adelta : 'model -> 'amodel -> unit
      subscribe : 'model -> Cmd<'msg>
      view : 'amodel -> Dispatch<'msg> -> aval<ViewElement>
      syncDispatch: Dispatch<'msg> -> Dispatch<'msg>
      syncAction: (unit -> unit) -> (unit -> unit)
      debug : bool
      onError : (string * exn) -> unit }

    override x.ToString() = "<program>"

/// Starts the Elmish dispatch loop for the page with the given Elmish program
type ProgramRunner<'arg, 'model, 'amodel, 'msg>(host: IHost, program: Program<'arg, 'model, 'amodel, 'msg>, arg: 'arg) = 

    do Debug.WriteLine "run: computing initial model"

    // Get the initial model
    let (initialModel, cmd) = program.init arg
    let amodel = program.ainit initialModel
    let mutable alternativeRunner : ProgramRunner<obj,obj,obj,obj> option = None

    let mutable lastModel = initialModel
    let dispatch = ProgramDispatch<'msg>.DispatchViaThunk
    let mutable reset = (fun () -> ())

    // Create the initial view
    let viewInfo = program.view amodel dispatch
    
    // The updater is an active AdaptiveObject receiving update notifications from the adaptive model.
    // It must be captured kept alive, see the KeepAlive call below.
    let updater = ViewElementUpdater.CreateAdaptive viewInfo id (fun _scope target -> host.SetRootView(target))
    do updater AdaptiveToken.Top host
 
    // Start Elmish dispatch loop  
    let rec processMsg msg = 
        try
            let (updatedModel,newCommands) = program.update msg lastModel
            lastModel <- updatedModel
            try 
                updateView updatedModel 
            with ex ->
                program.onError ("Unable to update view:", ex)
            for sub in newCommands do
                try 
                    sub dispatch
                with ex ->
                    program.onError ("Error executing commands:", ex)
        with ex ->
            program.onError ("Unable to process a message:", ex)

    and updateView updatedModel = 
        program.adelta updatedModel amodel
        updater AdaptiveToken.Top host
                      
    do 
        // Set up the global dispatch function
        ProgramDispatch<'msg>.SetDispatchThunk (processMsg |> program.syncDispatch)

        reset <- (fun () -> updateView lastModel) |> program.syncAction

        Debug.WriteLine "updating the initial view"

        updateView initialModel 

        Debug.WriteLine "dispatching initial commands"
        for sub in (program.subscribe initialModel @ cmd) do
            try 
                sub dispatch
            with ex ->
                program.onError ("Error executing commands:", ex)

    member __.Argument = arg
    
    member __.CurrentModel = lastModel 

    member __.Dispatch(msg) = dispatch msg

    member runner.ChangeProgram(newProgram: Program<obj,obj,obj,obj>) : unit =
        let action = program.syncAction (fun () -> 
            // TODO: transmogrify the model
            try
                alternativeRunner <- Some (ProgramRunner<obj,obj, obj, obj>(host, newProgram, runner.Argument))
            with ex ->
                program.onError ("Error changing the program:", ex)
        )
        action()

    member __.ResetView() : unit  =
        let action = program.syncAction (fun () -> 
            match alternativeRunner with 
            | Some r -> r.ResetView()
            | None -> reset()
        )
        action()

    /// Set the current model, e.g. on resume
    member __.SetCurrentModel(model, cmd: Cmd<_>) =
        let action = program.syncAction (fun () -> 
            match alternativeRunner with 
            | Some _ -> 
                // TODO: transmogrify the resurrected model
                printfn "SetCurrentModel: ignoring (can't the model after ChangeProgram has been called)"
            | None -> 
                Debug.WriteLine "updating the view after setting the model"
                lastModel <- model
                updateView model
                for sub in program.subscribe model @ cmd do
                    sub dispatch
        )
        action()

    override x.ToString() = "<ProgramRunner>"

/// Program module - functions to manipulate program instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Program =
    let internal onError (text: string, ex: exn) = 
        Console.WriteLine (sprintf "%s: %A" text ex)

    /// Typical program, new commands are produced by `init` and `update` along with the new state.
    let mkProgram (init : 'arg -> 'model * Cmd<'msg>) (update : 'msg -> 'model -> 'model * Cmd<'msg>) (ainit: 'model -> 'amodel) (adelta: 'model -> 'amodel -> unit) (view : 'amodel -> Dispatch<'msg> -> aval<ViewElement>) =
        { init = init
          update = update
          ainit = ainit
          adelta = adelta
          view = view
          syncDispatch = id
          syncAction = id
          subscribe = fun _ -> Cmd.none
          debug = false
          onError = onError }

    /// Simple program that produces only new state with `init` and `update`.
    let mkSimple (init : 'arg -> 'model) (update : 'msg -> 'model -> 'model) ainit adelta (view : 'amodel -> Dispatch<'msg> -> aval<ViewElement>) = 
        mkProgram (fun arg -> init arg, Cmd.none) (fun msg model -> update msg model, Cmd.none) ainit adelta view

    /// Typical program, new commands are produced discriminated unions returned by `init` and `update` along with the new state.
    let mkProgramWithCmdMsg (init: 'arg -> 'model * 'cmdMsg list) (update: 'msg -> 'model -> 'model * 'cmdMsg list) ainit adelta (view: 'amodel -> Dispatch<'msg> -> aval<ViewElement>) (mapToCmd: 'cmdMsg -> Cmd<'msg>) =
        let convert = fun (model, cmdMsgs) -> model, (cmdMsgs |> List.map mapToCmd |> Cmd.batch)
        mkProgram (fun arg -> init arg |> convert) (fun msg model -> update msg model |> convert) ainit adelta view

    /// Subscribe to external source of events.
    /// The subscription is called once - with the initial (or resumed) model, but can dispatch new messages at any time.
    let withSubscription (subscribe : 'model -> Cmd<'msg>) (program: Program<'arg, 'model, 'amodel, 'msg>) =
        let sub model =
            Cmd.batch [ program.subscribe model
                        subscribe model ]
        { program with subscribe = sub }

    /// Trace all the updates to the console
    let withConsoleTrace (program: Program<'arg, 'model, 'amodel, 'msg>) =
        let traceInit arg =
            try 
                let initModel,cmd = program.init arg
                Console.WriteLine (sprintf "Initial model: %0A" initModel)
                initModel,cmd
            with e -> 
                Console.WriteLine (sprintf "Error in init function: %0A" e)
                reraise ()

        let traceUpdate msg model =
            Console.WriteLine (sprintf "Message: %0A" msg)
            try 
                let newModel,cmd = program.update msg model
                Console.WriteLine (sprintf "Updated model: %0A" newModel)
                newModel,cmd
            with e -> 
                Console.WriteLine (sprintf "Error in model function: %0A" e)
                reraise ()

        let traceView amodel dispatch =
            Console.WriteLine (sprintf "View, model = %0A" amodel)
            try 
                let info = program.view amodel dispatch
                Console.WriteLine (sprintf "View result: %0A" info)
                info
            with e -> 
                Console.WriteLine (sprintf "Error in view function: %0A" e)
                reraise ()
                
        { program with
            init = traceInit 
            update = traceUpdate
            view = traceView }

    /// Trace all the messages as they update the model
    let withTrace trace (program: Program<'arg, 'model, 'amodel, 'msg>) =
        { program
            with update = fun msg model -> trace msg model; program.update msg model}

    /// Handle dispatch loop exceptions
    let withErrorHandler onError (program: Program<'arg, 'model, 'amodel, 'msg>) =
        { program
            with onError = onError }

    /// Set debugging to true
    let withDebug program = 
        { program with debug = true }

    /// Set the syncDispatch function
    let withSyncDispatch syncDispatch program = 
        { program with syncDispatch = syncDispatch }

    /// Set the syncAction function
    let withSyncAction syncAction program = 
        { program with syncAction = syncAction }
        
    /// Run the app with Fabulous
    let runWithFabulous (host : IHost) (arg: 'arg) (program: Program<'arg, 'model, 'amodel, 'msg>) = 
        ProgramRunner(host, program, arg)

    /// Run the app with Fabulous
    let runFabulous (host : IHost) (program: Program<unit, 'model, 'amodel, 'msg>) = 
        runWithFabulous host () program