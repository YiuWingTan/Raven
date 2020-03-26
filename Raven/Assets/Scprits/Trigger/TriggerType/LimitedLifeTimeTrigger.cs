using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 限制生命周期触发器
/// </summary>
public class LimitedLifeTimeTrigger : Trigger<RavenEntity> {

    //TODO Lua
    /// <summary>
    /// 生存时间
    /// </summary>
    private float liveTime;

    public float LiveTime;

    public override void UpdateTrigger(float detalTime)
    {

        if(this.isActive()&&(liveTime -= detalTime) <0)
        {
            SetInActive();

            SetToBeRemoveFromGame();
        }
    }
}
