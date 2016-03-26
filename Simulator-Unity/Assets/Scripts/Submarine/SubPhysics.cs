using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

// TODO: Correct spelling. Buoyancy not bouyancy.

public class SubPhysics : MonoBehaviour {
    Rigidbody rb;

    float port = 0f;
    float star = 0f;
    float front = 0f;
    float back = 0f;
    float top = 0f;
    float bot = 0f;

    [SerializeField]
    float MaxThrusterInput;
    [SerializeField]
    float Drag;
    [SerializeField]
    float AngularDrag;
    [SerializeField]
    bool ThrustersOn;
    [SerializeField]
    bool BouyancyOn;
    [SerializeField]
    bool GravityOn;

    float SubMass;
    float MaxMotorThrust;

    Vector3 CenterOfBouyancy;
    float COEF = 0.0f;
    string Joystick;
    public JObject settings;

    Vector3 rt_pos_rel;
    Vector3 ft_pos_rel;
    Vector3 pt_pos_rel;
    Vector3 st_pos_rel;
    Vector3 tt_pos_rel;
    Vector3 bt_pos_rel;

    Vector3 rt_orient_rel;
    Vector3 ft_orient_rel;
    Vector3 pt_orient_rel;
    Vector3 st_orient_rel;
    Vector3 tt_orient_rel;
    Vector3 bt_orient_rel;

    Communicator comm;

    // Use this for initialization
    void Start () {
		GameObject body = GameObject.Find ("SubBody");
		rb = GetComponent<Rigidbody> ();

        CenterOfBouyancy = rb.centerOfMass;
        CenterOfBouyancy.y += 0.1f;

		rb.drag = Drag;
		rb.angularDrag = AngularDrag;

        Physics.gravity = new Vector3(0,0,0);

		rb.SetDensity (1.0f);
        //rb.useGravity = true;

		if (GlobalManager.Instance.enableConnection) {
			comm = new Communicator ();
			comm.Initialize ("thruster");
		}

        // load settings file
        string path = "../../robosub/settings/modules/control.json";
        if(!File.Exists(path))
        {
            path = "Assets/settings/modules/control.json";
        }
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);

        GetSettings();

        rb.mass = SubMass;
        COEF = MaxMotorThrust / MaxThrusterInput;

        LoggingSystem.log.Info("Starting SubPhysics");
    }

    public void InitCommunicator()
    {
        comm = new Communicator();
        comm.Initialize("thruster");
    }

    // Update is called once per frame
    void Update () {
        if(ThrustersOn)
            UpdateThrusters();
        if(BouyancyOn)
            UpdateBouyancy();
        if(GravityOn)
            UpdateGravity();
    }

    void UpdateThrusters()
    {
        if (GlobalManager.Instance.enableConnection) {
            thruster_packet tp;
            List<string> received;

            received = comm.receive_messages ();
            if (received.Count > 0) {
                for (int i = 0; i < received.Count; ++i) {
                    // send every thruster packet received
                    message parsed_msg = new message (received [i]);
                    if (parsed_msg.mtype == "thruster") {
                        tp = new thruster_packet (parsed_msg.value);
                        port = (float)tp.ya;
                        star = (float)tp.yb;
                        front = (float)tp.xa;
                        back = (float)tp.xb;
                        top = (float)tp.za;
                        bot = (float)tp.zb;
                    }
                }
            }
        }

        // Rotate base thruster positions according to current sub orientation
        Vector3 rt_pos = rb.transform.localRotation * rt_pos_rel;
        Vector3 ft_pos = rb.transform.localRotation * ft_pos_rel;
        Vector3 pt_pos = rb.transform.localRotation * pt_pos_rel;
        Vector3 st_pos = rb.transform.localRotation * st_pos_rel;
        Vector3 tt_pos = rb.transform.localRotation * tt_pos_rel;
        Vector3 bt_pos = rb.transform.localRotation * bt_pos_rel;

        // Transform rotated thruster postions into world thruster positions
        Vector3 rt_pos_world = rb.transform.position + rt_pos;
        Vector3 ft_pos_world = rb.transform.position + ft_pos;
        Vector3 pt_pos_world = rb.transform.position + pt_pos;
        Vector3 st_pos_world = rb.transform.position + st_pos;
        Vector3 tt_pos_world = rb.transform.position + tt_pos;
        Vector3 bt_pos_world = rb.transform.position + bt_pos;

        // Calculate force vector for each thruster based on orientation in settings file
        Vector3 rt_force = rt_orient_rel * back * COEF;
        Vector3 ft_force = ft_orient_rel * front * COEF;
        Vector3 pt_force = pt_orient_rel * port * COEF;
        Vector3 st_force = st_orient_rel * star * COEF;
        Vector3 tt_force = tt_orient_rel * top * COEF;
        Vector3 bt_force = bt_orient_rel * bot * COEF;

        // Rotate force vector according to current sub orientation
        rt_force = rb.transform.localRotation * rt_force;
        ft_force = rb.transform.localRotation * ft_force;
        pt_force = rb.transform.localRotation * pt_force;
        st_force = rb.transform.localRotation * st_force;
        tt_force = rb.transform.localRotation * tt_force;
        bt_force = rb.transform.localRotation * bt_force;

        // Make thrusters go
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

        // Calculate force of bouyancy
        if(depth < 0.0f) 
        {
            force = Vector3.up * (9.8f + ForceOfBouyancy) * rb.mass;
        }
        else if(depth < 0.2)
        {
            // Decrease bouyancy as the sub gets close to the surface
            force = (depth) * (Vector3.up * (9.8f + ForceOfBouyancy) * rb.mass);
        }

        // Transform relative Center of Bouyancy to world
        Vector3 COB_World = rb.transform.localRotation * CenterOfBouyancy;
        COB_World += rb.transform.position;

        // Add bouyancy
        rb.AddForceAtPosition(force, COB_World);
    }

    void UpdateGravity()
    {
        // Not sure why but setting global gravity to 0 and then
        // just emulating gravity on the sub looks and acts much
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

    void GetSettings()
    {
        rt_pos_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["back"]["position"]["x"], 
                    (float)settings["thrusters"]["back"]["position"]["y"], 
                    (float)settings["thrusters"]["back"]["position"]["z"]));
        ft_pos_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["front"]["position"]["x"], 
                    (float)settings["thrusters"]["front"]["position"]["y"], 
                    (float)settings["thrusters"]["front"]["position"]["z"]));
        pt_pos_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["left"]["position"]["x"], 
                    (float)settings["thrusters"]["left"]["position"]["y"], 
                    (float)settings["thrusters"]["left"]["position"]["z"]));
        st_pos_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["right"]["position"]["x"], 
                    (float)settings["thrusters"]["right"]["position"]["y"], 
                    (float)settings["thrusters"]["right"]["position"]["z"]));
        tt_pos_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["top"]["position"]["x"], 
                    (float)settings["thrusters"]["top"]["position"]["y"], 
                    (float)settings["thrusters"]["top"]["position"]["z"]));
        bt_pos_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["bottom"]["position"]["x"], 
                    (float)settings["thrusters"]["bottom"]["position"]["y"], 
                    (float)settings["thrusters"]["bottom"]["position"]["z"]));

        rt_orient_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["back"]["orientation"]["x"], 
                    (float)settings["thrusters"]["back"]["orientation"]["y"], 
                    (float)settings["thrusters"]["back"]["orientation"]["z"]));
        ft_orient_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["front"]["orientation"]["x"], 
                    (float)settings["thrusters"]["front"]["orientation"]["y"], 
                    (float)settings["thrusters"]["front"]["orientation"]["z"]));
        pt_orient_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["left"]["orientation"]["x"], 
                    (float)settings["thrusters"]["left"]["orientation"]["y"], 
                    (float)settings["thrusters"]["left"]["orientation"]["z"]));
        st_orient_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["right"]["orientation"]["x"], 
                    (float)settings["thrusters"]["right"]["orientation"]["y"], 
                    (float)settings["thrusters"]["right"]["orientation"]["z"]));
        tt_orient_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["top"]["orientation"]["x"], 
                    (float)settings["thrusters"]["top"]["orientation"]["y"], 
                    (float)settings["thrusters"]["top"]["orientation"]["z"]));
        bt_orient_rel = SubToUnity(new Vector3(
                    (float)settings["thrusters"]["bottom"]["orientation"]["x"], 
                    (float)settings["thrusters"]["bottom"]["orientation"]["y"], 
                    (float)settings["thrusters"]["bottom"]["orientation"]["z"]));

        SubMass = (float)settings["mass"];
        MaxMotorThrust = (float)settings["max_thrust"];
    }

    Vector3 SubToUnity(Vector3 vec)
    {
        return new Vector3(-vec.y, vec.z, vec.x);
    }
}
