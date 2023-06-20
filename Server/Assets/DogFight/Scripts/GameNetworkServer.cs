using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using DogFightCommon.UDPpacket;
using DogFightCommon.Util;
using System.Collections.Generic;
using System.Linq;

public class GameNetworkServer : MonoBehaviour
{
    UdpClient multi = new UdpClient();
    UdpClient srv;

    // Start is called before the first frame update
    async void Start()
    {
        Application.targetFrameRate = 60;
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
        multi.DontFragment = true;
        multi.Send(buffer, buffer.Length, multicastEP);
    }

    void Update()
    {
        List<Transform> transforms = new List<Transform>();

        for(int i = 0; i < transform.childCount; i++)
        {
            transforms.Add(transform.GetChild(i).transform);
        }

        CutPacket packet = new CutPacket(transforms);
        SendMulticast(ByteUtil.ObjectToByteArray(packet));
    }
}
