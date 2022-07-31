using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointItem : MonoBehaviour
{
    public int CheckPointIndex;
    public CheckPointMonitor TheCheckPointMonitor; 

    // ===========================================================================
    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<RaceCarController>())
        if (other.GetComponent<RaceCarAgent>())
        {
            // Race Car has Passed Through
           // Debug.Log("Passed CheckPoint: " + CheckPointIndex.ToString());
            TheCheckPointMonitor.PassedCheckPoint(CheckPointIndex); 
        }

    } // OnTriggerEnter
    // ===============================================================================




}
