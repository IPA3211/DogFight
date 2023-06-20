using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DogFightCommon.UDPpacket;
using DogFightCommon.Util;

public class GameNetworkManager : MonoBehaviour
{
    UdpClient multi = new UdpClient();
    IPEndPoint epRemote = new IPEndPoint(IPAddress.Any, 0);

    long lastPacket = long.MinValue;
    public int packetArriveCount = 0;
    public int frameCount = 0;

    Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        UdpClient cli = new UdpClient();

        string msg = "æ»≥Á«œººø‰";
        byte[] datagram = Encoding.UTF8.GetBytes(msg);

        cli.Send(datagram, datagram.Length, "127.0.0.1", 7777);
        StartMulticastRecevier();

        // (4) UdpClient ∞¥√º ¥›±‚
        cli.Close();
    }

    async void StartMulticastRecevier()
    {
        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 5500);

        IPAddress multicastIP = IPAddress.Parse("229.1.1.229");

        multi.Client.Bind(localEp);

        multi.JoinMulticastGroup(multicastIP);

        while (true)
        {
            var result = await multi.ReceiveAsync();
            OnMulticastReceive(result);
        }
    }

    void OnMulticastReceive(UdpReceiveResult res)
    {
        CutPacket t = (CutPacket)ByteUtil.ByteArrayToObject(res.Buffer);

        if (lastPacket < t.getPacketTime())
        {
            lastPacket = t.getPacketTime();
            t.GetTransforms[0].OverWriteTransform(transform);
        }
        packetArriveCount++;
    }

    private void Update()
    {
        if (lastPos == transform.position)
        {
        }

        lastPos = transform.position;
    }
}
