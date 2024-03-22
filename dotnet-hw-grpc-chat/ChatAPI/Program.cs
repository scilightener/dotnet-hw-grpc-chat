using ChatAPI.Constants;
using ChatAPI.Extensions;
using ChatAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuth();
builder.Services.AddApplicationCors();
builder.Services.AddAuthorization();
builder.Services.AddSingleton(new MessageSenderService());
builder.Services.AddGrpc();

var app = builder.Build();

app.UseCors(Cors.AllowAnyPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ChatGrpcService>();
app.MapGrpcService<JwtGrpcService>();

app.Run();