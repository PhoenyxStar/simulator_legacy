using UnityEngine;
using System.Collections;

public class GlobalManager {
    private static GlobalManager _instance; 
    public bool enableConnection = true;
    public string _brokerIP;
    private GlobalSettingGUI _ui;
    public string BrokerIP
    {
        get
        {
            return _brokerIP;
        }
        set
        {
            _brokerIP = value;
            // update UI
            
            _ui.SetBrokerIPText(_brokerIP);
            SubPhysics sp = GameObject.Find("rb").GetComponent<SubPhysics>();
            // reregister thruster to communicator
            sp.InitCommunicator();
            Sensors ss = GameObject.Find("rb").GetComponent<Sensors>();
            // reregister sensor to communicator
            ss.InitCommunicator();
        }
    }

	public static GlobalManager Instance
	{
		get {
			if (_instance == null) {
				_instance = new GlobalManager ();
			}
			return _instance;
		}
		private set{}
	}

    public GlobalManager()
    {
        Instance = this;
        _ui = (GameObject.Find("GlobalSettingGUI")).GetComponent<GlobalSettingGUI>();
    }
		
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
