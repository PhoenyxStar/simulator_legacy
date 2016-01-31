using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {
    public static GlobalManager Instance; 
    public bool enableConnection = false;

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
