module RabbitSender.ElmishApp.Infrastructure

open MassTransit
open System

let inline send<'TContract, 'TDto when 'TDto: not struct and 'TDto: (member RoutingKey : string) and 'TContract: not struct> (dto: 'TDto, entityName: string, host: string, exchangeType: string) =
    task {
        let busControl : IBusControl = MassTransit.Bus.Factory.CreateUsingRabbitMq(fun cfg ->
            cfg.Host(
                Uri($"rabbitmq://localhost/{host}"),
                (fun (h: IRabbitMqHostConfigurator) ->
                    h.Username("guest")
                    h.Password("guest")
                )
            )

            cfg.Message<'TDto>(fun cfg ->
                cfg.SetEntityName(entityName)
            )

            cfg.Publish<'TDto>(fun (cfg: IRabbitMqMessagePublishTopologyConfigurator<'TDto>) ->
                cfg.ExchangeType <- exchangeType
            )

            cfg.Send<'TDto>(fun (cfg: IRabbitMqMessageSendTopologyConfigurator<'TDto>) ->
                cfg.UseRoutingKeyFormatter(fun f -> f.Message.RoutingKey)
            )
        )

        try
            busControl.Start()

            let! endpoint = busControl.GetPublishSendEndpoint<'TDto>()

            do! endpoint.Send<'TContract>(
                dto,
                (fun (sendCtx: SendContext<_>) ->
                    // sendCtx.Headers.Set("HeaderName", "=========_Header_Value_=========")
                    ()
                )
            )

        finally
            busControl.Stop()
    }