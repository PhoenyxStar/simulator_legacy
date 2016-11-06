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
    private float ramp_rate;
    private float ramp_rate_real;
    private float variance;

    public Thruster(string name, Vector3 relative_position, Vector3 relative_orientation, float max_thruster_input = 1.0f, float max_motor_force = 150.0f, float ramp_rate = 1.0f)
    {
        this.name = name;
        this.relative_position = relative_position;
        this.relative_orientation = relative_orientation;
        this.max_thruster_input = max_thruster_input;
        this.max_motor_force = max_motor_force;
        this.commanded_thrust = 0.0f;
        this.current_thrust = 0.0f;

        // Ramp rate is expressed as multiple of positive thruster range per second
        // E.g 2.0 means that it would take one second to go from full forward thrust
        // to full reverse thrust
        this.ramp_rate = ramp_rate;
        // Real ramp rate takes into account max thruster input to calculate
        // ramp rate as multiple of max thruster input
        this.ramp_rate_real = this.ramp_rate * max_thruster_input;
    }

    public void Update(Vector3 sub_pos, Quaternion sub_rot, float dt)
    {
        SetSubPositionAndRotation(sub_pos, sub_rot);
        CalcThrusterOutput(dt);
        CalcWorldPosition();
        CalcWorldThrust();
    }

    // This takes into account delay, ramping, etc
    private void CalcThrusterOutput(float dt)
    {
        float direction = Mathf.Sign(commanded_thrust - current_thrust);

        // If current thrust is within one "frame" of commanded thrust
        // just set thrust to commanded thrust
        if(WithinError(current_thrust, commanded_thrust, ramp_rate_real * dt))
            current_thrust = commanded_thrust;
        else
            current_thrust = current_thrust + (ramp_rate_real * dt * direction);
    }

    // Returns true if x is less than err away from y
    private bool WithinError(float x, float y, float err)
    {
        return (Mathf.Abs(y - x) < Mathf.Abs(err)) ? true : false;
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
