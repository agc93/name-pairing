using System.Text;
using Microsoft.JSInterop;

namespace NamePairing;

public class DownloadService
{
    private readonly IJSRuntime _jsRuntime;

    public DownloadService(IJSRuntime jsRuntime) {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveAs(string fileName, byte[] data, string contentType) {
        await _jsRuntime.InvokeAsync<object>("BlazorDownloadFile", fileName, contentType, Convert.ToBase64String(data));
    }

    public async Task SaveText(string fileName, string text, Encoding? encoding = null) {
        encoding ??= Encoding.UTF8;
        await SaveAs(fileName, encoding.GetBytes(text), "text/plain");
    }
}