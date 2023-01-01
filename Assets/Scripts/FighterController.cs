using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    int rollingStatus;
    bool isFiring;
    void FixedUpdate()
    {
        controlFighter();
    }

    void controlFighter()
    {
        fighter.Pitch(rollingStatus);

        if (isFiring)
        {
            var isFired = fighter.FireBullet();
            if (isFired)
            {

            }
        }
    }

    public void SetPitchStatus(int index = 0)
    {
        rollingStatus = index;
    }

    public void SetFiringStatus(bool isFiring = false)
    {
        this.isFiring = isFiring;
    }
}
