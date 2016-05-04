using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using Emgu.CV;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Capture : MonoBehaviour
{
    Camera cam;
    string name;
    private int width;
    private int height;
    private RenderTexture texture;
    private Texture2D frame;

    public void Init(string name, int width, int height)
    {
        this.width = width;
        this.height = height;
        cam = GameObject.Find(name + "Camera").GetComponent<Camera>();
        texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = texture;
        RenderTexture.active = cam.targetTexture;
        frame = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
    }

    public void Grab()
    {
        cam.Render();
        frame.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        frame.Apply();
    }

    public byte[] Retrieve()
    {
        return TexturetoMat(frame, width, height);
    }

    public byte[] Read()
    {
        Grab();
        return Retrieve();
    }

    byte[] TexturetoMat(Texture2D texture, int width, int height)
    {
        byte[] data = new byte[width * height * 3]; // 24 bit color
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[index] = (byte)(texture.GetPixel(x,y).b * 255);
                data[index] = (byte)(texture.GetPixel(x,y).g * 255);
                data[index] = (byte)(texture.GetPixel(x,y).r * 255);
                index++;
            }
        }
        return data;
    }
}
