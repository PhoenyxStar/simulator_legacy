using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            Logger.log.Error("Thruster packet exception: " + e);
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
        // add to json string
    }

    void setupJSON()
    {
        whole = d.ToString(Formatting.None);
    }
};
