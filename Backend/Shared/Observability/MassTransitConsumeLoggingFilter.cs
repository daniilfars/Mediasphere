using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shared.Observability;

public sealed class MassTransitConsumeLoggingFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly ILogger<MassTransitConsumeLoggingFilter<TMessage>> _logger;

    public MassTransitConsumeLoggingFilter(ILogger<MassTransitConsumeLoggingFilter<TMessage>> logger)
    {
        _logger = logger;
    }

    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["MessageId"] = context.MessageId,
            ["ConversationId"] = context.ConversationId,
            ["MessageType"] = typeof(TMessage).Name
        }))
        {
            await next.Send(context);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("consume-logging");
    }
}