using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheHUDDisplay : MonoBehaviour
{

    public Text TheSpeedTextDisplay;
    public Text TheLapDistanceDisplay;
    public Text TheInclineTextDisplay;
    public Text TheAlignmentTextDisplay;
    public Text TheLastCPDisplay;
    public Text CurrentLapTimeDisplay;

    public Text TheTrainingLevelTextDisplay;
    // =============================================================================


    private void Awake()
    {
        TheLapDistanceDisplay.text = "No Lap Time Yet";
        

    } // Awake
    // =============================================================================
    public void SetSpeed(float Speed)
    {
        // Round Speed to 2 Decomal Places
        TheSpeedTextDisplay.text = "Speed: " + (Mathf.Round(Speed * 200) / 100.0f).ToString();    
    }
    public void SetIncline(float Incline)
    {
        // Round Speed to 2 Decomal Places
        TheInclineTextDisplay.text = "Incline: " + (Mathf.Round(Incline * 100) / 100.0f).ToString();
    }

    public void SetLapDistance(float LapDistance)
    {
        // Round Speed to 2 Decomal Places
        TheLapDistanceDisplay.text = "Last Lap Time: " + (Mathf.Round(LapDistance * 50) / 100.0f).ToString();
    }

    public void SetAlignment(float CurrentAlignment)
    {
        // Round Alignment to 2 Decomal Places
        TheAlignmentTextDisplay.text = "Alignment: " + (Mathf.Round(CurrentAlignment * 100) / 100.0f).ToString();
    }

    public void SetTrainingTextDisplay(string TrainingLevelString)
    {
        TheTrainingLevelTextDisplay.text = TrainingLevelString;
    }

    public void SetLastCP(int LastCP)
    {
        TheLastCPDisplay.text = "Last CP: "+ LastCP.ToString();
    }

    public void DisplayCurrentLapTime(int LapTimeSeconds)
    {
        CurrentLapTimeDisplay.text = "Current Lap Time: " + LapTimeSeconds.ToString();
    }

    // =============================================================================
}
