using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class NetworkManager : MonoBehaviour
{
    TcpClient tcpClient;
    IPAddress iPAddress;
    NetworkStream stream;

    // Start is called before the first frame update
    async void Start()
    {
        tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("192.168.56.101", 7000);

        string msg = "Hello Server";
        byte[] buff = Encoding.ASCII.GetBytes(msg);
        // (2) NetworkStream을 얻어옴 
        stream = tcpClient.GetStream();

        // (3) 스트림에 바이트 데이타 전송
        await stream.WriteAsync(buff, 0, buff.Length);

        // (4) 서버가 Connection을 닫을 때가지 읽는 경우
        byte[] outbuf = new byte[1024];
        int nbytes;
        MemoryStream mem = new MemoryStream();
        while ((nbytes = await stream.ReadAsync(outbuf, 0, outbuf.Length)) > 0)
        {
            Debug.Log(Encoding.ASCII.GetString(outbuf));
            Array.Clear(outbuf, 0, outbuf.Length);
        }
        
        mem.Close();

        // (5) 스트림과 TcpClient 객체 닫기
        stream.Close();
        tcpClient.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
