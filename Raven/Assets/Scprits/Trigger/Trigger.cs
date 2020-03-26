using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    HealthGiverTrigger,
    WeaponGiverTrigger,
    BulletGiverTrigger
}

/// <summary>
/// 触发器基类
/// </summary>
public class Trigger<EntityType> :MonoBehaviour where EntityType : RavenEntity   
{
    protected TriggerType type;

    public TriggerType Type
    {
        get { return this.type; }
    }

    /// <summary>
    /// 表示当前可用的ID
    /// </summary>
    public static int ActiveID;

    //TODO 触发器的ID
    protected int id = -1;

    public int ID
    {
        get
        {
            return this.id;
        }
    }

    //触发器范围对象的引用
    protected TriggerRegion region;

    private bool active;

    private bool isRemoveFromGame;//是否要从游戏中进行去除

    private int graphNodeIndex;//和那一个图的节点相互绑定

    protected Vector2 pos;

    public Vector2 Pos
    {
        get
        {
            return pos;
        }
    }

    //TODO 传入一个触发器范围
    protected virtual void Init()
    {
        active = true;

        isRemoveFromGame = false;

        this.id = ActiveID++;

        TriggerSystem.Instance.RegisterTrigger(this as Trigger<RavenEntity>);
    }

    protected virtual void SetActive()
    {
        if (!active)
        {
            active = true;
            
        }
    }

    protected virtual void SetInActive()
    {
        if (active)
        {
            active = false;
            
        }
    }

    protected void SetNodeIndex(int index,NavMap map)
    {
        this.graphNodeIndex = index;

        pos = map.NavGraph.GetNode(index).Pos;

        this.transform.position = new Vector3(pos.x,pos.y,-5);
    }

    protected void SetToBeRemoveFromGame()
    {
        if (!isRemoveFromGame) isRemoveFromGame = true;
    }

    protected bool isTouchingTrigger(Vector2 pos, float entityRadius)
    {
        return region.isTouching(pos,entityRadius);
    }

    /// <summary>
    /// 设置一个圆形触发器范围
    /// </summary>
    protected void SetCircularTriggerRegion(Vector2 pos,float radius)
    {
        this.region = new CircularTriggerRegion(pos,radius);
    }

    /// <summary>
    /// 设置一个正方形的触发器范围
    /// </summary>
    protected void SetRectangleTriggerRegion(Vector2 topLeft,Vector2 bottomRight)
    {

    }

    public virtual void TryTrigger(EntityType entity)
    {

    }

    /// <summary>
    /// 更新触发器的内部状态
    /// </summary>
    public virtual void UpdateTrigger(float detalTime)
    {

    } 

    public int GetGraphNodeIndex()
    {
        return this.graphNodeIndex;
    }

    public bool isToBeRemove()
    {
        return this.isRemoveFromGame;
    }

    public bool isActive()
    {
        return this.active;
    }

}
