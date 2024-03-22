using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace ChatAPI.Services;

[Authorize]
public class ChatGrpcService : Chat.ChatBase
{
    private readonly MessageSenderService _senderService;

    public ChatGrpcService(MessageSenderService senderService)
    {
        _senderService = senderService;
    }

    public override Task<ChatHistory> JoinChat(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ChatHistory
        {
            Messages = { ChatMessageHistoryRepository.GetHistory() }
        });
    }

    public override async Task StartReceivingMessages(Empty request,
        IServerStreamWriter<ChatMessageResponse> responseStream, ServerCallContext context)
    {
        _senderService.Subscribe(responseStream);
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(500);
        }
    
        _senderService.Unsubscribe(responseStream);
    }

    public override async Task<Empty> SendChatMessage(ChatMessageRequest request, ServerCallContext context)
    {
        var messageResponse = MapToChatResponse(request, context);
        ChatMessageHistoryRepository.Add(messageResponse);
        await _senderService.SendMessageAsync(messageResponse);
        return new Empty();
    }
    
    private static ChatMessageResponse MapToChatResponse(ChatMessageRequest message, ServerCallContext context)
    {
        return new ChatMessageResponse
        {
            User = context.GetHttpContext().User.Identity?.Name,
            Content = message.Content
        };
    }
}