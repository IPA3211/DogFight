using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    public float pitchPower = 10f;
    public float speed = 100f;
    public float fireingInterval = 1;
    float intervalTime = 0;
    Rigidbody2D rigid;
    public void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        intervalTime -= Time.fixedDeltaTime;
        CalculateThrust();
        CalculateLift();
        CalculateDrag();
    }

    public void Pitch(int PitchDirection)
    {
        rigid.AddTorque(pitchPower * PitchDirection * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public bool FireBullet(){
        if(intervalTime < 0){
            intervalTime = fireingInterval;
            Instantiate(bullet, transform.position, transform.rotation);
            return true;
        }
        else{
            return false;
        }
    }

    public void CalculateThrust()
    {
        rigid.AddRelativeForce(speed * Vector2.right * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void CalculateLift()
    {
        rigid.AddRelativeForce(Mathf.Pow(rigid.velocity.magnitude, 2) * Vector2.up * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void CalculateDrag()
    {
        var volume = 3 * Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity, transform.right)) + 0.8f * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(rigid.velocity, transform.right));
        rigid.AddForce(0.1f * volume * Mathf.Pow(rigid.velocity.magnitude, 2) / 2 * (-rigid.velocity.normalized), ForceMode2D.Force);
    }
}
