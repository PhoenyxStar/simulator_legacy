using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class ThrusterModule : Module
{
    List<Thruster> thrusters;
    Rigidbody rb;

    public ThrusterModule(Rigidbody rb)
    {
        this.rb = rb;
    }

    protected override void init()
    {
        // load thrusters
        thrusters = new List<Thruster>();
        string path = "../settings/modules/control.json";
        string jsonString = File.ReadAllText(path);
        JObject tsettings = JObject.Parse(jsonString);
        try
        {
            foreach (JToken token in tsettings["thrusters"].Children())
            {
                JProperty property = (JProperty)token;
                string name = property.Name;
                JObject thruster = (JObject)property.Value;
                Vector3 position = SubToUnity(new Vector3(
                    (float)thruster["position"]["x"],
                    (float)thruster["position"]["y"],
                    (float)thruster["position"]["z"]));
                Vector3 orientation = SubToUnity(new Vector3(
                    (float)thruster["orientation"]["x"],
                    (float)thruster["orientation"]["y"],
                    (float)thruster["orientation"]["z"]));
                thrusters.Add(new Thruster(name, position, orientation));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    protected override void update()
    {
        // get thruster packets
        List<string> received;
        received = com.receive_messages();
        if (received.Count > 0)
        {
            for (int i = 0; i < received.Count; ++i)
            {
                // send every thruster packet received
                message parsed_msg = new message(received[i]);
                if (parsed_msg.mtype == "thruster")
                {
                    thruster_packet tp = new thruster_packet(parsed_msg.value);
                    for(int j = 0; j < tp.thrusters.Count; ++j)
                        thrusters[j].SetThrusterPower((float)tp.thrusters[j]);
                    com.send_message(new message("thruster", "control_gui", "thruster", parsed_msg.value));
                }
            }
        } else {
            for(int j = 0; j < thrusters.Count; ++j)
                thrusters[j].SetThrusterPower(0.0f); // zero thruster if no message
        }

        // update physics
        foreach(var thruster in thrusters)
        {
            thruster.Update(rb.transform.position, rb.transform.localRotation, dt);
            rb.AddForceAtPosition(thruster.WorldThrust, thruster.WorldPosition);
        }

        // draw force vectors
        foreach(var thruster in thrusters)
            Debug.DrawRay(thruster.WorldPosition, -thruster.WorldThrust * 0.05f, Color.red);
    }

    protected override void shutdown()
    {

    }

    Vector3 SubToUnity(Vector3 vec)
    {
        return new Vector3(-vec.y, vec.z, vec.x);
    }
}
