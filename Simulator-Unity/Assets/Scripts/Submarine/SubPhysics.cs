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
    float port = 0f;
    float star = 0f;
    float front = 0f;
    float back = 0f;
    float top = 0f;
    float bot = 0f;
    [SerializeField]
    float MaxMotorThrust;
    [SerializeField]
    float MaxThrusterInput;
    [SerializeField]
    float SubMass;
    //[SerializeField]
    //Vector3 CenterOfMass;
    [SerializeField]
    float Drag;
    //[SerializeField]
    int[] ThrusterReversals = {1, 1, 1, 1, 1, -1}; // xa, xb, ya, yb, za, zb
    //[SerializeField]
    Vector3 CenterOfBouyancy;
    [SerializeField]
    bool ThrustersOn;
    [SerializeField]
    bool BouyancyOn;
    [SerializeField]
    bool GravityOn;
    float COEF = 0.0f;
    string Joystick;

    Communicator comm;

    // Use this for initialization
    void Start () {
		GameObject body = GameObject.Find ("SubCenter");
		rb = GetComponent<Rigidbody> ();

		rt = GameObject.Find ("RT").GetComponent<Rigidbody> ();
		ft = GameObject.Find ("FT").GetComponent<Rigidbody> ();
		tt = GameObject.Find ("TT").GetComponent<Rigidbody> ();
		bt = GameObject.Find ("BT").GetComponent<Rigidbody> ();
		pt = GameObject.Find ("PT").GetComponent<Rigidbody> ();
		st = GameObject.Find ("ST").GetComponent<Rigidbody> ();

        CenterOfBouyancy = rb.centerOfMass;
        CenterOfBouyancy.y += 0.1f;

		rb.drag = Drag;
		rb.angularDrag = Drag;

        Physics.gravity = new Vector3(0,0,0);

		rb.SetDensity (1.0f);
        //rb.useGravity = true;
        rb.mass = SubMass;

        COEF = MaxMotorThrust / MaxThrusterInput;

		//if (GlobalManager.Instance.enableConnection) {
  //          InitCommunicator();
  //      }

        LoggingSystem.log.Info("Starting SubPhysics");
    }

    public void InitCommunicator()
    {
        comm = new Communicator();
        comm.Initialize("thruster");
    }

    // Update is called once per frame
    void Update () {
<<<<<<< HEAD
		if (GlobalManager.Instance.enableConnection) {

            if (comm == null)
            {
                InitCommunicator();
            }
			thruster_packet tp;
			List<string> received;

			received = comm.receive_messages ();
			foreach (string x in received) {
				LoggingSystem.log.Info (x);
			}
			if (received.Count > 0) {
				for (int i = 0; i < received.Count; ++i) {
					// send every thruster packet received
					message parsed_msg = new message (received [i]);
					if (parsed_msg.mtype == "thruster") {
						tp = new thruster_packet (parsed_msg.value);
						port = (float)tp.za;
						star = (float)tp.zb;
						front = (float)tp.xa;
						back = (float)tp.xb;
						top = (float)tp.ya;
						bot = (float)tp.yb;
						//LoggingSystem.log.Info (port);
						//LoggingSystem.log.Info (star);
						//LoggingSystem.log.Info (front);
						//LoggingSystem.log.Info (back);
						//LoggingSystem.log.Info (top);
						//LoggingSystem.log.Info (bot);
					}
				}
			}
		}
=======
        if(ThrustersOn)
            UpdateThrusters();
        if(BouyancyOn)
            UpdateBouyancy();
        if(GravityOn)
            UpdateGravity();
    }
>>>>>>> sub_rework

    void UpdateThrusters()
    {
        if (GlobalManager.Instance.enableConnection) {
            thruster_packet tp;
            List<string> received;

            received = comm.receive_messages ();
            //foreach (string x in received) {
            //LoggingSystem.log.Info (x);
            //}
            if (received.Count > 0) {
                for (int i = 0; i < received.Count; ++i) {
                    // send every thruster packet received
                    message parsed_msg = new message (received [i]);
                    if (parsed_msg.mtype == "thruster") {
                        tp = new thruster_packet (parsed_msg.value);
                        port = (float)tp.za;
                        star = (float)tp.zb;
                        front = (float)tp.xa;
                        back = (float)tp.xb;
                        top = (float)tp.ya;
                        bot = (float)tp.yb;
                        port *= ThrusterReversals[4]; // za
                        star *= ThrusterReversals[5]; // za
                        front *= ThrusterReversals[0]; // xa
                        back *= ThrusterReversals[1]; // xa
                        top *= ThrusterReversals[2]; // ya
                        bot *= ThrusterReversals[3]; // yb
                    }
                }
            }
        }

        Vector3 rt_pos = new Vector3(0, 0, (float)-0.4);
        Vector3 ft_pos = new Vector3(0, 0, (float)0.3);
        Vector3 pt_pos = new Vector3((float)-0.2, 0, 0);
        Vector3 st_pos = new Vector3((float)0.2, 0, 0);
        Vector3 tt_pos = new Vector3(0, (float)0.2, 0);
        Vector3 bt_pos = new Vector3(0, (float)-0.2, 0);

        rt_pos = rb.transform.localRotation * rt_pos;
        ft_pos = rb.transform.localRotation * ft_pos;
        pt_pos = rb.transform.localRotation * pt_pos;
        st_pos = rb.transform.localRotation * st_pos;
        tt_pos = rb.transform.localRotation * tt_pos;
        bt_pos = rb.transform.localRotation * bt_pos;

        Vector3 rt_pos_world = rb.transform.position + rt_pos;
        Vector3 ft_pos_world = rb.transform.position + ft_pos;
        Vector3 pt_pos_world = rb.transform.position + pt_pos;
        Vector3 st_pos_world = rb.transform.position + st_pos;
        Vector3 tt_pos_world = rb.transform.position + tt_pos;
        Vector3 bt_pos_world = rb.transform.position + bt_pos;

        Vector3 rt_force = new Vector3((float)(1.0*back*COEF), 0, 0);
        Vector3 ft_force = new Vector3((float)(1.0*front*COEF), 0, 0);
        Vector3 pt_force = new Vector3(0, (float)(1.0*port*COEF), 0);
        Vector3 st_force = new Vector3(0, (float)(1.0*star*COEF), 0);
        Vector3 tt_force = new Vector3(0, 0, (float)(1.0*top*COEF));
        Vector3 bt_force = new Vector3(0, 0, (float)(1.0*bot*COEF));

        rt_force = rb.transform.localRotation * rt_force;
        ft_force = rb.transform.localRotation * ft_force;
        pt_force = rb.transform.localRotation * pt_force;
        st_force = rb.transform.localRotation * st_force;
        tt_force = rb.transform.localRotation * tt_force;
        bt_force = rb.transform.localRotation * bt_force;

        rb.AddForceAtPosition(rt_force, rt_pos_world);
        rb.AddForceAtPosition(ft_force, ft_pos_world);
        rb.AddForceAtPosition(pt_force, pt_pos_world);
        rb.AddForceAtPosition(st_force, st_pos_world);
        rb.AddForceAtPosition(tt_force, tt_pos_world);
        rb.AddForceAtPosition(bt_force, bt_pos_world);
    }

    void UpdateBouyancy()
    {
        float depth = GetDepth();
        float ForceOfBouyancy = 1.0f;
        Vector3 force = new Vector3(0, 0, 0);

        if(depth < 0.0f) 
        {
            force = Vector3.up * (9.8f + ForceOfBouyancy) * rb.mass;
        }
        else if(depth < 0.2)
        {
            // Decrease bouyancy as the sub gets close to the surface
            force = (depth) * (Vector3.up * (9.8f + ForceOfBouyancy) * rb.mass);
        }

        Vector3 COB_World = rb.transform.localRotation * CenterOfBouyancy;
        COB_World += rb.transform.position;

        rb.AddForceAtPosition(force, COB_World);
    }

    void UpdateGravity()
    {
        // Not sure why but setting global physics to 0 and then
        // just emulating physics on the sub looks and acts much
        // better.
        rb.AddForce(new Vector3(0, -9.8f * rb.mass, 0));
    }

    float GetDepth()
    {
        GameObject water = GameObject.Find("WaterTop");

        float waterTop = ((Transform)water.GetComponent("Transform")).position.y;
        float subCenter = rb.position.y;

        return subCenter - waterTop;
    }
}
