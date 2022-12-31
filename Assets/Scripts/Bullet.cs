using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody2D rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.AddForce(transform.right * speed, ForceMode2D.Impulse);
        Destroy(gameObject, 3f);
    }
}
