using System.Collections.ObjectModel;

namespace FriendlyElectronics.HexFileLib;

public class HexRecord
{
    public HexRecord(HexRecordType type, int address, IList<byte> data = null, string comment = null, string comment2 = null)
    {
        Type = type;
        Address = address;
        Data = new ReadOnlyCollection<byte>(data ?? Array.Empty<byte>());
        Comment = comment ?? string.Empty;
        Comment2 = comment2 ?? string.Empty;
    }
    
    public HexRecordType Type { get; }
    public int Address { get; }
    public ReadOnlyCollection<byte> Data { get; }
    public string Comment { get; }
    public string Comment2 { get; }
}