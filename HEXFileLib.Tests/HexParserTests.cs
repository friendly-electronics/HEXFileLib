namespace FriendlyElectronics.HexFileLib.Tests;

[TestClass]
public class HexParserTests
{
    [DataTestMethod]
    // Empty record line:
    [DataRow("", HexRecordType.Undefined, 0, "", "")]
    [DataRow("  ", HexRecordType.Undefined, 0, "", "  ")]
    
    // EndOfFile Record:
    [DataRow(":00000001FF", HexRecordType.EndOfFile, 0, "", "")]
    [DataRow(":0000FF0100", HexRecordType.EndOfFile, 255, "", "")]
    [DataRow(":00FF000100", HexRecordType.EndOfFile, 255, "", "")]
    [DataRow(":01000001FFFF", HexRecordType.EndOfFile, 0, "FF", "")]
    
    // Comments:
    [DataRow("COMMENT:0000FF0100", HexRecordType.EndOfFile, 0, "", "COMMENT", "")]
    [DataRow(":0000FF0100;COMMENT", HexRecordType.EndOfFile, 0, "", "", ";COMMENT")]
    
    public void TestDataRecord(string line, HexRecordType type, int address, string hexData = "", string comment = "", string comment2 = "")
    {
        var data = HexStringToByteArray(hexData);
        
        var record = HexParser.ParseLine(line);
        Assert.AreEqual(type, record.Type);
        CollectionAssert.AreEqual(data, record.Data);
        Assert.AreEqual(comment, record.Comment);
    }

    private static byte[] HexStringToByteArray(string hex) {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }
}