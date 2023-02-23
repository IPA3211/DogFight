using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public delegate void AnswerCallback(TcpPacket packet);
public delegate void TimeoutCallBack();
public class NetworkRequest
{
    TcpPacket packet;
    AnswerCallback answerCallback;
    TimeoutCallBack timeoutCallBack;
    float duration;

    public TcpPacket Packet => packet;

    public NetworkRequest(TcpPacket pack, AnswerCallback ans, TimeoutCallBack time, float dur)
    {
        packet = pack;
        answerCallback = ans;
        timeoutCallBack = time;
        duration = dur;
    }

    public bool UpdateDuration(float delta)
    {
        duration -= delta;

        if (duration < 0)
        {
            return true;
        }
        return false;
    }

    public void OnAnswerArrive(TcpPacket packet)
    {
        answerCallback.Invoke(packet);
    }

    public void OnRemove()
    {
        timeoutCallBack?.Invoke();
    }
}
public class NetworkManager : MonoBehaviour
{
    static NetworkManager instance;
    TcpClient tcpClient;
    IPAddress iPAddress;
    NetworkStream stream;
    Dictionary<int, NetworkRequest> requestDict = new();

    public bool IsConnected => tcpClient.Connected;
    public static NetworkManager Instance => instance;

    void Awake()
    {
        instance = this;
    }

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

            await RecvPacket();

            Debug.Log("연결 종료");


            // (5) 스트림과 TcpClient 객체 닫기
            stream.Close();
            tcpClient.Close();
        }
        catch (SocketException)
        {
            Debug.Log("서버와 연결 할 수 없습니다.");
        }
    }

    public void FixedUpdate()
    {
        foreach (var item in requestDict.Values)
        {
            if (item.UpdateDuration(Time.fixedDeltaTime))
            {
                requestDict.Remove(item.Packet.Index);
            }
        }
    }

    public void SendPacket(TcpPacket packet, float duration = 0, AnswerCallback callback = null, TimeoutCallBack timeOutCallBack = null)
    {
        if (IsConnected)
        {
            var json = JsonUtility.ToJson(packet);
            byte[] buff = Encoding.UTF8.GetBytes(json);
            if (duration > 0)
            {
                NetworkRequest request = new NetworkRequest(packet, callback, timeOutCallBack, duration);
                requestDict.Add(packet.Index, request);
            }
            stream?.WriteAsync(buff, 0, buff.Length);
        }
        else
        {
            Debug.Log("서버와 연결 할 수 없습니다.");
        }
    }

    public async Task RecvPacket()
    {
        int nbytes;
        byte[] outbuf = new byte[1024];
        MemoryStream mem = new MemoryStream();
        List<Byte> ans = new();

        while (true)
        {
            nbytes = await stream.ReadAsync(outbuf, 0, outbuf.Length);

            if (nbytes <= 0)
            {
                break;
            }

            ans = ans.Concat(outbuf).ToList();
            if (nbytes < outbuf.Length)
            {
                var str = Encoding.UTF8.GetString(ans.ToArray());
                var packet = JsonUtility.FromJson<TcpPacket>(Encoding.UTF8.GetString(ans.ToArray()));
            }


            Array.Clear(outbuf, 0, outbuf.Length);
        }

        mem.Close();
    }
}
