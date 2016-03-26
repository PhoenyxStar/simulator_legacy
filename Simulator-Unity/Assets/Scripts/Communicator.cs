using UnityEngine;
using System.Collections;
using System.IO;
using System;
using NetMQ;
using AsyncIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class sensor_packet
{
	public int pitch;
	public int roll;
	public int yaw;
	public float depth;
	public float battery;
	public bool start_switch;
	public double dt;

	public string JSON = "{\"pitch\":0, \"roll\":0, \"yaw\":0, \"depth\":0, \"battery\":0, \"start_switch\":false, \"dt\":0}";
    public string whole;

    public sensor_packet(string raw)
    {
        JObject d = JObject.Parse(raw);
        try
        {
            pitch = (int)d["pitch"];
            roll = (int)d["roll"];
            yaw = (int)d["yaw"];
            depth = (float)d["depth"];
            battery = (float)d["battery"];
            start_switch = (bool)d["start_switch"];
            dt = (double)d["dt"];
            whole = raw;

        }
        catch (Exception e)
        {
            LoggingSystem.log.Error("Sensor packet: unable to parse string.\n");
            return;
        }

    }

    public sensor_packet(int pitch, int roll, int yaw, float depth, float battery, bool start_switch, double dt)
    {
        this.pitch = pitch;
        this.roll = roll;
        this.yaw = yaw;
        this.depth = depth;
        this.battery = battery;
        this.start_switch = start_switch;
        this.dt = dt;
        setupJSON();

    }

    void setupJSON()
    {
        JObject d = JObject.Parse(JSON);

        d["pitch"] = pitch;
        d["roll"] = roll;
        d["yaw"] = yaw;
        d["depth"] = depth;
        d["battery"] = battery;
        d["start_switch"] = start_switch;
        d["dt"] = dt;
		whole = d.ToString(Formatting.None);
    }
}

public class thruster_packet
{
    public thruster_packet(string raw)
    {
        JObject d = JObject.Parse(raw);
        try
        {
			xa = (double)d["xa"];
			xb = (double)d["xb"];
			ya = (double)d["ya"];
			yb = (double)d["yb"];
			za = (double)d["za"];
			zb = (double)d["zb"];
            whole = d.ToString();

        }
        catch (Exception e)
        {
            LoggingSystem.log.Error("Thrustet packet: unable to parse string.\n");
            return;
        }
    }
	public thruster_packet(double xa, double xb, double ya, double yb, double za, double zb)
    {
        this.xa = xa;
        this.xb = xb;
        this.ya = ya;
        this.yb = yb;
        this.za = za;
        this.zb = zb;
        setupJSON();
    }
    string JSON = "{ \"xa\": \"\", \"xb\": \"\", \"ya\": \"\", \"yb\": \"\", \"za\": \"\", \"zb\": \"\"}";
    public double xa;
	public double xb;
	public double ya;
	public double yb;
	public double za;
	public double zb;
    public string whole;
    
    void setupJSON()
    {
        JObject d = JObject.Parse(JSON);

        d["xa"] = xa;
        d["xb"] = xb;
        d["ya"] = ya;
        d["yb"] = yb;
        d["za"] = za;
        d["zb"] = zb;
        whole = d.ToString(Formatting.None);
    }
    thruster_packet(int a) { setupJSON(); }
};

public class message
{
    public string JSON = "{\"sender\": \"\", \"recipient\": \"\",\"mtype\": \"\", \"value\": \"\"}";
    public string sender;
	public string recipient;
    public string whole;
    public string mtype;
	public string value;

    public message(string raw)
    {
        JObject d = JObject.Parse(raw);

        try
        {
            sender = (string)d["sender"];
            recipient = (string)d["recipient"];
            value = (string)d["value"];
            mtype = (string)d["mtype"];
            whole = raw;
//			whole = whole.Replace(@"\", @"");
//			whole = whole.Replace(@"""{", @"{");
//			whole = whole.Replace(@"}""", @"}");
        }
        catch (Exception e)
        {
            LoggingSystem.log.Error(e.Message);
        }
    }
    public message(string sender, string recipient, string mtype, string value)
    {
        this.sender = sender;
        this.recipient = recipient;
        this.mtype = mtype;
        this.value = value;
        setupJSON();
    }

    private void setupJSON()
    {
        JObject d = JObject.Parse(JSON);
        d["sender"] = sender;
        d["recipient"] = recipient;
        d["mtype"] = mtype;
        d["value"] = value;
		whole = d.ToString(Formatting.None);
    }

};

public class Communicator
{
    private int sndbuf;
    private int hwm;
    //private string initPortPub;
    //private string initPortSub;
    //private string pubAddress;
    //private string subAddress;
    [SerializeField]
    private string module_name;
    private string broker_ip;
    public static JObject settings;
    public static int num_instance = 0; // how many communicator instances have been created
    //NetMQ.Sockets.PublisherSocket pub;
    //NetMQ.Sockets.SubscriberSocket sub;
    NetMQ.Sockets.DealerSocket socket;
    List<string> messages = new List<string>();

    public Communicator()
    {
        // increase instance number
        num_instance++;
    }
    ~Communicator()
    {
        // decrease instance number
        num_instance--;
    }

    // The communicator handles requesting the module be added to the
    // broker and sets up the modules pub and sub sockets.
    // Sending and recieving messages is also handled here.
    public void Initialize(string module_name)
    {
        //if (!GlobalManager.Instance.enableConnection || module_name == "")
            //return;
        ForceDotNet.Force();
        this.module_name = module_name;
        // clear it first(disconnect the formel connection)
        OnDestroy();
        // load settings file
        string path = "../../robosub/settings/modules/broker.json";
        if(!File.Exists(path))
        {
            path = "Assets/settings/modules/broker.json";
        }
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);

        LoggingSystem.log.Info("starting communicator for module: " + module_name);

        // load settings
        try
        {
            //string initp, inits;
            broker_ip = (string)settings["broker_ip"];
            //GlobalManager.Instance.BrokerIP = broker_ip;
            // use global setting ip if exist
            if(GlobalManager.Instance.BrokerIP != null && GlobalManager.Instance.BrokerIP.Length > 10)
            {
                broker_ip = GlobalManager.Instance.BrokerIP;
            }
            LoggingSystem.log.Info("broker_ip " + broker_ip);
            //initPortPub = (string)settings["initportp"];
            //initPortSub = (string)settings["initports"];
            sndbuf = (int)settings["sndbuf"];//GetInt();
            hwm = (int)settings["hwm"];//.GetInt();
            LoggingSystem.log.Info("sndbuf: " + sndbuf);
        }
        catch (Exception e)
        {
            LoggingSystem.log.Warn(e.Message);
        }

        // Start zmq context
        socket = new NetMQ.Sockets.DealerSocket();
        socket.Options.SendBuffer = sndbuf; //setsockopt(ZMQ_SNDBUF, &sndbuf, sizeof(sndbuf));
        socket.Options.SendHighWatermark = hwm; //(ZMQ_SNDHWM, &hwm, sizeof(hwm));
        socket.Options.Identity = System.Text.Encoding.ASCII.GetBytes(module_name);
        socket.Connect(broker_ip);
        LoggingSystem.log.Info("Module conneted");
    }

    public void ConnectBroker()
    {
        socket.Connect(broker_ip);
    }

    // Recieves all messages sent to this module as raw string.
    public List<string> receive_messages()
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return null;
        //}
        List<string> messagesv = new List<string>();

        // If socket is not setup return nothing.
        if (socket == null) return null;

        string message; //zmq::message_t message;
        while (true)
        {
            bool ret = socket.TryReceiveFrameString(out message);  //recv(&message, ZMQ_DONTWAIT);
            // If there are multiple messages keep getting them
            if (string.IsNullOrEmpty(message))
            {
                break;
            }
            else
                messagesv.Add(message);
        }

        // store for helper function use
        messages.Clear();
        messages = messagesv;

        return messagesv;
    }

    // Send message as raw string
    public bool send_message(string msg)
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return false;
        //}
        return socket.TrySendFrame(msg);
    }

    public bool send_message(message msg)
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return false;
        //}
        return socket.TrySendFrame(msg.whole);
    }
    private bool sendSensorPacket(string recipient)
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return false;
        //}
        float[] ypr = { UnityEngine.Random.Range(0.0f, 100.0f), UnityEngine.Random.Range(0.0f, 100.0f), UnityEngine.Random.Range(0.0f, 100.0f) };
        //s->getYPR(ypr);
        return send_message(new message("sensor", recipient, "sensor",
                          new sensor_packet((int)(ypr[0]),
                          (int)(ypr[1]),
                          (int)(ypr[2]),
                          (float)Math.Ceiling(UnityEngine.Random.Range(0.0f, 50.0f)/*s->getDepth(), 2*/),
                          (float)Math.Ceiling(UnityEngine.Random.Range(1.0f, 100.0f)/*s->getBattery(), 2*/),
                          true/*s->getStart()*/, UnityEngine.Random.Range(0.0f, 2.0f)/*s->getDT()*/).whole).whole);
    }
    //private bool sendSensorPacket(string recipient)
    //{
    //    float[] ypr = { 64.0f, 15.5f, 20.6f };
    //    //s->getYPR(ypr);
    //    return send_message(new message("sensor", recipient, "sensor",
    //                      new sensor_packet((int)(ypr[1]),
    //                      (int)(ypr[2]),
    //                      (int)(ypr[0]),
    //                      (float)Math.Ceiling(5.0/*s->getDepth(), 2*/),
    //                      (float)Math.Ceiling(1.0/*s->getBattery(), 2*/),
    //                      true/*s->getStart()*/, 0.2/*s->getDT()*/).whole).whole);
    //}

    private bool sendThrusterPacket(string recipient)
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return false;
        //}
        //s->getYPR(ypr);
        return send_message(new message("thruster", recipient, "thruster",
                          new thruster_packet((int)UnityEngine.Random.Range(1.0f, 100.0f),
                          (int)UnityEngine.Random.Range(1.0f, 100.0f),
                          (int)UnityEngine.Random.Range(1.0f, 100.0f),
                          (int)UnityEngine.Random.Range(1.0f, 100.0f),
                          (int)UnityEngine.Random.Range(1.0f, 100.0f),
                          (int)UnityEngine.Random.Range(1.0f, 100.0f)).whole).whole);
    }

    private void TestSensorData()
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return;
        //}
        // send test packets every 2 seconds
        sendSensorPacket("helm");
        sendThrusterPacket("helm");
    }

    // Use this for initialization
    void Start()
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return;
        //}
        //Initialize(module_name);
        //TestSensorData();
    }

    float elapseTime = 0;
    // Update is called once per frame
    void Update()
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return;
        //}
        // send test packets every two seconds
        elapseTime += Time.deltaTime;
        if(elapseTime >= 2.0f)
        {
            //TestSensorData();
            elapseTime = 0f;
        }
        
        if (socket == null)
        {
            return;
        }
        List<string> received;

        received = receive_messages();
        if (received.Count > 0)
        {
            for (int i = 0; i < received.Count; ++i)
            {
                // send every thruster packet received
                message parsed_msg = new message(received[i]);
                if (parsed_msg.mtype == "thruster")
                {
                    //thruster_packet tp(parsed_msg.value);
                    //thruster_X_A->Send(tp.xa);
                    //thruster_X_B->Send(tp.xb);
                    //thruster_Y_A->Send(tp.ya);
                    //thruster_Y_B->Send(tp.yb);
                    //thruster_Z_A->Send(tp.za);
                    //thruster_Z_B->Send(tp.zb);

                    //INFO("tp.xa: " + to_string(tp.xa));
                    //INFO("tp.xb: " + to_string(tp.xb));
                    //INFO("tp.ya: " + to_string(tp.ya));
                    //INFO("tp.yb: " + to_string(tp.ya));
                    //INFO("tp.za: " + to_string(tp.za));
                    //INFO("tp.zb: " + to_string(tp.zb));
                }
            }
        }
    }

    public void OnDestroy()
    {
        if(socket != null)
        { socket.Disconnect(broker_ip); socket.Close(); }
        
        //socket.Dispose();
        //ctx.Dispose();
    }
}
