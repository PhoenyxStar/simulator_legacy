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
    float COEF = 1/50f;
    string Joystick;

    Communicator comm;

    // Use this for initialization
    void Start () {
		GameObject body = GameObject.Find ("SubBody");

		rb = GetComponent<Rigidbody> ();
		rt = GameObject.Find ("RT").GetComponent<Rigidbody> ();
		ft = GameObject.Find ("FT").GetComponent<Rigidbody> ();
		tt = GameObject.Find ("TT").GetComponent<Rigidbody> ();
		bt = GameObject.Find ("BT").GetComponent<Rigidbody> ();
		pt = GameObject.Find ("PT").GetComponent<Rigidbody> ();
		st = GameObject.Find ("ST").GetComponent<Rigidbody> ();

		rb.centerOfMass += new Vector3 (0, 0, -.01f);
		rb.drag = 0.9f;
		rb.angularDrag = 0.9f;
		rb.SetDensity (1.0f);
		rb.useGravity = true;
		Physics.gravity = new Vector3 (0, 0, 0);

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

        //rt.AddRelativeForce(new Vector3(0, 0, back*COEF));
        //ft.AddRelativeForce(new Vector3(0, 0, front*COEF));
        //pt.AddRelativeForce(new Vector3(0, port*COEF, 0));
        //st.AddRelativeForce(new Vector3(0, star*COEF, 0));
        //tt.AddRelativeForce(new Vector3(0, 0, top*COEF));
        //bt.AddRelativeForce(new Vector3(0, 0, bot*COEF));

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

        Vector3 rt_pos_world = rb.transform.position - rt_pos;
        Vector3 ft_pos_world = rb.transform.position - ft_pos;
        Vector3 pt_pos_world = rb.transform.position - pt_pos;
        Vector3 st_pos_world = rb.transform.position - st_pos;
        Vector3 tt_pos_world = rb.transform.position - tt_pos;
        Vector3 bt_pos_world = rb.transform.position - bt_pos;

        //LoggingSystem.log.Info("Thruster Positions:");
        //LoggingSystem.log.Info("RT: " + rt_pos_world);
        //LoggingSystem.log.Info("FT: " + ft_pos_world);
        //LoggingSystem.log.Info("PT: " + pt_pos_world);
        //LoggingSystem.log.Info("ST: " + st_pos_world);
        //LoggingSystem.log.Info("TT: " + tt_pos_world);
        //LoggingSystem.log.Info("BT: " + bt_pos_world);

        Vector3 rt_force = new Vector3((float)(1.0*back*COEF), 0, 0);
        Vector3 ft_force = new Vector3((float)(1.0*front*COEF), 0, 0);
        Vector3 pt_force = new Vector3(0, (float)(1.0*port*COEF), 0);
        Vector3 st_force = new Vector3(0, (float)(1.0*star*COEF), 0);
        Vector3 tt_force = new Vector3(0, 0, (float)(1.0*top*COEF));
        Vector3 bt_force = new Vector3(0, 0, (float)(1.0*bot*COEF));

        rb.AddForceAtPosition(rt_force, rt_pos_world);
        rb.AddForceAtPosition(ft_force, ft_pos_world);
        rb.AddForceAtPosition(pt_force, pt_pos_world);
        rb.AddForceAtPosition(st_force, st_pos_world);
        rb.AddForceAtPosition(tt_force, tt_pos_world);
        rb.AddForceAtPosition(bt_force, bt_pos_world);

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
