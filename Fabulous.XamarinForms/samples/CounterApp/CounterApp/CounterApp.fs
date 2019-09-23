// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open FSharp.Data.Adaptive
open System.Diagnostics

module App = 
    type Model = 
      { Count : int 
        Step : int
        TimerOn: bool }

    type Msg = 
        | Increment 
        | Decrement 
        | Reset
        | SetStep of int
        | TimerToggled of bool
        | TimedTick

    type CmdMsg =
        | TickTimer

    let timerCmd () =
        async { do! Async.Sleep 200
                return TimedTick }
        |> Cmd.ofAsyncMsg

    let mapCmdMsgToCmd cmdMsg =
        match cmdMsg with
        | TickTimer -> timerCmd()

    let initModel () = { Count = 0; Step = 1; TimerOn=false }

    let init () = initModel () , []

    let update msg (model: aref<Model>) =
        adaptive {
           let! model = model
           match msg with
            | Increment -> { model with Count = model.Count + model.Step }, []
            | Decrement -> { model with Count = model.Count - model.Step }, []
            | Reset -> init ()
            | SetStep n -> { model with Step = n }, []
            | TimerToggled on -> { model with TimerOn = on }, (if on then [ TickTimer ] else [])
            | TimedTick -> if model.TimerOn then { model with Count = model.Count + model.Step }, [ TickTimer ] else model, [] 

    let view (model: aref<Model>) dispatch =  
        adaptiveView { 
            View.ContentPage(
              content=View.StackLayout(padding=30.0,verticalOptions = LayoutOptions.Center,
                children=[
                  View.Label(automationId="CountLabel", text=sprintf "%d" (dep model).Count, horizontalOptions=LayoutOptions.Center, widthRequest=200.0, horizontalTextAlignment=TextAlignment.Center)
                  View.Button(automationId="IncrementButton", text="Increment", command= (fun () -> dispatch Increment))
                  View.Button(automationId="DecrementButton", text="Decrement", command= (fun () -> dispatch Decrement)) 
                  View.StackLayout(padding=20.0, orientation=StackOrientation.Horizontal, horizontalOptions=LayoutOptions.Center,
                                  children = [ View.Label(text="Timer")
                                               View.Switch(automationId="TimerSwitch", isToggled=(dep model).TimerOn, toggled=(fun on -> dispatch (TimerToggled on.Value))) ])
                  View.Slider(automationId="StepSlider", minimumMaximum=(0.0, 10.0), value= double (dep model).Step, valueChanged=(fun args -> dispatch (SetStep (int (args.NewValue + 0.5)))))
                  View.Label(automationId="StepSizeLabel", text=sprintf "Step size: %d" (dep model).Step, horizontalOptions=LayoutOptions.Center)
                  View.Button(text="Reset", horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch Reset), canExecute = (dep model <> initModel () ))
                ]))
        }
(*
        View.ContentPageA(
            contentA=
              View.StackLayoutA(
                paddingA=ARef.constant 30.0,
                verticalOptionsA = ARef.constant LayoutOptions.Center,
                childrenA= alist [
                    View.Label(automationId="CountLabel", text=sprintf "%d" (dep model).Count, horizontalOptions=LayoutOptions.Center, widthRequest=200.0, horizontalTextAlignment=TextAlignment.Center)
                    ARef.constant (View.Button(automationId="IncrementButton", text="Increment", command= (fun () -> dispatch Increment)))
                    ARef.constant (View.Button(automationId="DecrementButton", 
                                        text="Decrement", 
                                        command= (fun () -> dispatch Decrement))) 
                    View.StackLayout(
                        padding=20.0,
                        orientation=StackOrientation.Horizontal,
                        horizontalOptions=LayoutOptions.Center,
                        children = [ 
                          View.Label(text="Timer")
                          View.Switch(automationId="TimerSwitch", isToggled=(dep model).TimerOn, toggled=(fun on -> dispatch (TimerToggled on.Value))) ])

                    View.Slider(automationId="StepSlider", 
                        minimumMaximum=(0.0, 10.0),
                        value= double (dep model).Step,
                        valueChanged=(fun args -> dispatch (SetStep (int (args.NewValue + 0.5)))))

                    View.Label(automationId="StepSizeLabel", 
                        text=sprintf "Step size: %d" (dep model).Step, 
                        horizontalOptions=LayoutOptions.Center)

                    View.Button(text="Reset", 
                        horizontalOptions=LayoutOptions.Center, 
                        command=(fun () -> dispatch Reset), 
                        canExecute = (dep model <> initModel () ))
                ]))
  *)
  
    let program = 
        Program.mkProgramWithCmdMsg init update view mapCmdMsgToCmd

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.program
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

#if DEBUG
    // Run LiveUpdate using: 
    //    
    do runner.EnableLiveUpdate ()
#endif


#if SAVE_MODEL_WITH_JSON
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Debug.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Debug.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Debug.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Debug.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex ->
            runner.OnError ("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Debug.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()

#endif
