using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// AI的感知组件
/// </summary>
public class RavenSensoryMemory
{
    private Dictionary<int, MemoryRecord> memorys;
    private List<int> removeList;
    /// <summary>
    /// AI的感知时长
    /// </summary>
    private float memorySpan = 8;
    protected RavenEntity owner;



    public float MemorySpan
    {
        get
        {
            return memorySpan;
        }
    }
    public Dictionary<int, MemoryRecord> Memorys
    {
        get
        {
            return memorys;
        }
    }
    public RavenSensoryMemory(RavenEntity owner)
    {
        memorys = new Dictionary<int, MemoryRecord>();

        removeList = new List<int>();

        this.owner = owner;
    }
    /// <summary>
    /// 添加一个记录
    /// </summary>
    public void AddMemoryRecord(RavenEntity entity,MemoryRecord record)
    {
        if(Memorys.ContainsKey(entity.ID))
        {
            //如果记忆中已经有这个记录了
            var memory = Memorys[entity.ID];
            memory.TimeLastSensed = record.TimeLastSensed;
            memory.LastSensedPosition = record.LastSensedPosition;
            memory.TimeLastVisible = record.TimeLastVisible;
            memory.IsShootable = record.IsShootable;
            memory.IsWithinFov = record.IsWithinFov;
            //Debug.Log("记忆中已经有这个记录了");
        }else
        {
            record.TimeBecameVisible = record.TimeLastVisible;
            record.TimeBecomeSense = record.TimeLastSensed;

            Memorys.Add(entity.ID,record);

            //Debug.Log("记忆中没有这个记录已经进行添加,当前的记忆有"+Memorys.Values.Count);
        }

        
    }
    /// <summary>
    /// 通过声音来进行记忆的更新
    /// </summary>
    public void UpdateMemoryBySound()
    {

    }
    /// <summary>
    /// 跟新哪些在AI视野范围内的记忆
    /// </summary>
    public void UpdateVision()
    {

    }
    /// <summary>
    /// 是否对手可以射击
    /// </summary>
    /// <returns></returns>
    public bool isOpponentShootable(RavenEntity entity) 
    {
        if(Memorys.ContainsKey(entity.ID))
        {
            return Memorys[entity.ID].IsShootable;
        }

        return false;
    }
    /// <summary>
    /// 判断对手是否在自己的视野之中
    /// </summary>
    /// <returns></returns>
    public bool isOpponentWithinFOV(RavenEntity entity)
    {
        if (Memorys.ContainsKey(entity.ID))
        {
            return Memorys[entity.ID].IsWithinFov;
        }

        return false;
    }
    /// <summary>
    /// 获取对手上一次被记录的位置
    /// </summary>
    /// <returns>当记忆中没有这样的记录的时候返回-1</returns>
    public int GetLastRecordedPositionOfOpponent(RavenEntity entity,ref Vector2 pos)
    {
        if(Memorys.ContainsKey(entity.ID))
        {
            pos = Memorys[entity.ID].LastSensedPosition;

            return 1;
        }

        pos = new Vector2();
        return -1;
        
    }
    /// <summary>
    /// 获取对手被看到的时间
    /// </summary>
    /// <returns></returns>
    public float GetTimeOpponentHasBeenVisible(RavenEntity entity)
    {
        if (Memorys.ContainsKey(entity.ID))
        {
            var record = Memorys[entity.ID];
            return record.TimeLastVisible - record.TimeBecameVisible;
        }
        return 0;
    }
    /// <summary>
    /// 获取对手上次给感知的时间
    /// </summary>
    /// <returns></returns>
    public float GetTimeSinceLastSensed(RavenEntity entity)
    {
        if (Memorys.ContainsKey(entity.ID))
        {
            var record = Memorys[entity.ID];
            return record.TimeLastSensed - record.TimeLastSensed;
        }
        return 0;
    }
    /// <summary>
    /// 获取对手离开AI视野的时间
    /// </summary>
    /// <returns></returns>
    public float GetTimeOpponentHasBeenOutOfView(RavenEntity entity)
    {

        if (Memorys.ContainsKey(entity.ID))
        {
            var record = Memorys[entity.ID];
            if(record.TimeLastVisible!=float.PositiveInfinity)
                return Time.time - record.TimeLastVisible;
        }

        return float.PositiveInfinity;
    }
    /// <summary>
    ///获取最近(在最近的一个记忆间隔内)被更新过的对手的列表
    /// </summary>
    /// <returns></returns>
    public List<RavenEntity> GetListOfRecentlySense()
    {
        return new List<RavenEntity>();
    }
    /// <summary>
    /// 更新记忆,进行记忆的移除和记忆的状态更新
    /// </summary>
    public void UpdateMemory()
    {
        
        foreach (var m in Memorys.Values)
        {
            if (m.IsBeremove)
            {
                //根据标志进行记忆的移除
                removeList.Add(m.TargetID);
                //Debug.Log("记忆已经到期了");
                continue;
            }

            float remainTime = Time.time - m.TimeLastSensed;
            
            if (remainTime > memorySpan)
            {
                //当记忆到期时设置标记
                m.SetRemoveMark();
            }else if(remainTime>0.5f)
            {
                //这个记忆已经是过去的记忆了
                m.IsShootable = false;
                m.IsWithinFov = false;
            }
        }

        foreach(var l in removeList)
        {
            Memorys.Remove(l);
        }

        removeList.Clear();
    }
    /// <summary>
    /// 移除一个记忆
    /// </summary>
    /// <returns></returns>
    public bool remove(int id)
    {
        if(memorys.ContainsKey(id))
        {
            
            this.memorys.Remove(id);
            return true;
        }

        return false;
    }
    
}
