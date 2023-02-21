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

    // Start is called before the first frame update
    void Start()
    {
        TcpClient tc = new TcpClient("192.168.56.101", 7000);
        string msg = "Hello Server";
        byte[] buff = Encoding.ASCII.GetBytes(msg);

        // (2) NetworkStream을 얻어옴 
        NetworkStream stream = tc.GetStream();

        // (3) 스트림에 바이트 데이타 전송
        stream.Write(buff, 0, buff.Length);

        // (4) 서버가 Connection을 닫을 때가지 읽는 경우
        byte[] outbuf = new byte[1024];
        int nbytes;
        MemoryStream mem = new MemoryStream();
        while ((nbytes = stream.Read(outbuf, 0, outbuf.Length)) > 0)
        {
            mem.Write(outbuf, 0, nbytes);
        }
        byte[] outbytes = mem.ToArray();
        mem.Close();

        // (5) 스트림과 TcpClient 객체 닫기
        stream.Close();
        tc.Close();

        Debug.Log(Encoding.ASCII.GetString(outbytes));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
