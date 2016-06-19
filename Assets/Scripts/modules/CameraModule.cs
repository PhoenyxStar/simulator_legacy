using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

public class CameraModule : Module
{
    [DllImport ("SharedImage")]
    unsafe private static extern void SharedImage(string name, int width, int height, IntPtr data);

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
            int size = Marshal.SizeOf(frame[0]) * frame.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(frame, 0, ptr, frame.Length);
            SharedImage(iter.Key, iter.Value.width, iter.Value.height, ptr);
        }
    }
}
