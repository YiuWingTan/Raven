using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 健康供给器
/// </summary>
public class HealthGiverTrigger : RespawingTrigger<RavenEntity> {
    

    //TODO lua
    /// <summary>
    /// 触发器给予实体的生命值
    /// </summary>
    [SerializeField]
    private int givenHealth;

    public float GivenHealth
    {
        get
        {
            return givenHealth;
        }
    }

    protected override void Init()
    {
        base.Init();
        this.type = TriggerType.HealthGiverTrigger;
        SetRandomPositionInMap();
        
    }
    private void Start()
    {
        this.Init();
    }
    public override void TryTrigger(RavenEntity entity)
    {
        if(this.isActive()&&region.isTouching(entity.Pos,entity.Radius))
        {
            //当实体在触发器的触发范围而且实体是被激活的
            //entity.
            entity.IncreaseHealth(givenHealth);
            Debug.Log("成功触发了一个触发器");
            
            Deactive();
            Debug.LogWarning("生命触发器成功增加了10点生命值");

            MessageDispatcher.Instance.BroadCast(this.id, EntityMsgType.HealthPackIsGone,1);
        }

        //print("啦啦啦啦啦啦");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position,0.2f);
    }
}
