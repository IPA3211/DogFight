using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUIManager : MonoBehaviour
{
    [SerializeField] Button reconnectBtn;

    void Start()
    {
        NetworkManager.Instance.onDisconnect.AddListener(OnDisconnect);
        reconnectBtn.onClick.AddListener(OnReconnectBtnClick);
    }

    void OnDisconnect()
    {
        reconnectBtn.gameObject.SetActive(true);
    }

    async void OnReconnectBtnClick()
    {
        reconnectBtn.gameObject.SetActive(false);
        await NetworkManager.Instance.StartConnection();
    }
}
