using UnityEngine;
using System.Collections;
using System.IO;
using System;
using NetMQ;
using AsyncIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class sensor_packet
{
    private int pitch;
    int roll;
    int yaw;
    float depth;
    float battery;
    bool start_switch;
    double dt;

    string JSON = "{ \"pitch\": 0,  \"roll\": 0,  \"yaw\":  0,  \"depth\": 0,  \"battery\": 0, \"start_switch\": false, \"dt\": 0}";
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
        whole = d.ToString();
    }
}
class message
{
    public string JSON = "{\"sender\": \"\", \"recipient\": \"\",\"mtype\": \"\", \"value\": \"\"}";
    private string sender;
    private string recipient;
    public string whole;
    public string mtype;
    private string value;
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
        whole = d.ToString();
    }

};

public class Communicator : MonoBehaviour
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
    private NetMQContext ctx;
    //NetMQ.Sockets.PublisherSocket pub;
    //NetMQ.Sockets.SubscriberSocket sub;
    NetMQ.Sockets.DealerSocket socket;
    List<string> messages;

    // The communicator handles requesting the module be added to the
    // broker and sets up the modules pub and sub sockets.
    // Sending and recieving messages is also handled here.
    public void Initialize(string module_name)
    {
        if (module_name == "")
            return;
        this.module_name = module_name;

        // load settings file
        string path = "Assets/settings/modules/broker.json";
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);

        LoggingSystem.log.Info("starting communicator for module: " + module_name);

        // load settings
        try
        {
            //string initp, inits;
            broker_ip = (string)settings["broker_ip"];
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
        ctx = NetMQContext.Create();
        socket = ctx.CreateDealerSocket();
        socket.Options.SendBuffer = sndbuf; //setsockopt(ZMQ_SNDBUF, &sndbuf, sizeof(sndbuf));
        socket.Options.SendHighWatermark = hwm; //(ZMQ_SNDHWM, &hwm, sizeof(hwm));
        socket.Options.Identity = System.Text.Encoding.ASCII.GetBytes(module_name);
        socket.Connect(broker_ip);
        LoggingSystem.log.Info("Module conneted");
    }

    // Recieves all messages sent to this module as raw string.
    public List<string> receive_messages()
    {
        List<string> messagesv = new List<string>();

        // If socket is not setup return nothing.
        if (socket == null) return null;

        string message; //zmq::message_t message;
        while(true)
        {
            bool ret = socket.TryReceiveFrameString(out message);  //recv(&message, ZMQ_DONTWAIT);
            // If there are multiple messages keep getting them
            if (message.Equals(""))
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
    private bool send_message(string msg)
    {
        LoggingSystem.log.Info("Sending message: " + msg);
        return socket.TrySendFrame(msg);
    }

    private bool send_message(message msg)
    {
        LoggingSystem.log.Info("Sending message: " + msg.whole);
        return socket.TrySendFrame(msg.whole);
    }
    private bool sendSensorPacket(string recipient)
    {
        float[] ypr = { 1.0f, 0.5f, 0.6f };
        //s->getYPR(ypr);
        return send_message(new message(name, recipient, "sensor",
                          new sensor_packet((int)(ypr[1]),
                          (int)(ypr[2]),
                          (int)(ypr[0]),
                          (float)Math.Ceiling(5.0/*s->getDepth(), 2*/),
                          (float)Math.Ceiling(1.0/*s->getBattery(), 2*/),
                          true/*s->getStart()*/, 0.2/*s->getDT()*/).whole).whole);
    }

    private void TestSensorData()
    {
        sendSensorPacket("Helm");
    }

    // Use this for initialization
    void Start()
    {
        ForceDotNet.Force();
        Initialize(module_name);
        TestSensorData();
    }

    // Update is called once per frame
    void Update()
    {
        return;
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
}
