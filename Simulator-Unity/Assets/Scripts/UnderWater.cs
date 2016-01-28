using UnityEngine;
using System.Collections;

public class UnderWater : MonoBehaviour
{

    //This script enables underwater effects. Attach to main camera.

    //Define variable
    public float underwaterLevel = 16;

    //The scene's default fog settings
    private bool defaultFog;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private Material defaultSkybox;
    private Material noSkybox;
    private int settingsToggle;
    private bool allowSettingsChange;

    void Start()
    {
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        Material defaultSkybox = RenderSettings.skybox;
        Material noSkybox = RenderSettings.skybox;
        //Set the background color
        GetComponent<Camera>().backgroundColor = new Color(0, 0.4f, 0.7f, 1);
        settingsToggle = 0;
        allowSettingsChange = false;
    }

    void Update()
    {
        // if the user hasnt toggled settings, these are the default values for the water rendering
        if (allowSettingsChange == false)
        {
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
                RenderSettings.fogDensity = 0.04f;
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
        


        // Changeset by Mario Migliacio: Add an input driven response to 
        // change the value of settings for the water conditions.
        if (Input.GetKeyUp(KeyCode.M))
        {
            ChangeSettings();
        }

        // show the latest RenderSettings through this update pass.
        if (allowSettingsChange == true)
        {
            RefreshChanges();
        }
    }

    /// <summary>
    /// RefreshChanges gets the exact same settings as the ChangeSettings functions previous call. 
    /// This method is primarily used to get the RenderSettings which have been toggled through ChangeSettings()
    /// and reflect those changes through the update() method.
    /// </summary>
    void RefreshChanges()
    {
        if (settingsToggle == 1)
        {
            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.5f, 0.8f, 0.6f);
                RenderSettings.fogDensity = 0.05f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }
        }

        else if (settingsToggle == 2)
        {
            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.6f, 0.6f, 0.6f);
                RenderSettings.fogDensity = 0.6f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }
        }

        else if (settingsToggle == 3)
        {
            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.7f, 0.8f, 0.6f);
                RenderSettings.fogDensity = 0.07f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }
        }

        else if (settingsToggle == 4)
        {
            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.8f, 0.8f, 0.6f);
                RenderSettings.fogDensity = 0.08f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }
        }
    }
    
    /// <summary>
    /// ChangeSettings is a temporary hack together, for the implementation to change the 
    /// conditions of the water seen in the simulator. It is ideally to simulate less then 
    /// ideal conditions (water murky, bad visibility) and potentially ideal conditions for
    /// the simulator.
    /// </summary>
    void ChangeSettings()
    {
        // for the first click of the toggle water settings, change the fog color and density values slightly.
        if (settingsToggle == 0)
        {
            allowSettingsChange = true;

            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.5f, 0.8f, 0.6f);
                RenderSettings.fogDensity = 0.05f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }

            settingsToggle++;
        }

        else if (settingsToggle == 1)
        {
            allowSettingsChange = true;

            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.6f, 0.6f, 0.6f);
                RenderSettings.fogDensity = 0.6f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }

            settingsToggle++;
        }

        else if (settingsToggle == 2)
        {
            allowSettingsChange = true;

            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.7f, 0.8f, 0.6f);
                RenderSettings.fogDensity = 0.07f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }

            settingsToggle++;
        }

        else if (settingsToggle == 3)
        {
            allowSettingsChange = true;

            // if underwater..
            if (transform.position.y < underwaterLevel)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.8f, 0.8f, 0.6f);
                RenderSettings.fogDensity = 0.08f;
                RenderSettings.skybox = noSkybox;
            }
            // at surface..
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }

            settingsToggle++;
        }

        else if (settingsToggle == 4)
        {
            allowSettingsChange = false;
            settingsToggle = 0;
        }
    }
}