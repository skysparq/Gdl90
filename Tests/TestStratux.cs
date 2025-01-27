using System.Net;
using System.Net.Sockets;
using Gdl90;

namespace Tests;

public class TestStratux
{
    private Gdl90Manager _gdl90;
    private int _0x65messagesReceived = 0;
    private int _0x4cMessagesReceived = 0;
    private int _otherMessagesReceived = 0;

    [Test]
    public void TestStratuxUdp()
    {
        
        _gdl90 = new Gdl90Manager();
        _gdl90.NewMessage += (_, msg) =>
        {
            if (msg[0] == 0x65)
            {
                _0x65messagesReceived++;
            } else if (msg[0] == 0x4c)
            {
                _0x4cMessagesReceived++;
            }
            else
            {
                _otherMessagesReceived++;
            }
        };
        var socket = new UdpClient(4000);
        socket.BeginReceive(OnUdpData, socket);
        Thread.Sleep(30000);
        Console.WriteLine($"0x65={_0x65messagesReceived}, 0x4c={_0x4cMessagesReceived}, other={_otherMessagesReceived}");
    }

    private void OnUdpData(IAsyncResult result)
    {
        UdpClient socket = result.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0,0);
        byte[] message = socket.EndReceive(result, ref source);
        _gdl90.Process(message);
        socket.BeginReceive(OnUdpData, socket);
    }
}