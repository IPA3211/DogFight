using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    public void CheckIdDuplication(string id)
    {
        var packet = new TcpPacket(TcpPacketType.IdDuplication, id);
        NetworkManager.Instance.SendPacket(packet, 1000,
        (p) =>
        {
            if (Convert.ToInt16(p.Msg) == 1)
            {
                idCheck.enabled = true;
                idCheck.sprite = checkSprite;
            }
            else
            {
                idCheck.enabled = true;
                idCheck.sprite = warnSprite;
            }
        },
        () =>
        {
            Debug.Log("ID TimeOut");
        });
    }

    public void NickNameDuplication(string nickName)
    {
        var packet = new TcpPacket(TcpPacketType.NickDuplication, nickName);
        NetworkManager.Instance.SendPacket(packet, 1000,
        (p) =>
        {
            if (Convert.ToInt16(p.Msg) == 1)
            {
                nickCheck.enabled = true;
                nickCheck.sprite = checkSprite;
            }
            else
            {
                nickCheck.enabled = true;
                nickCheck.sprite = warnSprite;
            }
        },
        () =>
        {
            Debug.Log("nick TimeOut");
        });
    }

    public void CheckPasswordConfirm(string confimePass)
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

    public void IsValidEmail(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");

        if (valid)
        {
            var packet = new TcpPacket(TcpPacketType.EmailDuplication, email);
            NetworkManager.Instance.SendPacket(packet, 1000,
            (p) =>
            {
                if (Convert.ToInt16(p.Msg) == 1)
                {
                    emailCheck.enabled = true;
                    emailCheck.sprite = checkSprite;
                }
                else
                {
                    emailCheck.enabled = true;
                    emailCheck.sprite = warnSprite;
                }
            },

            () =>
            {
                Debug.Log("email TimeOut");
            });
        }
    }

    public void OnConfirmClick()
    {

    }
}
