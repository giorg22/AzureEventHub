using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;

var connectionString = "Endpoint=sb://testeventhub99.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=eWrcfH3/K9J94Led8bVpfV+qi7ogOt2aU+AEhESu3tE=";
var eventHubName = "hahhub";

// It is recommended that you cache the Event Hubs clients for the lifetime of your
// application, closing or disposing when application ends.  This example disposes
// after the immediate scope for simplicity.

await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
{
    using EventDataBatch eventBatch = await producer.CreateBatchAsync();

    if ((!eventBatch.TryAdd(new EventData("First"))) ||
        (!eventBatch.TryAdd(new EventData("Second"))))
    {
        throw new ApplicationException("Not all events could be added to the batch!");
    }

    await producer.SendAsync(eventBatch);
}