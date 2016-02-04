using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SubPhysics : MonoBehaviour {
    Rigidbody rb;
    Rigidbody rt;
    Rigidbody ft;
    Rigidbody tt;
    Rigidbody bt;
    Rigidbody pt;
    Rigidbody st;
    int COEF = 3;
    string Joystick;

    Communicator comm = new Communicator();

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rt = GameObject.Find("RT").GetComponent<Rigidbody>();
        ft = GameObject.Find("FT").GetComponent<Rigidbody>();
        tt = GameObject.Find("TT").GetComponent<Rigidbody>();
        bt = GameObject.Find("BT").GetComponent<Rigidbody>();
        pt = GameObject.Find("PT").GetComponent<Rigidbody>();
        st = GameObject.Find("ST").GetComponent<Rigidbody>();

        rb.drag = 0.75f;
        rb.angularDrag = 0.75f;
        comm.Initialize("thruster");


    }
    // Update is called once per frame
    void Update () {

        /*
        float roll = Input.GetAxis("JoyAxisX");
        float pitch = Input.GetAxis("JoyAxisY");
        float yaw = Input.GetAxis("JoyAxisZ");
        float vert = Input.GetAxis("JoyAxisA");
        float hori = Input.GetAxis("JoyAxisB");
        float forw = Input.GetAxis("JoyAxisC");
        */



        thruster_packet tp;
        List<string> received;
        float port = 0f;
        float star = 0f;
        float front = 0f;
        float back = 0f;
        float top = 0f;
        float bot = 0f;
        received = comm.receive_messages();
        foreach(string x in received)
        { LoggingSystem.log.Info(x); }
        if (received.Count > 0)
        {
            for (int i = 0; i < received.Count; ++i)
            {
                // send every thruster packet received
                message parsed_msg = new message(received[i]);
                if (parsed_msg.mtype == "thruster")
                {
                    tp = new thruster_packet(parsed_msg.whole);
                    port = tp.za;
                    star = tp.zb;
                    front = tp.xa;
                    back = tp.xb;
                    top = tp.ya;
                    bot = tp.yb;
                }
            }
        }


        rt.AddRelativeForce(new Vector3(0, 0, back));
        ft.AddRelativeForce(new Vector3(0, 0, front));
        pt.AddRelativeForce(new Vector3(0, 0, port));
        st.AddRelativeForce(new Vector3(0, 0, star));
        tt.AddRelativeForce(new Vector3(0, 0, top));
        bt.AddRelativeForce(new Vector3(0, 0, bot));
        /*
        if (rb.name == "RT")
        {
            rb.AddRelativeForce(new Vector3(0, 0, back));
        }
        if (rb.name == "FT")
        {
            rb.AddRelativeForce(new Vector3(0, 0, front));
        }
        if (rb.name == "PT")
        {
            rb.AddRelativeForce(new Vector3(0, 0, port));
        }
        if (rb.name == "ST")
        {
            rb.AddRelativeForce(new Vector3(0, 0, star));
        }
        if (rb.name == "TT")
        {
            rb.AddRelativeForce(new Vector3(0, 0, top));
        }
        if (rb.name == "BT")
        {
            rb.AddRelativeForce(new Vector3(0, 0, bot));
        }
        */
        //Vector3 force = new Vector3(1, 0, 0);
        //rb.AddForce(force);
        //rb.AddRelativeForce(force);
        //rb.AddRelativeTorque(force);
        //rb.AddTorque(force);

        //Vector3 force = new Vector3(hori * COEF, vert*COEF, forw*COEF);
        //Vector3 torq = new Vector3(pitch*COEF, yaw*COEF, roll*COEF);

        //rb.AddRelativeForce(force);
        //rb.AddRelativeTorque(torq);

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
