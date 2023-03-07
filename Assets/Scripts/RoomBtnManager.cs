using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomBtnManager : MonoBehaviour
{
    [SerializeField] TMP_Text idText;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text modeText;
    [SerializeField] TMP_Text capaText;
    [SerializeField] TMP_Text hostText;
    [SerializeField] TMP_Text priText;
    [SerializeField] TMP_Text pingText;

    public void SetInfo(string id, string title, string mode, string capa, string host, string pri, string ping)
    {
        idText.text = id;
        titleText.text = title;
        modeText.text = mode;
        capaText.text = capa;
        hostText.text = host;
        priText.text = pri;
        pingText.text = ping;
    }
}
