using UnityEngine;
using System;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public abstract class Module : MonoBehaviour
{
    protected string name;
    protected int fps;
    protected double dt;
    private double accumulator;
    protected Communicator com;
    protected static JObject settings;

    protected abstract void init();
    protected abstract void update();

    public void Init(string name)
    {
        // init defaults
        fps = 5;
        dt = 0;
        accumulator = 0;

        // setup communicator
        this.name = name;
        com = new Communicator();
        com.Initialize(name);

        // load settings
        string path = "../../settings/modules/" + name + ".json";
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);
        try
        {
            fps = (int)settings["fps"];
        }
        catch (Exception e)
        {
            Logger.log.Warn(e.Message);
        }

        // init derived
        init();
    }

    public void Update()
    {
        // safe enough if running at 60Hz
        accumulator += Time.unscaledDeltaTime;
        if(accumulator >= 1.0 / fps)
        {
            dt = accumulator;
            accumulator = 0; // reset to prevent spiral of death

            // update derived
            update();
        }
    }
}
