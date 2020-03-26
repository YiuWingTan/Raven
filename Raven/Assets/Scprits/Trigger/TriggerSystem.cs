using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发器管理类
/// </summary>
public class TriggerSystem : MonoBehaviour {

    private static TriggerSystem _instance;

    public static TriggerSystem Instance
    {
        get
        {
            return _instance;
        }
    }

    private List<Trigger<RavenEntity>> triggers;

    private void Awake()
    {
        _instance = this;

        triggers = new List<Trigger<RavenEntity>>();
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// 进行触发器的更新
    /// </summary>
    private void Update()
    {
        UpdateTrigger();

        TryTrigger();
    }

    public void RegisterTrigger(Trigger<RavenEntity> trigger)
    {
        triggers.Add(trigger);
    }

    public void RemoveTrigger(Trigger<RavenEntity> trigger)
    {
        triggers.Remove(trigger);
    }

    /// <summary>
    /// 尝试每一个触发器
    /// </summary>
    public void TryTrigger()
    {


        foreach (var entity in EntityManager.Instance.EntityContainer.Values)
        {
            foreach (var trigger in triggers)
            {
                if (entity is MovingEntity && entity.IsAlive)
                {
                    //如果entity可以转换为MovingEntity
                    trigger.TryTrigger((RavenEntity)entity);
                }
            }
        }
    }

    /// <summary>
    /// 更新每一个触发器
    /// </summary>
    public void UpdateTrigger()
    {
        for (int i = 0; i < triggers.Count; i++)
        {
            if (!triggers[i].isToBeRemove())
            {
                triggers[i].UpdateTrigger(Time.deltaTime);
            }
            else 
            {
                triggers.RemoveAt(i);

                i--;
            }
        }
    }

    /// <summary>
    /// 获取一个离指定AI最近的触发器
    /// </summary>
    public Trigger<RavenEntity> GetTriggerByType(TriggerType type,RavenEntity entity)
    {
        float sqrtDist = float.PositiveInfinity;
        Trigger<RavenEntity> target = null;

        foreach (var r in triggers)
        {
            if (r.Type == type&&r.isActive())
            {
                float sdist = (r.Pos - entity.Pos).sqrMagnitude;//乘以一个偏移因子方便计算

                if (sdist < sqrtDist)
                {
                    target = r;
                    sqrtDist = sdist;
                }
            }
        }

        return target;
    }

    /// <summary>
    /// 获取一个和指定的AI距离最近的触发器
    /// </summary>
    /// <param name="type">指定触发器的类型</param>
    /// <param name="entity">指定一个AI</param>
    /// <param name="trigger">要获取的触发器的引用</param>
    /// <returns>返回实体和触发器的范围不存在这样一个触发器的时候返回无穷大</returns>
    public float GetClostTriggerByRaven(TriggerType type,RavenEntity entity,ref Trigger<RavenEntity> trigger)
    {
        float sqrtDist = float.PositiveInfinity;

        foreach(var r in triggers)
        {
            if(r.Type == type&&r.isActive())
            {
                float sdist = (r.Pos - entity.Pos).sqrMagnitude;//乘以一个偏移因子方便计算

                if (sdist < sqrtDist)
                {
                    if(trigger!=null)
                    trigger = r;
                    sqrtDist = sdist;
                }
            }
        }

        return sqrtDist;
    }

    public float GetClostTriggerByRaven(TriggerType type, RavenEntity entity)
    {
        float sqrtDist = float.PositiveInfinity;

        foreach (var r in triggers)
        {
            if (r.Type == type&&r.isActive())
            {
                float sdist = (r.Pos - entity.Pos).sqrMagnitude;//乘以一个偏移因子方便计算

                if (sdist < sqrtDist)
                {
                    sqrtDist = sdist;
                }
            }
        }

        return sqrtDist;
    }
}
