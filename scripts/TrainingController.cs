using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingController : MonoBehaviour
{
    // =================================================================================
    public RaceCarAgent TheRacingCar;
    public TheHUDDisplay TheHUDDisplaySystem;
    public CheckPointMonitor TheRaceTrackMonitor; 

    public int TrainingLevel;

    private int IncreaseTLThreshold = 20;
    private int DecrementTLThreshold = 10;
    private int CurrentSuccessCount;
    private int CurrentFailureCount;

    // =================================================================================
    void Start()
    {
        TrainingLevel = 75;
        CurrentSuccessCount = 0;
        CurrentFailureCount = 0;
        TheRaceTrackMonitor.ResetTrack(TrainingLevel);

        TheHUDDisplaySystem.SetTrainingTextDisplay("Training Level: " + TrainingLevel.ToString()); 

    } // Start()
    // =================================================================================
    // Update is called once per frame
    void Update()
    {
        // UI Input 
        if (Input.GetKeyDown(KeyCode.R))
        {
            TheRaceTrackMonitor.ResetTrack(TrainingLevel); 
            TheRacingCar.ResetEpisode();
        }

    } // Update
    // =================================================================================
    





    // =================================================================================
    public void ReviewTrainingLevel(bool LastEpisodeSuccessful)
    {
        if(LastEpisodeSuccessful)
        {
            // Review Positive Episode Result
            CurrentSuccessCount = CurrentSuccessCount+1;
            if(CurrentSuccessCount> IncreaseTLThreshold)
            {
                if (TrainingLevel < 75) TrainingLevel = TrainingLevel + 1;
                CurrentSuccessCount = 0;
                Debug.Log("[INFO]: New Training Level +: " + TrainingLevel.ToString()); 
            }
            CurrentFailureCount = 0;
            TheHUDDisplaySystem.SetTrainingTextDisplay("Training Level: " + TrainingLevel.ToString() + ":  +" + CurrentSuccessCount.ToString());
        }
        else
        {
            // Review Negative Episode Result
            CurrentFailureCount = CurrentFailureCount + 1;
            if (CurrentFailureCount > DecrementTLThreshold)
            {
                if(TrainingLevel>1) TrainingLevel = TrainingLevel - 1;
                Debug.Log("[INFO]: New Training Level -: " + TrainingLevel.ToString());

                CurrentFailureCount = 0;
            }
            CurrentSuccessCount = 0;
            TheHUDDisplaySystem.SetTrainingTextDisplay("Training Level: " + TrainingLevel.ToString() + ":  -" + CurrentFailureCount.ToString());
        }
        // Reset and Update the Track Monitor with the new Training Level
        TheRaceTrackMonitor.ResetTrack(TrainingLevel);

    } // ReviewTrainingLevel
    // =================================================================================
}
