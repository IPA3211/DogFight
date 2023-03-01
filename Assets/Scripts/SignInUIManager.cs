using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.Json;
using System;

public class SignInUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField pwInput;
    [SerializeField] Toggle saveToggle;

    public void OnEnable()
    {
        SetDefault();
    }

    public void SetDefault()
    {
        idInput.text = PlayerPrefs.GetString("id", "");
        pwInput.text = PlayerPrefs.GetString("pw", "");
    }

    public async void OnSignInBtnClick()
    {
        TaskCompletionSource<TcpPacket> tcs = new TaskCompletionSource<TcpPacket>();
        JsonObjectCollection jsonObj = new JsonObjectCollection();

        var sha256Pass = Encryptor.EncryptionSHA256(pwInput.text);

        jsonObj.Add(new JsonStringValue("id", idInput.text));
        jsonObj.Add(new JsonStringValue("pw", sha256Pass));

        var packet = new TcpPacket(TcpPacketType.SignIn, jsonObj.ToString());
        NetworkManager.Instance.SendPacket(packet, tcs, 1000);

        try
        {
            var ans = await tcs.Task;
            JsonTextParser parser = new JsonTextParser();
            var msgJson = (JsonObjectCollection)parser.Parse(ans.Msg);
            if (Convert.ToInt16(msgJson["result"].GetValue()) != -1)
            {
                PlayerPrefs.SetString("id", idInput.text);
                PlayerPrefs.SetString("pw", pwInput.text);
                PlayerPrefs.Save();
                
                Debug.Log("로그인 성공");
            }
            else
            {
                Debug.Log("로그인 실패");
            }
        }
        catch (TcpTimeOutException)
        {
            Debug.Log("LogIn TimeOut");
        }
    }

    public void OnShutdownBtnClick()
    {
        NetworkManager.Instance.StopConnection();
        Application.Quit();
    }
}
