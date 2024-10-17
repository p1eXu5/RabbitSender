module RabbitSender.ElmishApp.JsonHelpers

open System.Text.Json
open System.Text.Json.Serialization

let jsonSerializerOptions = JsonSerializerOptions()
jsonSerializerOptions.Converters.Add(JsonStringEnumConverter())
jsonSerializerOptions.WriteIndented <- true

