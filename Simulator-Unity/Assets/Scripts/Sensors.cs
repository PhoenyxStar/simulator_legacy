using UnityEngine;
using System.Collections;

// NOTE: 
// Pitch Range: -90 - 90
// Roll Range: -180 - 180
// Yaw Range: -180 - 180

public class Sensors : MonoBehaviour
{

    private GameObject Sub;
    private Rigidbody rb;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private double depth;
    private Vector3 acceleration;
    private Vector3 magneticField;
    private Vector3 angularVelocity;
    private float _Yaw;
    private float _Pitch;
    private float _Roll;

    Communicator communicator; 
    private string[] messageRecipients;

    public double Depth
    {
        get
        {
            return depth;
        }
        set
        {
            depth = value;
        }
    }

    public Vector3 AngularVelocity
    {
        get
        {
            return angularVelocity;
        }
        set
        {
            angularVelocity = value;
        }
    }

    public Vector3 Acceleration
    {
        get
        {
            return acceleration;
        }
        set
        {
            acceleration = value;
        }
    }


    // Use this for initialization
    void Start()
    {
        Sub = GameObject.Find("SubCenter");
		rb = Sub.GetComponent<Rigidbody>();

        velocity = new Vector3(0, 0, 0);
        lastVelocity = new Vector3(0, 0, 0);
        depth = 0;
        acceleration = new Vector3(0, 0, 0);
        magneticField = new Vector3(0, 0, 0);
        angularVelocity = new Vector3(0, 0, 0);

		if (GlobalManager.Instance.enableConnection) {
			communicator = new Communicator ();
			communicator.Initialize ("sensor");
			messageRecipients = new string[] { "control", "ai" };
		}
    }

    // Update is called once per frame
    void Update()
	{
		UpdateDepth ();
		UpdateVelocity ();
		UpdateAcceleration ();
		UpdateMagneticField ();
		UpdateAngularVelocity ();
        UpdateYPR();
		if (GlobalManager.Instance.enableConnection) {
			SendSensorMessage ();
		}
    }

    void UpdateDepth()
    {
        GameObject water = GameObject.Find("WaterTop");

        // TODO: Figure out how Unity water works. Is this correct for its top? 
        double waterTop = ((Transform)water.GetComponent("Transform")).position.y;
        double subCenter = rb.position.y;

        depth = subCenter - waterTop;
    }

    void UpdateVelocity()
    {
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
        var output = (acceleration * (float)(256 / 9.81));

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
        // Assuming 12 V
        double voltage = 12;
        return voltage / (3.3 / (double)1024 * 7.5);
    }

    // TODO: Mission Start Switch
    // TODO: Remove hard-coded start switch value in "SendSensorMessage()"

    public void SendSensorMessage()
    {
        // "dt" is time since last message
		sensor_packet sensorPacket = new sensor_packet(
                (int)(this.Pitch),
                (int)(this.Roll),
                (int)(this.Yaw),
                (float)Depth, 
                (float)GetBatteryOutput(), 
                true, 
                Time.unscaledDeltaTime);

        LoggingSystem.log.Info(sensorPacket.whole);
        LoggingSystem.log.Info("----------------");
        // Send a sensorPacket to each recipient
        foreach (string recipient in messageRecipients)
        {
            message msg = new message("sensor", recipient, "sensor", sensorPacket.whole);
            communicator.send_message(msg);
        }
    }

    public void UpdateYPR()
    {
        Vector3 sub_forward = rb.transform.forward;
        Vector3 sub_up = rb.transform.up;
        Vector3 sub_right = rb.transform.right;

        Vector3 ref_forward = Vector3.forward;
        Vector3 ref_up = Vector3.up;
        Vector3 ref_right = Vector3.right;

        //DisplayVector(sub_right, Color.red);
        //DisplayVector(ref_right, Color.red);
        //DisplayVector(sub_forward, Color.white);
        //DisplayVector(ref_forward, Color.white);

        Vector3 vec = new Vector3();
        float yaw = Mathf.Atan2(sub_forward.x, sub_forward.z);

        vec.x =  sub_forward.x*Mathf.Cos(-yaw) + sub_forward.z*Mathf.Sin(-yaw);
        vec.y =  sub_forward.y;
        vec.z = -sub_forward.x*Mathf.Sin(-yaw) + sub_forward.z*Mathf.Cos(-yaw);

        //DisplayVector(vec, Color.white);

        float pitch = Mathf.Atan2(vec.y, vec.z);

        vec.x =  sub_right.x*Mathf.Cos(-yaw) + sub_right.z*Mathf.Sin(-yaw);
        vec.y =  sub_right.y;
        vec.z = -sub_right.x*Mathf.Sin(-yaw) + sub_right.z*Mathf.Cos(-yaw);

        //DisplayVector(vec, Color.red);

        float roll = Mathf.Atan2(vec.y, vec.x);

        _Yaw = ToDegrees(yaw);
        _Pitch = ToDegrees(pitch);
        _Roll = ToDegrees(roll); // Roll may be reversed. Need to check.
    }

    public void DisplayVector(Vector3 vec, Color c)
    {
        Debug.DrawLine(rb.transform.position, rb.transform.position + vec, c);
    }

    public float ToRadians(float theta) { return Mathf.Deg2Rad * theta; }

    public float ToDegrees(float theta) { return Mathf.Rad2Deg * theta; }

    public float Yaw { get { return _Yaw; } }

    public float Pitch { get { return _Pitch; } }

    public float Roll { get { return _Roll; } }
}
