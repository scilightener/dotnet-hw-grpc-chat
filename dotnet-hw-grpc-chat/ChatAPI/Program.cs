using ChatAPI;
using ChatAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtConstants.Issuer,
            ValidAudience = JwtConstants.Audience,
            IssuerSigningKey =
                new SymmetricSecurityKey(JwtConstants.Key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton(new MessageSenderService());

builder.Services.AddGrpc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<ChatService>();
app.MapGrpcService<JwtService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();