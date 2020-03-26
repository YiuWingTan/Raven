using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularTriggerRegion : TriggerRegion {

    private float r;

    private Vector2 center;

    public CircularTriggerRegion(Vector2 pos,float r)
    {
        this.r = r;
        this.center = pos;
    }

    public override bool isTouching(Vector2 pos, float entityRadius)
    {
        float dist = (pos - this.center).sqrMagnitude*100;

        float rdist = r + entityRadius;
        
        rdist *= rdist;

        rdist *= 100;

        return dist < rdist;
    }
}
