using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class Thruster
{
    private string name;
    private Vector3 relative_position;
    private Vector3 relative_orientation;
    private Vector3 world_force;
    private Vector3 world_position;
    private Vector3 sub_position;
    private Quaternion sub_rotation;
    private float max_thruster_input;
    private float max_motor_force;
    private float commanded_thrust;
    private float current_thrust;
    //float noise; ??

    public Thruster(string name, Vector3 relative_position, Vector3 relative_orientation, float max_thruster_input, float max_motor_force)
    {
        this.name = name;
        this.relative_position = relative_position;
        this.relative_orientation = relative_orientation;
        this.max_thruster_input = max_thruster_input;
        this.max_motor_force = max_motor_force;
        this.commanded_thrust = 0.0f;
        this.current_thrust = 0.0f;
    }

    public void Update(Vector3 sub_pos, Quaternion sub_rot, float dt)
    {
        SetSubPositionAndRotation(sub_pos, sub_rot);
        current_thrust = commanded_thrust; // TODO: Ramp up
        CalcWorldPosition();
        CalcWorldThrust();
    }
    
    public void SetThrusterPower(float thrust)
    {
        commanded_thrust = thrust;
    }

    public Vector3 WorldPosition
    {
        get
        {
            return world_position;
        }
    }

    public Vector3 WorldThrust
    {
        get
        {
            return world_force;
        }
    }

    private void SetSubPositionAndRotation(Vector3 sub_pos, Quaternion sub_rot)
    {
        sub_position = sub_pos;
        sub_rotation = sub_rot;
    }

    private void CalcWorldPosition()
    {
        world_position = (sub_rotation * relative_position) + sub_position;
    }

    private void CalcWorldThrust()
    {
        world_force = sub_rotation * (relative_orientation * current_thrust * (max_motor_force / max_thruster_input));
    }

    public float CurrentThrust
    {
        get
        {
            return current_thrust * (max_motor_force / max_thruster_input);
        }
    }
}
