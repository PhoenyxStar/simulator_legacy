using UnityEngine;
using UnityEngine.UI;

public class menuScript : MonoBehaviour
{
    /// <summary>
    /// quitMenu represents the HideInterface Canvas in the Unity engine.
    /// 
    /// </summary>
    public Canvas mainMenu;
    public Canvas quitMenu;
    public Button hideText;

    void Start()
    {
        mainMenu = mainMenu.GetComponent<Canvas>();
        quitMenu = quitMenu.GetComponent<Canvas>();
        hideText = hideText.GetComponent<Button>();
        mainMenu.enabled = true;
        hideText.enabled = true;
        quitMenu.enabled = false;
    }

    /// <summary>
    /// When the user presses the HideInterface button, the Canvas with 
    /// options to quit should display to the user.
    /// </summary>
    public void HidePressed()
    {
        quitMenu.enabled = true;
        mainMenu.enabled = false;
        hideText.enabled = false;
    }

    /// <summary>
    /// When the user has navigated to the Hide Interface sub menu and clicked the 
    /// 'NO' I don't want to hide the interface dialogue, the quitMenu should disappear
    /// and the mainMenu should become visible again.
    /// </summary>
    public void NoPressed()
    {
        quitMenu.enabled = false;
        mainMenu.enabled = true;
        hideText.enabled = true;
    }

    /// <summary>
    /// When the user has navigated to the Hide Interface sub menu and clicked the 
    /// 'YES' I want to hide interface dialogue, the interfaces all become disabled.
    /// </summary>
    public void YesPressed()
    {
        mainMenu.enabled = false;
        quitMenu.enabled = false;
        hideText.enabled = false;
    }
}
