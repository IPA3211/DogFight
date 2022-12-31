using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public float rollingPower = 10f;
    public float speed = 10f;
    Rigidbody2D rigid;
    public void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        rigid.AddRelativeForce(speed * Vector2.right * Time.deltaTime);
    }

    public void rollingUp(int rollingDirection)
    {
        rigid.AddTorque(rollingPower * rollingDirection * Time.deltaTime, ForceMode2D.Force);
    }
}
