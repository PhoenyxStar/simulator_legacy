using UnityEngine;
using UnityEngine.UI;

public class UnderWater : MonoBehaviour
{
    // This script enables underwater effects. Attach to main camera.
    // Last modified by Mario Migliacio on 2/18/2016.

    #region Field variables
    /// <summary>
    /// Important variables to consider in the UnderWater RenderSettings:
    /// fogDensity: controls the RenderSettings.fogDensity values, which determine how dense the water will appear.
    /// fogColor: controls the Cameras background color for the water fog, uses R/G/B/A values as floats.
    /// defaultSkybox/noSkybox: currently not doing any actual changes to light shading, this is a good feature for the future if time permits.
    /// lightControl: a boolean control that would be utilized if plans for lighting ever come to light.... (sorry).
    /// old/new __ Green/Blue/Alpha: these values are tracked through the GUI event system and Update method to maintain the fogColor.
    /// Slider objects represent the slider bars on the GUI for fogDensity, blueColor, and greenColor.
    /// </summary>
    public float underwaterLevel = 16;

    //The scene's default fog settings.
    private Color fogColor;
    private float fogDensity;
    private Material defaultSkybox;
    private Material noSkybox;
    private bool lightControl;
    public Slider fogSlider, blueSlider, greenSlider;

    // relating to colors.
    private float oldBlue, oldGreen;
    private float newBlue, newGreen;

    #endregion

    #region Upon Start
    /// <summary>
    /// Default selected values are below.
    /// </summary>
    void Start()
    {
        fogColor = RenderSettings.fogColor;
        fogDensity = RenderSettings.fogDensity;
        Material defaultSkybox = RenderSettings.skybox;
        Material noSkybox = RenderSettings.skybox;
        oldGreen = 0.4f; oldBlue = 0.7f;
        newGreen = 0.4f; newBlue = 0.7f;
        //Set the background color
        GetComponent<Camera>().backgroundColor = new Color(0, oldGreen, oldBlue, 0);
        fogSlider = fogSlider.GetComponent<Slider>();
        blueSlider = blueSlider.GetComponent<Slider>();
        greenSlider = greenSlider.GetComponent<Slider>();
    }

    #endregion

    #region Update Method
    // the big hauncho, responsible for the changes that are made to the Underwater RenderSettings.
    void Update()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.0f, newGreen, newBlue, 0.0f);
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.skybox = noSkybox;
    }

    #endregion

    #region GUI On Value Changing Events
    /// <summary>
    /// This script is responsible for toggling the light source in the simulator environment..
    /// At the momment, the default and noSkybox are the same thing, so until we decide upon some 
    /// alternative method of lighting shading this method doesn't do anything. 
    /// </summary>
    public void ToggleLightSource()
    {
        if (lightControl == false)
        {
            Material noSkyBox = RenderSettings.skybox;
            lightControl = true;
        }

        else if (lightControl == true)
        {
            Material defaultSkyBox = RenderSettings.skybox;
            lightControl = false;
        }
    }

    /// <summary>
    /// This simplistic script is in charge of modifying the fog density of the RenderSettings object
    /// every time the slider value with this script attached changes.
    /// </summary>
    /// <param name="fogValue">from the FogSlider.value property</param>
    public void AlterFogDensity(float fogValue)
    {
        // with the changes to the FogDensity property in place, make a call to update.
        fogDensity = fogValue;

        Update();
    }

    /// <summary>
    /// The BlueSlider is responsible for modifying the internal private blue values.
    /// This value is changed based on the event ValueChanged attached to the controlling slider.
    /// </summary>
    /// <param name="blueValue">from the BlueSlider.value property</param>
    public void AdjustBlueValue(float blueValue)
    {
        // the old colors are constantly being modified through the event system, these color changes are not seen 
        // until the new___ colors are set to these modified values. As Update() calls for the new___ colors.
        oldBlue = blueValue;
    }

    /// <summary>
    /// The GreenSlider is responsible for modifying the internal private green values.
    /// This value is changed based on the event ValueChanged attached to the controlling slider.
    /// </summary>
    /// <param name="greenValue">from the GreenSlider.value property</param>
    public void AdjustGreenValue(float greenValue)
    {
        // the old colors are constantly being modified through the event system, these color changes are not seen 
        // until the new___ colors are set to these modified values. As Update() calls for the new___ colors.
        oldGreen = greenValue;
    }

    /// <summary>
    /// When the user selects the Apply Blue/Green/Alpha button, the event fires to apply the internal
    /// private color values, and create the appropriate color settings to the fogColor.
    /// </summary>
    public void ApplyChangesButton()
    {
        // the old colors are constantly being modified through the event system, these color changes are not seen 
        // until the new___ colors are set to these modified values. As Update() calls for the new___ colors.
        newBlue = oldBlue;
        newGreen = oldGreen;

        Update();
    }

    /// <summary>
    /// PresetClicks:
    /// Sets the fogDensity, newBlue, and newGreen to a pre configured setting
    /// </summary>
    public void Preset1Click()
    {
        fogDensity = fogSlider.value = 0.15f;
        newBlue = blueSlider.value = 0.5f;
        newGreen = greenSlider.value = 0.45f;
        Update();
    }
    
    public void Preset2Click()
    {
        fogDensity = fogSlider.value = 0.3321429f;
        newBlue = blueSlider.value = 1.8f;
        newGreen = greenSlider.value = 0.6785714f;
        Update();
    }

    public void Preset3Click()
    {
        fogDensity = fogSlider.value = 0.4839286f;
        newBlue = blueSlider.value = 1.357143f;
        newGreen = greenSlider.value = 0.2642857f;
        Update();
    }

    public void Preset4Click()
    {
        fogDensity = fogSlider.value = 1.0f;
        newBlue = blueSlider.value = 0.2f;
        newGreen = greenSlider.value = 0.08571429f;
        Update();
    }

    #endregion
}