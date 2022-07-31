using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RaceCarAgent : Agent
{
    // =============================================================================================================
    public WheelCollider FrontLeftWheelCollider;
    public WheelCollider FrontRightWheelCollider;
    public WheelCollider RearLeftWheelCollider;
    public WheelCollider RearRightWheelCollider;

    public Transform FrontLeftWheelTransform;
    public Transform FrontRightWheelTransform;
    public Transform RearLeftWheelTransform;
    public Transform RearRightWheelTransform;

    public TheHUDDisplay TheHUDDisplayController;
    public CheckPointMonitor TheRaceTrackMonitor;
    public TrainingController TheTrainingController; 

    public Transform TheCofMass;
    private Rigidbody TheRaceCarRigidBody;
    public VehicleAgentConfiguration TheRaceCarConfiguration;

    // Local Variables
    private float CarSpeed;
    private float CarPitchIncline; 

    // ==============================================================================================================
    private void Awake()
    {

        TheRaceCarRigidBody = GetComponent<Rigidbody>();
        TheRaceCarRigidBody.centerOfMass = TheCofMass.localPosition;

    } // Awake
    // ==============================================================================================================
    public override void Initialize()
    {
        ResetEpisode();
    } // Initialize
    // ================================================================================================
    public override void OnEpisodeBegin()
    {
        //Debug.Log(" [INFO] Race Car Agent Episode Begin");

        ResetEpisode();
    } // OnEpisodeBegin
    // =================================================================================================
    public void ResetEpisode()
    {

        // Clear down any Velocities and Reset the Car to Start Position
        TheRaceCarRigidBody.angularVelocity = Vector3.zero;
        TheRaceCarRigidBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(-277.5f, 61.75f, 1783.7f);
        transform.rotation = Quaternion.Euler(0.0f, -86.5f, 0.0f);
        CarSpeed = 0.0f;
        CarPitchIncline = 0.0f; 

    }  // ResetEpisode
    // ==============================================================================================================
    public override void CollectObservations(VectorSensor sensor)
    {
        // Calculate Car Speed and Incline Angles
        Vector3 LocalVelocity = TheRaceCarRigidBody.transform.InverseTransformDirection(TheRaceCarRigidBody.velocity);
        CarSpeed = LocalVelocity.z * 2.0f;
        TheHUDDisplayController.SetSpeed(CarSpeed);

        //  Incline Angle Function
        // https://answers.unity.com/questions/1366142/get-pitch-and-roll-values-from-object.html
        var right = transform.right;
        right.y = 0;
        right *= Mathf.Sign(transform.up.y);
        var fwd = Vector3.Cross(right, Vector3.up).normalized;
        CarPitchIncline = Vector3.Angle(fwd, transform.forward) * Mathf.Sign(transform.forward.y);
        TheHUDDisplayController.SetIncline(CarPitchIncline);


        // =========================================
        // Now Collect the Observations
        // Car Speed
        sensor.AddObservation(CarSpeed/100.0f);

        // Track Incline
        sensor.AddObservation(CarPitchIncline/10.0f);

        // Car Track Alignment 
        sensor.AddObservation(TheRaceTrackMonitor.CarToRaceTrackAlignment);

        // So 3 x Obervations
    } // CollectObservations
    // ========================================================================================

    // Update is called once per Display frame
    void Update()
    {
        // Wheel Rotations
        Vector3 WheelPos = Vector3.zero;
        Quaternion WheelRot = Quaternion.identity;

        FrontLeftWheelCollider.GetWorldPose(out WheelPos, out WheelRot);
        FrontLeftWheelTransform.position = WheelPos;
        FrontLeftWheelTransform.rotation = WheelRot;// * Quaternion.Euler(0, 180.0f, 0);

        FrontRightWheelCollider.GetWorldPose(out WheelPos, out WheelRot);
        FrontRightWheelTransform.position = WheelPos;
        FrontRightWheelTransform.rotation = WheelRot * Quaternion.Euler(0, 180.0f, 0); // note on some models  have to flip right wheels 180

        RearLeftWheelCollider.GetWorldPose(out WheelPos, out WheelRot);
        RearLeftWheelTransform.position = WheelPos;
        RearLeftWheelTransform.rotation = WheelRot;// * Quaternion.Euler(0, 180.0f, 0); ;

        RearRightWheelCollider.GetWorldPose(out WheelPos, out WheelRot);
        RearRightWheelTransform.position = WheelPos;
        RearRightWheelTransform.rotation = WheelRot * Quaternion.Euler(0, 180.0f, 0);  // note on some models may have to flip right wheels by 180 

    } // Update
    // ========================================================================
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Two Action Branches
        // [0]:  None, Forward, (Brake), Reverse
        // [1]: None, Turn Right, Turn Left

        //  Rear Wheel Drive  Forward Torque if DiscreteActions[0] == 1  and Reverse if DiscreteActions[0] == 3
        float CurrentWheelTorque = 0.0f;
        if (actionBuffers.DiscreteActions[0] == 1) CurrentWheelTorque = TheRaceCarConfiguration.RearEngineTorque; 
        if (actionBuffers.DiscreteActions[0] == 2) CurrentWheelTorque = -TheRaceCarConfiguration.RearEngineTorque;

        RearLeftWheelCollider.motorTorque = CurrentWheelTorque;
        RearRightWheelCollider.motorTorque = CurrentWheelTorque;

        /*
        // Braking Controls from Simple Vehicle - https://www.youtube.com/watch?v=Z4HA8zJhGEk
        // Check and Apply Brakes on  if DiscreteActions[0] == 2
        float CurrentBrakeForce = 0.0f;
        if (actionBuffers.DiscreteActions[0] == 2) CurrentBrakeForce = TheRaceCarConfiguration.BrakingTorque;
        FrontLeftWheelCollider.brakeTorque = CurrentBrakeForce;
        FrontRightWheelCollider.brakeTorque = CurrentBrakeForce;
        RearLeftWheelCollider.brakeTorque = CurrentBrakeForce;
        RearRightWheelCollider.brakeTorque = CurrentBrakeForce;
        */

        // Steer Angle Actions [1]:     // Front Wheel Steer
        float CurrentSteerAngle = 0.0f;
        if (actionBuffers.DiscreteActions[1] == 1) CurrentSteerAngle = TheRaceCarConfiguration.MaxSteerAngle;
        if (actionBuffers.DiscreteActions[1] == 2) CurrentSteerAngle = -TheRaceCarConfiguration.MaxSteerAngle;

        FrontLeftWheelCollider.steerAngle = CurrentSteerAngle;
        FrontRightWheelCollider.steerAngle = CurrentSteerAngle;
        // =============================================================

    } // OnActionReceived
    // ==========================================================================================================================
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Manual Control Actions

        // Branch 0: Forward Motion Actions: discreteActionsOut[0] = 0:NOOP, 1: Foward 2: Backward
        // Branch 1: Steer Actions: discreteActionsOut[1] = 0:NOOP,  1: Turn Left, 2: Turn Right,  

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;      // 0:NOOP, 1: Forward Action
        discreteActionsOut[1] = 0;      // 0:NOOP 1: Turn Left, 2: Turn Right Action

        // Keyboard Motion Actions 
        if (Input.GetKey(KeyCode.UpArrow)) discreteActionsOut[0] = 1;       // Move Forward
        //if (Input.GetKey(KeyCode.Space)) discreteActionsOut[0] = 2;         // Brake
        if (Input.GetKey(KeyCode.DownArrow)) discreteActionsOut[0] = 2;      // Reverse
        if (Input.GetKey(KeyCode.RightArrow)) discreteActionsOut[1] = 1;    // Rotate Left
        if (Input.GetKey(KeyCode.LeftArrow)) discreteActionsOut[1] = 2;     // Rotate Right

    } // Heuristic  Controls
    // =========================================================================================

    private void OnCollisionEnter(Collision collision)
    {
        // Barrier Bump Events
        if (collision.gameObject.tag == "Barriers")
        {
            //Debug.Log("Bumped Into The Barrier");
            AddReward(-0.025f);

        } // Barrier Bump
    } // OnCollisionEnter
    // ================================================================
    public void LapCompleted(float FractionalLaptime)
    {
        // Positive Reward for completing the Lap
        SetReward(1.0f - 0.5f*FractionalLaptime);  // So worst case if only just complete Lap in time will be +0.5f

        // Review Training Level and Reset the Track Monitor
        TheTrainingController.ReviewTrainingLevel(true); 

        // End Episode
        EndEpisode();

    }  // LapCompleted
    // ==================================================================
    public void LapTimeExceeded()
    {
        // Negative Reward for Not completing the Lap
        SetReward(-1.0f);

        // Review Training Level and Reset the Track Monitor
        TheTrainingController.ReviewTrainingLevel(false);

        // End Episode
        EndEpisode();

    }  // LapTimeExceeded
    // ==================================================================
    public void AddCheckPointReward(float CheckPointFraction)
    {
        AddReward(0.25f * CheckPointFraction);   // Add a total of +0.25 for the accumulated Check points
    }

    // ==============================================================================================================
}
