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

    Dictionary<string,Capture> monocameras;
	Dictionary<string,StereoCapture> stereocameras;

    public CameraModule()
    {
        monocameras = new Dictionary<string,Capture>();
		stereocameras = new Dictionary<string,StereoCapture>();
    }

    protected override void init()
    {
        try
        {
			// load monocameras
            foreach (JToken token in settings["monocameras"].Children())
            {
                JProperty property = (JProperty)token;
                string name = property.Name;
                JObject camera = (JObject)property.Value;
                monocameras.Add(name, GameObject.Find(name).GetComponent<Capture>());
            }

			// load stereocameras
			foreach (JToken token in settings["stereocameras"].Children())
            {
                JProperty property = (JProperty)token;
                string name = property.Name;
                JArray camera = (JArray)property.Value;
				string[] cams = camera.ToObject<string[]>();
				string left = (string)cams[0];
				string right = (string)cams[1];
                stereocameras.Add(name, new StereoCapture(name, fps, monocameras[left], monocameras[right]));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    protected override void update()
    {
		// update stereocameras
		foreach(var stereo in stereocameras)
		{
			stereo.Value.Update();
		}
    }

    protected override void shutdown()
    {

    }
}
