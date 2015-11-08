using UnityEngine;
using System.Collections;
using Emgu.CV;
using System.IO;

public class Vision : MonoBehaviour {

    // Maximum framers per second
    double FRAME_RATE = 10;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    Mat Texture2DToMat(Texture2D texture)
    {
        // Encode texture to png byte[], then create a mat from that byte[].
        Mat mat = new Mat();
        CvInvoke.Imdecode(texture.EncodeToPNG(), Emgu.CV.CvEnum.LoadImageType.Color, mat);
        return mat;
    }

    void ImageConverstionTest()
    {
        string filename = "C:\\Users\\Shane\\Desktop\\bunny.jpg";

        // Will auto-resize upon loading picture
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(File.ReadAllBytes(filename));

        // Save texture as png
        File.WriteAllBytes("C:\\Users\\Shane\\Desktop\\tex.png", texture.EncodeToPNG());

        // Create Mat
        Mat mat = Texture2DToMat(texture);

        // Save Mat as png
        mat.Save("C:\\Users\\Shane\\Desktop\\mat.png");
    }
}
