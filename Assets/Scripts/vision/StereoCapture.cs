using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class StereoCapture
{
    [DllImport ("libSharedImage")]
    unsafe private static extern int Range(string name, int width, int height, float baseline, float focal_length, IntPtr left, IntPtr right);

    Capture left;
	Capture right;
	string name;
	int fps;
	float baseline;
	float focal_length;

    public StereoCapture(string name, int fps, Capture left, Capture right)
    {
		this.name = name;
		this.fps = fps;
		this.left = left;
		this.right = right;
		baseline = Math.Abs(left.transform.position.x - right.transform.position.x);
		focal_length = 1000;
    }

    public void Update()
    {
		// grab framebuffers
		byte[] lbuf = left.GetFrame();
		byte[] rbuf = right.GetFrame();

		// create unmanaged memory
		IntPtr lptr = Marshal.AllocHGlobal(lbuf.Length);
		IntPtr rptr = Marshal.AllocHGlobal(rbuf.Length);

		// copy data
		Marshal.Copy(lbuf, 0, lptr, lbuf.Length);
		Marshal.Copy(rbuf, 0, rptr, rbuf.Length);

		// save to shared memory
		//Range(name, left.width, left.height, baseline, focal_length, lptr, rptr);
    }
}
