using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using Emgu.CV;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class SharedMem : MonoBehaviour
{
    [DllImport ("RSSharedMem")]
    unsafe private static extern int PlacePNGInSharedMemory(int cam_id, IntPtr img_bytes, int img_bytes_len);
    [DllImport ("RSSharedMem")]
    unsafe private static extern int ShowImage(int cam_id, IntPtr img_bytes, int img_bytes_len);
    [DllImport ("RSSharedMem")]
    unsafe private static extern void DeinitSharedMemory(int cam_id);

    int cam_id;
    RenderTexture currentRT;
    Texture2D image;
    Camera cam;

    void Start()
    {
        string cam_name = gameObject.name;
        if(cam_name == "LeftCamera")
            cam_id = 0;
        else
            cam_id = 1;

        cam = gameObject.GetComponent<Camera>();

        currentRT = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
        cam.targetTexture = currentRT;
        RenderTexture.active = cam.targetTexture;
        image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);

        Logger.log.Info("Starting camera: " + cam_name + " with cam id: " + cam_id);

        InvokeRepeating("_CaptureCameraImage", 0, 0.1f);
    }

    void _CaptureCameraImage()
    {
        StartCoroutine("CaptureCameraImage");
    }

    void CaptureCameraImage()
    {
        cam.targetTexture = currentRT;
        RenderTexture.active = cam.targetTexture;

        cam.enabled = false;
        cam.Render();

        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();

        //RenderTexture.active = currentRT;

        CallPlacePNGInSharedMemory(cam_id, image.EncodeToPNG());
    }

    // This works!
    unsafe static int CallShowImage(int cam_id, byte[] bytes)
    {
        int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);

        int retval = ShowImage(cam_id, ptr, bytes.Length);

        Marshal.FreeHGlobal(ptr);

        return retval;
    }

    unsafe static int CallPlacePNGInSharedMemory(int cam_id, byte[] bytes)
    {
        int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);

        int retval = PlacePNGInSharedMemory(cam_id, ptr, bytes.Length);

        Marshal.FreeHGlobal(ptr);

        return retval;
    }

    void OnDestroy()
    {
        //LoggingSystem.log.Info("async destroyed");
        DeinitSharedMemory(cam_id);
        //DeinitSharedMemory(1);
    }
}
