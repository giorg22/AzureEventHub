using Azure.Messaging.EventHubs.Consumer;
using System.Text;

var connectionString = "Endpoint=sb://testeventhub99.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=eWrcfH3/K9J94Led8bVpfV+qi7ogOt2aU+AEhESu3tE=";
var eventHubName = "hahhub";

string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

// It is recommended that you cache the Event Hubs clients for the lifetime of your
// application, closing or disposing when application ends.  This example disposes
// after the immediate scope for simplicity.

await using (var consumer = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
{
    EventPosition startingPosition = EventPosition.Earliest;
    string partitionId = (await consumer.GetPartitionIdsAsync()).First();

    using var cancellationSource = new CancellationTokenSource();
    cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

    await foreach (PartitionEvent receivedEvent in consumer.ReadEventsFromPartitionAsync(partitionId, startingPosition, cancellationSource.Token))
    {
        // At this point, the loop will wait for events to be available in the partition.  When an event
        // is available, the loop will iterate with the event that was received.  Because we did not
        // specify a maximum wait time, the loop will wait forever unless cancellation is requested using
        // the cancellation token.
        string eventBody = Encoding.UTF8.GetString(receivedEvent.Data.Body.ToArray());
        Console.WriteLine($"Received event from partition {receivedEvent.Partition.PartitionId}: {eventBody}");
    }
}