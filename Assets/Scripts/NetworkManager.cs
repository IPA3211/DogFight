using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

public delegate void AnswerCallback(TcpPacket packet);
public delegate void TimeoutCallBack();

public class NetworkRequest
{
    TcpPacket packet;
    TaskCompletionSource<TcpPacket> tsc;
    float duration;

    public TcpPacket Packet => packet;

    public NetworkRequest(TcpPacket pack, TaskCompletionSource<TcpPacket> inTsc, float dur)
    {
        packet = pack;
        tsc = inTsc;
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
        tsc.SetResult(packet);
    }

    public void OnRemove()
    {
        TimeoutException e = new TimeoutException();
        tsc.SetException(e);
    }
}
public class NetworkManager : MonoBehaviour
{
    static NetworkManager instance;
    TcpClient tcpClient;
    IPAddress iPAddress;
    SslStream stream;
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
        await StartConnection();
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
    public static bool ValidateServerCertificate(
                  object sender,
                  X509Certificate certificate,
                  X509Chain chain,
                  SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        Debug.Log(sslPolicyErrors);

        // Do not allow this client to communicate with unauthenticated servers.
        return true;
    }
    public async Task StartConnection()
    {
        if (tcpClient != null && tcpClient.Connected)
        {
            stream.Close();
            tcpClient.Close();

            tcpClient = null;
            stream = null;
        }


        try
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("192.168.56.101", 7000);
            Debug.Log("연결 성공");

            stream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            stream.AuthenticateAsClient("dogfight.com");

            await RecvPacket();

            Debug.Log("연결 종료");

            stream.Close();
            tcpClient.Close();
        }
        catch (SocketException)
        {
            Debug.Log("서버와 연결 할 수 없습니다.");
        }
        catch (AuthenticationException e)
        {
            Debug.Log("서버와 SSL 연결에 실패했습니다.");
            Debug.LogErrorFormat("Error authenticating: {0}", e.Message);
            if (e.InnerException != null)
            {
                Debug.LogErrorFormat("Inner exception: {0}", e.InnerException.Message);
            }
        }
    }

    public void StopConnection()
    {
        if (tcpClient.Connected)
        {
            stream.Close();
            tcpClient.Close();
        }
    }

    public void SendPacket(TcpPacket packet, TaskCompletionSource<TcpPacket> tsc, float duration = 0)
    {
        if (IsConnected)
        {
            var json = JsonUtility.ToJson(packet);
            byte[] buff = Encoding.UTF8.GetBytes(json);
            if (duration > 0)
            {
                NetworkRequest request = new NetworkRequest(packet, tsc, duration);
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

                Debug.Log(str);
                switch ((TcpPacketType)packet.Order)
                {
                    case TcpPacketType.Answer:
                        if (requestDict.ContainsKey(packet.Index))
                        {
                            requestDict[packet.Index].OnAnswerArrive(packet);
                        }
                        break;
                    case TcpPacketType.Msg:
                        Debug.Log(packet.Msg);
                        break;
                }
            }

            Array.Clear(outbuf, 0, outbuf.Length);
            ans.Clear();
        }

        mem.Close();
    }
}
