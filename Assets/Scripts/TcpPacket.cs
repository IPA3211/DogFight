using UnityEngine;
using System;

[Serializable]
public class TcpPacket
{
    [SerializeField] int index;
    [SerializeField] int order;
    [SerializeField] string msg;

    public int Index => index;

    public TcpPacket(int orderNum, string message)
    {
        index = this.GetHashCode();
        order = orderNum;
        msg = message;
    }
}