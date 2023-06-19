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

    // Start is called before the first frame update
    void Start()
    {

        UdpClient cli = new UdpClient();

        string msg = "æ»≥Á«œººø‰";
        byte[] datagram = Encoding.UTF8.GetBytes(msg);

        cli.Send(datagram, datagram.Length, "127.0.0.1", 7777);

        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 5500);
        multi.Client.Bind(localEp);

        IPAddress multicastIP = IPAddress.Parse("229.1.1.229");
        multi.JoinMulticastGroup(multicastIP);


        // (4) UdpClient ∞¥√º ¥›±‚
        cli.Close();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        byte[] bytes = multi.Receive(ref epRemote);

        CutPacket t = (CutPacket)ByteUtil.ByteArrayToObject(bytes);
        
        if(lastPacket < t.getPacketTime())
        {
            lastPacket = t.getPacketTime();
            t.GetTransforms[0].OverWriteTransform(transform);
            Debug.Log(transform.position);
        }
    }
}
