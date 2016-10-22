using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class sensor_packet
{
    public double w;
    public double x;
    public double y;
    public double z;
    public double depth;
    public double battery;
    public int start_switch;
    public double dt;
    public double ddepth;

    public string JSON = "{ \"w\": 0,  \"x\": 0,  \"y\":  0, \"z\":  0,  \"depth\": 0,  \"battery\": 0, \"start_switch\": 0, \"dt\": 0,  \"ddepth\": 0 }";
    public string whole;

    public sensor_packet(string raw)
    {
        JObject d = JObject.Parse(raw);
        try
        {
            w = (double)d["w"];
            x = (double)d["x"];
            y = (double)d["y"];
            z = (double)d["z"];
            depth = (double)d["depth"];
            battery = (double)d["battery"];
            start_switch = (int)d["start_switch"];
            dt = (double)d["dt"];
            ddepth = (double)d["ddepth"];
            whole = raw;
        }
        catch (Exception e)
        {
            Debug.Log("Sensor packet exception: " + e.Message);
        }

    }

    public sensor_packet(double w, double x, double y, double z, double depth, double battery, int start_switch, double dt, double ddepth=0.0)
    {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
        this.depth = depth;
        this.battery = battery;
        this.start_switch = start_switch;
        this.dt = dt;
        this.ddepth = ddepth;
        setupJSON();
    }

    void setupJSON()
    {
        JObject d = JObject.Parse(JSON);
        d["w"] = w;
        d["x"] = x;
        d["y"] = y;
        d["z"] = z;
        d["depth"] = depth;
        d["battery"] = battery;
        d["start_switch"] = start_switch;
        d["dt"] = dt;
        d["ddepth"] = ddepth;
        whole = d.ToString(Formatting.None);
    }
}
