using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using Emgu.CV;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class CameraModule : Module
{
    [DllImport ("SharedImage")]
    unsafe private static extern int Write(string name, IntPtr data, int size);

    Dictionary<string,Capture> cameras;

    protected override void init()
    {
        // load cameras
    }

    protected override void update()
    {
        foreach(KeyValuePair<string,Capture> iter in cameras)
        {
            byte[] frame = iter.Value.Read();
            int size = Marshal.SizeOf(frame[0]) * frame.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(frame, 0, ptr, frame.Length);
            Write(iter.Key, ptr, size);
        }
    }
}
