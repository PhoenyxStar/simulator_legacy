using UnityEngine;
using System.Collections;

public class SubPhysics : MonoBehaviour {
    Rigidbody rb;
    int COEF = 2;
    string Joystick;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.drag = 0.75f;
        rb.angularDrag = 0.75f;

    }

    // Update is called once per frame
    void Update () {

        float roll = Input.GetAxis("JoyAxisX");
        float pitch = Input.GetAxis("JoyAxisY");
        float yaw = Input.GetAxis("JoyAxisZ");
        float vert = Input.GetAxis("JoyAxisA");
        float hori = Input.GetAxis("JoyAxisB");
        float forw = Input.GetAxis("JoyAxisC");


        Vector3 force = new Vector3(hori * COEF,-vert*COEF, forw*COEF);
        Vector3 torq = new Vector3(pitch*COEF, yaw*COEF, roll*COEF);
        rb.AddRelativeForce(force);
        rb.AddRelativeTorque(torq);
        
        //Quaternion target = Quaternion.Euler(y, x, 0);
        //transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);



        /*
        if (Input.GetKeyDown(KeyCode.R))
         {
             Vector3 force = new Vector3(COEF, 0, 0);
             rb.AddRelativeTorque(force);

         }
        else if (Input.GetKeyDown(KeyCode.F))
         {
             Vector3 force = new Vector3(-COEF, 0, 0);
             rb.AddRelativeTorque(force);
         }
         if (Input.GetKeyDown(KeyCode.T))
         {
             Vector3 force = new Vector3(0, COEF, 0);
             rb.AddRelativeTorque(force);
         }
         if (Input.GetKeyDown(KeyCode.G))
         {
             Vector3 force = new Vector3(0, -COEF, 0);
             rb.AddRelativeTorque(force);
         }
         if (Input.GetKeyDown(KeyCode.Y))
         {
             Vector3 force = new Vector3(0, 0, COEF);
             rb.AddRelativeTorque(force);
         }
         if (Input.GetKeyDown(KeyCode.H))
         {
             Vector3 force = new Vector3(0, 0, -COEF);
             rb.AddRelativeTorque(force);
         }
         if (Input.GetKeyDown(KeyCode.N))
         {
             Vector3 force = new Vector3(COEF, 0, 0);
             rb.AddRelativeForce(force);
         }
         if (Input.GetKeyDown(KeyCode.M))
         {
             Vector3 force = new Vector3(-COEF, 0, 0);
             rb.AddRelativeForce(force);
         }
         if (Input.GetKeyDown(KeyCode.V))
         {
             Vector3 force = new Vector3(0, COEF, 0);
             rb.AddRelativeForce(force);
         }
         if (Input.GetKeyDown(KeyCode.B))
         {
             Vector3 force = new Vector3(0, -COEF, 0);
             rb.AddRelativeForce(force);
         }
         if (Input.GetKeyDown(KeyCode.X))
         {
             Vector3 force = new Vector3(0, 0, COEF);
             rb.AddRelativeForce(force);
         }
         if (Input.GetKeyDown(KeyCode.C))
         {
             Vector3 force = new Vector3(0, 0, -COEF);
             rb.AddRelativeForce(force);
         }*/
    }



}
