using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Infrastructure.Data;
using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Infrastructure.Repositories;
using WebApiBuddyGames.Infrastructure.Options;
using WebApiBuddyGames.Infrastructure.Storage;

namespace WebApiBuddyGames.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' non trovata.");

        var r2Options = configuration.GetSection(CloudflareR2Options.SectionName).Get<CloudflareR2Options>()
            ?? throw new InvalidOperationException($"Configurazione '{CloudflareR2Options.SectionName}' non trovata.");

        var hasR2Config =
            !string.IsNullOrWhiteSpace(r2Options.AccessKeyId)
            && !string.IsNullOrWhiteSpace(r2Options.SecretAccessKey)
            && !string.IsNullOrWhiteSpace(r2Options.BucketName)
            && (!string.IsNullOrWhiteSpace(r2Options.ServiceUrl) || !string.IsNullOrWhiteSpace(r2Options.AccountId));

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.Configure<CloudflareR2Options>(configuration.GetSection(CloudflareR2Options.SectionName));

        if (hasR2Config)
        {
            var resolvedServiceUrl = !string.IsNullOrWhiteSpace(r2Options.ServiceUrl)
                ? r2Options.ServiceUrl
                : $"https://{r2Options.AccountId}.r2.cloudflarestorage.com";

            services.AddSingleton<IAmazonS3>(_ =>
            {
                var credentials = new BasicAWSCredentials(r2Options.AccessKeyId, r2Options.SecretAccessKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = resolvedServiceUrl,
                    AuthenticationRegion = "auto",
                    ForcePathStyle = true
                };

                return new AmazonS3Client(credentials, config);
            });

            services.AddScoped<IProfileImageStorage, CloudflareR2ProfileImageStorage>();
        }
        else
        {
            services.AddScoped<IProfileImageStorage, UnavailableProfileImageStorage>();
        }

        services.AddScoped<IUtenteRepository, UtenteRepository>();
        services.AddScoped<IRuoloRepository, RuoloRepository>();
        services.AddScoped<IUtenteRuoloRepository, UtenteRuoloRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPartitaRepository, PartitaRepository>();
        services.AddScoped<IPartitaUtenteRepository, PartitaUtenteRepository>();
        services.AddScoped<IRisultatoRepository, RisultatoRepository>();

        return services;
    }
}
