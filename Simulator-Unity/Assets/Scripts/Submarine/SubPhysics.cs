using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

// TODO: Correct spelling. Buoyancy not bouyancy.

public class SubPhysics : MonoBehaviour {
    Rigidbody rb;

    public float port = 0f;
    public float star = 0f;
    public float front = 0f;
    public float back = 0f;
    public float top = 0f;
    public float bot = 0f;

    [SerializeField]
    float MaxThrusterInput;
    [SerializeField]
    float Drag;
    [SerializeField]
    float AngularDrag;
    [SerializeField]
    float Thruster_Variance;
    [SerializeField]
    bool ThrustersOn;
    [SerializeField]
    bool BouyancyOn;
    [SerializeField]
    bool GravityOn;
    [SerializeField]
    bool ThrusterDebugOn;

    float SubMass;
    float MaxMotorForce;

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

    Thruster rear_thruster;
    Thruster front_thruster;
    Thruster port_thruster;
    Thruster star_thruster;
    Thruster top_thruster;
    Thruster bot_thruster;

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

        rear_thruster = new Thruster("rear", rt_pos_rel, rt_orient_rel, MaxThrusterInput, MaxMotorForce);
        front_thruster = new Thruster("front", ft_pos_rel, ft_orient_rel, MaxThrusterInput, MaxMotorForce);
        port_thruster = new Thruster("port", pt_pos_rel, pt_orient_rel, MaxThrusterInput, MaxMotorForce);
        star_thruster = new Thruster("star", st_pos_rel, st_orient_rel, MaxThrusterInput, MaxMotorForce);
        top_thruster = new Thruster("top", tt_pos_rel, tt_orient_rel, MaxThrusterInput, MaxMotorForce);
        bot_thruster = new Thruster("bot", bt_pos_rel, bt_orient_rel, MaxThrusterInput, MaxMotorForce);

        rb.mass = SubMass;
        COEF = MaxMotorForce / MaxThrusterInput;

        LoggingSystem.log.Info("Starting SubPhysics");
    }

    public void InitCommunicator()
    {
        comm = new Communicator();
        comm.Initialize("thruster");
    }

    // Update is called once per frame
    void FixedUpdate () {
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
                        front = (float)tp.thrusters[0];
                        back = (float)tp.thrusters[1];
                        port = (float)tp.thrusters[2];
                        star = (float)tp.thrusters[3];
                        top = (float)tp.thrusters[4];
                        bot = (float)tp.thrusters[5];

                        rear_thruster.SetThrusterPower(back);
                        front_thruster.SetThrusterPower(front);
                        port_thruster.SetThrusterPower(port);
                        star_thruster.SetThrusterPower(star);
                        top_thruster.SetThrusterPower(top);
                        bot_thruster.SetThrusterPower(bot);
                    }                                                        
                }
            }
        }

        // TODO: Change dt
        rear_thruster.Update(rb.transform.position, rb.transform.localRotation, 0.02f);
        front_thruster.Update(rb.transform.position, rb.transform.localRotation, 0.02f);
        port_thruster.Update(rb.transform.position, rb.transform.localRotation, 0.02f);
        star_thruster.Update(rb.transform.position, rb.transform.localRotation, 0.02f);
        top_thruster.Update(rb.transform.position, rb.transform.localRotation, 0.02f);
        bot_thruster.Update(rb.transform.position, rb.transform.localRotation, 0.02f);

        rb.AddForceAtPosition(rear_thruster.WorldThrust, rear_thruster.WorldPosition);
        rb.AddForceAtPosition(front_thruster.WorldThrust, front_thruster.WorldPosition);
        rb.AddForceAtPosition(port_thruster.WorldThrust, port_thruster.WorldPosition);
        rb.AddForceAtPosition(star_thruster.WorldThrust, star_thruster.WorldPosition);
        rb.AddForceAtPosition(top_thruster.WorldThrust, top_thruster.WorldPosition);
        rb.AddForceAtPosition(bot_thruster.WorldThrust, bot_thruster.WorldPosition);

        if(ThrusterDebugOn)
        {
            // Draw thruster output
            Debug.DrawRay(rear_thruster.WorldPosition, -rear_thruster.WorldThrust * 0.1f, Color.red);
            Debug.DrawRay(front_thruster.WorldPosition, -front_thruster.WorldThrust * 0.1f, Color.red);
            Debug.DrawRay(port_thruster.WorldPosition, -port_thruster.WorldThrust * 0.1f, Color.blue);
            Debug.DrawRay(star_thruster.WorldPosition, -star_thruster.WorldThrust * 0.1f, Color.blue);
            Debug.DrawRay(top_thruster.WorldPosition, -top_thruster.WorldThrust * 0.1f, Color.green);
            Debug.DrawRay(bot_thruster.WorldPosition, -bot_thruster.WorldThrust * 0.1f, Color.green);
        }
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
        MaxMotorForce = (float)settings["max_thrust"];
    }

    Vector3 SubToUnity(Vector3 vec)
    {
        return new Vector3(-vec.y, vec.z, vec.x);
    }
}
