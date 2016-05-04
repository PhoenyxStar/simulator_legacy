using UnityEngine;
using UnityEngine.UI;

public class UnderWater : MonoBehaviour
{
    public float underwaterLevel = 16;
    private Material defaultSkybox;
    private Material noSkybox;
    private Color fogColor;

    public float fogDensity
    {
        get
        {
            return fogDensity;
        }

        set
        {
            fogDensity = value;
            RenderSettings.fogDensity = fogDensity;
        }
    }

    public float green
    {
        get
        {
            return green;
        }

        set
        {
            green = value;
            RenderSettings.fogColor = new Color(0.0f, green, blue, 0.0f);
        }
    }

    public float blue
    {
        get
        {
            return blue;
        }

        set
        {
            blue = value;
            RenderSettings.fogColor = new Color(0.0f, green, blue, 0.0f);
        }
    }

    void Start()
    {
        fogColor = RenderSettings.fogColor;
        fogDensity = RenderSettings.fogDensity;
        defaultSkybox = RenderSettings.skybox;
        noSkybox = RenderSettings.skybox;
        green = 0.4f;
        blue = 0.7f;
        GetComponent<Camera>().backgroundColor = new Color(0, green, blue, 0);
    }
}
