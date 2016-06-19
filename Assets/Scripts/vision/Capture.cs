using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using Emgu.CV;
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

    public Capture(string name, int width = 640, int height = 480)
    {
        this.width = width;
        this.height = height;
        cam = GameObject.Find(name + "Camera").GetComponent<Camera>();
        texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = texture;
        frame = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
    }

    public IEnumerator Render()
    {
        yield return new WaitForEndOfFrame(); // wait for draw call
        frame.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        frame.Apply();
    }

    public byte[] Retrieve()
    {
        return TexturetoMat(frame, width, height);
    }

    public byte[] Read()
    {
        Render();
        return Retrieve();
    }

    byte[] TexturetoMat(Texture2D texture, int width, int height)
    {
        byte[] data = new byte[width * height * 3]; // 24 bit color
        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                data[index] = (byte)(texture.GetPixel(x,y).b * 255);
                data[index + 1] = (byte)(texture.GetPixel(x,y).g * 255);
                data[index + 2] = (byte)(texture.GetPixel(x,y).r * 255);
                index += 3;
            }
        }
        return data;
    }
}
