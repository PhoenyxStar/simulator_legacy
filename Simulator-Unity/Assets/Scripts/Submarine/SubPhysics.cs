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

   // Communicator comm = new Communicator();

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rt = GameObject.Find("RT").GetComponent<Rigidbody>();
        ft = GameObject.Find("FT").GetComponent<Rigidbody>();
        tt = GameObject.Find("TT").GetComponent<Rigidbody>();
        bt = GameObject.Find("BT").GetComponent<Rigidbody>();
        pt = GameObject.Find("PT").GetComponent<Rigidbody>();
        st = GameObject.Find("ST").GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0,0,-.25f);
        rb.drag = 0.75f;
        rb.angularDrag = 0.75f;
        //comm.Initialize("thruster");
        rb.SetDensity(.97f);

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
        //received = comm.receive_messages();
        string path = "Assets/settings/modules/thruster.json";
        string jsonString = File.ReadAllText(path);
        try {
            JObject thrust = JObject.Parse(jsonString);
            string temp = (string)thrust["value"];

            JObject jtemp = JObject.Parse(temp);
            front = (int)jtemp["xa"];
            port = (int)jtemp["za"];
            star = (int)jtemp["zb"];
            back = (int)jtemp["xb"];
            top = (int)jtemp["ya"];
            bot = (int)jtemp["yb"];
        }
        catch (Exception e)
        {
            LoggingSystem.log.Error("Thruster packet: unable to parse string.\n");
            return;
        }


        rt.AddRelativeForce(new Vector3(0, back*COEF, 0));
        ft.AddRelativeForce(new Vector3(0, front*COEF, 0));
        pt.AddRelativeForce(new Vector3(0, port*COEF, 0));
        st.AddRelativeForce(new Vector3(0, star*COEF, 0));
        tt.AddRelativeForce(new Vector3(0, 0, top*COEF));
        bt.AddRelativeForce(new Vector3(0, 0, bot*COEF));
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3 force = new Vector3(0, 5, 0);
            rt.AddRelativeForce(force);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3 force = new Vector3(0, -5, 0);
            rt.AddRelativeForce(force);
        }
    }



}
