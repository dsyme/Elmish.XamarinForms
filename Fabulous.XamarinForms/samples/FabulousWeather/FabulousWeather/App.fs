namespace FabulousWeather

open System
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module AppStyles =
    let coldStartColor = Color.FromHex("#BDE3FA")
    let coldEndColor = Color.FromHex("#A5C9FD")
    let WarmStartColor = Color.FromHex("#F6CC66")
    let WarmEndColor  = Color.FromHex("#FCA184")
    let NightStartColor = Color.FromHex("#172941")
    let NightEndColor = Color.FromHex("#3C6683")
    let MainTextColor = Color.White
    let itemStartColor = Color.FromHex("#98FFFFFF")
    let itemEndColor = Color.FromHex("#60FFFFFF")
    
    let getStartGradientColor temp =
        if temp > 60 then 
            WarmStartColor
        else if temp = -100 then 
            NightStartColor
        else
            coldStartColor

    let getEndGradientColor temp =
        if temp > 60 then 
            WarmEndColor
        else if temp = -100 then 
            NightEndColor
        else
            coldEndColor

    let createLabel value =
        View.Label(
            text = value,
            fontSize = FontSize 100.,
            horizontalOptions = LayoutOptions.Center,
            textColor = MainTextColor
        )

module App =
    type DataItem = 
        { Name: string
          Value: decimal }
        
    type WeatherData =
        { CityName: string
          CityLogo: string
          Date: DateTime
          Temp: int
          Items: DataItem list
          Weather: string }
        
    type Model = 
        { Data: WeatherData option
          IsRefreshing: bool }
        
    type Msg =
        | RequestRefresh
        | WeatherRefreshed of WeatherData
        
    // Get an API key from https://openweathermap.org/appid and replace the value here
    let apiKey = "API KEY HERE"

    let kelvinToFahrenheit K = 9.0m / 5.0m * (K - 273.0m) + 32.0m

    type jsonProvider = FSharp.Data.JsonProvider<"https://samples.openweathermap.org/data/2.5/weather?q=London,uk&appid=b6907d289e10d714a6e88b30761fae22">

    let getOpenWeatherData apiKey =
        async {
            let url = sprintf "http://api.openweathermap.org/data/2.5/weather?q=seattle&APPID=%s" apiKey
            let! result = jsonProvider.AsyncLoad(url)
            let model = 
                { CityName = result.Name
                  CityLogo = "spaceneedle.png"
                  Date = DateTime.Now
                  Temp = result.Main.Temp |> kelvinToFahrenheit |> Math.Round |> int
                  Weather = result.Weather.[0].Main
                  Items =
                    [ { Name = "Pressure";   Value = result.Main.Pressure |> decimal }
                      { Name = "Humidity";   Value = result.Main.Humidity |> decimal }
                      { Name = "Wind Speed"; Value = result.Wind.Speed }
                      { Name = "Min Temp";   Value = result.Main.TempMin |> kelvinToFahrenheit }
                      { Name = "Max Temp";   Value = result.Main.TempMax |> kelvinToFahrenheit } ] } 
            return WeatherRefreshed model
        } |> Cmd.ofAsyncMsg
                
    let initModel =
        { Data = None
          IsRefreshing = true }
        
    let initial() = initModel, getOpenWeatherData apiKey

    let update msg model =
        match msg with
        | RequestRefresh -> { model with IsRefreshing = true }, getOpenWeatherData apiKey
        | WeatherRefreshed data -> { model with IsRefreshing = false; Data = Some data }, Cmd.none

    let view (model: Model) dispatch =
        
        let itemsView (items: DataItem list) =
            [ for r in items -> 
                View.PancakeView(
                    content = View.Label(
                        text = sprintf "%s%s%O" r.Name System.Environment.NewLine r.Value,
                        textColor = AppStyles.MainTextColor
                    ),
                    cornerRadius = CornerRadius(20.,20.,20.,0.),
                    backgroundGradientStartColor = AppStyles.itemStartColor,
                    backgroundGradientEndColor = AppStyles.itemEndColor,
                    padding = Thickness(8.),
                    backgroundGradientAngle = 315
                ) ]

        let grid (data: WeatherData) =
            View.Grid(
                rowdefs = [ Auto; Star; Auto; Auto; Auto; Auto ],
                rowSpacing = 0.,
                padding = (if Device.RuntimePlatform = Device.Android then Thickness(0.0,24.0,0.0,0.0) else Thickness(0.,44.,0.,0.)),
                gestureRecognizers = [
                    View.TapGestureRecognizer(fun ()-> dispatch RequestRefresh)
                ],
                children = [
                    View.Label(
                        text = data.CityName,
                        horizontalOptions = LayoutOptions.Center,
                        fontSize = Named NamedSize.Large,
                        textColor = AppStyles.MainTextColor
                    )
                    View.Image(
                        source = Path data.CityLogo,
                        margin = Thickness(0.,0.,0.,-80.),
                        opacity = 0.8, 
                        verticalOptions = LayoutOptions.FillAndExpand
                    ).Row(1)
                    View.Label(
                        text = sprintf "%i°" data.Temp,
                        fontSize = FontSize 100.,
                        horizontalOptions = LayoutOptions.Center,
                        textColor = AppStyles.MainTextColor
                    ).Row(2)
                    View.Label(
                        horizontalOptions = LayoutOptions.Center,
                        text = data.Weather.ToUpper(),
                        fontSize = Named NamedSize.Large,
                        textColor = AppStyles.MainTextColor
                    ).Row(3)
                    View.Label(
                        horizontalOptions = LayoutOptions.Center,
                        text = data.Date.ToString("D"),
                        fontSize = Named NamedSize.Small,
                        textColor = AppStyles.MainTextColor
                    ).Row(4)
                    View.ScrollView(
                        orientation = ScrollOrientation.Horizontal,
                        padding = Thickness(20., 0., 20., 10.),
                        margin = Thickness(0., 10., 0., 0.),
                        content = View.StackLayout(
                            orientation = StackOrientation.Horizontal,
                            children = itemsView data.Items
                        )
                    ).Row(5)
                ]
            )

        let pancakeBackgroundPadding =
            match Device.RuntimePlatform with
            | Device.iOS -> new Thickness(0., 40., 0., 20.)
            | _ -> new Thickness(0.)
        
        View.ContentPage(
            created = (fun self -> Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(self, false)),
            content =
                match model.Data with
                | None ->
                    View.ActivityIndicator(isRunning = true)
                | Some data ->
                    View.PancakeView(
                        padding = pancakeBackgroundPadding,
                        backgroundGradientStartColor = AppStyles.getStartGradientColor data.Temp, 
                        backgroundGradientEndColor = AppStyles.getEndGradientColor data.Temp,
                        content =
                            if model.IsRefreshing then
                                View.ActivityIndicator(isRunning = true)
                            else
                                grid data
                    )
        )

    let program = 
        Program.mkProgram initial update view
        |> Program.withConsoleTrace

type App () as app = 
    inherit Application ()

    let runner = App.program |> XamarinFormsProgram.run app

#if DEBUG
    do runner.EnableLiveUpdate ()
#endif