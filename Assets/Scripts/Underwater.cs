using UnityEngine;
using System.Collections;

public class Underwater : MonoBehaviour
{
    public int underwaterLevel;
    private bool defaultFog;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private Material defaultSkybox;
    private Material noSkybox;

    void Start ()
    {
        underwaterLevel = 0;
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultSkybox = RenderSettings.skybox;
	    GetComponent<Camera>().backgroundColor = new Color(0, 0.7f, 0.7f, 1);
    }

    void Update ()
    {
        if (GetComponent<Camera>().transform.position.y < underwaterLevel)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0, 0.7f, 0.7f, 1);
            RenderSettings.fogDensity = 0.1f;
            RenderSettings.skybox = noSkybox;
        }
        else
        {
            RenderSettings.fog = defaultFog;
            RenderSettings.fogColor = defaultFogColor;
            RenderSettings.fogDensity = defaultFogDensity;
            RenderSettings.skybox = defaultSkybox;
        }
    }
}
