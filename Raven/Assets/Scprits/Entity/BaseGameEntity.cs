using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础的实体类
/// </summary>
public class BaseGameEntity : MonoBehaviour {

    /// <summary>
    /// 当前的实体的有效ID
    /// </summary>
    protected static int activeID;

    protected int id;

    public int ID
    {
        get
        {
            return id;
        }
    }

    protected bool isAlive;

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected virtual void Init()
    {
        isAlive = true;

        this.id = activeID++;

        EntityManager.Instance.RegisterEntity(this);
    }

    protected virtual void EntityUpdate()
    {

    }

    public virtual bool OnMessage(Msg msg)
    {
        return false;
    }
}
