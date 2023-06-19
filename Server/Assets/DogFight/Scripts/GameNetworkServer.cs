using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using DogFightCommon.UDPpacket;
using DogFightCommon.Util;

public class GameNetworkServer : MonoBehaviour
{
    UdpClient multi = new UdpClient();
    UdpClient srv;

    // Start is called before the first frame update
    async void Start()
    {
        srv = new UdpClient(7777);
        while (true)
        {
            var packet = await srv.ReceiveAsync();
            OnReceiveUdpPacket(packet);
        }
    }

    void OnReceiveUdpPacket(UdpReceiveResult packet)
    {
        Debug.Log(Encoding.UTF8.GetString(packet.Buffer));
    }

    void SendUdpPacket(byte[] buffer, IPEndPoint endPoint)
    {
        srv.Send(buffer, buffer.Length, endPoint);
    }

    void SendMulticast(byte[] buffer)
    {
        IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("229.1.1.229"), 5500);
        multi.Send(buffer, buffer.Length, multicastEP);
    }

    void Update()
    {
        SerializableTransform transform = new SerializableTransform(gameObject.transform);
        SendMulticast(ByteUtil.ObjectToByteArray(transform));
    }
}
