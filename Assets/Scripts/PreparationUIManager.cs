using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationUIManager : MonoBehaviour
{
    [SerializeField] GameObject connectUI;
    [SerializeField] GameObject signInUI;
    [SerializeField] GameObject signUpUI;
    [SerializeField] GameObject selectRoomUI;
    [SerializeField] GameObject roomUI;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Instance.onConnect.AddListener(ShowSignInUI);
        NetworkManager.Instance.onDisconnect.AddListener(ShowConnectUI);
    }

    void closeAllUI()
    {
        connectUI.SetActive(false);
        signInUI.SetActive(false);
        signUpUI.SetActive(false);
        selectRoomUI.SetActive(false);
        roomUI.SetActive(false);
    }

    public void ShowSignInUI()
    {
        closeAllUI();
        signInUI.SetActive(true);
    }

    public void ShowSignUpUI()
    {
        closeAllUI();
        signUpUI.SetActive(true);
    }

    public void ShowConnectUI()
    {
        closeAllUI();
        connectUI.SetActive(true);
    }

    public void ShowSelectRoomUI()
    {
        closeAllUI();
        selectRoomUI.SetActive(true);
    }

    public void ShowRoomUI()
    {
        closeAllUI();
        roomUI.SetActive(true);
    }
}
