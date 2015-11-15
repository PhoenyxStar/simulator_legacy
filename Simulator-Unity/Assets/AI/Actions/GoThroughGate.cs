using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Motion;

[RAINAction]
public class GoThroughGate : RAINAction
{
    private RAIN.Core.AI _ai;
    Vector3 _dest;
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
        GameObject sub = GameObject.Find("Body");
        _ai = ai;
        //RAIN.Motion.BasicMotor tMotor = ai.Motor as RAIN.Motion.BasicMotor;
        //RAINAspect gateInSight = 
        MoveLookTarget target = _ai.Motor.MoveTarget;
        Vector3 orientation = ai.Body.transform.forward.normalized;//target.Orientation.normalized;
        orientation.y = 0f;
        _dest = target.Position + orientation * 10;
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        if (ai.Motor.MoveTo(_dest))
        {
            //RAIN.Navigation.Waypoints.WaypointSet _wayset = RAIN.Navigation.NavigationManager.Instance.GetWaypointSet("ThroughGateRoute");
            //_ai.WorkingMemory.SetItem<Vector3>("nextStop", _dest);
            return ActionResult.SUCCESS;
        }
        else
        {
            return ActionResult.RUNNING;
        }
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}