using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameNetworkServer : MonoBehaviour
{
    UdpClient srv = new UdpClient(7777);
    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

    // Start is called before the first frame update
    void Start()
    {
        while (true)
        {
            byte[] dgram = srv.Receive(ref remoteEP);
            Console.WriteLine("[Receive] {0} 로부터 {1} 바이트 수신", remoteEP.ToString(), dgram.Length);

            srv.Send(dgram, dgram.Length);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
