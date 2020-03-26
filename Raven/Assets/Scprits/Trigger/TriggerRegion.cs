using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发器范围基类
/// </summary>
public class TriggerRegion {

	public virtual bool isTouching(Vector2 pos, float entityRadius)
    {
        return false;
    }
}
