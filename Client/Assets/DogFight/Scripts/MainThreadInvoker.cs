using System.Collections.Generic;
using UnityEngine;

public delegate void MainThreadToInvoke();

//Multithread로 들어온 요청들을 MainThread로 보내서 처리하도록 함

public class MainThreadInvoker : MonoBehaviour
{
    Queue<MainThreadToInvoke> mainThreadInvoke = new();

    static MainThreadInvoker instance;
    public static MainThreadInvoker Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        lock (mainThreadInvoke)
        {
            while (mainThreadInvoke.Count != 0)
            {
                var toInvoke = mainThreadInvoke.Dequeue();
                toInvoke.Invoke();
            }
        }
    }

    public void Enqueue(MainThreadToInvoke toInvoke)
    {
        lock (mainThreadInvoke)
        {
            mainThreadInvoke.Enqueue(toInvoke);
        }
    }
}
