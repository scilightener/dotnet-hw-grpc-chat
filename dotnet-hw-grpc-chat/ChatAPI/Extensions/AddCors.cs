using ChatAPI.Constants;

namespace ChatAPI.Extensions;

public static class AddCorsExtension
{
    public static IServiceCollection AddApplicationCors(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy(Cors.AllowAnyPolicyName,
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }
}