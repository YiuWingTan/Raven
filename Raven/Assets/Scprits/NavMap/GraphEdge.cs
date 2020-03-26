using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 图的边类
/// </summary>
public class GraphEdge {

    protected bool isActive;

    public bool IsActive
    {
        get { return isActive; }
    }

    protected int from;//起始点

    protected int to;//结束点

    protected float cost;//这条边的花费

    public int From
    {
        get { return from; }
    }

    public int To
    {
        get { return to; }
    }

    public float Cost
    {
        get { return cost; }
    }

    public GraphEdge(int from,int to)
    {
        this.from = from;

        this.to = to;

        this.cost = 0;
    }

    public GraphEdge(int from,int to,float cost = 0)
    {
        this.from = from;

        this.to = to;

        this.cost = cost;

        this.isActive = true;
    }

    public GraphEdge()
    {

    }

    public void SetFrom(int from)
    {
        this.from = from;
    }

    public void SetTo(int to)
    {
        this.to = to;
    }

    public void SetCost(float cost)
    {
        this.cost = cost;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }
}

/// <summary>
/// 导航图的边类
/// </summary>
public class NavGraphEdge:GraphEdge
{

    public NavGraphEdge():base()
    {

    }

    public NavGraphEdge(int from,int to,float cost = 0):base(from,to,cost)
    {

    }

 
}

/// <summary>
/// 边的行为
/// </summary>
public enum EdgeBehavious
{
    /// <summary>
    /// 正常行为
    /// </summary>
    Normal,

    /// <summary>
    /// 慢速行驶
    /// </summary>
    Slow
}

/// <summary>
/// 路径边类
/// </summary>
public class PathEdge
{
    private Vector2 source;

    private Vector2 target;

    private GraphEdge edge;

    public int from;

    public int to;

    private EdgeBehavious behavious;


    public PathEdge(Vector2 source,Vector2 target,EdgeBehavious behavious = EdgeBehavious.Normal)
    {
        this.source = source;

        this.target = target;

        this.behavious = behavious;
    }

    public PathEdge(GraphEdge edge,Vector2 source, Vector2 target, EdgeBehavious behavious = EdgeBehavious.Normal):this(source, target, behavious)
    {
        this.edge = edge;

        this.from = edge.From;

        this.to = edge.To;
    }

    public Vector2 Destination()
    {
        return this.target;
    }

    /// <summary>
    /// 设置目标点
    /// </summary>
    public void SetDestination(Vector2 destination)
    {
        this.target = destination;
    }

    public Vector2 Source()
    {
        return this.source;
    }

    /// <summary>
    /// 返回起始点
    /// </summary>
    /// <returns></returns>
    public void SetSource(Vector2 source)
    {
        this.source = source;
    }

    public EdgeBehavious Behavious()
    {
        return this.behavious;
    }

    public void SetEdge(GraphEdge e)
    {
        this.edge = e;
    }

    public GraphEdge GetEdge()
    {
        return this.edge;
    }

    public void SetTarget(int index)
    {
        this.to = index;
    }
}