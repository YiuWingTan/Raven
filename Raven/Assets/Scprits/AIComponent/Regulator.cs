using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一个帮助AI组件按照自己的更新频率进行更新的类
/// </summary>
public class Regulator  {

    private float updatePeriod;//组件的更新周期

    private float updateOffset;//组件的更新偏移

    public Regulator(float period)
    {
        this.updatePeriod = period;
    }

    public float UpdatePeriod
    {
        get
        {
            return updatePeriod;
        }
    }

    /// <summary>
    /// 组件是否准备好更新
    /// </summary>
    /// <returns></returns>
    public bool isReady()
    {
        return false;
    }
}
