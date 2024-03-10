using Grpc.Core;

namespace ChatAPI.Services;

public class MessageSenderService
{
    private readonly List<IServerStreamWriter<ChatMessageResponse>> _streams = new();

    public void Subscribe(IServerStreamWriter<ChatMessageResponse> stream) => _streams.Add(stream);
    
    public void Unsubscribe(IServerStreamWriter<ChatMessageResponse> stream) => _streams.Remove(stream);

    public async Task SendMessageAsync(ChatMessageResponse message)
    {
        await Parallel.ForEachAsync(_streams, async (stream, ctx) =>
        {
            await stream.WriteAsync(message, ctx);
        });
    }
}