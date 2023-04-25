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
    [SerializeField] float renewLoopTime;
    float sumTime;
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

    void Update()
    {
        sumTime += Time.deltaTime;
        if (renewLoopTime < sumTime)
        {
            RenewRoomList();
            sumTime = 0;
        }
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

            var sender = (msgJson["sender"] as JsonStringValue).Value;
            var msg = (msgJson["msg"] as JsonStringValue).Value;

            Instantiate(chatObject, chatScroll).GetComponent<TMP_Text>().text = $"{sender} : {msg}";
        }
    }

    public void OnRoomListPacketArrive(TcpPacket packet)
    {
        if (packet.Order == (int)TcpPacketType.GetRoomList)
        {
            JsonTextParser parser = new JsonTextParser();
            var msgJson = (JsonObjectCollection)parser.Parse(packet.Msg);

            var name = (msgJson["name"] as JsonStringValue).Value;
            var isPrivate = (msgJson["isPrivate"] as JsonBooleanValue).Value;
            var maxPlayer = (msgJson["maxPlayer"] as JsonNumericValue).Value;
            var curPlayer = (msgJson["curPlayer"] as JsonNumericValue).Value;
            var nickname = (msgJson["nickname"] as JsonStringValue).Value;

            Instantiate(roomObject, roomScroll).GetComponent<RoomBtnManager>().SetInfo(
                "0", name, "0", $"{curPlayer} / {maxPlayer}", nickname, isPrivate.ToString(), "0"
            );
        }
    }

    public void RenewRoomList()
    {
        foreach (Transform child in roomScroll.transform)
        {
            Destroy(child.gameObject);
        }

        var packet = new TcpPacket(TcpPacketType.GetRoomList, "{}");
        NetworkManager.Instance.SendPacket(packet);
    }

    public void ClearChat()
    {
        foreach (Transform child in chatScroll.transform)
        {
            Destroy(child.gameObject);
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
