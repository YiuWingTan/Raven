using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 导航图节点类
/// </summary>
public class NavGraphNode :GraphNode {

    /// <summary>
    /// 当前可用的ID号
    /// </summary>
    public static int activeID = 0;

    protected Vector2 pos;

    public Vector2 Pos
    {
        get { return pos; }
    }

    protected bool isActive;//标记这个节点是否是激活的

    public bool IsActive
    {
        get { return isActive; }
    }

    protected object extraInfo;//这个导航节点图中的额外信息

    public object ExtraInfo
    {
        get { return extraInfo; }
    }

    public NavGraphNode(int id):base(id)
    {

    }

    public NavGraphNode(int id,float cost):base(id,cost)
    {
    }

    public NavGraphNode(int id,float x,float y,float cost = 0)
    {
        this.id = id;

        pos = new Vector2(x,y);
    }

    public NavGraphNode(int id,Vector2 pos,float cost = 0)
    {
        this.id = id;

        this.pos = pos;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

}
