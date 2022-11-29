using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NamePairing;

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
    
    private static readonly byte[] Magic = {
        0xFF, 0xCC
    };
        
    public string Serialize() {
        
        var utfKey = Encoding.UTF8.GetBytes(this.Name);
        var mem = new MemoryStream();
        var writer = new BinaryWriter(mem);
        writer.Write(Magic);
        writer.Write7BitEncodedInt(utfKey.Length);
        writer.Write(utfKey);
        if (string.IsNullOrWhiteSpace(Notes)) {
            writer.Write(new byte[] {0x00});
        }
        else {
            writer.Write((byte)0xFE);
            // writer.Write7BitEncodedInt(Notes.Length);
            writer.Write(Notes);
        }
        writer.Flush();
        var bytes = mem.ToArray();
        var binaryEncoded = Convert.ToBase64String(bytes).TrimEnd(Padding).Replace('+','-').Replace('/','_');
        return binaryEncoded;
    }

    public static Participant Deserialize(string encodedInput) {
        if (encodedInput.StartsWith("{")) {
            return JsonSerializer.Deserialize<Participant>(encodedInput)!;
        }

        // var incoming = encodedInput;
        var incoming = encodedInput
            .Replace('_', '/').Replace('-', '+');
        if (!encodedInput.EndsWith("=")) {
            switch (encodedInput.Length % 4) {
                case 2:
                    incoming += "==";
                    break;
                case 3:
                    incoming += "=";
                    break;
            }
        }

        var bytes = Convert.FromBase64String(incoming).ToArray();
        if (!bytes.Take(2).SequenceEqual(Magic)) {
            throw new InvalidOperationException("Invalid magic found when deserializing participant!");
        }
        var mem = new MemoryStream(bytes.Skip(2).ToArray());
        var reader = new BinaryReader(mem);
        var name = reader.ReadString();
        if (reader.ReadByte() == 0x00) {
            //no notes, EOF
            return new Participant(name);
        }

        var notes = reader.ReadString();
        return new Participant(name) {
            Notes = notes
        };
    }

    private static readonly char[] Padding = { '=' };
        

        
}