using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Infrastructure.Data;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Infrastructure.Repositories;

namespace WebApiBuddyGames.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' non trovata.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
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
