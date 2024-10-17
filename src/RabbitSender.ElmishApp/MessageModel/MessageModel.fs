namespace RabbitSender.ElmishApp.Models

open System.Threading.Tasks
open RabbitSender.ElmishApp

type MessageModel =
    {
        MessageName: string
        RabbitEntityName: string
        Host: string
        Dto: obj
        PayloadJson: string
        Exchange: string option
        Send: (obj * string * string * string) -> Task<unit>
        Deserializer: string -> obj
        SelectedExchangeTypeId: int option
    }

module MessageModel =

    type Msg =
        | SetHost of string
        | SetEntityName of string
        | SetPayloadJson of string
        | SelectExchangeTypeId of int option
        | Send
        | OnError of exn

    open System.Text.Json

    let init messageName rabbitEntityName host exchange dto send deserializer =
        {
            MessageName = messageName
            RabbitEntityName = rabbitEntityName
            Host = host
            Dto = dto
            Exchange = None
            PayloadJson =
                JsonSerializer.Serialize(dto, JsonHelpers.jsonSerializerOptions)
            Send = send
            Deserializer = deserializer
            SelectedExchangeTypeId =
                match exchange with
                | RabbitMQ.Client.ExchangeType.Direct ->  0 |> Some
                | RabbitMQ.Client.ExchangeType.Topic ->   1 |> Some
                | RabbitMQ.Client.ExchangeType.Fanout ->  2 |> Some
                | RabbitMQ.Client.ExchangeType.Headers -> 3 |> Some
                | _ -> failwith "unknown exchange"
        }