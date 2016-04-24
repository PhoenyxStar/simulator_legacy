using UnityEngine;
using System.Collections;
using System.IO;
using System;
using NetMQ;
using AsyncIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class thruster_packet
{
    JObject d;
    string JSON;
    public List<double> thrusters;
    public string whole;

    public thruster_packet(string raw)
    {
        d = JObject.Parse(raw);
        try
        {
            thrusters = new List<double>();

            foreach(var x in d)
            {
                thrusters.Add((double)x.Value);
            }
            whole = raw;

        }
        catch (Exception e)
        {
            LoggingSystem.log.Error("Thrustet packet: unable to parse string.\n");
            return;
        }
    }
    public thruster_packet(List<double> speeds)
    {
        d = new JObject();
        thrusters = speeds;

        for(int i=0; i<speeds.Count; ++i)
        {
            JProperty j = new JProperty(Convert.ToString(i), speeds[i]); 
            d.Add(j);
        }

        setupJSON();
    }

    void add_thruster(double val)
    {
        string s = Convert.ToString(thrusters.Count);
        JObject j = new JObject(s, val);

    }
    
    void setupJSON()
    {
        whole = d.ToString(Formatting.None);
    }
};
