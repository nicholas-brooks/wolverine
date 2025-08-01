# Interoperability

Hey, it's a complicated world and Wolverine is a relative newcomer, so it's somewhat likely you'll find yourself needing to make a Wolverine application talk via AWS SQS to
a non-Wolverine application. Not to worry (too much), Wolverine has you covered with the ability to customize Wolverine to Amazon SQS mapping.

## Receive Raw JSON

If you need to receive raw JSON from an upstream system *and* you can expect only one message type for the current
queue, you can do that with this option:

<!-- snippet: sample_receive_raw_json_in_sqs -->
<a id='snippet-sample_receive_raw_json_in_sqs'></a>
```cs
using var host = await Host.CreateDefaultBuilder()
    .UseWolverine(opts =>
    {
        opts.UseAmazonSqsTransport();

        opts.ListenToSqsQueue("incoming").ReceiveRawJsonMessage(
            // Specify the single message type for this queue
            typeof(Message1),

            // Optionally customize System.Text.Json configuration
            o => { o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });
    }).StartAsync();
```
<sup><a href='https://github.com/JasperFx/wolverine/blob/main/src/Transports/AWS/Wolverine.AmazonSqs.Tests/Samples/Bootstrapping.cs#L250-L265' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_receive_raw_json_in_sqs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Likewise, to send raw JSON to external systems, you have this option:

<!-- snippet: sample_publish_raw_json_in_sqs -->
<a id='snippet-sample_publish_raw_json_in_sqs'></a>
```cs
using var host = await Host.CreateDefaultBuilder()
    .UseWolverine(opts =>
    {
        opts.UseAmazonSqsTransport();

        opts.PublishAllMessages().ToSqsQueue("outgoing").SendRawJsonMessage(
            // Specify the single message type for this queue
            typeof(Message1),

            // Optionally customize System.Text.Json configuration
            o => { o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });
    }).StartAsync();
```
<sup><a href='https://github.com/JasperFx/wolverine/blob/main/src/Transports/AWS/Wolverine.AmazonSqs.Tests/Samples/Bootstrapping.cs#L307-L322' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_publish_raw_json_in_sqs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Advanced Interoperability

For any kind of advanced interoperability between Wolverine and any other kind of application communicating with your
Wolverine application using SQS, you can build custom implementations of the `ISqsEnvelopeMapper` like this one:

<!-- snippet: sample_custom_sqs_mapper -->
<a id='snippet-sample_custom_sqs_mapper'></a>
```cs
public class CustomSqsMapper : ISqsEnvelopeMapper
{
    public string BuildMessageBody(Envelope envelope)
    {
        // Serialized data from the Wolverine message
        return Encoding.Default.GetString(envelope.Data);
    }

    // Specify header values for the SQS message from the Wolverine envelope
    public IEnumerable<KeyValuePair<string, MessageAttributeValue>> ToAttributes(Envelope envelope)
    {
        if (envelope.TenantId.IsNotEmpty())
        {
            yield return new KeyValuePair<string, MessageAttributeValue>("tenant-id",
                new MessageAttributeValue { StringValue = envelope.TenantId });
        }
    }

    public void ReadEnvelopeData(Envelope envelope, string messageBody,
        IDictionary<string, MessageAttributeValue> attributes)
    {
        envelope.Data = Encoding.Default.GetBytes(messageBody);

        if (attributes.TryGetValue("tenant-id", out var att))
        {
            envelope.TenantId = att.StringValue;
        }
    }
}
```
<sup><a href='https://github.com/JasperFx/wolverine/blob/main/src/Transports/AWS/Wolverine.AmazonSqs.Tests/Samples/Bootstrapping.cs#L344-L376' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_custom_sqs_mapper' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

And apply this to any or all of your SQS endpoints with the configuration fluent interface as shown in this sample:

<!-- snippet: sample_apply_custom_sqs_mapping -->
<a id='snippet-sample_apply_custom_sqs_mapping'></a>
```cs
using var host = await Host.CreateDefaultBuilder()
    .UseWolverine(opts =>
    {
        opts.UseAmazonSqsTransport()
            .UseConventionalRouting()
            .DisableAllNativeDeadLetterQueues()
            .ConfigureListeners(l => l.InteropWith(new CustomSqsMapper()))
            .ConfigureSenders(s => s.InteropWith(new CustomSqsMapper()));
    }).StartAsync();
```
<sup><a href='https://github.com/JasperFx/wolverine/blob/main/src/Transports/AWS/Wolverine.AmazonSqs.Tests/Samples/Bootstrapping.cs#L328-L340' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_apply_custom_sqs_mapping' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Receive messages from Amazon SNS

By default, Amazon SNS wraps their messages in a structured format that includes metadata.
You can either turn this feature off in SNS or let Wolverine handle the mapping like this:

<!-- snippet: sample_receive_sns_topic_metadata_in_sqs -->
<a id='snippet-sample_receive_sns_topic_metadata_in_sqs'></a>
```cs
using var host = await Host.CreateDefaultBuilder()
    .UseWolverine(opts =>
    {
        opts.UseAmazonSqsTransport();

        opts.ListenToSqsQueue("incoming")
            // Interops with SNS structured metadata
            .ReceiveSnsTopicMessage();
    }).StartAsync();
```
<sup><a href='https://github.com/JasperFx/wolverine/blob/main/src/Transports/AWS/Wolverine.AmazonSqs.Tests/Samples/Bootstrapping.cs#L271-L283' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_receive_sns_topic_metadata_in_sqs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

It's possible for the original message that was sent to SNS to be in a different format. 
You can also specify a custom mapper to deal with the format of the original message as shown here:

<!-- snippet: sample_receive_sns_topic_metadata_with_custom_mapper_in_sqs -->
<a id='snippet-sample_receive_sns_topic_metadata_with_custom_mapper_in_sqs'></a>
```cs
using var host = await Host.CreateDefaultBuilder()
    .UseWolverine(opts =>
    {
        opts.UseAmazonSqsTransport();

        opts.ListenToSqsQueue("incoming")
            // Interops with SNS structured metadata
            .ReceiveSnsTopicMessage(
                // Sets inner mapper for original message
                new RawJsonSqsEnvelopeMapper(typeof(Message1), new JsonSerializerOptions()));
    }).StartAsync();
```
<sup><a href='https://github.com/JasperFx/wolverine/blob/main/src/Transports/AWS/Wolverine.AmazonSqs.Tests/Samples/Bootstrapping.cs#L288-L302' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_receive_sns_topic_metadata_with_custom_mapper_in_sqs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
