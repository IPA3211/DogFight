using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    int rollingStatus;
    bool isFiring;
    void Update()
    {
        
        fighter.rollingUp(rollingStatus);

        switch (isFiring)
        {
            case true:
                Debug.Log("Fire");
                break;
            case false:
                break;
        }
    }

    public void SetRollingStatus(int index = 0)
    {
        rollingStatus = index;
    }

    public void SetFiringStatus(bool isFiring = false)
    {
        this.isFiring = isFiring;
    }
}
