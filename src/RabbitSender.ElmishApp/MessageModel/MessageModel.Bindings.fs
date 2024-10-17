module RabbitSender.ElmishApp.MessageModel.Bindings

open Elmish.WPF
open RabbitSender.ElmishApp.Models
open RabbitSender.ElmishApp.Models.MessageModel
open System.Text.Json

let exchangeBindings () : Binding<int * string, Msg> list =
    [
        "Name" |> Binding.oneWay (fun (_, m) -> m)
    ]

let bindings () : Binding<MessageModel, Msg> list =
    [
        "MessageName" |> Binding.oneWay (fun m -> m.MessageName)
        "RabbitEntityName" |> Binding.twoWay ((fun m -> m.RabbitEntityName), Msg.SetEntityName)
        "Host" |> Binding.twoWay ((fun m -> m.Host), Msg.SetHost)
        "PayloadJson" |> Binding.twoWay ((fun m -> m.PayloadJson), Msg.SetPayloadJson)
        "SendCommand" |> Binding.cmd Msg.Send
        "ExchangeTypes"
        |> Binding.subModelSeq (
            exchangeBindings,
            (fun ((ind, m)) -> ind)
        )
        |> Binding.mapModel (fun m -> [
            (0, RabbitMQ.Client.ExchangeType.Direct)
            (1, RabbitMQ.Client.ExchangeType.Topic)
            (2, RabbitMQ.Client.ExchangeType.Fanout)
            (3, RabbitMQ.Client.ExchangeType.Headers)
        ])
        |> Binding.mapMsg (fun (_, msg) -> msg)

        "SelectedExchangeTypeIdlId" 
        |> Binding.subModelSelectedItem (
            "ExchangeTypes",
            (fun m -> m.SelectedExchangeTypeId),
            Msg.SelectExchangeTypeId
        )
    ]