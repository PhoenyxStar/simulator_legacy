using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class ThrusterModule : Module
{
    List<Thruster> thrusters;
    Rigidbody sub;

    protected override void init()
    {
        // load thrusters
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
                }
            }
        }

        // update physics
        foreach(Thruster thruster in thrusters)
            thruster.Update(sub.transform.position, sub.transform.localRotation, Time.deltaTime);
    }
}
