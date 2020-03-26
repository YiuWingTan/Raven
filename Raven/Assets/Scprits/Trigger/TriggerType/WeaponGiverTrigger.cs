using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器供给触发器
/// </summary>
public class WeaponGiverTrigger : RespawingTrigger<RavenEntity> {

    protected override void Init()
    {
        base.Init();

        this.type = TriggerType.WeaponGiverTrigger;
    }

    public override void TryTrigger(RavenEntity entity)
    {
        if (region.isTouching(entity.Pos, entity.Radius))
        {
            //TODO 发送一个消息给角色供给一个武器
            MessageDispatcher.Instance.AddMsg(this.ID,entity.ID,0,0,null);
        }
    }
}
