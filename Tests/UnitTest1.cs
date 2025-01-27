using System.Buffers.Binary;
using Gdl90;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestComputeCrc()
    {
        var gdl90 = new Gdl90Manager();
        var message = new byte[] { 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02 };
        ushort expected = BinaryPrimitives.ReadUInt16LittleEndian([0xB3, 0x8B]);
        var actual = gdl90.ComputeCrc(message);
        Assert.That(actual, Is.EqualTo(expected));
        var crc = gdl90.ComputeCrc([0x80, 0x7D, 0x7E, 0x80]);
        var crcBuffer = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(crcBuffer, crc);
        Console.WriteLine("[{0}]", string.Join(", ", crcBuffer));
    }
    
    [Test]
    public void TestParseHeartbeat()
    {
        var heartbeat = new byte[]{0x7E, 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02, 0xB3, 0x8B, 0x7E};
        var actual = new List<byte[]>();
        var gdl90 = new Gdl90Manager();
        gdl90.NewMessage += (_, msg) =>
        {
            actual.Add(msg);
        };
        gdl90.Process(heartbeat);
        var expected = new byte[] { 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02 };
        Assert.That(actual.Count, Is.EqualTo(1));
        Assert.That(actual[0], Is.EqualTo(expected));
    }

    [Test]
    public void TestParseHeartbeatAcrossTwoPayloads()
    {
        var payload1 = new byte[]{0x7E, 0x00, 0x81, 0x41, 0xDB};
        var payload2 = new byte[]{0xD0, 0x08, 0x02, 0xB3, 0x8B, 0x7E};
        var actual = new List<byte[]>();
        var gdl90 = new Gdl90Manager();
        gdl90.NewMessage += (_, msg) =>
        {
            actual.Add(msg);
        };
        gdl90.Process(payload1);
        Assert.That(actual.Count, Is.EqualTo(0));
        gdl90.Process(payload2);
        var expected = new byte[] { 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02 };
        Assert.That(actual.Count, Is.EqualTo(1));
        Assert.That(actual[0], Is.EqualTo(expected));
    }
    
    [Test]
    public void TestParseHeartbeatInMiddle()
    {
        var payload = new byte[]{0x00, 0x81, 0x41, 0xDB, 0x7E, 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02, 0xB3, 0x8B, 0x7E, 0xB3, 0x8B};
        var actual = new List<byte[]>();
        var gdl90 = new Gdl90Manager();
        gdl90.NewMessage += (_, msg) =>
        {
            actual.Add(msg);
        };
        gdl90.Process(payload);
        var expected = new byte[] { 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02 };
        Assert.That(actual.Count, Is.EqualTo(1));
        Assert.That(actual[0], Is.EqualTo(expected));
    }
    
    [Test]
    public void TestParseWithEscapes()
    {
        var payload = new byte[]{0x7E, 0x80, 0x7D, 0x5D, 0x7D, 0x5E, 0x80, 0x22, 0xCA, 0x7E};
        var actual = new List<byte[]>();
        var gdl90 = new Gdl90Manager();
        gdl90.NewMessage += (_, msg) =>
        {
            actual.Add(msg);
        };
        gdl90.Process(payload);
        var expected = new byte[] { 0x80, 0x7D, 0x7E, 0x80 };
        Assert.That(actual.Count, Is.EqualTo(1));
        Assert.That(actual[0], Is.EqualTo(expected));
    }

    [Test]
    public void TestHeartbeatWithInvalidCrc()
    {
        var heartbeat = new byte[]{0x7E, 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02, 0x00, 0x00, 0x7E};
        var actual = new List<byte[]>();
        var gdl90 = new Gdl90Manager();
        gdl90.NewMessage += (_, msg) =>
        {
            actual.Add(msg);
        };
        gdl90.Process(heartbeat);
        Assert.That(actual.Count, Is.EqualTo(0));
    }
}