using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Text;
using Azure.Identity;
using Azure.Messaging.EventHubs;

var connectionString = "Endpoint=sb://namespaca.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=EMErWDVKjC/IGlOoX43RbZ1bNwCewnMo6+AEhP/f3/o=";
var eventHubName = "bbb";

string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

await using (var consumer = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
{
    EventPosition startingPosition = EventPosition.Earliest;
    string partitionId = (await consumer.GetPartitionIdsAsync()).First();

    using var cancellationSource = new CancellationTokenSource();
    cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

    await foreach (PartitionEvent receivedEvent in consumer.ReadEventsFromPartitionAsync(partitionId, startingPosition, cancellationSource.Token))
    {
        string eventBody = Encoding.UTF8.GetString(receivedEvent.Data.Body.ToArray());
        Console.WriteLine($"Received event from partition {receivedEvent.Partition.PartitionId}: {eventBody}");
    }
}

BlobContainerClient storageClient = new BlobContainerClient("DefaultEndpointsProtocol=https;AccountName=staccount55;AccountKey=Glki0PfVvpOycqkTNLXMchfNjW2axsyXvZYTjxn/mIOce0RO0jTIdomdyBqEAjRzTLv1xw5yxl5J+AStadRsuw==;EndpointSuffix=core.windows.net", "checkpointa");

var processor = new EventProcessorClient(
    storageClient,
    EventHubConsumerClient.DefaultConsumerGroupName,
    connectionString,
    eventHubName);

// Register handlers for processing events and handling errors
processor.ProcessEventAsync += ProcessEventHandler;
processor.ProcessErrorAsync += ProcessErrorHandler;

// Start the processing
await processor.StartProcessingAsync();

// Wait for 30 seconds for the events to be processed
await Task.Delay(TimeSpan.FromSeconds(60));

// Stop the processing
await processor.StopProcessingAsync();

static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
{
    // Write the body of the event to the console window
    Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
    await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
}

Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
{
    // Write details about the error to the console window
    Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
    Console.WriteLine(eventArgs.Exception.Message);
    return Task.CompletedTask;
}