using System.Security.Cryptography;
using System.Text;
using YourOwnJournal.Repositories;

namespace YourOwnJournal.Services;

public class SecurityService
{
    private const string PinHashKey = "PinHash";
    private const string PinSaltKey = "PinSalt";
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    private readonly IAppLockRepository _appLockRepository;

    public SecurityService(IAppLockRepository appLockRepository)
    {
        _appLockRepository = appLockRepository;
    }

    public async Task<bool> IsPinSetAsync()
    {
        var hash = await _appLockRepository.GetValueAsync(PinHashKey);
        var salt = await _appLockRepository.GetValueAsync(PinSaltKey);
        return !string.IsNullOrWhiteSpace(hash) && !string.IsNullOrWhiteSpace(salt);
    }

    public async Task<bool> SetPinAsync(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
        {
            return false;
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPin(pin, salt);

        await _appLockRepository.SetValueAsync(PinSaltKey, Convert.ToBase64String(salt));
        await _appLockRepository.SetValueAsync(PinHashKey, Convert.ToBase64String(hash));
        return true;
    }

    public async Task<bool> VerifyPinAsync(string pin)
    {
        var saltBase64 = await _appLockRepository.GetValueAsync(PinSaltKey);
        var hashBase64 = await _appLockRepository.GetValueAsync(PinHashKey);
        if (string.IsNullOrWhiteSpace(saltBase64) || string.IsNullOrWhiteSpace(hashBase64))
        {
            return false;
        }

        var salt = Convert.FromBase64String(saltBase64);
        var expectedHash = Convert.FromBase64String(hashBase64);
        var actualHash = HashPin(pin, salt);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    public async Task<(bool Success, string Message)> ChangePinAsync(string currentPin, string newPin)
    {
        if (!await VerifyPinAsync(currentPin))
        {
            return (false, "Current PIN is incorrect.");
        }

        var set = await SetPinAsync(newPin);
        return set ? (true, "PIN updated.") : (false, "New PIN must be at least 4 characters.");
    }

    private static byte[] HashPin(string pin, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(pin), salt, Iterations, HashAlgorithmName.SHA256, KeySize);
    }
}
