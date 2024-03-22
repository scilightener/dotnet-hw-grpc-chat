using Grpc.Core;

namespace ChatAPI.Services;

public class MessageSenderService
{
    private readonly HashSet<IServerStreamWriter<ChatMessageResponse>> _streams = new();
    private readonly object _locker = new();

    public void Subscribe(IServerStreamWriter<ChatMessageResponse> stream)
    {
        lock (_locker)
        {
            _streams.Add(stream);
        }
    }

    public void Unsubscribe(IServerStreamWriter<ChatMessageResponse> stream)
    {
        lock (_locker)
        {
            _streams.Remove(stream);
        }
    }

    public async Task SendMessageAsync(ChatMessageResponse message)
    {
        await Parallel.ForEachAsync(_streams, async (stream, ctx) =>
        {
            await stream.WriteAsync(message, ctx);
        });
    }
}