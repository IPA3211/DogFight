using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DogFightCommon.UDPpacket;
using DogFightCommon.Util;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

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

        string msg = "¾È³çÇÏ¼¼¿ä";
        byte[] datagram = Encoding.UTF8.GetBytes(msg);

        cli.Send(datagram, datagram.Length, "127.0.0.1", 7777);
        Thread t1 = new Thread(StartMulticastRecevieThread);

        t1.Start();

        // (4) UdpClient °´Ã¼ ´Ý±â
        cli.Close();
    }

    async void StartMulticastRecevieThread()
    {
        try
        {
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 5500);
            IPAddress multicastIP = IPAddress.Parse("229.1.1.229");

            multi.Client.Bind(localEp);
            multi.JoinMulticastGroup(multicastIP);

            while (true)
            {
                var result = await multi.ReceiveAsync();
                MainThreadInvoker.Instance.Enqueue(() => OnMulticastReceive(result));
            }
        }
        catch(System.Exception e)
        {
            MainThreadInvoker.Instance.Enqueue(() => throw e);
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
