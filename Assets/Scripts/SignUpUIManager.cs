using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Json;
using System.Threading.Tasks;

public class SignUpUIManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField pwInput;
    [SerializeField] TMP_InputField pwConfirmInput;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField nickInput;
    [Header("Images")]
    [SerializeField] Image idCheck;
    [SerializeField] Image pwCheck;
    [SerializeField] Image emailCheck;
    [SerializeField] Image nickCheck;
    [Header("Sprite")]
    [SerializeField] Sprite checkSprite;
    [SerializeField] Sprite warnSprite;

    public void OnEnable()
    {
        SetDefault();
    }

    public void SetDefault()
    {
        idInput.text = "";
        pwInput.text = "";
        pwConfirmInput.text = "";
        emailInput.text = "";
        nickInput.text = "";

        idCheck.enabled = false;
        pwCheck.enabled = false;
        emailCheck.enabled = false;
        nickCheck.enabled = false;

        idCheck.sprite = warnSprite;
        pwCheck.sprite = warnSprite;
        emailCheck.sprite = warnSprite;
        nickCheck.sprite = warnSprite;
    }

    void SendDuplicationCheckPacket(string table, int column, string check, TaskCompletionSource<TcpPacket> tcs)
    {
        JsonObjectCollection jsonObj = new JsonObjectCollection();
        jsonObj.Add(new JsonStringValue("table", table));
        jsonObj.Add(new JsonNumericValue("column", column));
        jsonObj.Add(new JsonStringValue("check", check));

        var packet = new TcpPacket(TcpPacketType.DuplicationCheck, jsonObj.ToString());
        NetworkManager.Instance.SendPacket(packet, tcs, 1000);
    }

    public async void CheckIdAsync(string id)
    {
        TaskCompletionSource<TcpPacket> tcs = new TaskCompletionSource<TcpPacket>();
        SendDuplicationCheckPacket("user", (int)UserTableColumn.UserId, id, tcs);
        try
        {
            var ans = await tcs.Task;
            JsonTextParser parser = new JsonTextParser();
            Debug.Log(ans.Msg);
            var msgJson = (JsonObjectCollection)parser.Parse(ans.Msg);
            if (Convert.ToInt16(msgJson["result"].GetValue()) == 1)
            {
                idCheck.enabled = true;
                idCheck.sprite = checkSprite;
            }
            else
            {
                idCheck.enabled = true;
                idCheck.sprite = warnSprite;
            }
        }
        catch (TcpTimeOutException)
        {
            Debug.Log("id TimeOut");
        }
    }

    public async void CheckNickNameAsync(string nickName)
    {
        TaskCompletionSource<TcpPacket> tcs = new TaskCompletionSource<TcpPacket>();
        SendDuplicationCheckPacket("user", (int)UserTableColumn.NickName, nickName, tcs);
        try
        {
            var ans = await tcs.Task;
            JsonTextParser parser = new JsonTextParser();
            var msgJson = (JsonObjectCollection)parser.Parse(ans.Msg);
            if (Convert.ToInt16(msgJson["result"].GetValue()) == 1)
            {
                nickCheck.enabled = true;
                nickCheck.sprite = checkSprite;
            }
            else
            {
                nickCheck.enabled = true;
                nickCheck.sprite = warnSprite;
            }
        }
        catch (TcpTimeOutException)
        {
            Debug.Log("nick TimeOut");
        }
    }

    public void CheckPassword(string confimePass)
    {
        if (pwInput.text == "")
        {
            return;
        }
        pwCheck.enabled = true;

        if (pwInput.text == pwConfirmInput.text)
        {
            pwCheck.sprite = checkSprite;
        }
        else
        {
            pwCheck.sprite = warnSprite;
        }
    }

    public async void CheckEmailAsync(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");

        if (valid)
        {
            TaskCompletionSource<TcpPacket> tcs = new TaskCompletionSource<TcpPacket>();
            SendDuplicationCheckPacket("user", (int)UserTableColumn.Email, email, tcs);
            try
            {
                var ans = await tcs.Task;
                JsonTextParser parser = new JsonTextParser();
                var msgJson = (JsonObjectCollection)parser.Parse(ans.Msg);
                if (Convert.ToInt16(msgJson["result"].GetValue()) == 1)
                {
                    emailCheck.enabled = true;
                    emailCheck.sprite = checkSprite;
                }
                else
                {
                    emailCheck.enabled = true;
                    emailCheck.sprite = warnSprite;
                }
            }
            catch (TcpTimeOutException)
            {
                Debug.Log("Email TimeOut");
            }
        }
    }

    public async void OnConfirmClick()
    {
        TaskCompletionSource<TcpPacket> tcs = new TaskCompletionSource<TcpPacket>();
        JsonObjectCollection jsonObj = new JsonObjectCollection();

        var sha256Pass = Encryptor.EncryptionSHA256(pwInput.text);

        jsonObj.Add(new JsonStringValue("id", idInput.text));
        jsonObj.Add(new JsonStringValue("pw", sha256Pass));
        jsonObj.Add(new JsonStringValue("nick", nickInput.text));
        jsonObj.Add(new JsonStringValue("email", emailInput.text));

        var packet = new TcpPacket(TcpPacketType.SignUp, jsonObj.ToString());
        NetworkManager.Instance.SendPacket(packet, tcs, 1000);

        try
        {
            var ans = await tcs.Task;
            JsonTextParser parser = new JsonTextParser();
            var msgJson = (JsonObjectCollection)parser.Parse(ans.Msg);
            if (Convert.ToInt16(msgJson["result"].GetValue()) != -1)
            {
                Debug.Log("회원가입 성공");
            }
            else
            {
                Debug.Log("회원가입 실패");
            }
        }
        catch (TcpTimeOutException)
        {
            Debug.Log("Email TimeOut");
        }
    }
}
/*
JsonTextParser parser = new JsonTextParser();
JsonObject obj = parser.Parse(strResponse);
JsonObjectCollection col = (JsonObjectCollection)obj;

String accno = (String)col["accno"].GetValue();
*/
