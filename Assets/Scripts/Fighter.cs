using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public float pitchPower = 10f;
    public float speed = 100f;
    Rigidbody2D rigid;
    public void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        CalculateThrust();
        CalculateLift();
        CalculateDrag();
    }

    public void PitchUp(int PitchDirection)
    {
        rigid.AddTorque(pitchPower * PitchDirection * Time.deltaTime, ForceMode2D.Force);
    }

    public void CalculateThrust()
    {
        rigid.AddRelativeForce(speed * Vector2.right * Time.deltaTime, ForceMode2D.Force);
    }

    public void CalculateLift()
    {
        rigid.AddRelativeForce(0.4f * Mathf.Pow(rigid.velocity.magnitude, 2) * Vector2.up * Time.deltaTime, ForceMode2D.Force);
    }

    public void CalculateDrag()
    {
        var volume = 3 * Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity, transform.right)) + 0.8f * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity, transform.right));
        rigid.AddForce(0.1f * volume * Mathf.Pow(rigid.velocity.magnitude, 2) / 2 * (-rigid.velocity.normalized), ForceMode2D.Force);
    }
}
