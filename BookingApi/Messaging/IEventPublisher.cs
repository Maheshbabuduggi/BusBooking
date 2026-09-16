namespace BookingApi.Messaging;

public interface IEventPublisher
{
    Task PublishBookingCreatedAsync(BookingCreatedEvent bookingEvent, CancellationToken ct = default);
}
