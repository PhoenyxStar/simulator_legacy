using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class SubPhysics : MonoBehaviour {
    Rigidbody rb;
    Rigidbody rt;
    Rigidbody ft;
    Rigidbody tt;
    Rigidbody bt;
    Rigidbody pt;
    Rigidbody st;
    int COEF = 1/125;
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
        rb.centerOfMass += new Vector3(0,0,-.01f);
        rb.drag = 0.9f;
        rb.angularDrag = 0.9f;
        comm.Initialize("thruster");
        rb.SetDensity(.97f);
        rb.useGravity = true;
        Physics.gravity = new Vector3(0, .3f, 0);

    }
    // Update is called once per frame
    void Update() {


        thruster_packet tp;
        List<string> received;
        float port = 0f;
        float star = 0f;
        float front = 0f;
        float back = 0f;
        float top = 0f;
        float bot = 0f;
        received = comm.receive_messages();
        foreach (string x in received)
        {
            loggingsystem.log.info(x);
        }
        if (received.count > 0)
        {
            for (int i = 0; i < received.count; ++i)
            {
                // send every thruster packet received
                message parsed_msg = new message(received[i]);
                if (parsed_msg.mtype == "thruster")
                {
                    tp = new thruster_packet(parsed_msg.value);
                    port = (float)tp.za;
                    star = (float)tp.zb;
                    front = (float)tp.xa;
                    back = (float)tp.xb;
                    top = (float)tp.ya;
                    bot = (float)tp.yb;
                }
            }
        }


        rt.AddRelativeForce(new Vector3(0, back*COEF, 0));
        ft.AddRelativeForce(new Vector3(0, front*COEF, 0));
        pt.AddRelativeForce(new Vector3(0, port*COEF, 0));
        st.AddRelativeForce(new Vector3(0, star*COEF, 0));
        tt.AddRelativeForce(new Vector3(0, 0, top*COEF));
        bt.AddRelativeForce(new Vector3(0, 0, bot*COEF));
    }



}
