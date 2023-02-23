using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] Sprite wranSprite;

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
    }

    public void CheckIdDuplication(string id)
    {
    }

    public void NickNameDuplication(string nickName)
    {
    }

    public void CheckPasswordConfime(string confimePass)
    {
        pwCheck.enabled = true;
        if (pwInput.text == pwConfirmInput.text)
        {
            pwCheck.sprite = checkSprite;
        }
        else
        {
            pwCheck.sprite = wranSprite;
        }
    }

    public void IsValidEmail(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");

        if (valid)
        {
            
        }
    }
}
