using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Capture
{
    Camera cam;
    string name;
    public int width;
    public int height;
    private RenderTexture texture;
    private Texture2D frame;

    public Capture(string name, int width = 500, int height = 500)
    {
        this.width = width;
        this.height = height;
        cam = GameObject.Find(name + "Camera").GetComponent<Camera>();
        texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = texture;
        frame = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
    }

    public void Update()
    {
        RenderTexture.active = texture;
        cam.Render();
        frame.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        frame.Apply();
    }

    public byte[] Retrieve()
    {
        return TexturetoMat(frame);
    }

    public byte[] Read()
    {
        Update();
        return Retrieve();
    }

    byte[] TexturetoMat(Texture2D texture)
    {
        byte[] data = new byte[texture.width * texture.height * 3]; // 24 bit color
        Color32[] pixels = texture.GetPixels32();
        for (int x = 0; x < texture.width; ++x)
        {
            for (int y = 0; y < texture.height; ++y)
            {
                data[(y * texture.width + x) * 3] = pixels[(texture.height - y - 1) * width + x].b;
                data[(y * texture.width + x) * 3 + 1] = pixels[(texture.height - y - 1) * width + x].g;
                data[(y * texture.width + x) * 3 + 2] = pixels[(texture.height - y - 1) * width + x].r;
            }
        }
        return data;
    }
}
