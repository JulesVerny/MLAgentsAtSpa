using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCarController : MonoBehaviour
{
    // ================================================================
    public WheelCollider FrontLeftWheelCollider;
    public WheelCollider FrontRightWheelCollider;
    public WheelCollider RearLeftWheelCollider;
    public WheelCollider RearRightWheelCollider;

    public Transform FrontLeftWheelTransform;
    public Transform FrontRightWheelTransform;
    public Transform RearLeftWheelTransform;
    public Transform RearRightWheelTransform;

    public TheHUDDisplay TheHUDDisplayController; 

    public Transform TheCofMass; 

    private Rigidbody TheRaceCarRigidBody;

    public VehicleAgentConfiguration TheRaceCarConfiguration; 
    // ================================================================
    private void Awake()
    {
        TheRaceCarRigidBody = GetComponent<Rigidbody>();
        TheRaceCarRigidBody.centerOfMass = TheCofMass.localPosition; 

    } // Awake

    // ================================================================
    void Start()
    {
        
    }
    // ================================================================
    public void ResetCarPosition()
    {

        // Clear down any Velocities and reset to centr of the Plane
        TheRaceCarRigidBody.angularVelocity = Vector3.zero;
        TheRaceCarRigidBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(-277.5f, 61.75f, 1783.7f);
        transform.rotation = Quaternion.Euler(0.0f, -86.5f, 0.0f);


    }  // ResetCarPosition
    // ================================================================

    // Update is called once per frame
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

        Vector3 LocalVelocity = TheRaceCarRigidBody.transform.InverseTransformDirection(TheRaceCarRigidBody.velocity); 
        TheHUDDisplayController.SetSpeed(LocalVelocity.z);

        //  Pitch Angle Function
        // https://answers.unity.com/questions/1366142/get-pitch-and-roll-values-from-object.html
        var right = transform.right;
        right.y = 0;
        right *= Mathf.Sign(transform.up.y);
        var fwd = Vector3.Cross(right, Vector3.up).normalized;
        float pitch = Vector3.Angle(fwd, transform.forward) * Mathf.Sign(transform.forward.y);
        TheHUDDisplayController.SetIncline(pitch);

    } // Update
    // ================================================================
    private void FixedUpdate()
    {
        // Apply the Rear Wheel Torque
        RearLeftWheelCollider.motorTorque = Input.GetAxis("Vertical") * TheRaceCarConfiguration.RearEngineTorque;
        RearRightWheelCollider.motorTorque = Input.GetAxis("Vertical") * TheRaceCarConfiguration.RearEngineTorque;

        // Steer on Front Wheels (Colliders)
        FrontLeftWheelCollider.steerAngle = Input.GetAxis("Horizontal") * TheRaceCarConfiguration.MaxSteerAngle;
        FrontRightWheelCollider.steerAngle = Input.GetAxis("Horizontal") * TheRaceCarConfiguration.MaxSteerAngle;

    } // FixedUpdate

    // ================================================================
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Barriers")
        {
            Debug.Log("Bumped The Barrier"); 


        } // Barrier Bump
    } // OnCollisionEnter

    // ================================================================



    // ================================================================



    // ================================================================
}
