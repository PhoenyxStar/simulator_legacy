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
        }
        catch (Exception e)
        {
            Debug.Log("message exception: " + e.Message);
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
