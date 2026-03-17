using WebApiBuddyGames.Application.Services.Implementations;
using WebApiBuddyGames.Application.Services.Interfaces;

namespace WebApiBuddyGames.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUtenteService, UtenteService>();
        services.AddScoped<IPartitaService, PartitaService>();
        services.AddScoped<IPartitaUtenteService, PartitaUtenteService>();
        services.AddScoped<IRisultatoService, RisultatoService>();

        return services;
    }
}
