using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class Submarine : MonoBehaviour
{
    // physics attributes
    Vector3 buoyant_center;
    float buoyant_force;
    Rigidbody rb;

    // modules
    SensorModule smod;
    ThrusterModule tmod;
    CameraModule cmod;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();

        buoyant_center = rb.centerOfMass;
        buoyant_center.y += 0.1f;
        buoyant_force = 1.0f;

        smod = new SensorModule(rb);
        smod.Init("sensor");
        tmod = new ThrusterModule(rb);
        tmod.Init("thruster");
        //cmod = new CameraModule();
        //cmod.Init("camera");
    }

    void Update()
    {
        smod.Update();
        tmod.Update();
        //cmod.Update();
        UpdateBouyancy();
    }

    void UpdateBouyancy()
    {
        float depth = GetDepth();
        Vector3 force = new Vector3(0, 0, 0);

        // Calculate force of bouyancy
        if(depth < -0.5f)
            force = Vector3.up * buoyant_force;

        // Transform relative Center of Bouyancy to world
        Vector3 cob_world = rb.transform.localRotation * buoyant_center;
        cob_world += rb.transform.position;

        // Add bouyancy
        rb.AddForceAtPosition(force, cob_world);
    }

    float GetDepth()
    {
        GameObject water = GameObject.Find("WaterTop");
        float waterTop = water.GetComponent<Transform>().position.y;
        float subCenter = rb.position.y;
        return subCenter - waterTop;
    }
}
