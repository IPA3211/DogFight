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
    Vector3 drag;
    Vector3 lift;
    Vector3 thrust;
    public void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        Flip();
    }

    void OnDrawGizmos(){
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + drag);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + lift);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + thrust);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + thrust + drag + lift + new Vector3(0, -9.81f, 0) / 2);
        Debug.Log(lift);
        Debug.Log(lift + new Vector3(0, -9.81f, 0) / 2);
    }

    public void FixedUpdate()
    {
        drag = CalculateDrag() / 2;
        lift = CalculateLift() / 2;
        thrust = CalculateThrust() / 2;
        intervalTime -= Time.fixedDeltaTime;
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

    public Vector2 CalculateThrust()
    {
        var f = speed * front * Time.fixedDeltaTime;
        rigid.AddForce(f, ForceMode2D.Force);
        return f;
    }

    public Vector2 CalculateLift()
    {
        var f = Mathf.Pow(rigid.velocity.magnitude, 2) * up * Time.fixedDeltaTime;
        rigid.AddForce(f, ForceMode2D.Force);
        return f;
    }

    public Vector2 CalculateDrag()
    {
        var volume = 3 * Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity.normalized, front)) + 0.8f * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity.normalized, front));
        var f = 0.1f * volume * Mathf.Pow(rigid.velocity.magnitude, 2) / 2 * (-rigid.velocity.normalized);
        rigid.AddForce(f, ForceMode2D.Force);
        return f;
    }
}
