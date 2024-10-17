namespace RabbitSender.ElmishApp.Models

open RabbitSender.ElmishApp.Interfaces
open System.Text.Json

type MainModel =
    {
        ErrorMessageQueue: IErrorMessageQueue
        SettingsManager: ISettingsManager
        Messages: MessageModel list
        SelectedMessageModelId: int option
    }

module MainModel =

    type Msg =
        | OnError of exn
        | MessageMsg of int * MessageModel.Msg
        | MessageMsg2 of MessageModel.Msg
        | SelectMessageModelId of int option

    open RabbitSender.ElmishApp.Infrastructure

    let inline foo<'T, 'D when 'T: not struct and 'D: not struct> a b c = task { return () }

    let inline send'<'TContract, 'TDto when 'TDto: not struct and 'TDto: (member RoutingKey : string) and 'TContract: not struct> =
        fun (dto: obj, entityName, host, exchange) ->
            match dto with
            | :? 'TDto as d ->
                send<'TContract, 'TDto> (d, entityName, host, exchange)
            | _ -> failwith "wrong type"

    let messages =
        [
            // MessageModel.init 
            //     "MessageName"
            //     ""
            //     ""
            //     RabbitMQ.Client.ExchangeType.Direct
            //     (.Generate() |> box)
            //     send'<>
            //     (fun s -> JsonSerializer.Deserialize<>(s))
        ]

    open System
    open Elmish

    let init (errorMessageQueue: IErrorMessageQueue) (settingsManager: ISettingsManager) =
        let getSaved setting = 
            match (settingsManager.Load setting) with
            | :? string as s when not (String.IsNullOrWhiteSpace(s)) -> s |> Some
            | _ -> None

        fun () ->
        
            // let kibanaBaseUrl = getSaved "KibanaBaseUri"
            // let kibanaLogin = getSaved "KibanaLogin"
            let assemblyVer = "Version" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()
            {
                ErrorMessageQueue = errorMessageQueue
                SettingsManager = settingsManager
                Messages = messages
                SelectedMessageModelId = None
            },
            Cmd.none
