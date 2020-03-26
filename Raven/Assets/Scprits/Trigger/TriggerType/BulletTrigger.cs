using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹触发器
/// </summary>
public class BulletTrigger : RespawingTrigger<RavenEntity> {
    [SerializeField]
    protected int mIncreaseBulletNumberPerTime;

    private void Start()
    {
        this.type = TriggerType.BulletGiverTrigger;
        Init();
        SetRandomPositionInMap();
    }
    
    public override void TryTrigger(RavenEntity entity)
    {
        if(this.isActive()&&isTouchingTrigger(entity.Pos,entity.Radius))
        {
            //增加AI的子弹
            entity.IncreaseBullet(mIncreaseBulletNumberPerTime);
            this.Deactive();
            //进行一个广播
            MessageDispatcher.Instance.BroadCast(this.id, EntityMsgType.BulletPackIsGone,1);
        }
    }

}
