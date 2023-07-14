namespace FriendlyElectronics.HexFileLib;

public static class HexParser
{
    public static HexRecord ParseLine(string line)
    {
        var index = 0;
        var comment = ReadComment(line, ref index);
        if (index >= line.Length)
            return new HexRecord(HexRecordType.Undefined, 0, Array.Empty<byte>(), comment);
        ReadStartCode(line, ref index);
        var byteCount = ReadByteCount(line, ref index);
        var address = ReadAddress(line, ref index);
        var type = ReadRecordType(line, ref index);
        var data = ReadData(line, ref index, byteCount);
        var checkSum = ReadCheckSum(line, ref index);
        VerifyCheckSum(byteCount, address, type, data, checkSum);
        var recordType = GetHexRecordType(type);
        return new HexRecord(recordType, address, data, comment);
    }

    private static string ReadComment(string line, ref int index)
    {
        var startCodeIndex = line.IndexOf(':', index);
        var result = startCodeIndex >= index ? line.Substring(index, startCodeIndex - index) : line.Substring(index, line.Length - index);
        index += result.Length;
        return result;
    }
    
    private static void ReadStartCode(string line, ref int index)
    {
        if (index >= line.Length)
            throw new Exception($"Unexpected end of line at position {index}. Start code ':' expected.");
        if (line[index] != ':')
            throw new Exception($"Unexpected character '{line[index]}' at position {index}. Start code ':' expected.");
        index++;
    }

    private static int ReadByteCount(string line, ref int index)
    {
        return ReadHexByte(line, ref index, "byte count");
    }
    
    private static int ReadAddress(string line, ref int index)
    {
        var b1 = ReadHexByte(line, ref index, "address");
        var b2 = ReadHexByte(line, ref index, "address");
        return b1 << 8 | b2;
    }
    
    private static byte ReadRecordType(string line, ref int index)
    {
        return ReadHexByte(line, ref index, "record type");
    }

    private static byte[] ReadData(string line, ref int index, int byteCount)
    {
        var result = new byte[byteCount];
        for (var i = 0; i < byteCount; i++)
            result[i] = ReadHexByte(line, ref index, "data");
        return result;
    }
    
    private static byte ReadCheckSum(string line, ref int index)
    {
        return ReadHexByte(line, ref index, "checksum");
    }
    
    private static void VerifyCheckSum(int byteCount, int address, int type, byte[] data, byte checkSum)
    {
        var calculated = 0;
        calculated += byteCount;
        calculated += address >> 8;
        calculated += address & 0xFF;
        calculated += type;
        foreach (var b in data)
            calculated += b;
        calculated = (~calculated + 1) & 0xFF;
        if (calculated != checkSum)
            throw new Exception("Checksum verification failed. Data is corrupted.");
    }

    private static HexRecordType GetHexRecordType(byte type)
    {
        if (type > 5)
            throw new Exception($"Not supported record type {type:X2}");
        return (HexRecordType) type;
    }

    private static byte ReadHexByte(string line, ref int index, string recordSegmentName)
    {
        var a = ReadHexHalfByte(line, ref index, recordSegmentName);
        var b = ReadHexHalfByte(line, ref index, recordSegmentName);
        return (byte)(a << 4 | b);
    }

    private static int ReadHexHalfByte(string line, ref int index, string recordSegmentName)
    {
        if (index >= line.Length)
            throw new Exception($"Unexpected end of line while reading {recordSegmentName} at position {index}.");
        var hex = line[index];
        if (hex is (< '0' or > '9') and (< 'A' or > 'F') and (< 'a' or > 'f'))
            throw new Exception($"Unexpected character '{line[index]}' at position {index} while reading {recordSegmentName} segment.");
        index++;
        return Convert.ToInt32(hex.ToString(), 16);
    }
}