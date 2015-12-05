using UnityEngine;
using System.Collections;
using System.IO;
using System;
using NetMQ;
using AsyncIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class request_packet
{
    private string JSON = "{\"action\": \"\", \"target\": \"\"}";
    private string action;
    private string target;
    public string whole;
    public request_packet(string raw)
    {
        JObject d = JObject.Parse(raw);

        try
        {
            action = (string)d["action"];
            target = (string)d["target"];
            whole = raw;
        }
        catch (Exception e)
        {
            LoggingSystem.log.Error("Request string was not structured properly!");
        }

    }

    public request_packet(string a, string t)
    {
        action = a;
        target = t;
        setupJSON();
    }

    private void setupJSON()
    {
        JObject d = JObject.Parse(JSON);
        d["action"] = action;
        d["target"] = target;
        whole = d.ToString();
        //StringBuffer buffer;
        //Writer<StringBuffer> writer(buffer);
        //d.Accept(writer);
        //whole = buffer.GetString();
    }
}

class reply_packet
{
    private string JSON = "{\"action\": \"\", \"target\": \"\"}";
    public string action;
    public string whole;
    public string target;
    public reply_packet(string raw)
    {
        JObject d = JObject.Parse(raw);

        try
        {
            action = (string)d["action"];
            target = (string)d["target"];
            whole = raw;
        }
        catch (Exception e)
        {
            LoggingSystem.log.Error("Reply string was not structured properly!");
        }
    }
    private void setupJSON()
    {
        JObject d = JObject.Parse(JSON);
        d["action"] = action;
        d["target"] = target;
        whole = d.ToString();
    }
    public reply_packet(string a, string t)
    {
        action = a;
        target = t;
        setupJSON();
    }
};

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
    private string initPortPub;
    private string initPortSub;
    private string pubAddress;
    private string subAddress;
    [SerializeField]
    private string moduleName;
    public static JObject settings;
    private NetMQContext ctx;
    NetMQ.Sockets.PublisherSocket pub;
    NetMQ.Sockets.SubscriberSocket sub;
    List<string> messages;

    // The communicator handles requesting the module be added to the
    // broker and sets up the modules pub and sub sockets.
    // Sending and recieving messages is also handled here.
    public void Initialize(string module_name)
    {
        if (module_name == "")
            return;
        this.moduleName = module_name;

        // load settings file
        string path = "Assets/settings/modules/broker.json";
        string jsonString = File.ReadAllText(path);
        settings = JObject.Parse(jsonString);
        //JsonTextReader settings = new JsonTextReader(new StringReader(jsonString));
        //JsonReader settings = new JsonReader()
        //char buf[4096];

        // load settings
        try
        {
            string initp, inits;
            initp = (string)settings["initportp"];//.GetString(); //reverse of broker
            inits = (string)settings["initports"];//.GetString(); //reverse of broker
            sndbuf = (int)settings["sndbuf"];//GetInt();
            hwm = (int)settings["hwm"];//.GetInt();
            initPortPub = initp;// (char*)malloc(sizeof(char) * strlen(initp.c_str()));
            initPortSub = inits;// (char*)malloc(sizeof(char) * strlen(inits.c_str()));

            //strcpy(initPortPub, initp.c_str());
            //strcpy(initPortSub, inits.c_str());
        }
        catch (Exception e)
        {
            LoggingSystem.log.Error(e.Message);
        }

        bool pubSetup = false;
        bool subSetup = false;


        // Start zmq context
        ctx = NetMQContext.Create();
        pub = ctx.CreatePublisherSocket();
        {
            // code in here. exit this block to dispose the context,
            // only when communication is no longer required
            // setting initial ports.
            pub.Options.SendBuffer = sndbuf; //setsockopt(ZMQ_SNDBUF, &sndbuf, sizeof(sndbuf));
            pub.Options.SendHighWatermark = hwm; //(ZMQ_SNDHWM, &hwm, sizeof(hwm));

            // Attempt to bind to the initial module publisher port.
            // If this fails it means that another module is attempting to register
            // with the broker and should be completed soon. Therefore this
            // will repeat until the bind succeeds.
            while (true)
            {
                try
                {
                    pub.Bind(initPortPub);
                    break;
                }
                catch (Exception e)
                {
                    LoggingSystem.log.Error(e.Message);
                }
                System.Threading.Thread.Sleep(1000);
                //yield return new WaitForSeconds(100.0f);//usleep(100000);
            }

            try
            {
                sub = ctx.CreateSubscriberSocket();
                {
                    // Set up initial subscriber and connect to initial subscriber port.
                    // Connect shouldn't fail since multiple sockets can connect to a publisher.
                    //sub->setsockopt(ZMQ_SUBSCRIBE, "", 0);
                    // subscribe all
                    sub.Subscribe("");
                    sub.Options.ReceiveBuffer = sndbuf;// setsockopt(ZMQ_RCVBUF, &sndbuf, sizeof(sndbuf));
                    sub.Options.ReceiveHighWatermark = hwm;// sub->setsockopt(ZMQ_RCVHWM, &hwm, sizeof(hwm));
                    sub.Connect(initPortSub);// sub->connect(initPortSub);

                    // Initialize the message that will be sent to the broker
                    // asking to register the current module
                    request_packet newPorts = new request_packet("add", module_name);
                    //reply *rep;

                    // Request pub and sub ports from the broker.
                    // The broker will send out two reply packets containing
                    // the ports.
                    // This will repeat until both the ports are recieved.
                    string temp = "";
                    while (!pubSetup || !subSetup)
                    {
                        try
                        {
                            //s_send(*pub, newPorts->whole.c_str());
                            pub.SendFrame(newPorts.whole);
                            //sub.SendFrame(newPorts.whole);
                            //yield return new WaitForSeconds(100.0f);// usleep(100000);
                            System.Threading.Thread.Sleep(100);
                            // s_recv is non-blocking and returns an empty string if no message is recieved.
                            bool ret = sub.TryReceiveFrameString(out temp); //s_recv(*sub);
                            if (ret && temp != "")
                            {
                                // If a message is recieved this will parse it into a reply packet.
                                reply_packet rep = new reply_packet(temp);

                                // rep.action should contain the module name and the type of port in rep.target
                                string str = rep.action.ToString();

                                string[] strs = str.Split(" ".ToCharArray());
                                // The first half of the string should be the module name
                                //  strtok(str, " ");
                                if (strs.Length > 1 && strs[0] == module_name)
                                {
                                    // Store the returned address
                                    if (strs[1] == "pub" && !pubSetup)
                                    {
                                        pubAddress = rep.target;
                                        pubSetup = true;
                                    }
                                    else if (strs[1] == "sub" && !subSetup)
                                    {
                                        subAddress = rep.target;
                                        subSetup = true;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // nothing should happen, will just keep trying.
                            LoggingSystem.log.Error(e.Message);
                        }

                    }

                    // Our initial ports must be unbound and disconnected before setting up
                    // our new sockets
                    // This will also allow other modules to bind to the publisher port; until
                    // this happens no other modules can communicate with the broker.
                    //pub->unbind(initPortPub);
                    //pub->bind(pubAddress.c_str());

                    sub.Disconnect(initPortSub);
                    sub.Connect(subAddress);

                    //request reqM = request("clearing", "socket");
                    message newMsg = new message(module_name, module_name, "request", "");
                    System.Threading.Thread.Sleep(100);
                    send_message(newMsg.whole);
                    send_message(newMsg.whole);

                    System.Threading.Thread.Sleep(100);
                    receive_messages();
                }
            }
            catch (Exception e)
            {
                LoggingSystem.log.Error(e.Message);
            }
        }
    }

    void OnDestory()
    {
        closeCom();
    }

    // Tell the broker to remove this module from it's records
    // The module is not using the initial pub/sub ports at this point
    // so were using a base message with "broker" set as the recipient
    // to inform the broker of the module closing.
    private void closeCom()
    {
        message msg = new message(moduleName, "broker", "request", "");

        // Send it multiple times to make sure the broker gets it
        // This step may not be necessary
        pub.SendFrame(msg.whole);
    }
    // Recieves all messages sent to this module as raw string.
    public List<string> receive_messages()
    {
        List<string> messagesv = new List<string>();

        // If socket is not setup return nothing.
        if (sub == null) return null;

        string message; //zmq::message_t message;
        try
        {
            bool ret = sub.TryReceiveFrameString(out message);  //recv(&message, ZMQ_DONTWAIT);
            // If there are multiple messages keep getting them
            while (!message.Equals(""))
            {
                messagesv.Add(message);
                sub.TryReceiveFrameString(out message);
            }
        }
        catch (Exception e)
        {
            LoggingSystem.log.Error(e.Message);
        }

        // store for helper function use
        messages.Clear();
        messages = messagesv;

        return messagesv;
    }

    // Send message as raw string
    private bool send_message(string msg)
    {
        return pub.TrySendFrame(msg);// s_send(*pub, msg);
    }

    private bool send_message(message msg)
    {
        return pub.TrySendFrame(msg.whole); //s_send(*pub, msg.whole);
    }

    // Use this for initialization
    void Start()
    {
        ForceDotNet.Force();
        Initialize(moduleName);
    }

    // Update is called once per frame
    void Update()
    {
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
