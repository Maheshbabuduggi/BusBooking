using System.Text;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace BookingApi.Messaging;

public class EventHubPublisher : IEventPublisher
{
    private readonly EventHubProducerClient _producerClient;
    private readonly ILogger<EventHubPublisher> _logger;

    public EventHubPublisher(EventHubProducerClient producerClient, ILogger<EventHubPublisher> logger)
    {
        _producerClient = producerClient;
        _logger = logger;
    }

    public async Task PublishBookingCreatedAsync(BookingCreatedEvent bookingEvent, CancellationToken ct = default)
    {
        using EventDataBatch batch = await _producerClient.CreateBatchAsync(ct);

        var json = JsonSerializer.Serialize(bookingEvent);
        var eventData = new EventData(Encoding.UTF8.GetBytes(json));
        eventData.Properties["EventType"] = "BookingCreated";

        if (!batch.TryAdd(eventData))
            throw new InvalidOperationException("BookingCreated event is too large for the batch.");

        await _producerClient.SendAsync(batch, ct);
        _logger.LogInformation("Published BookingCreated event for BookingId {BookingId}", bookingEvent.BookingId);
    }
}
