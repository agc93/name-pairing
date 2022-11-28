using System.Security.Cryptography;
using Blazor.SubtleCrypto;
using Microsoft.JSInterop;

namespace NamePairing;

public interface IEncryptService
{
    Task<(string Key, string Cipher)> Encrypt(string content);
    Task<string?> Decrypt(string content, string key);
}

public class InteropEncryptService : IEncryptService
{
    private readonly IJSRuntime _jsRuntime;

    public InteropEncryptService(IJSRuntime jsRuntime) {
        _jsRuntime = jsRuntime;
    }
    public async Task<(string Key, string Cipher)> Encrypt(string content) {
        var key = Guid.NewGuid().ToString("N");
        var jsResult = await _jsRuntime.InvokeAsync<string>("aesGcmEncrypt", content, key);
        return (key, jsResult);
    }

    public async Task<string?> Decrypt(string content, string key) {
        try {
            var jsResult = await _jsRuntime.InvokeAsync<string?>("aesGcmDecrypt", content, key);
            return jsResult;
        }
        catch (JSException) {
            return null;
        }
    }
}

public class ManagedEncryptService : IEncryptService
{
    private readonly ICryptoService _crypto;
    private readonly IJSRuntime _jsRuntime;

    public ManagedEncryptService(ICryptoService cryptoService, IJSRuntime jsRuntime) {
        _crypto = cryptoService;
        _jsRuntime = jsRuntime;
    }

    public async Task<(string Key, string Cipher)> Encrypt(string content) {
        var result = await _crypto.EncryptAsync(content);
        var jsResult = await _jsRuntime.InvokeAsync<string>("aesGcmEncrypt", content, Guid.NewGuid().ToString("N"));
        return (Key: result.Secret.Key, Cipher: result.Value);
    }
    
    [Obsolete("Not currently implemented!", false)]
    public async Task<(string Key, string Cipher)> Encrypt(string content, string key) {
        var result = await _crypto.EncryptAsync(content);
        return (Key: result.Secret.Key, Cipher: result.Value);
    }

    public async Task<string?> Decrypt(string content, string key) {
        var result = await _crypto.DecryptAsync(new CryptoInput() {
            Key = key,
            Value = content
        });
        return result;
    }
}