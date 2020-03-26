using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI的记忆类
/// </summary>
public class MemoryRecord {

    private int targetID;

    /// <summary>
    /// 记录最新一次被感知的时间
    /// </summary>
    private float timeLastSensed;
    /// <summary>
    /// 保存对手最早被看到的时间
    /// </summary>
    private float timeBecameVisible;
    /// <summary>
    /// 保存对手最新一次被看到的时间
    /// </summary>
    private float timeLastVisible;
    /// <summary>
    /// 保存对手最新一次被感知到时的位置
    /// </summary>
    private Vector2 lastSensedPosition;
    /// <summary>
    /// 被感知到的对手是否在自己的视野中
    /// </summary>
    private bool isWithinFov;
    /// <summary>
    /// 被感知到的对手是否在无遮挡
    /// </summary>
    private bool isShootable;
    /// <summary>
    /// 最早一次被感知到的时间
    /// </summary>
    private float timeBecomeSense;
    //是否要被移除了
    private bool isBeremove;

    public float TimeLastSensed
    {
        get
        {
            return timeLastSensed;
        }

        set
        {
            timeLastSensed = value;
        }
    }

    public float TimeBecameVisible
    {
        get
        {
            return timeBecameVisible;
        }

        set
        {
            timeBecameVisible = value;
        }
    }

    public float TimeLastVisible
    {
        get
        {
            return timeLastVisible;
        }

        set
        {
            timeLastVisible = value;
        }
    }

    public Vector2 LastSensedPosition
    {
        get
        {
            return lastSensedPosition;
        }

        set
        {
            lastSensedPosition = value;
        }
    }

    public bool IsWithinFov
    {
        get
        {
            return isWithinFov;
        }

        set
        {
            isWithinFov = value;
        }
    }

    public bool IsShootable
    {
        get
        {
            return isShootable;
        }

        set
        {
            isShootable = value;
        }
    }

    public float TimeBecomeSense
    {
        get
        {
            return timeBecomeSense;
        }

        set
        {
            timeBecomeSense = value;
        }
    }

    public bool IsBeremove
    {
        get
        {
            return isBeremove;
        }
    }

    public int TargetID
    {
        get
        {
            return targetID;
        }

        set
        {
            targetID = value;
        }
    }

    public void SetRemoveMark()
    {
        this.isBeremove = true;
    }
}
