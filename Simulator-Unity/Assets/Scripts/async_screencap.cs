using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using Emgu.CV;

/// <summary>
/// Special thanks to Eric Haines for providing a multi threaded Image Rescaling source file
/// Source code for TextureScale provided by Author: Eric Haines (Eric5h5)
/// from: http://wiki.unity3d.com/index.php/TextureScale 
/// sample use: TextureScale.Bilinear (currText, screenWidth, screenHeight);
/// </summary>
#region TEXTURE SCALE SOURCE 
public class TextureScale
{
    public class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;
    private static Mutex mutex;

    public static void Point(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }

    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }

    private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
            ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
        }
        else
        {
            ratioX = ((float)tex.width) / newWidth;
            ratioY = ((float)tex.height) / newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        var slice = newHeight / cores;

        finishCount = 0;
        if (mutex == null)
        {
            mutex = new Mutex(false);
        }
        if (cores > 1)
        {
            int i = 0;
            ThreadData threadData;
            for (i = 0; i < cores - 1; i++)
            {
                threadData = new ThreadData(slice * i, slice * (i + 1));
                ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                Thread thread = new Thread(ts);
                thread.Start(threadData);
            }
            threadData = new ThreadData(slice * i, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
            while (finishCount < cores)
            {
                Thread.Sleep(1);
            }
        }
        else
        {
            ThreadData threadData = new ThreadData(0, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
        }

        tex.Resize(newWidth, newHeight);
        tex.SetPixels(newColors);
        tex.Apply();
    }

    public static void BilinearScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            int yFloor = (int)Mathf.Floor(y * ratioY);
            var y1 = yFloor * w;
            var y2 = (yFloor + 1) * w;
            var yw = y * w2;

            for (var x = 0; x < w2; x++)
            {
                int xFloor = (int)Mathf.Floor(x * ratioX);
                var xLerp = x * ratioX - xFloor;
                newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                       ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                       y * ratioY - yFloor);
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    public static void PointScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            var thisY = (int)(ratioY * y) * w;
            var yw = y * w2;
            for (var x = 0; x < w2; x++)
            {
                newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a + (c2.a - c1.a) * value);
    }
}
#endregion

/// <summary>
/// A new attempt at capturing images from the screen using Async methods
/// to alleviate system management and reduce lag from image related work.
/// Developed by: Mario Migliacio, Fall 2015 semester, ROBOSUB SIMULATOR.
/// </summary>
public class async_screencap : MonoBehaviour
{
    #region Field Variables
    /// <summary>
    /// Width and Height of 720p resolution: 1280 x 720.
    /// iter represents the iteration count when creating a image.
    /// currText represents the most new version of the Texture2D image.
    /// </summary>
    private int screenWidth = 1280;
    private int screenHeight = 720;
    private int i = 1;
    private int j = 1;
    private Texture2D currText;
    #endregion

    #region START
    // Use this for initialization
    /// <summary>
    /// I utilize InvokeRepeating rather then Update because we only need to send image files off 
    /// at every tenth of a second. Update calls at 60 frames in one second, therefore we need only
    /// some function which calls continuously to a rate of 1/10 for each second. Luckily InvokeRepeating()
    /// does just that.
    /// </summary>
    private void Start ()
    {
        //InvokeRepeating("CaptureAsync", 0, 0.2f);                     // test. Complete.
        InvokeRepeating("_CaptureAsync_ReadPixelsAsync", 0, 0.1f);
	}
    #endregion

    #region Initial Test
    /// <summary>
    /// CaptureAsync() is a test method, to first ensure images are correctly capture.
    /// To test the image functionality, ensure that a png file type image is stored into
    /// the "images" directory under the main unity application file root.
    /// </summary>
    /// NOTE: works as intended, no further testing on this method needed.
    private void CaptureAsync()
    {
        Application.CaptureScreenshot(Application.dataPath + "/images/image" + i + ".png");
        i++;
    }
    #endregion

    #region Image Processing
    /// <summary>
    /// Method responsible for converting a Texture2D object into a Mat type image. Which is 
    /// used in shared memory to have AI section process what it sees.
    /// </summary>
    /// <returns>a Mat type image</returns>
    private Mat Texture2DToMat()
    {
        // Encode texture to png byte[], then create a mat from that byte[].
        Mat mat = new Mat();
        CvInvoke.Imdecode(currText.EncodeToPNG(), Emgu.CV.CvEnum.LoadImageType.Color, mat);
        return mat;
    }

    /// <summary>
    /// Method to Rescale the Texture2D image captured from the camera to 720p format.
    /// 720p resolution is 1280 pixels wide x 720 pixels high.
    /// </summary>
    private void ResizeTo720p()
    {
        // resizing the image starts to make the screen capture lag, which is not desired.
        TextureScale.Bilinear(currText, screenWidth, screenHeight);
    }

    /// <summary>
    /// Method to write a Texture2D image to file.
    /// Great for Testing purposes, visible image ready for viewing!
    /// </summary>
    private void SaveTextureToFile()
    {
        byte[] bytes = currText.EncodeToPNG();

        //Save our test image 
        File.WriteAllBytes(Application.dataPath + "/images/image" + i + ".png", bytes);
        i++;     // so that the capture image doesnt continuously overwrite itself
    }

    /// <summary>
    /// Method to write a Mat image to file.
    /// Great for Testing purposes, visible image ready for viewing!
    /// </summary>
    private void SaveMatToFile()
    {
        Mat mat = Texture2DToMat();
        mat.Save(Application.dataPath + "/images/mat" + j + ".png");
        j++;
    }
    #endregion

    #region Texture GET/SET 
    /// <summary>
    /// Methods which allow access to the Texture field variable.
    /// GetTexture must be called with correct camera string name: "LeftCamera"
    /// else you will receive null.
    /// SetTexture is used within the async screen capture, which continuously
    /// updates the currText field variable so that you get the newest image on Get call.
    /// </summary>
    public Texture2D GetTexture(string cameraName)
    {
        if (cameraName == "LeftCamera")
        {
            return currText;
        }
        else if (cameraName == "RightCamera")
        {
            return currText;
        }

        else return null;
    }

    private void SetTexture(Texture2D image)
    {
        currText = image;
    }
    #endregion

    #region Async Image Capture
    /// <summary>
    /// NOTE: IEnumerator type functions allow the process to be split further, therefore I found
    /// a way to reduce input lag when images were being stored. This _method is a helper function
    /// which begins a Coroutine on the CaptureAsync_ReadPixelsAsync() IEnumerator function described 
    /// below. Coroutine is unique to Unity, in that it is how IEnumerable methods must be called.
    /// </summary>
    private void _CaptureAsync_ReadPixelsAsync()
    {
        StartCoroutine("CaptureAsync_ReadPixelsAsync");
    }

    private IEnumerator CaptureAsync_ReadPixelsAsync()
    {
        //Wait for graphics to render
        yield return new WaitForEndOfFrame();

        //Create a texture to pass to encoding
        Texture2D image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //Put buffer into texture
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        SetTexture(image);
        //Begin Image processing before clean up.
        //ResizeTo720p();
        //SaveTextureToFile();
        //SaveMatToFile();
        //End   Image processing before clean up.

        //Split the processes up
        yield return 0;

        //stop memory from causing Unity to crash unexpectedly
        DestroyObject(image);
    }
}
#endregion