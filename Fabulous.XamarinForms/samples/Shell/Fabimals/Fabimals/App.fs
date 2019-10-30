// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabimals

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open Xamarin.Essentials
open System
open Fabimals.Views
open Fabimals.Data
open Fabimals.Models
open Fabimals.Components
open Fabimals.Routes

module AppStyles =
    let applyDefaultShellItemStyle (element: ViewElement) =
        element.ShellBackgroundColor(Color.FromHex("#455A64"))
               .ShellForegroundColor(Color.White)
               .ShellTitleColor(Color.White)
               .ShellDisabledColor(Color.FromHex("#B4FFFFFF"))
               .ShellUnselectedColor(Color.FromHex("#95FFFFFF"))

    let applyDomesticItemStyle element = (applyDefaultShellItemStyle element).ShellBackgroundColor(Color.FromHex("#039BE6"))
    let applyMonkeysItemStyle element = (applyDefaultShellItemStyle element).ShellBackgroundColor(Color.FromHex("#689F39"))
    let applyElephantsItemStyle element = (applyDefaultShellItemStyle element).ShellBackgroundColor(Color.FromHex("#ED3B3B"))
    let applyBearsItemStyle element = (applyDefaultShellItemStyle element).ShellBackgroundColor(Color.FromHex("#546DFE"))
    let applyAboutItemStyle element = (applyDefaultShellItemStyle element).ShellBackgroundColor(Color.FromHex("#96D1FF"))


module App =
    type Model = 
      { CatsPageModel: AnimalList.Model
        DogsPageModel: AnimalList.Model
        MonkeysPageModel: AnimalList.Model
        ElephantsPageModel: AnimalList.Model
        BearsPageModel: AnimalList.Model
        DetailsPageModel: AnimalDetails.Model option }

    type Msg =
        | ShowHelp
        | NavigateToDetails of Animal
        | AboutPageMsg of About.Msg
        | CatsPageMsg of AnimalList.Msg
        | DogsPageMsg of AnimalList.Msg
        | MonkeysPageMsg of AnimalList.Msg
        | ElephantsPageMsg of AnimalList.Msg
        | BearsPageMsg of AnimalList.Msg

    type CmdMsg =
        | GoToHelpWebsite
        | GoToAnimal of Animal
        | AboutPageCmdMsgs of About.CmdMsg list
        
    let shellRef = ViewRef<Shell>()

    let goToHelpWebsite () =
        Fabimals.Helper.openUri("https://docs.microsoft.com/xamarin/xamarin-forms/app-fundamentals/shell")
        Cmd.none
        
    let navigateToAnimal (animal: Animal) =
        match shellRef.TryValue with
        | None -> ()
        | Some shell ->
            let route = ShellNavigationState.op_Implicit (sprintf "%Adetails?name=%s" animal.Type animal.Name)

            async {
                // Selecting an item in SearchHandler and immediately asking for navigation doesn't work on iOS.
                // This is a bug in Xamarin.Forms (https://github.com/xamarin/Xamarin.Forms/issues/5713)
                // The workaround is to wait for the fade out animation of SearchHandler to finish
                if Device.RuntimePlatform = Device.iOS then
                    do! Async.Sleep 1000 

                shell.FlyoutIsPresented <- false
                do! shell.GoToAsync route |> Async.AwaitTask
            } |> Async.StartImmediate

        Cmd.none

    let mapCmdMsgToCmd cmdMsg =
        match cmdMsg with
        | GoToHelpWebsite -> goToHelpWebsite()
        | GoToAnimal animal -> navigateToAnimal animal
        | AboutPageCmdMsgs cmdMsgs -> cmdMsgs |> List.map About.mapCommands |> Cmd.batch
    
    let init () =
        Routing.RegisterRoute("Catdetails", typeof<CatRoutingPage>)
        Routing.RegisterRoute("Dogdetails", typeof<DogRoutingPage>)
        Routing.RegisterRoute("Monkeydetails", typeof<MonkeyRoutingPage>)
        Routing.RegisterRoute("Elephantdetails", typeof<ElephantRoutingPage>)
        Routing.RegisterRoute("Beardetails", typeof<BearRoutingPage>)
        
        { CatsPageModel = Cats.init ()
          DogsPageModel = Dogs.init ()
          MonkeysPageModel = Monkeys.init ()
          ElephantsPageModel = Elephants.init ()
          BearsPageModel = Bears.init ()
          DetailsPageModel = None }, []
        
    let updateAnimalListPage updateFn setFn msg model =
        let newModel, _, externalMsgs = updateFn msg model
        let cmdMsgs =
            externalMsgs |> List.map (fun msg ->
                match msg with
                | AnimalList.ExternalMsg.NavigateToDetails animal -> GoToAnimal animal
            )
        (setFn newModel), cmdMsgs

    let update msg model =
        match msg with
        | ShowHelp -> model, [GoToHelpWebsite]
        | NavigateToDetails animal ->
            { model with DetailsPageModel = Some (AnimalDetails.init animal) }, [GoToAnimal animal]
        | AboutPageMsg msg ->
            model, [(About.update msg |> AboutPageCmdMsgs)]
        | CatsPageMsg msg ->
            updateAnimalListPage Cats.update (fun m -> { model with CatsPageModel = m }) msg model.CatsPageModel
        | DogsPageMsg msg ->
            updateAnimalListPage Dogs.update (fun m -> { model with DogsPageModel = m }) msg model.DogsPageModel
        | MonkeysPageMsg msg ->
            updateAnimalListPage Monkeys.update (fun m -> { model with MonkeysPageModel = m }) msg model.MonkeysPageModel
        | ElephantsPageMsg msg ->
            updateAnimalListPage Elephants.update (fun m -> { model with ElephantsPageModel = m }) msg model.ElephantsPageModel
        | BearsPageMsg msg ->
            updateAnimalListPage Bears.update (fun m -> { model with BearsPageModel = m }) msg model.BearsPageModel

    let view (model: Model) dispatch =
        View.Shell(
            ref=shellRef,
            flyoutHeaderBehavior=FlyoutHeaderBehavior.CollapseOnScroll,
            flyoutHeader=View.ContentView(
                height=200.,
                content=View.Grid(
                    backgroundColor=Color.Black,
                    children=[
                        View.Image(
                            aspect=Aspect.AspectFill,
                            source=Image.Path "xamarinstore.jpg",
                            opacity=0.6
                        )
                        View.Label(
                            text="Animals",
                            textColor=Color.White,
                            fontAttributes=FontAttributes.Bold,
                            horizontalOptions=LayoutOptions.Center,
                            verticalOptions=LayoutOptions.Center
                        )
                    ]
                )
            ),
            items=[
                View.FlyoutItem(
                    title="Animals",
                    route="animals",
                    icon=Image.Path "cat.png",
                    flyoutDisplayOptions=FlyoutDisplayOptions.AsMultipleItems,
                    items=[
                        View.Tab(
                            title="Domestic",
                            route="domestic",
                            icon=Image.Path "paw.png",
                            items=[
                                View.ShellContent(
                                    title="Cats",
                                    route="cats",
                                    icon=Image.Path "cat.png",
                                    content=Cats.view model.CatsPageModel (CatsPageMsg >> dispatch)
                                )
                                View.ShellContent(
                                    title="Dogs",
                                    route="dogs",
                                    icon=Image.Path "dog.png",
                                    content=Dogs.view model.DogsPageModel (DogsPageMsg >> dispatch)
                                )
                            ]
                        ) |> AppStyles.applyDomesticItemStyle
                        View.ShellContent(
                            title="Monkeys",
                            route="monkeys",
                            icon=Image.Path "monkey.png",
                            content=Monkeys.view model.MonkeysPageModel (MonkeysPageMsg >> dispatch)
                        ) |> AppStyles.applyMonkeysItemStyle
                        View.ShellContent(
                            title="Elephants",
                            route="elephants",
                            icon=Image.Path "elephant.png",
                            content=Elephants.view model.ElephantsPageModel (ElephantsPageMsg >> dispatch)
                        ) |> AppStyles.applyElephantsItemStyle
                        View.ShellContent(
                            title="Bears",
                            route="bears",
                            icon=Image.Path "bear.png",
                            content=Bears.view model.BearsPageModel (BearsPageMsg >> dispatch)
                        ) |> AppStyles.applyBearsItemStyle
                    ]
                ) |> AppStyles.applyDefaultShellItemStyle

                View.ShellContent(
                    title="About",
                    route="about",
                    icon=Image.Path "info.png",
                    content=About.view (AboutPageMsg >> dispatch)
                ) |> AppStyles.applyAboutItemStyle

                View.MenuItem(
                    text="Random",
                    icon=Image.Path "random.png",
                    command=(fun () ->
                        let random = Random()
                        let categories = [ Cats.data; Dogs.data; Monkeys.data; Elephants.data; Bears.data ]
                        let category = categories.[random.Next(categories.Length)]
                        let animal = category.[random.Next(category.Length)]
                        dispatch (NavigateToDetails animal)
                    )
                )
                View.MenuItem(
                    text="Help",
                    icon=Image.Path "help.png",
                    command=(fun () -> dispatch ShowHelp)
                )
            ]
        )

    let program = 
        Program.mkProgramWithCmdMsg init update view mapCmdMsgToCmd

type FabimalsApp () as app = 
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
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Debug.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()

#endif
