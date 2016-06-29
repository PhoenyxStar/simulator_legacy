using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

public class CameraModule : Module
{
    [DllImport ("libSharedImage")]
    unsafe private static extern int UpdateShared(string name, int rows, int cols, IntPtr buf);
    [DllImport ("libSharedImage")]
    unsafe private static extern int ShutdownShared(string name);

    Dictionary<string,Capture> cameras;

    public CameraModule()
    {
        cameras = new Dictionary<string,Capture>();
    }

    protected override void init()
    {
        try
        {
            foreach (JToken token in settings["monocameras"].Children())
            {
                JProperty property = (JProperty)token;
                string name = property.Name;
                JObject camera = (JObject)property.Value;
                cameras.Add(name, GameObject.Find(name).GetComponent<Capture>());
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    protected override void update()
    {
        /*
        foreach(KeyValuePair<string,Capture> iter in cameras)
        {
            byte[] frame = iter.Value.GetFrame();
            IntPtr ptr = Marshal.AllocHGlobal(frame.Length);
            Marshal.Copy(frame, 0, ptr, frame.Length);
            UpdateShared(iter.Key, iter.Value.height, iter.Value.width, ptr);
        }
        */
    }

    protected override void shutdown()
    {
        foreach(KeyValuePair<string,Capture> iter in cameras)
        {
            //ShutdownShared(iter.Key);
        }
    }
}
