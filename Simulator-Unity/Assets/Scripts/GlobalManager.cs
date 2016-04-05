using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalManager {
    private static GlobalManager _instance; 
    public bool enableConnection = true;
    private string _brokerIP;
    private GlobalSettingGUI _ui;

    static string SubObjName = "SubCenter";
    
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
            SubPhysics sp = GameObject.Find(SubObjName).GetComponent<SubPhysics>();
            // reregister thruster to communicator
            sp.InitCommunicator();
            Sensors ss = GameObject.Find(SubObjName).GetComponent<Sensors>();
            // reregister sensor to communicator
            ss.InitCommunicator();
        }
    }

    public float WaterSurfaceLevel
    {
        get
        {
            GameObject water = GameObject.Find("WaterTop");
            return water.transform.position.y;
        }

        // not supposed to set
        private set { }
    }

    // return the submarine depth underwater
    public float SubDepth
    {
        get
        {
            SubPhysics sp = GameObject.Find(SubObjName).GetComponent<SubPhysics>();
            return WaterSurfaceLevel - sp.transform.position.y;
        }
        // not supposed to set
        private set { }
    }

    // return the submarine position
    public Vector3 SubPosition
    {
        get
        {
            SubPhysics sp = GameObject.Find(SubObjName).GetComponent<SubPhysics>();
            return sp.transform.position;
        }
        // not supposed to set
        private set { }
    }

    public Vector3 CameraOrientation
    {
        get
        {
            return Camera.main.transform.forward;
        }
        // not supposed to set
        private set { }
    }

    public Dictionary<string, float> SubInfo
    {
        get
        {
            Dictionary<string, float> infoDict = new Dictionary<string, float>();
            SubPhysics sp = GameObject.Find(SubObjName).GetComponent<SubPhysics>();
            Rigidbody rb = sp.GetComponent<Rigidbody>();
            Vector3 localAngularVelocity = rb.transform.InverseTransformDirection(rb.angularVelocity);
            infoDict["Port"] = sp.port;
            infoDict["Star"] = sp.star;
            infoDict["Front"] = sp.front;
            infoDict["Back"] = sp.back;
            infoDict["Top"] = sp.top;
            infoDict["Bot"] = sp.bot;
            infoDict["Pitch"] = localAngularVelocity.x;
            infoDict["Yaw"] = localAngularVelocity.y;
            infoDict["Roll"] = localAngularVelocity.z;
            return infoDict;
        }
        // not supposed to set
        private set { }
    }

    /* 
        Vector3 pos = GlobalManager.Instance.SubPosition;
        LoggingSystem.log.Info("Sub depth = " + GlobalManager.Instance.SubDepth + "\t Sub pos " + pos.ToString() + "\t Camera Dir " + GlobalManager.Instance.CameraOrientation);
        var dict = GlobalManager.Instance.SubInfo;
        string str = "";
        foreach (var pair in dict)
        {
            str += String.Format("Key = {0},Value = {1}  ", pair.Key, pair.Value);
        }
        LoggingSystem.log.Info(str);
    */

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
