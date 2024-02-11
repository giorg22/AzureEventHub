using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;

var connectionString = "Endpoint=sb://namespaca.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=EMErWDVKjC/IGlOoX43RbZ1bNwCewnMo6+AEhP/f3/o=";
var eventHubName = "bbb";

await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
{
    using EventDataBatch eventBatch = await producer.CreateBatchAsync();

    if ((!eventBatch.TryAdd(new EventData("a"))) ||
        (!eventBatch.TryAdd(new EventData("b"))))
    {
        throw new ApplicationException("Not all events could be added to the batch!");
    }

    await producer.SendAsync(eventBatch);
}