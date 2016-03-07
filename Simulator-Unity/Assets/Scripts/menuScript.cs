using UnityEngine;
using UnityEngine.UI;

public class menuScript : MonoBehaviour
{
    // This script enables menu functionality effects.
    // Last modified by Mario Migliacio on 3/6/2016.

    #region Field members and properties
    /// <summary>
    /// mainMenu represents the MainMenuGUI Canvas in the Unity engine.
    /// quitMenu represents the QuitControlGUI Canvas in the Unity engine.
    /// waterMenu represents the WaterControlGUI Canvas in the Unity engine.
    /// imageMenu represents the ImageProcessingGUI Canvas in the Unity engine.
    /// taskMenu represents the TaskMissionGUI Canvas in the Unity engine.
    /// hideText represents the HideButtonMenu under the mainMenu Canvas.
    /// waterControls represents the WaterControlMenu under the mainMenu Canvas.
    /// imageControls represents the ImageProcessingMenu under the mainMenu Canvas.
    /// missionControls represents the TaskMissionsMenu under the mainMenu Canvas.
    /// the boolean properties are used to help aid in the logic of the menu states.
    /// </summary>
    public Canvas mainMenu, quitMenu, waterMenu, imageMenu, taskMenu;
    public Button hideText, waterControls, imageControls, missionControls;
    public bool HideMenu { get; set; }
    public bool WaterControlsMenu { get; set; }
    public bool ImageControlsMenu { get; set; }
    public bool MissionControlsMenu { get; set; }

    #endregion

    #region Start
    /// <summary>
    /// connect the GUI components in the UNITY engine with the code behind logic.
    /// </summary>
    public void Start()
    {        
        // MENUS:
        mainMenu = mainMenu.GetComponent<Canvas>();
        quitMenu = quitMenu.GetComponent<Canvas>();
        waterMenu = waterMenu.GetComponent<Canvas>();
        imageMenu = imageMenu.GetComponent<Canvas>();
        taskMenu = taskMenu.GetComponent<Canvas>();

        // BUTTONS:
        hideText = hideText.GetComponent<Button>();
        waterControls = waterControls.GetComponent<Button>();
        imageControls = imageControls.GetComponent<Button>();
        missionControls = missionControls.GetComponent<Button>();

        // initial menu states:
        mainMenu.enabled = true;
        quitMenu.enabled = false;
        waterMenu.enabled = false;
        imageMenu.enabled = false;
        taskMenu.enabled = false;

        // initial button states:
        hideText.enabled = true;
        waterControls.enabled = true;
        imageControls.enabled = true;
        missionControls.enabled = true;

        // initial boolean states:
        HideMenu = false;
        WaterControlsMenu = false;
        ImageControlsMenu = false;
        MissionControlsMenu = false;
    }

    #endregion

    #region Update
    /// <summary>
    /// In the event that the user wants to bring back up the menu after accidentally hiding it, pressing 'M' key
    /// will bring back the menu in its original state.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Start();
        }
    }

    #endregion

    #region Gui features
    /// <summary>
    /// When the user presses the HideInterface button, the Canvas with 
    /// options to quit should display to the user.
    /// </summary>
    public void HidePressed()
    {
        if (HideMenu == false)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = true;
            waterMenu.enabled = false;
            imageMenu.enabled = false;
            taskMenu.enabled = false;

            HideMenu = true;
            WaterControlsMenu = false;
            ImageControlsMenu = false;
            MissionControlsMenu = false;
        }

        else if (HideMenu)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = false;
            imageMenu.enabled = false;
            taskMenu.enabled = false;

            HideMenu = false;
            WaterControlsMenu = false;
            ImageControlsMenu = false;
            MissionControlsMenu = false;
        }
    }

    /// <summary>
    /// If the user selects the WaterControls button on the MainMenu, open the WaterControlsGUI menu.
    /// </summary>
    public void OpenWaterControls()
    {
        if (WaterControlsMenu == false)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = true;
            imageMenu.enabled = false;
            taskMenu.enabled = false;

            HideMenu = false;
            WaterControlsMenu = true;
            ImageControlsMenu = false;
            MissionControlsMenu = false;
        }

        else if (WaterControlsMenu == true)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = false;
            imageMenu.enabled = false;
            taskMenu.enabled = false;

            HideMenu = false;
            WaterControlsMenu = false;
            ImageControlsMenu = false;
            MissionControlsMenu = false;
        }
    }

    /// <summary>
    /// If the user selects the ImageControls button on the MainMenu, open the ImageControlsGUI menu.
    /// </summary>
    public void OpenImageControls()
    {
        if (ImageControlsMenu == false)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = false;
            imageMenu.enabled = true;
            taskMenu.enabled = false;

            HideMenu = false;
            WaterControlsMenu = false;
            ImageControlsMenu = true;
            MissionControlsMenu = false;
        }

        else if (ImageControlsMenu)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = false;
            imageMenu.enabled = false;
            taskMenu.enabled = false;

            HideMenu = false;
            WaterControlsMenu = false;
            ImageControlsMenu = false;
            MissionControlsMenu = false;
        }
    }

    /// <summary>
    /// If the user selects the TaskMission button on the MainMenu, open the TaskMissionsGUI menu.
    /// </summary>
    public void OpenMissionControls()
    {
        if (MissionControlsMenu == false)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = false;
            imageMenu.enabled = false;
            taskMenu.enabled = true;

            HideMenu = false;
            WaterControlsMenu = false;
            ImageControlsMenu = false;
            MissionControlsMenu = true;
        }

        else if (MissionControlsMenu)
        {
            mainMenu.enabled = true;
            quitMenu.enabled = false;
            waterMenu.enabled = false;
            imageMenu.enabled = false;
            taskMenu.enabled = false;

            HideMenu = false;
            WaterControlsMenu = false;
            ImageControlsMenu = false;
            MissionControlsMenu = false;
        }
    }

    /// <summary>
    /// When the user has navigated to the Hide Interface sub menu and clicked the 
    /// 'NO' I don't want to hide the interface dialogue, the quitMenu should disappear
    /// and the mainMenu should become visible again.
    /// </summary>
    public void NoPressed()
    {
        Start();
    }

    /// <summary>
    /// When the user has navigated to the Hide Interface sub menu and clicked the 
    /// 'YES' I want to hide interface dialogue, the interfaces all become disabled.
    /// </summary>
    public void YesPressed()
    {
        mainMenu.enabled = false;
        quitMenu.enabled = false;
        waterMenu.enabled = false;
        imageMenu.enabled = false;
        taskMenu.enabled = false;
        
        hideText.enabled = false;
        waterControls.enabled = false;
        imageControls.enabled = false;
        missionControls.enabled = false;

        HideMenu = false;
        WaterControlsMenu = false;
        ImageControlsMenu = false;
        MissionControlsMenu = false;
    }

    #endregion
}
