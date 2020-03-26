using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenSensoryComponent {

    /// <summary>
    /// 记忆感知管理器
    /// </summary>
    protected RavenSensoryMemory sensoryMemory;
    public RavenSensoryMemory SensoryMemory
    {
        get
        {
            return sensoryMemory;
        }
    }
    protected RaycastHit2D []hits;
    protected RavenEntity owner;
    /// <summary>
    /// 进行感知的角度
    /// </summary>
    protected float angle = 50;//TODO LUA
    protected float distance = 3f;//进行感知的距离(Lua)

    public RavenSensoryComponent(RavenEntity owner)
    {
        this.owner = owner;

        sensoryMemory = new RavenSensoryMemory(this.owner);
    }
    public void UpdateComponent()
    {
        SightDetection();
        SoundDetection();
        sensoryMemory.UpdateMemory();
    }
    /// <summary>
    /// 判断一个对象是否在自己的可视范围内
    /// </summary>
    /// <returns></returns>
    protected bool isInSight(RavenEntity entity)
    {
        float angleBetweenEntity = Vector2.Angle(owner.transform.up,entity.Pos - owner.Pos);
        //Debug.Log("相隔的角度是"+angleBetweenEntity);
        //如果在实体的视锥体范围内
        if(angleBetweenEntity<this.angle)
        {
            float sqrtDistanceBetweenEntity = (entity.Pos - this.owner.Pos).sqrMagnitude;

            if(sqrtDistanceBetweenEntity<(distance*distance))
            {
                //在视锥体的视野范围内

                hits = Physics2D.LinecastAll(owner.Pos,entity.Pos,~(1<<0));

                if(hits.Length>=1)
                {
                    if(hits[1].collider.tag == "Wall")
                        return false;
                    else
                        return true;
                }
                
                
            }
        }

        return false;
    }
    /// <summary>
    /// 进行视觉检测
    /// </summary>
    protected void SightDetection()
    {
        foreach (var entity in EntityManager.Instance.EntityContainer.Values)
        {
            if (entity.ID != this.owner.ID)
            {
               var e = (RavenEntity)entity;
                if (isInSight(e))
                {
                    MemoryRecord record = new MemoryRecord();
                   
                    record.IsWithinFov = true;
                    record.LastSensedPosition = e.Pos;
                    record.TimeLastSensed = Time.time;
                    record.TimeLastVisible = Time.time;
                    record.TargetID = e.ID;



                    if(this.isCanAttack(e))
                    {
                        //这个目标是可以进行射击的
                        //Debug.LogError(this.owner.gameObject.name+"检测到一个可以进行设计的目标");
                        record.IsShootable = true;
                    }else
                    {
                        record.IsShootable = false;
                    }

                    //构建一个MemroyRecord 
                    this.sensoryMemory.AddMemoryRecord(e, record);
                }
            }
        }
    }
    /// <summary>
    /// 听觉测试
    /// </summary>
    protected void SoundDetection()
    {

    }
    /// <summary>
    /// 从记忆中获取被感知的最近的AI
    /// </summary>
    /// <returns></returns>
    public int GetCloseEntityInMemory(ref Vector2 pos)
    {

        MemoryRecord record = null;
        float sqrtDist = float.PositiveInfinity;

        foreach (var r in this.sensoryMemory.Memorys.Values)
        {
            float sdist = (r.LastSensedPosition - this.owner.Pos).sqrMagnitude;

            if (sqrtDist > sdist)
            {
                record = r;
                sqrtDist = sdist;
            }
        }

        if (record != null)
        {
            pos = record.LastSensedPosition;
            return record.TargetID;
        }

        return -1;
    }
    /// <summary>
    /// 从记忆中获取一个可以进行射击的AI的位置和ID
    /// </summary>
    /// <returns></returns>
    public int GetClostCanShootEntityInMemory(ref Vector2 pos)
    {
        
        foreach(var m in this.sensoryMemory.Memorys.Values)
        {
            if (m.IsShootable)
            {
                pos = m.LastSensedPosition;

                return m.TargetID;
            }
        }

        return -1;
    }
    /// <summary>
    /// 获取最近的一个可以进行射击的AI
    /// </summary>
    /// <returns></returns>
    public MemoryRecord GetClostCanShootEntityInMemory()
    {
        float sqrtDist = this.owner.AttackDistance;
        MemoryRecord target = null;
        int targetID = -1;

        foreach (var m in sensoryMemory.Memorys.Values)
        {
            float sdist = (this.owner.Pos - m.LastSensedPosition).sqrMagnitude;

            if(sqrtDist>sdist)
            {
                targetID = m.TargetID;
                target = m;
                sqrtDist = sdist;
            }
        }
        
        return target;    
    }
    /// <summary>
    /// 获取一个视野范围内最近的一个目标
    /// </summary>
    /// <returns></returns>
    public MemoryRecord GetClostEntityInFOV()
    {
        float sqrtDist = distance;
        MemoryRecord target = null;
        int targetID = -1;
        foreach (var m in sensoryMemory.Memorys.Values)
        {
            float sdist = (this.owner.Pos - m.LastSensedPosition).sqrMagnitude;

            if (sqrtDist > sdist)
            {
                targetID = m.TargetID;
                target = m;
                sqrtDist = sdist;
            }
        }
        
        return target;
    }
    /// <summary>
    /// 获取一个最近被感知的实体
    /// </summary>
    /// <returns></returns>
    public MemoryRecord GetClostEntityLastSense()
    {
        float time = float.PositiveInfinity;

        MemoryRecord target = null;
        int targetID = -1;

        foreach(var m in this.sensoryMemory.Memorys.Values)
        {
            if(time>m.TimeLastSensed)
            {
                time = m.TimeLastSensed;
                target = m;
                targetID = m.TargetID;
            }
        }

        return target;
    }
    /// <summary>
    /// 判断一个对象是否可以被攻击
    /// </summary>
    /// <returns></returns>
    public bool isCanAttack(RavenEntity entity)
    {
        float sqrtDist = (entity.Pos - this.owner.Pos).sqrMagnitude;

        if (sqrtDist < this.owner.AttackDistance * this.owner.AttackDistance)
            return true;
        return false;
    }
    /// <summary>
    /// 移除一个记忆
    /// </summary>
    /// <returns></returns>
    public bool RemoveMemeory(int id)
    {
        return this.sensoryMemory.remove(id);
    }
    /// <summary>
    /// 感知管理器的消息处理函数
    /// </summary>
    /// <returns></returns>
    public bool OnMessage(Msg msg)
    {
        switch((EntityMsgType)msg.Op)
        {
            case EntityMsgType.HasBeenKilled:
                //将这个已经死亡的AI记忆进行移除
                this.sensoryMemory.remove(msg.SenderID);
                
                return true;
                
        }

        return false;
    }

    public void DebugComponent()
    {
        //Debug.Log("哈哈哈哈哈");
        Vector3 leftPoint = owner.transform.position + (Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * owner.transform.up) * distance;
        Vector3 rightPoint = owner.transform.position + (Quaternion.AngleAxis(-angle, new Vector3(0, 0, 1)) * owner.transform.up) * distance;

        Debug.DrawLine(owner.transform.position, leftPoint);
        Debug.DrawLine(owner.transform.position, rightPoint);
    }
}
