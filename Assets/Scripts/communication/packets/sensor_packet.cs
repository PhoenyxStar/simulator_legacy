using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class sensor_packet
{
    public double pitch;
    public double roll;
    public double yaw;
    public double depth;
    public double battery;
    public int start_switch;
    public double dt;
    public double dpitch;
    public double droll;
    public double dyaw;
    public double ddepth;

    public string JSON = "{ \"pitch\": 0,  \"roll\": 0,  \"yaw\":  0,  \"depth\": 0,  \"battery\": 0, \"start_switch\": 0, \"dt\": 0, \"dpitch\": 0,  \"droll\": 0,  \"dyaw\":  0,  \"ddepth\": 0 }";
    public string whole;

    public sensor_packet(string raw)
    {
        JObject d = JObject.Parse(raw);
        try
        {
            pitch = (double)d["pitch"];
            roll = (double)d["roll"];
            yaw = (double)d["yaw"];
            depth = (double)d["depth"];
            battery = (double)d["battery"];
            start_switch = (int)d["start_switch"];
            dt = (double)d["dt"];
            dpitch = (double)d["dpitch"];
            droll = (double)d["droll"];
            dyaw = (double)d["dyaw"];
            ddepth = (double)d["ddepth"];
            whole = raw;
        }
        catch (Exception e)
        {
            Debug.Log("Sensor packet exception: " + e.Message);
        }

    }

    public sensor_packet(double pitch, double roll, double yaw, double depth, double battery, int start_switch, double dt, double dpitch=0.0, double droll=0.0, double dyaw=0.0, double ddepth=0.0)
    {
        this.pitch = pitch;
        this.roll = roll;
        this.yaw = yaw;
        this.depth = depth;
        this.battery = battery;
        this.start_switch = start_switch;
        this.dt = dt;
        this.dpitch = dpitch;
        this.droll = droll;
        this.dyaw = dyaw;
        this.ddepth = ddepth;
        setupJSON();
    }

    void setupJSON()
    {
        JObject d = JObject.Parse(JSON);
        d["pitch"] = pitch;
        d["roll"] = roll;
        d["yaw"] = yaw;
        d["depth"] = depth;
        d["battery"] = battery;
        d["start_switch"] = start_switch;
        d["dt"] = dt;
        d["dpitch"] = dpitch;
        d["droll"] = droll;
        d["dyaw"] = dyaw;
        d["ddepth"] = ddepth;
        whole = d.ToString(Formatting.None);
    }
}
