module RabbitSender.ElmishApp.MessageModel.Program

open Elmish
open RabbitSender.ElmishApp.Models
open RabbitSender.ElmishApp.Models.MessageModel

open RabbitSender.ElmishApp.Interfaces

let exchangeType id =

    match id |> Option.get with
    | 0 -> RabbitMQ.Client.ExchangeType.Direct
    | 1 -> RabbitMQ.Client.ExchangeType.Topic
    | 2 -> RabbitMQ.Client.ExchangeType.Fanout
    | 3 -> RabbitMQ.Client.ExchangeType.Headers
    | _ -> failwith "Unknown exchange type"

let update (errorMessageQueue: IErrorMessageQueue) (msg: Msg) (model: MessageModel) =
    match msg with
    | SetHost host ->
        { model with Host = host }, Cmd.none

    | SetEntityName entityName ->
        { model with RabbitEntityName = entityName }, Cmd.none

    | SetPayloadJson json ->
        try
            let dto = model.Deserializer json
            { model with PayloadJson = json; Dto = dto }, Cmd.none
        with
        | ex -> model, Cmd.ofMsg (OnError ex)

    | SelectExchangeTypeId id ->
        { model with SelectedExchangeTypeId = id }, Cmd.none

    | Send ->
        model, Cmd.OfTask.attempt model.Send (model.Dto, model.RabbitEntityName, model.Host, exchangeType model.SelectedExchangeTypeId) OnError
    
    | Msg.OnError ex ->
        errorMessageQueue.EnqueuError(ex.Message)
        model, Cmd.none