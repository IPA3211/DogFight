#define SEND_PACKET_WITH_BYTE

using PcgPacket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;
using WebSocketSharp.Server;

/* 수신, 송신 처리 */

public class WsGameService : WebSocketBehavior
{
    NetworkManager networkManager;
    public void SetNetworkManager(NetworkManager inNetworkManager)
    {
        networkManager = inNetworkManager;
    }

    public void SendTo(object data, string id)
    {
#if(SEND_PACKET_WITH_BYTE)
        var json = ByteUtil.ObjectToByteArray(data);
        Debug.Log($"SEND TO {id} : {json.Length} Byte");
#else
        var json = JsonUtility.ToJson(data);
        Debug.Log($"SEND TO {id} : {System.Text.Encoding.Default.GetBytes(json).Length} Byte : {json.Substring(0, Math.Min(100, json.Length))}");
#endif

        Sessions.SendToAsync(json, id, (b) => { });
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        Debug.Log("Hello " + ID);
        MainThreadInvoker.Instance.Enqueue(() =>
        {
            networkManager.onOpen.Invoke(ID, null);
        });
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        MainThreadInvoker.Instance.Enqueue(() =>
        {
            networkManager.onMessage.Invoke(ID, e);
        });
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Debug.LogError(e.Exception);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        base.OnClose(e);

        Debug.Log("Bye " + ID);
        MainThreadInvoker.Instance.Enqueue(() => { networkManager.onClose.Invoke(ID, e); });

    }
}

public class NetworkManager : MonoBehaviour
{
    WebSocketServer wssv;
    WsGameService webSocketService;

    Dictionary<int, NetworkRequest> requestDict = new();

    public UnityEvent<string, object> onOpen;
    public UnityEvent<string, object> onClose;
    public UnityEvent<string, object> onMessage;
    public UnityEvent<string, WebSocketPacket> onPacketArrive;

    // Start is called before the first frame update
    void Start()
    {
        wssv = new WebSocketServer(System.Net.IPAddress.Any, 7000);
        Action<WsGameService> SetupService = AddBehaviorHandler;

        wssv.AddWebSocketService("/Game", SetupService);

        onMessage.AddListener(OnMessage);

        wssv.Start();
        Debug.Log("Server Open!");
    }

    private void AddBehaviorHandler(WsGameService wsBehavior)
    {
        webSocketService = wsBehavior;
        wsBehavior.SetNetworkManager(this);
    }

    public void SendPacket(WebSocketPacket packet, string id, TaskCompletionSource<WebSocketPacket> tsc = null, float duration = 0)
    {
        if (tsc != null)
        {
            NetworkRequest request = new NetworkRequest(packet, tsc, duration);
            requestDict.Add(packet.Index, request);
        }

        webSocketService.SendTo(packet, id);
    }

    public void OnMessage(string id, object data)
    {
        var e = data as MessageEventArgs;
#if (SEND_PACKET_WITH_BYTE)
        var packet = ByteUtil.ByteArrayToObject(e.RawData) as WebSocketPacket;
        Debug.Log($"ARRIVE FROM {id} : {e.RawData.Length} Byte");
#else
        Debug.Log($"ARRIVE FROM {id} : {System.Text.Encoding.Default.GetBytes(e.Data).Length} Byte : {e.Data.Substring(0, Math.Min(100, e.Data.Length))}");

        var packet_raw = JsonUtility.FromJson<WebSocketPacket>(e.Data);
        Type t = GetType("PcgPacket." + packet_raw.Order);
        var packet = (WebSocketPacket)JsonUtility.FromJson(e.Data, t);
#endif

        if (packet.IsAns)
        {
            if (requestDict.ContainsKey(packet.AnsTo))
            {
                requestDict[packet.AnsTo].OnAnswerArrive(packet);
            }

            return;
        }

        onPacketArrive.Invoke(id, packet);
    }

    private void OnDestroy()
    {
        wssv?.Stop();
    }

    public static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = a.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }
}
