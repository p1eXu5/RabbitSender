namespace RabbitSender.WpfClient;

using System;
using RabbitSender.ElmishApp.Interfaces;

internal class ErrorMessageQueue : MaterialDesignThemes.Wpf.SnackbarMessageQueue, IErrorMessageQueue
{
    public void EnqueuError(string value)
    {
        this.Enqueue(value, "Clear", _ => this.Clear(), null, false, true, TimeSpan.FromSeconds(15));
    }
}
