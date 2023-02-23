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
        try
        {
            await tcpClient.ConnectAsync("192.168.56.101", 7000);
            Debug.Log("연결 성공");
            string msg = "Hello Server";
            byte[] buff = Encoding.ASCII.GetBytes(msg);
            stream = tcpClient.GetStream();

            await stream.WriteAsync(buff, 0, buff.Length);

            byte[] outbuf = new byte[1024];
            int nbytes;
            MemoryStream mem = new MemoryStream();
            while ((nbytes = await stream.ReadAsync(outbuf, 0, outbuf.Length)) > 0)
            {
                Debug.Log(nbytes);
                Debug.Log(Encoding.UTF8.GetString(outbuf));
                Array.Clear(outbuf, 0, outbuf.Length);
            }
            
            Debug.Log("연결 종료");

            mem.Close();

            // (5) 스트림과 TcpClient 객체 닫기
            stream.Close();
            tcpClient.Close();
        }
        catch (SocketException)
        {
            Debug.Log("서버와 연결 할 수 없습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
