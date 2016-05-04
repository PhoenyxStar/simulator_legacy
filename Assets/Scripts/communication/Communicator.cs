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
    [SerializeField]
    private string module_name;
    private string broker_ip = "127.0.0.1:2000";
    public static JObject settings;
    public static int num_instance = 0; // how many communicator instances have been created
    NetMQ.Sockets.DealerSocket socket;
    List<string> messages = new List<string>();

    public Communicator()
    {
        num_instance++;
    }

    ~Communicator()
    {
        num_instance--;
    }

    public void Initialize(string module_name)
    {
        //if (!GlobalManager.Instance.enableConnection || module_name == "")
            //return;
        ForceDotNet.Force();
        this.module_name = module_name;
        // clear it first(disconnect the formel connection)
        OnDestroy();
        // load settings file
        string path = "../../settings/modules/broker.json";
        if(!File.Exists(path))
        {
            path = "Assets/settings/modules/broker.json";
        }
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);

        Logger.log.Info("starting communicator for module: " + module_name);

        // load settings
        try
        {
            broker_ip = (string)settings["broker_ip"];
            Logger.log.Info("broker_ip " + broker_ip);
            sndbuf = (int)settings["sndbuf"];
            hwm = (int)settings["hwm"];
            Logger.log.Info("sndbuf: " + sndbuf);
        }
        catch (Exception e)
        {
            Logger.log.Warn(e.Message);
        }

        // Start zmq context
        socket = new NetMQ.Sockets.DealerSocket();
        socket.Options.SendBuffer = sndbuf;
        socket.Options.SendHighWatermark = hwm;
        socket.Options.Identity = System.Text.Encoding.ASCII.GetBytes(module_name);
        socket.Connect(broker_ip);
        Logger.log.Info("Module conneted");
    }

    public void ConnectBroker()
    {
        socket.Connect(broker_ip);
    }

    public List<string> receive_messages()
    {
        if (socket == null)
            return null;

        List<string> messagesv = new List<string>();
        string message; //zmq::message_t message;
        while (true)
        {
            socket.TryReceiveFrameString(out message);  //recv(&message, ZMQ_DONTWAIT);
            if (string.IsNullOrEmpty(message))
                break;
            else
                messagesv.Add(message);
        }
        messages.Clear();
        messages = messagesv;
        return messagesv;
    }

    public bool send_message(string msg)
    {
        return socket.TrySendFrame(msg);
    }

    public bool send_message(message msg)
    {
        return socket.TrySendFrame(msg.whole);
    }

    void Start()
    {

    }

    public void OnDestroy()
    {
        if(socket != null)
        {
            socket.Disconnect(broker_ip);
            socket.Close();
        }
    }
}
