using UnityEngine;
using System.Collections;

public class Sensors : MonoBehaviour {

    private GameObject Sub = GameObject.Find("Sub");

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private double depth;
    private Vector3 acceleration;
    private Vector3 magneticField;
    private Vector3 angularVelocity;

	// Use this for initialization
	void Start ()
    {
        velocity = new Vector3(0, 0, 0);
        lastVelocity = new Vector3(0, 0, 0);
        depth = 0;
        acceleration = new Vector3(0, 0, 0);
        magneticField = new Vector3(0, 0, 0);
        angularVelocity = new Vector3(0, 0, 0);
    }
	
	// Update is called once per frame
	void LastUpdate () {
        UpdateDepth();
        UpdateVelocity();
        UpdateAcceleration();
        UpdateMagneticField();
        UpdateAngularVelocity();
	}

    void UpdateDepth()
    {
        GameObject water = GameObject.Find("WaterBasicDaytime");

        // TODO: Figure out how Unity water works. Is this correct for its top? 
        double waterTop = ((Transform)water.GetComponent("Transform")).position.y;
        double subCenter = ((Transform)Sub.GetComponent("Transform")).position.y;

        depth = waterTop - subCenter;
    }

    void UpdateVelocity()
    {
        Rigidbody rb = (Rigidbody)Sub.GetComponent("Rigidbody");
        lastVelocity = velocity;
        velocity = rb.velocity;
    }

    /// <summary>
    /// Updates acceleration. Must be called after UpdateVelocity.
    /// </summary>
    void UpdateAcceleration()
    {
        // TODO: Add gravity
        acceleration = velocity - lastVelocity;
    }

    void UpdateMagneticField()
    {
        // TODO
    }

    void UpdateAngularVelocity()
    {
        Rigidbody rb = (Rigidbody)Sub.GetComponent("Rigidbody");
        angularVelocity = rb.angularVelocity;
    }

    /// <summary>
    /// Measures the submarine's depth.
    /// depth_feet = (sensor_value * 0.1728) - 10.341
    /// </summary>
    /// <returns></returns>
    public double GetDepthOutput()
    {
        // 1 Unity unit = 1 meter
        double depthInFeet = depth * 3.280839895;

        return (depthInFeet + 10.341) / 0.1728;
    }

    /// <summary>
    /// Measures acceleration in x,y,z direction. 
    /// The units are such that a value of ~256 on the sensor is equal to the force of gravity (1 g, or 9.81 m/s2)
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAccelerometerOutput()
    {
        var output = (acceleration * (float)(256/9.81));

        return output;
    }

    /// <summary>
    /// Measures angular velocity in the X, Y, and Z axes.
    /// The units are such that a value of ~14.375 on the sensor is equal to a rotation speed of 1 degree/second.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGyroscopeOutput()
    {
        // Convert radians/second to degrees/second (* 57.2958),
        // then adjust to sensor specs (* 14.375)
        return angularVelocity * (float)57.2958 * (float)14.375;
    }

    /// <summary>
    /// Measures magnetic field strength in the X, Y, and Z axes.
    /// This will not just give us an angle of degrees from north, post-processing is needed.
    /// The units are such that a value of ~10 on the sensor is equal to 1 µT
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMagnetometerOutput()
    {
        // TODO (Currently unimplemented in real AUV)
        return magneticField;
    }

    /// <summary>
    /// Always shows full battery within simulator.
    /// actual_voltage = sensor_value * (3.3/1024*7.5)
    /// </summary>
    /// <returns></returns>
    public double GetBatteryOutput()
    {
        // TODO: Check sub's actual voltage
        // Assuming 24 V
        double voltage = 24;
        return voltage / (3.3 / (double)1024 * 7.5);
    }

    // TODO: Mission Start Switch
}
