using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Dto.RequestDto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class AuthenticationService(
    IUtenteRepository utenteRepository,
    IRuoloRepository ruoloRepository,
    IUtenteRuoloRepository utenteRuoloRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtOptions> jwtOptions) : IAuthenticationService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

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
            if (id <= 0)
            {
                return ServiceResult<int>.Failure("Inserimento non riuscito.");
            }

            var ruoloUtente = await ruoloRepository.GetByDescrizioneAsync("Utente", cancellationToken);
            if (ruoloUtente is not null)
            {
                await utenteRuoloRepository.CreateAsync(new UtenteRuoloDto
                {
                    UtenteId = id,
                    RuoloId = ruoloUtente.Id,
                    Attivo = true
                }, cancellationToken);
            }

            return ServiceResult<int>.Success(id);
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

    public async Task<ServiceResult<AuthTokensDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
        {
            return ServiceResult<AuthTokensDto>.Failure("Username o email obbligatori.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<AuthTokensDto>.Failure("Password obbligatoria.");
        }

        var user = await utenteRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail, cancellationToken);
        if (user is null)
        {
            return ServiceResult<AuthTokensDto>.Failure("Credenziali non valide.");
        }

        var isValidPassword = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isValidPassword)
        {
            return ServiceResult<AuthTokensDto>.Failure("Credenziali non valide.");
        }

        var tokens = await CreateTokensAsync(user, cancellationToken);
        return ServiceResult<AuthTokensDto>.Success(tokens);
    }

    public async Task<ServiceResult<AuthTokensDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ServiceResult<AuthTokensDto>.Failure("Refresh token obbligatorio.");
        }

        var tokenHash = ComputeRefreshTokenHash(request.RefreshToken);
        var savedToken = await refreshTokenRepository.GetActiveByTokenHashAsync(tokenHash, cancellationToken);
        if (savedToken is null)
        {
            return ServiceResult<AuthTokensDto>.Failure("Refresh token non valido o scaduto.");
        }

        var user = await utenteRepository.GetByIdAsync(savedToken.UtenteId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<AuthTokensDto>.Failure("Utente non trovato.");
        }

        await refreshTokenRepository.RevokeAsync(savedToken.Id, DateTime.UtcNow, cancellationToken);

        var tokens = await CreateTokensAsync(user, cancellationToken);
        return ServiceResult<AuthTokensDto>.Success(tokens);
    }

    public async Task<ServiceResult<MeResponseDto>> GetMeAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return ServiceResult<MeResponseDto>.Failure("Utente non valido.");
        }

        var user = await utenteRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<MeResponseDto>.Failure("Utente non trovato.");
        }

        var roles = await utenteRuoloRepository.GetRoleDescriptionsByUserIdAsync(user.Id, cancellationToken);

        return ServiceResult<MeResponseDto>.Success(new MeResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Nome = user.Nome,
            Cognome = user.Cognome,
            Email = user.Email,
            Ruoli = roles
        });
    }

    private async Task<AuthTokensDto> CreateTokensAsync(UtenteDto user, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var accessTokenExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var refreshTokenExpiresAtUtc = now.AddDays(_jwtOptions.RefreshTokenDays);
        var roleDescriptions = await utenteRuoloRepository.GetRoleDescriptionsByUserIdAsync(user.Id, cancellationToken);

        var accessToken = GenerateAccessToken(user, roleDescriptions, now, accessTokenExpiresAtUtc);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = ComputeRefreshTokenHash(refreshToken);

        await refreshTokenRepository.CreateAsync(new RefreshTokenDto
        {
            UtenteId = user.Id,
            TokenHash = refreshTokenHash,
            ScadenzaUtc = refreshTokenExpiresAtUtc,
            Attivo = true
        }, cancellationToken);

        return new AuthTokensDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc
        };
    }

    private string GenerateAccessToken(UtenteDto user, IReadOnlyList<string> roleDescriptions, DateTime issuedAtUtc, DateTime expiresAtUtc)
    {
        ValidateJwtOptions();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("nome", user.Nome),
            new(ClaimTypes.GivenName, user.Nome),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roleDescriptions.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToHexString(randomBytes);
    }

    private static string ComputeRefreshTokenHash(string refreshToken)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(refreshToken.Trim());
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToHexString(hashBytes);
    }

    private void ValidateJwtOptions()
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey) || _jwtOptions.SecretKey.Length < 32)
        {
            throw new InvalidOperationException("Configurazione JWT non valida: SecretKey deve avere almeno 32 caratteri.");
        }

        if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer) || string.IsNullOrWhiteSpace(_jwtOptions.Audience))
        {
            throw new InvalidOperationException("Configurazione JWT non valida: Issuer e Audience sono obbligatori.");
        }
    }
}
