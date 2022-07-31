using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointMonitor : MonoBehaviour
{
    // ======================================================
    public int NextExpectedCheckPoint;

    public GameObject[] CheckPointList;

    public TheHUDDisplay TheCarHUDDisplay;
    public Transform TheRaceCarTransform;
    public RaceCarAgent TheRaceCarAgent; 

    public float CarToRaceTrackAlignment;
    private float LapStartTime;
    private int TrainingLevel;
    private float MaxLapTime;
    private int MaxCheckPoint;
    public float CurrentLapTime;

    // ======================================================
    private void Awake()
    {
        NextExpectedCheckPoint = 1;
        CarToRaceTrackAlignment = 0.0f;
        LapStartTime = Time.time;
        MaxLapTime = 500.0f;
        MaxCheckPoint = 70; 

    } // Awake
    // =======================================================
    public void ResetTrack(int AssignedTrainingLevel)
    {
        NextExpectedCheckPoint = 1;
        CarToRaceTrackAlignment = 0.0f;
        LapStartTime = Time.time;
        CurrentLapTime = 0.0f;
        TrainingLevel = AssignedTrainingLevel;
        SetMaxLapTime();
        SetMaxCheckPoint(); 

    } // ResetTrack

    // =======================================================
    public void PassedCheckPoint(int CheckPointPassed)
    {
        if(CheckPointPassed != NextExpectedCheckPoint)
        {
            Debug.Log("[ERROR]: Failed Check Point");
        }
        else
        {

            TheCarHUDDisplay.SetLastCP(CheckPointPassed); 

            NextExpectedCheckPoint = NextExpectedCheckPoint + 1;

            // Add CP Reward to Agent
            TheRaceCarAgent.AddCheckPointReward(1.0f / MaxCheckPoint); // Add a Fractional Reward for Each CheckPoint Passed

            if (NextExpectedCheckPoint > MaxCheckPoint)
            {
                CurrentLapTime = Time.time - LapStartTime;
                Debug.Log("[INFO] Completed a Lap: " + CurrentLapTime.ToString());
                TheCarHUDDisplay.SetLapDistance(CurrentLapTime);

                // Send Lap Completed to the Race Car Agent  - With Fractional LapTime
                TheRaceCarAgent.LapCompleted(CurrentLapTime / MaxLapTime); 

            } // Passed Max CheckPoint: End of Lap
           


        } // Correct Check Point Passed
    } // PassedCheckPoint
    // =======================================================
    private void FixedUpdate()
    {
        // Monitor the Race Car Progress

        // Current Alignment
        Vector3 NextCheckPointOrientation = -CheckPointList[NextExpectedCheckPoint-1].transform.right;    // The Local Negative X Axis - As can be seen with Scale editor Gizmo 
        // Only interestd in the X,Z components
        Vector3 NextCheckPointOrientationXZ = (new Vector3(NextCheckPointOrientation.x, 0.0f, NextCheckPointOrientation.z)).normalized;

        int PrevCheckPoint = NextExpectedCheckPoint - 1;
        if (PrevCheckPoint < 0) PrevCheckPoint = 69; 
        Vector3 PrevCheckPointOrientation = -CheckPointList[PrevCheckPoint].transform.right;    // The Local Negative X Axis - As can be seen with Scale editor Gizmo 
        // Only interestd in the X,Z components
        Vector3 PrevCheckPointOrientationXZ = (new Vector3(NextCheckPointOrientation.x, 0.0f, NextCheckPointOrientation.z)).normalized;

        // Calculate the Torr - Fractional Distance between CheckPoints
        float DistanceBetweenCPs = Vector3.Distance(CheckPointList[NextExpectedCheckPoint - 1].transform.position, CheckPointList[PrevCheckPoint].transform.position);
        float DistanceRcarCarToPreviousCP = Vector3.Distance(TheRaceCarTransform.position, CheckPointList[PrevCheckPoint].transform.position);
        float Torr = DistanceRcarCarToPreviousCP / DistanceBetweenCPs;

        Vector3 CurrentRaceTrackOrientation = Vector3.Lerp(PrevCheckPointOrientationXZ, NextCheckPointOrientationXZ, Torr); 

        // Now the Current Race Car Alignment - Z Forward confirmed by Scale Gizmo
        Vector3 RaceCarOrientationXZ = (new Vector3(TheRaceCarTransform.forward.x, 0.0f, TheRaceCarTransform.forward.z)).normalized;

        CarToRaceTrackAlignment = 1.0f - 2.5f * (1.0f - Vector3.Dot(CurrentRaceTrackOrientation, RaceCarOrientationXZ)); // Adjusted Track Alignment 

        // Update the HUD with the Current Alignment
        TheCarHUDDisplay.SetAlignment(CarToRaceTrackAlignment);

        // ==============================================================
        // Now Check if Run Out of Time
        CurrentLapTime = Time.time - LapStartTime;
        if (CurrentLapTime > MaxLapTime)
        {
            TheCarHUDDisplay.SetLapDistance(999.99f);  // Just to display Last Lap Time Failed
            TheRaceCarAgent.LapTimeExceeded();
        }
        TheCarHUDDisplay.DisplayCurrentLapTime((int)(0.5f*CurrentLapTime));

        // ===============================================================

    } // FixedUpdate

    // =======================================================
    private void SetMaxLapTime()
    {

        MaxLapTime = 200.0f + 300.0f * TrainingLevel / 70.0f;
        if (TrainingLevel <=1 ) MaxLapTime = 200.0f;
        if (TrainingLevel >= 70) MaxLapTime = 500.0f; 

    } // FindMaxLapTime
    // =======================================================
    private void SetMaxCheckPoint()
    {
        MaxCheckPoint = TrainingLevel;
        
        if (TrainingLevel >= 70) MaxCheckPoint = 70;

    } // FindMaxLapTime
    // =======================================================


}
