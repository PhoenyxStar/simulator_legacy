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
    private string module_name;
    private int sndbuf = 1024;
    private int hwm = 1024;
    private string broker_ip = "tcp://127.0.0.1:2222";
    public static JObject settings;
    NetMQ.Sockets.DealerSocket socket;
    List<string> messages = new List<string>();

    public Communicator(string name)
    {
        this.module_name = name;
        Debug.Log("starting communicator for module: " + module_name);
        /*
        string path = "../settings/modules/broker.json";
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);

        // load settings
        try
        {
            broker_ip = (string)settings["broker_ip"];
            sndbuf = (int)settings["sndbuf"];
            hwm = (int)settings["hwm"];
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        */

        // Start zmq context
        socket = new NetMQ.Sockets.DealerSocket();
        socket.Options.SendBuffer = sndbuf;
        socket.Options.SendHighWatermark = hwm;
        socket.Options.Identity = System.Text.Encoding.ASCII.GetBytes(module_name);
        socket.Connect(broker_ip);
        Debug.Log("Module Connected");
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

    public void OnDestroy()
    {
        if(socket != null)
        {
            socket.Disconnect(broker_ip);
            socket.Close();
        }
    }
}
