using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音通告触发器
/// </summary>
public class SoundNoityTrigger : LimitedLifeTimeTrigger {

    public override void TryTrigger(RavenEntity entity)
    {
        if(isActive()&&region.isTouching(entity.Pos,entity.Radius))
        {
            //TODO 发出一个消息通知实体这个触发器范围内有声音
        }
    }
}
