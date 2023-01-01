using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] GameObject fighter;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject body;
    [SerializeField] GameObject gun;
    [SerializeField] GameObject gear;
    [SerializeField] bool isGearUp;
    [SerializeField] public bool IsXfliped;
    [SerializeField] public bool IsYfliped;
    Vector2 front => (gun.transform.position - body.transform.position).normalized;
    Vector2 up => (body.transform.position - gear.transform.position).normalized;
    public float pitchPower = 10f;
    public float speed = 100f;
    public float fireingInterval = 1;
    float intervalTime = 0;
    Rigidbody2D rigid;
    public void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        Flip();
    }

    public void FixedUpdate()
    {
        intervalTime -= Time.fixedDeltaTime;
        CalculateThrust();
        CalculateLift();
        CalculateDrag();
    }

    public void Flip()
    {
        fighter.transform.localRotation = Quaternion.Euler(IsYfliped ? 180 : 0, IsXfliped ? 180 : 0, 0);
        Debug.Log(IsYfliped);
    }

    public void Pitch(int pitchDriection)
    {
        rigid.AddTorque(((IsXfliped && IsYfliped) || (!IsXfliped && !IsYfliped) ? -1 : 1) * pitchPower * pitchDriection * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public bool FireBullet()
    {
        if (intervalTime < 0)
        {
            var newRot = Quaternion.Euler((IsXfliped ? new Vector3(0, 0, 180) : Vector3.zero) + transform.rotation.eulerAngles);
            intervalTime = fireingInterval;
            Instantiate(bullet, transform.position, newRot);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RollFighter(bool isFlip)
    {
        IsYfliped = isFlip;
        Flip();
        return true;
    }

    public void CalculateThrust()
    {
        rigid.AddForce(speed * front * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void CalculateLift()
    {
        rigid.AddForce(Mathf.Pow(rigid.velocity.magnitude, 2) * up * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void CalculateDrag()
    {
        var volume = 3 * Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity.normalized, front)) + 0.8f * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity.normalized, front));
        rigid.AddForce(0.1f * volume * Mathf.Pow(rigid.velocity.magnitude, 2) / 2 * (-rigid.velocity.normalized), ForceMode2D.Force);
    }
}
