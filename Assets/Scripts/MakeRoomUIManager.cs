using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Net.Json;
using System;

public class MakeRoomUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField pwInput;
    [SerializeField] Toggle privateToggle;
    [SerializeField] TMP_Dropdown capaDrop;
    [SerializeField] GameObject warningText;
    bool isPocessing;

    void OnEnable()
    {
        idInput.text = "";
        pwInput.text = "";
        privateToggle.isOn = false;
        warningText.SetActive(false);
        capaDrop.value = 0;
        pwInput.interactable = false;
    }

    void Start()
    {
        capaDrop.onValueChanged.AddListener(OnCapaValueChange);
        privateToggle.onValueChanged.AddListener(OnPrivateValueChange);
    }

    public async void OnConfirmClick()
    {
        if (!isPocessing)
        {
            isPocessing = true;
            TaskCompletionSource<TcpPacket> tcs = new TaskCompletionSource<TcpPacket>();
            JsonObjectCollection jsonObj = new JsonObjectCollection();

            var sha256Pass = Encryptor.EncryptionSHA256(pwInput.text);

            jsonObj.Add(new JsonStringValue("id", idInput.text));
            jsonObj.Add(new JsonStringValue("pw", sha256Pass));

            var packet = new TcpPacket(TcpPacketType.SignUp, jsonObj.ToString());
            NetworkManager.Instance.SendPacket(packet, tcs, 1000);

            try
            {
                var ans = await tcs.Task;
                JsonTextParser parser = new JsonTextParser();
                var msgJson = (JsonObjectCollection)parser.Parse(ans.Msg);
                if (Convert.ToInt16(msgJson["result"].GetValue()) != -1)
                {
                    gameObject.SetActive(false);
                    GetComponentInParent<PreparationUIManager>().ShowRoomUI();
                }
                else
                {
                    warningText.SetActive(true);
                }
            }
            catch (TcpTimeOutException)
            {
                warningText.SetActive(true);
                Debug.Log("make room TimeOut");
            }
            isPocessing = false;
        }
    }

    void OnCapaValueChange(int value)
    {

    }

    void OnPrivateValueChange(bool value)
    {
        pwInput.interactable = value;
    }
}
