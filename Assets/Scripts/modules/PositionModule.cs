using System;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PositionModule : Module
{
    // sub physics
    private Rigidbody rb;
    public string JSON = "{ \"pos_x\": 0,  \"pos_y\": 0,  \"pos_z\":  0, \"vel_x\": 0,  \"vel_y\": 0,  \"vel_z\":  0 }";
    Vector3 last_pos;

    public PositionModule(Rigidbody rb)
    {
        this.rb = rb;
    }

    protected override void init()
    {
        last_pos = new Vector3(0.0f,0.0f,0.0f);
    }

    protected override void update()
	{
        // TODO: Add a list of objects to get positions of and
        // define a message for it's output to ros
        JObject d = JObject.Parse(JSON);

        Vector3 pos = UnityToSubCoords(rb.position);

        // I'm calculating velocity this way because rb.velocity
        // only takes into account some velocity changes (yea...)
        // This results in pretty a pretty jumpy velocity output
        // The rb.velocity output was just as jumpy if you're wondering
        Vector3 vel = (pos - last_pos) / Time.deltaTime;
        last_pos = pos;

        //Debug.Log(pos);
        //Debug.Log(vel);

        d["pos_x"] = pos.x;
        d["pos_y"] = pos.y;
        d["pos_z"] = pos.z;

        d["vel_x"] = vel.x;
        d["vel_y"] = vel.y;
        d["vel_z"] = vel.z;

        string whole = d.ToString(Formatting.None);
        com.send_message(whole);
    }

    protected override void shutdown()
    {
    }

    Vector3 UnityToSubCoords(Vector3 vec)
    {
        return new Vector3(vec.z, -vec.x, vec.y);
    }

}
