module RabbitSender.ElmishApp.MainModel.Bindings

open Elmish.WPF
open RabbitSender.ElmishApp
open RabbitSender.ElmishApp.Models
open RabbitSender.ElmishApp.Models.MainModel

let bindings () : Binding<MainModel, Msg> list =
    [
        "ErrorMessageQueue" |> Binding.oneWay (fun m -> m.ErrorMessageQueue)

        "MessageModelList"
        |> Binding.subModelSeq (
            (fun () -> MessageModel.Bindings.bindings () |> List.map (Binding.mapModel (fun ((_, m)) -> m))),
            (fun ((ind, m)) -> ind)
        )
        |> Binding.mapModel (fun m -> m.Messages |> List.mapi (fun ind sm -> (ind, sm)) |> Seq.ofList)
        |> Binding.mapMsg MessageMsg

        "SelectedMessageModelId" 
        |> Binding.subModelSelectedItem ("MessageModelList", (fun m -> m.SelectedMessageModelId), Msg.SelectMessageModelId)

        "IsMessageModelSelected" |> Binding.oneWay (fun m -> m.SelectedMessageModelId |> Option.isSome)

        "SelectedMessage"
        |> Binding.SubModel.opt (fun () -> MessageModel.Bindings.bindings () |> List.map (Binding.mapModel (fun ((_, m)) -> m)))
        |> Binding.mapModel (fun m -> m.SelectedMessageModelId |> Option.map (fun ind -> (ind, m.Messages |> List.item ind)))
        |> Binding.mapMsg MessageMsg2

    ]