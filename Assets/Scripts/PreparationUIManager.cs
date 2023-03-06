using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationUIManager : MonoBehaviour
{
    [SerializeField] GameObject connectUI;
    [SerializeField] GameObject signInUI;
    [SerializeField] GameObject signUpUI;
    [SerializeField] GameObject selectRoomUI;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Instance.onConnect.AddListener(OnConnect);
        NetworkManager.Instance.onDisconnect.AddListener(onDisconnect);
        NetworkManager.Instance.onSignIn.AddListener(onSignIn);
    }

    void OnConnect()
    {
        connectUI.SetActive(false);
        signInUI.SetActive(true);
        signUpUI.SetActive(false);
        selectRoomUI.SetActive(false);
    }

    void onDisconnect()
    {
        connectUI.SetActive(true);
        signInUI.SetActive(false);
        signUpUI.SetActive(false);
        selectRoomUI.SetActive(false);
    }

    void onSignIn()
    {
        connectUI.SetActive(false);
        signInUI.SetActive(false);
        signUpUI.SetActive(false);
        selectRoomUI.SetActive(true);
    }
}
