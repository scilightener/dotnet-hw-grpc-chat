namespace ChatAPI.Services;

public static class ChatMessageHistoryRepository
{
    private static readonly List<ChatMessageResponse> ChatHistory = new();
    private static readonly object Locker = new();

    public static IEnumerable<ChatMessageResponse> GetHistory()
    {
        IReadOnlyCollection<ChatMessageResponse> result;
        lock (Locker)
        {
            result = ChatHistory.AsReadOnly();
        }

        return result;
    }

    public static void Add(ChatMessageResponse message)
    {
        lock (Locker)
        {
            ChatHistory.Add(message);
        }
    }
}