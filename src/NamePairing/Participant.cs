using System.Text;
using System.Text.Json;

namespace NamePairing;

public record Participant
{
    public string Name {get;set;} = string.Empty;
    public string? Notes {get;set;} = string.Empty;
    public List<string> Exclusions { get; set; } = new();
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
        var utfKey = Encoding.UTF8.GetBytes(Name);
        using var writer = new BinaryLinkWriter(Magic);
        writer.WriteStringBytes(utfKey);
        if (string.IsNullOrWhiteSpace(Notes)) {
            writer.WriteNullByte();
        }
        else {
            writer.Write((byte)0xFE);
            // writer.Write7BitEncodedInt(Notes.Length);
            writer.Write(Notes);
        }
        writer.Flush();
        var bytes = writer.Stream.ToArray();
        var binaryEncoded = Convert.ToBase64String(bytes);
        return binaryEncoded;
    }

    public static Participant Deserialize(string encodedInput) {
        if (encodedInput.StartsWith("{")) {
            return JsonSerializer.Deserialize<Participant>(encodedInput)!;
        }

        // var incoming = encodedInput;
        // var encodedInput = encodedInput.UrlDecode();
        var bytes = Convert.FromBase64String(encodedInput).ToArray();
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

    public static IEqualityComparer<Participant> Comparer { get; } = new ParticipantComparer();
}

public sealed class ParticipantComparer : IEqualityComparer<Participant>
{
    public bool Equals(Participant? x, Participant? y) {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return string.Equals(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase) && string.Equals(x.Notes, y.Notes, StringComparison.CurrentCultureIgnoreCase);
    }

    public int GetHashCode(Participant obj) {
        var hashCode = new HashCode();
        hashCode.Add(obj.Name, StringComparer.CurrentCultureIgnoreCase);
        hashCode.Add(obj.Notes, StringComparer.CurrentCultureIgnoreCase);
        return hashCode.ToHashCode();
    }
}