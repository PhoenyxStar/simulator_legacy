using UnityEngine;
using System.Collections;

public class GlobalManager {
    private static GlobalManager _instance; 
    public bool enableConnection = false;

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
    }
		
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
