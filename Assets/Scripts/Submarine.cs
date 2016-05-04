using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class Submarine : MonoBehaviour
{
    // physics attributes
    [SerializeField]
    float Drag;
    [SerializeField]
    float AngularDrag;
    [SerializeField]
    bool BouyancyOn;
    [SerializeField]
    bool GravityOn;
    float mass;
    Vector3 CenterOfBouyancy;
    Rigidbody rb;

    // modules
    SensorModule smod;
    ThrusterModule tmod;
    CameraModule cmod;

    void Start ()
    {
		GameObject body = GameObject.Find("Submarine");
		rb = GetComponent<Rigidbody>();

        CenterOfBouyancy = rb.centerOfMass;
        CenterOfBouyancy.y += 0.1f;

		rb.drag = Drag;
		rb.angularDrag = AngularDrag;

        Physics.gravity = new Vector3(0,0,0);
        //rb.useGravity = true;

		rb.SetDensity(1.0f);
        rb.mass = mass;
    }

    void Update()
    {
        smod.Update();
        tmod.Update();
        cmod.Update();
        if(BouyancyOn)
            UpdateBouyancy();
        if(GravityOn)
            UpdateGravity();
    }

    void UpdateBouyancy()
    {
        float depth = GetDepth();
        float ForceOfBouyancy = 1.0f;
        Vector3 force = new Vector3(0, 0, 0);

        // Calculate force of bouyancy
        if(depth < 0.0f)
            force = Vector3.up * (9.8f + ForceOfBouyancy) * rb.mass;
        else if(depth < 0.2) // Decrease bouyancy as the sub gets close to the surface
            force = (depth) * (Vector3.up * (9.8f + ForceOfBouyancy) * rb.mass);

        // Transform relative Center of Bouyancy to world
        Vector3 cob_world = rb.transform.localRotation * CenterOfBouyancy;
        cob_world += rb.transform.position;

        // Add bouyancy
        rb.AddForceAtPosition(force, cob_world);
    }

    void UpdateGravity()
    {
        rb.AddForce(new Vector3(0, -9.8f * rb.mass, 0));
    }

    float GetDepth()
    {
        GameObject water = GameObject.Find("WaterTop");
        float waterTop = ((Transform)water.GetComponent("Transform")).position.y;
        float subCenter = rb.position.y;
        return subCenter - waterTop;
    }

    Vector3 SubToUnity(Vector3 vec)
    {
        return new Vector3(-vec.y, vec.z, vec.x);
    }
}
