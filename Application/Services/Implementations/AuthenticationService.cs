using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Dto.RequestDto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class AuthenticationService(IUtenteRepository utenteRepository) : IAuthenticationService
{
    public async Task<ServiceResult<int>> RegisterNewUser(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateRegistrationRequest(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return validationResult;

        var (hash, salt) = PasswordHasher.HashPassword(request.Password);

        var dto = new UtenteDto
        {
            Username = request.Username.Trim(),
            Nome = request.Nome.Trim(),
            Cognome = request.Cognome.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Attivo = true
        };

        try
        {
            var id = await utenteRepository.CreateAsync(dto, cancellationToken);
            return id > 0
                ? ServiceResult<int>.Success(id)
                : ServiceResult<int>.Failure("Inserimento non riuscito.");
        }
        catch (DbUpdateException)
        {
            return ServiceResult<int>.Failure("Inserimento non riuscito: vincolo dati violato.");
        }
        catch (Exception)
        {
            return ServiceResult<int>.Failure("Inserimento non riuscito: errore inatteso.");
        }
    }

    private async Task<ServiceResult<int>> ValidateRegistrationRequest(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return ServiceResult<int>.Failure("Username obbligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<int>.Failure("Password obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<int>.Failure("Email obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Cognome))
        {
            return ServiceResult<int>.Failure("Nome e cognome sono obbligatori.");
        }

        if (await utenteRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
        {
            return ServiceResult<int>.Failure("Username già in uso.");
        }

        if (await utenteRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return ServiceResult<int>.Failure("Email già in uso.");
        }

        return ServiceResult<int>.Success(0);
    }

    public async Task<ServiceResult<bool>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
        {
            return ServiceResult<bool>.Failure("Username o email obbligatori.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<bool>.Failure("Password obbligatoria.");
        }

        var user = await utenteRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail, cancellationToken);
        if (user is null)
        {
            return ServiceResult<bool>.Failure("Credenziali non valide.");
        }

        var isValidPassword = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isValidPassword)
        {
            return ServiceResult<bool>.Failure("Credenziali non valide.");
        }

        return ServiceResult<bool>.Success(true);
    }
}
