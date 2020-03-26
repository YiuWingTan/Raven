using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 再生触发器
/// </summary>
public class RespawingTrigger<EntityType> : Trigger<EntityType> where EntityType : RavenEntity{
    [SerializeField]
    //TODO LUA 控制
    //再生触发器重生时间
    protected float updateBetweenRespawn;
    //倒计时
    protected float remainingTime;
    protected SpriteRenderer mSpriteRenderer;
    protected override void Init()
    {
        base.Init();
        this.SetActive();
        mSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        SetRespawnDelay(this.updateBetweenRespawn);
    }
    public override void UpdateTrigger(float detalTime)
    {
        if(!this.isActive())
        //Debug.LogWarning("健康包正在更新  " + remainingTime);
        ///只有当这个触发器无效并且，倒计时已经到达时候重启这个触发器
        if (!this.isActive()&&(remainingTime-=detalTime)<=0)
        {
            SetActive();
        }
    }

    /// <summary>
    /// 将再生类型的触发器无效一段时间
    /// </summary>
    public virtual void Deactive()
    {
        this.SetInActive();   
    }
    protected override void SetActive()
    {
        if (!isActive())
        {
            SetRandomPositionInMap();

            this.mSpriteRenderer.enabled = true;
        }
        base.SetActive();
    }
    protected override void SetInActive()
    {
        if (isActive())
        {
            mSpriteRenderer.enabled = false;
            remainingTime = updateBetweenRespawn;
        }
        base.SetInActive();

    }
    /// <summary>
    /// 设置再生触发器再生的时间
    /// </summary>
    public void SetRespawnDelay(float time)
    {
        this.updateBetweenRespawn = time;
    }
    protected virtual void SetRandomPositionInMap()
    {
        int index = Random.Range(0, NavMap.Instance.NavGraph.TotalV);
        var node = NavMap.Instance.NavGraph.GetNode(index);
        this.SetNodeIndex(index, NavMap.Instance);
        this.SetCircularTriggerRegion(pos, 0.2f);
    }
}
