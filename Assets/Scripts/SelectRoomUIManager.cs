using System.Collections;
using System.Collections.Generic;
using System.Net.Json;
using TMPro;
using UnityEngine;

public class SelectRoomUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] Transform chatScroll;
    [SerializeField] GameObject chatObject;

    public void OnEnable()
    {
        NetworkManager.Instance.chatEvent.AddListener(setChat);
        chatInput.onSubmit.AddListener(OnSubmitChat);
    }
    public void setChat(TcpPacket packet)
    {
        JsonTextParser parser = new JsonTextParser();
        var msgJson = (JsonObjectCollection)parser.Parse(packet.Msg);

        var sender = (string)msgJson["result"].GetValue();
        var msg = (string)msgJson["msg"].GetValue();

        Instantiate(chatObject, chatScroll).GetComponent<TMP_Text>().text = $"{sender} : {msg}";
    }

    public void OnSubmitChat(string msg)
    {
        JsonObjectCollection jsonObj = new JsonObjectCollection();
        jsonObj.Add(new JsonStringValue("msg", msg));
        var packet = new TcpPacket(TcpPacketType.Chat, jsonObj.ToString());
        
        NetworkManager.Instance.SendPacket(packet);
    }
}
