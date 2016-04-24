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

        /*
        thruster_packet tp = new thruster_packet("{\"0\":55.0,\"1\":55.0,\"2\":-33.0,\"3\":33.0,\"4\":0.0,\"5\":0.0}");
        LoggingSystem.log.Info(tp.thrusters);
        LoggingSystem.log.Info(tp.whole);

        List<double> lst = new List<double>();
        lst.Add(10.0);
        lst.Add(-10.0);
        lst.Add(0.0);
        lst.Add(0.0);
        lst.Add(43.0);
        lst.Add(43.0);
        tp = new thruster_packet(lst);
        LoggingSystem.log.Info(tp.thrusters);
        LoggingSystem.log.Info(tp.whole);
        */
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
                          new sensor_packet((double)(ypr[0]),
                          (double)(ypr[1]),
                          (double)(ypr[2]),
                          (double)Math.Ceiling(UnityEngine.Random.Range(0.0f, 50.0f)/*s->getDepth(), 2*/),
                          (double)Math.Ceiling(UnityEngine.Random.Range(1.0f, 100.0f)/*s->getBattery(), 2*/),
                          0/*s->getStart()*/, UnityEngine.Random.Range(0.0f, 2.0f),/*s->getDT()*/
                          0.0, 0.0, 0.0, 0.0).whole).whole);
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

    /*
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
    */

    private void TestSensorData()
    {
        //if (!GlobalManager.Instance.enableConnection)
        //{
            //return;
        //}
        // send test packets every 2 seconds
        //sendSensorPacket("helm");
        //sendThrusterPacket("helm");
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

    /*
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
    */

    public void OnDestroy()
    {
        if(socket != null)
        { socket.Disconnect(broker_ip); socket.Close(); }
        
        //socket.Dispose();
        //ctx.Dispose();
    }
}
