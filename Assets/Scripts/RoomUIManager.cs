using System.Collections;
using System.Collections.Generic;
using System.Net.Json;
using TMPro;
using UnityEngine;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] Transform chatScroll;
    [SerializeField] GameObject chatObject;

    public void Start()
    {
        chatInput.onSubmit.AddListener(OnSubmitChat);
    }
    
    public void OnEnable()
    {
        NetworkManager.Instance.onPacketArrive.AddListener(OnChatPacketArrive);
    }
    public void OnDisable()
    {
        NetworkManager.Instance.onPacketArrive.RemoveListener(OnChatPacketArrive);
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

    public void OnSubmitChat(string msg)
    {
        chatInput.text = "";
        JsonObjectCollection jsonObj = new JsonObjectCollection();
        jsonObj.Add(new JsonStringValue("msg", msg));
        var packet = new TcpPacket(TcpPacketType.Chat, jsonObj.ToString());

        NetworkManager.Instance.SendPacket(packet);
    }
}
