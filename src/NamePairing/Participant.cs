using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace NamePairing
{
    public record Participant
    {
        public string Name {get;set;} = string.Empty;
        public string? Notes {get;set;} = string.Empty;
        public Participant()
        {
            
        }

        public Participant(string name)
        {
            Name = name;
        }
    }

    public record ShareLink(string Key, string Text)
    {
        // public static ShareLink Create(KeyValuePair<Participant, Participant> pairing) {
        //     var json = JsonSerializer.Serialize(pairing.Value);
        //     var key = Guid.NewGuid().ToString("N");
        //     
        //     return new ShareLink(pairing.Key.Name, )
        // }

        // public string LinkRecipient { get; set; } = LinkRecipient;
        public string Key { get; set; } = Key;
        public string Text { get; set; } = Text;

        public string GetLinkFragment() {
            var json = JsonSerializer.Serialize(this);
            var encoded = json.EncodeToBase64();
            return WebUtility.UrlEncode(encoded);
        }

        public static ShareLink? ParseLinkFragment(string fragment) {
            var urlDecoded = WebUtility.UrlDecode(fragment);
            var decoded = urlDecoded.DecodeFromBase64();
            var link = JsonSerializer.Deserialize<ShareLink>(decoded);
            return link;
        }
        
    }
}