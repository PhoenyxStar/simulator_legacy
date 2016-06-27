using UnityEngine;
using System;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public abstract class Module
{
    protected string name;
    protected int fps;
    protected float dt;
    private float accumulator;
    protected Communicator com;
    protected static JObject settings;

    protected abstract void init();
    protected abstract void update();
    protected abstract void shutdown();

    public void Init(string name)
    {
        // init defaults
        fps = 5;
        dt = 0;
        accumulator = 0;

        // setup communicator
        this.name = name;
        com = new Communicator(name);

        // load settings
        string path = "../settings/modules/" + name + ".json";
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);
        try
        {
            fps = (int)settings["fps"];
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // init derived
        init();
    }

    public void Update()
    {
        // safe enough if running at 60Hz
        accumulator += Time.unscaledDeltaTime;
        if(accumulator >= 1.0 / (float)fps)
        {
            dt = accumulator;
            accumulator = 0; // reset to prevent spiral of death

            // update derived
            update();
        }
    }

    public void Shutdown()
    {
        shutdown();
    }
}
