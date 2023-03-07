using System.Collections;
using System.Collections.Generic;
using System.Net.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SelectRoomUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] Transform roomScroll;
    [SerializeField] GameObject roomObject;
    [SerializeField] Transform chatScroll;
    [SerializeField] GameObject chatObject;

    public void Start()
    {
        chatInput.onSubmit.AddListener(OnSubmitChat);
    }

    public void OnEnable()
    {
        NetworkManager.Instance.onPacketArrive.AddListener(OnChatPacketArrive);
        NetworkManager.Instance.onPacketArrive.AddListener(OnRoomListPacketArrive);
        RenewRoomList();
        ClearChat();
    }

    public void OnDisable()
    {
        NetworkManager.Instance.onPacketArrive.RemoveListener(OnChatPacketArrive);
        NetworkManager.Instance.onPacketArrive.RemoveListener(OnRoomListPacketArrive);
    }

    public void OnChatPacketArrive(TcpPacket packet)
    {
        if (packet.Order == (int)TcpPacketType.Chat)
        {
            JsonTextParser parser = new JsonTextParser();
            var msgJson = (JsonObjectCollection)parser.Parse(packet.Msg);

            var sender = (string)msgJson["sender"].GetValue();
            var msg = (string)msgJson["msg"].GetValue();

            Instantiate(chatObject, chatScroll).GetComponent<TMP_Text>().text = $"{sender} : {msg}";
        }
    }

    public void OnRoomListPacketArrive(TcpPacket packet)
    {
        if (packet.Order == (int)TcpPacketType.GetRoomList)
        {
            JsonTextParser parser = new JsonTextParser();
            var msgJson = (JsonObjectCollection)parser.Parse(packet.Msg);

            var name = (string)msgJson["name"].GetValue();
            var isPrivate = (bool)msgJson["isPrivate"].GetValue();
            var maxPlayer = (int)msgJson["maxPlayer"].GetValue();
            var curPlayer = (int)msgJson["curPlayer"].GetValue();
            var nickname = (string)msgJson["nickname"].GetValue();

            Instantiate(roomObject, roomScroll).GetComponent<RoomBtnManager>().SetInfo(
                "0", name, "0", $"{curPlayer} / {maxPlayer}", nickname, isPrivate.ToString(), "0"
            );
        }
    }

    public void RenewRoomList()
    {
        var children = roomScroll.GetComponentsInChildren<Transform>();
        for (int i = 0; i < roomScroll.childCount; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }

        var packet = new TcpPacket(TcpPacketType.GetRoomList, "");
        NetworkManager.Instance.SendPacket(packet);
    }

    public void ClearChat()
    {
        var children = chatScroll.GetComponentsInChildren<Transform>();
        for (int i = 0; i < chatScroll.childCount; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }
    }

    public void OnSubmitChat(string msg)
    {
        chatInput.text = "";
        JsonObjectCollection jsonObj = new JsonObjectCollection();
        jsonObj.Add(new JsonStringValue("msg", msg));
        var packet = new TcpPacket(TcpPacketType.Chat, jsonObj.ToString());

        NetworkManager.Instance.SendPacket(packet);
    }
}
