using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] FighterController controller;

    void Update()
    {
        if (Input.GetButtonDown("Vertical") || Input.GetButtonUp("Vertical"))
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                controller.SetPitchStatus(1);
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                controller.SetPitchStatus(-1);
            }
            else
            {
                controller.SetPitchStatus(0);
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            controller.SetFiringStatus(true);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            controller.SetFiringStatus(false);
        }


        if(Input.GetKeyDown("r")){
            controller.SetFlipStatus();
        }
    }
}
