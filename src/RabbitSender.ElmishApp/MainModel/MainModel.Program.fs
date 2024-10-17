module RabbitSender.ElmishApp.MainModel.Program

open Elmish
open RabbitSender.ElmishApp
open RabbitSender.ElmishApp.Models
open RabbitSender.ElmishApp.Models.MainModel
open RabbitSender.ElmishApp.Interfaces


let update (msg: Msg) (model: MainModel) =
    let messageModelUpdate = MessageModel.Program.update model.ErrorMessageQueue

    match msg with
    | SelectMessageModelId ind ->
        { model with SelectedMessageModelId = ind }, Cmd.none

    | MessageMsg (ind, messageModelMsg) ->
        let (messageModel', messageModelMsg') = messageModelUpdate messageModelMsg (model.Messages |> List.item ind)
        
        { model with Messages = model.Messages |> List.removeAt ind |> List.insertAt ind messageModel' }
        , Cmd.map Msg.MessageMsg2 messageModelMsg'

    | MessageMsg2 messageModelMsg ->
        let ind = model.SelectedMessageModelId |> Option.get
        let (messageModel', messageModelMsg') = messageModelUpdate messageModelMsg (model.Messages |> List.item ind)
        
        { model with Messages = model.Messages |> List.removeAt ind |> List.insertAt ind messageModel' }
        , Cmd.map Msg.MessageMsg2 messageModelMsg'

    | Msg.OnError ex ->
        model.ErrorMessageQueue.EnqueuError(ex.Message)
        model, Cmd.none