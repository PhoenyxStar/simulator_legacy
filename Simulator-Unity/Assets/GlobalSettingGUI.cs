using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalSettingGUI : MonoBehaviour {
    // text box to show and change broker ip
    private Canvas MainCanvas;
    private InputField BrokerIPText;
    private Button ConnectBtn;
    GlobalSettingGUI()
    {
    }

    void Awake()
    {
        MainCanvas = GetComponent<Canvas>();
        BrokerIPText = MainCanvas.GetComponentInChildren<InputField>();
        ConnectBtn = MainCanvas.GetComponentInChildren<Button>();
        // reconnect
        ConnectBtn.onClick.AddListener(delegate ()
        {
            string ip = BrokerIPText.text;
            GlobalManager.Instance.BrokerIP = ip;
        });
    }
    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void SetBrokerIPText(string ipText)
    {
        BrokerIPText.GetComponentInChildren<Text>().text = ipText;
    }
}
