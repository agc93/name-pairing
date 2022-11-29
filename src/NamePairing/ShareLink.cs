using System.Net;
using System.Text;
using System.Text.Json;

namespace NamePairing;

public record ShareLink(string Key, string Text)
{
    public string Key { get; set; } = Key;
    public string Text { get; set; } = Text;

    public string GetLinkFragment() {
        var json = JsonSerializer.Serialize(this);
        var encoded = json.EncodeToBase64();
        var urlEncoded = WebUtility.UrlEncode(encoded);
        var binary = Serialize();
        var recon = Deserialize(binary);
        var isValid = Key == recon.Key && Text == recon.Text;
        return binary;
    }

    public static ShareLink? ParseLinkFragment(string fragment) {
        var urlDecoded = WebUtility.UrlDecode(fragment);
        urlDecoded = DecodeFragment(urlDecoded);
        var rawDecoded = Convert.FromBase64String(urlDecoded);
        if (rawDecoded.Take(2).SequenceEqual(Magic)) {
            return Deserialize(fragment);
        }
        var decoded = urlDecoded.DecodeFromBase64();
        var link = JsonSerializer.Deserialize<ShareLink>(decoded);
        return link;
    }

    private static readonly char[] Padding = { '=' };
    private static readonly byte[] Magic = {
        0xFF, 0xAA
    };

    public static byte[] WriteLink(ShareLink link) {
        var utfKey = Encoding.UTF8.GetBytes(link.Key);
        var cipherBytes = Encoding.UTF8.GetBytes(link.Text);
        var mem = new MemoryStream();
        using var writer = new BinaryWriter(mem);
        writer.Write(Magic);
        writer.Write(utfKey);
        writer.Write7BitEncodedInt(cipherBytes.Length);
        writer.Write(cipherBytes);
        writer.Write(new byte[] {0x00});
        writer.Flush();
        var completeOutput = mem.ToArray();
        return completeOutput;
    }

    public string Serialize() {
        var bytes = WriteLink(this);
        var binaryEncoded = Convert.ToBase64String(bytes).TrimEnd(Padding).Replace('+','-').Replace('/','_');
        return binaryEncoded;
    }

    private static string DecodeFragment(string encodedFragment) {
        var incoming = encodedFragment
            .Replace('_', '/').Replace('-', '+');
        if (!encodedFragment.EndsWith("=")) {
            switch (encodedFragment.Length % 4) {
                case 2:
                    incoming += "==";
                    break;
                case 3:
                    incoming += "=";
                    break;
            }
        }

        return incoming;
    }

    public static ShareLink Deserialize(string encodedLink) {
        string incoming = encodedLink
            .Replace('_', '/').Replace('-', '+');
        switch(encodedLink.Length % 4) {
            case 2: incoming += "=="; break;
            case 3: incoming += "="; break;
        }
        var bytes = Convert.FromBase64String(incoming).Skip(2).ToArray();
        using var reader = new BinaryLinkReader(bytes);
        var guidBytes = reader.ReadBytes(32);
        var guid = Encoding.UTF8.GetString(guidBytes);
        var cipher = reader.ReadString();
        return new ShareLink(guid, cipher);
    }
}