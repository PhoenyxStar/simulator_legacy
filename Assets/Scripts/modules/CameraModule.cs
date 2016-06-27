using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

public class CameraModule : Module
{
    [DllImport ("libSharedImage")]
    unsafe private static extern void ShowImage(string name, int rows, int cols, IntPtr buf);

    Dictionary<string,Capture> cameras;

    public CameraModule()
    {
        cameras = new Dictionary<string,Capture>();
    }

    protected override void init()
    {
        try
        {
            foreach (JToken token in settings["cameras"].Children())
            {
                JProperty property = (JProperty)token;
                string name = property.Name;
                JObject camera = (JObject)property.Value;
                Capture c = new Capture(name);
                cameras.Add(name, c);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    protected override void update()
    {
        foreach(KeyValuePair<string,Capture> iter in cameras)
        {
            byte[] frame = iter.Value.Read();
            IntPtr ptr = Marshal.AllocHGlobal(frame.Length);
            Marshal.Copy(frame, 0, ptr, frame.Length);
            ShowImage(iter.Key, iter.Value.height, iter.Value.width, ptr);
            int ret = 0;
            switch(ret)
            {
                case -1:
                    Debug.Log("Failed to open SHM:" + iter.Key);
                    break;

                case -2:
                    Debug.Log("Failed to open SEM:" + iter.Key);
                    break;

                case -3:
                    Debug.Log("Failed to map SHM:" + iter.Key);
                    break;
            }
        }
    }

    protected override void shutdown()
    {
        foreach(KeyValuePair<string,Capture> iter in cameras)
        {
            //ShutdownShared(iter.Key);
        }
    }
}
