using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI的目标选择系统
/// </summary>
public class TargetSelectSystem  {

    private RavenEntity owner;
    public RavenEntity Owner
    {
        get
        {
            return owner;
        }
    }
    private MemoryRecord currentTarget;
    private RavenSensoryComponent ravenSensoryComponent;



	public TargetSelectSystem(RavenEntity entity)
    {
        this.owner = entity;

        this.ravenSensoryComponent = this.owner.SensoryComponent;
    }
    /// <summary>
    /// 返回当前时候有一个目标
    /// </summary>
    /// <returns></returns>
    public bool isHaveTarget()
    {
        return currentTarget == null ? false : true;
    }
    /// <summary>
    /// 返回当前目标是否在视野内
    /// </summary>
    /// <returns></returns>
    public bool isTargetWithinFOV()
    {
        return currentTarget.IsWithinFov;
    }
    /// <summary>
    /// 返回当前的目标是否可以射击
    /// </summary>
    /// <returns></returns>
    public bool isTargetShootable()
    {
        return currentTarget.IsShootable;
    }
    /// <summary>
    /// 返回上一次被感知到的位置
    /// </summary>
    /// <returns></returns>
    public Vector2 GetLastRecoredPosition()
    {
        return currentTarget.LastSensedPosition;
    }
    /// <summary>
    /// 重载
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool GetLastRecoredPosition( ref Vector2 pos)
    {
        if (currentTarget == null)
            return false;
        else
        {
            pos = currentTarget.LastSensedPosition;
            return true;
        }

    }
    /// <summary>
    /// 获取目标被看到的时间
    /// </summary>
    /// <returns></returns>
    public float GetTimeTargetHasbeenVisible()
    { 
        return Time.time - currentTarget.TimeBecameVisible;
    }
    /// <summary>
    /// 获取目标离开视野的时间间隔
    /// </summary>
    /// <returns></returns>
    public float GetTimeTargetHasbeenOutOfView()
    {
        return Time.time - currentTarget.TimeLastVisible;
    }
    /// <summary>
    /// 获取当前的目标
    /// </summary>
    /// <returns></returns>
    public RavenEntity GetCurrentTarget()
    {
        return (RavenEntity)EntityManager.Instance.GetEntityByID(currentTarget.TargetID);
    }
    /// <summary>
    /// 清楚目标的方法
    /// </summary>
    public void ClearTarget()
    {

        //同时也清除这个目标对应的记忆
        if(this.owner.Raven_TargetSelectionSystem.isHaveTarget())
        this.owner.SensoryComponent.RemoveMemeory(currentTarget.TargetID);

        currentTarget = null;


    }
    /// <summary>
    /// 更新目标选择系统的状态
    /// </summary>
    public void Update(float detalTime)
    {
        //每次更新目标是什么
        //目标的选择规则
        //1.目标要么是在攻击距离内的AI
        //2.目标要么是在视野范围内的AI
        //3.目标要么是最近被感知到的AI
        
        if((currentTarget = ravenSensoryComponent.GetClostCanShootEntityInMemory())!=null)
        {
            //Debug.Log("选中了一个可以攻击的目标");
        }else if((currentTarget = ravenSensoryComponent.GetClostEntityInFOV())!=null)
        {
            //Debug.Log("选中了一个在视野范围内的目标");
        }else if((currentTarget = ravenSensoryComponent.GetClostEntityLastSense())!=null)
        {
            //Debug.Log("选中了一个在视野中最近被感知到的目标");
        }


    }
    /// <summary>
    /// 目标选择组件的消息处理函数
    /// </summary>
    public bool OnMessage(Msg msg)
    {
        switch((EntityMsgType)msg.Op)
        {
            case EntityMsgType.HasBeenKilled:
                //正中红心
                if(currentTarget!=null&&msg.SenderID == currentTarget.TargetID)
                {
                    currentTarget = null;
                }
                return true;
        }

        return false;
    }
}
