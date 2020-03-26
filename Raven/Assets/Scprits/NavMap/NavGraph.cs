//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeToEdges
{
    private NavGraphNode node;

    private List<GraphEdge> edges;

    public NodeToEdges(NavGraphNode node)
    {
        this.node = node;

        edges = new List<GraphEdge>();

        node.SetActive(true);
    }

    public void SetActive(bool isActive)
    {
        node.SetActive(isActive);
    }

    public void AddEdge(GraphEdge e)
    {
        if(e!=null)
        edges.Add(e);
        
    }

    public void AddEdge(int from ,int to,float cost = 0)
    {
        GraphEdge e = new GraphEdge(from,to,cost);

        AddEdge(e);
    }

    public void RemoveEdge(int from,int to)
    {
        foreach(var e in edges)
        {
            if(e.To == to)
            {
                e.SetActive(false);

                break ;
            }
        }
    }

    public NavGraphNode GetNode()
    {
        return node;
    }

    /// <summary>
    /// 判断节点是否被激活
    /// </summary>
    /// <returns></returns>
    public bool IsActive
    {
        get { return node.IsActive; }
    }

    public GraphEdge GetEdge( int to)
    {

        foreach(var e in edges)
        {
            if (e.To == to)
                return e;
        }

        return null;
    }

    public List<GraphEdge> GetAdjoinEdges()
    {
        return edges;
    }
}

/// <summary>
/// 导航图采用邻接表的实现
/// </summary>
public class NavGraph{

    private int totalV;//顶点个数

    private int totalE;//边的个数

    private int activeV;//激活的点的个数

    private int activeE;//激活边的个数

    public int TotalV
    {
        get { return totalV; }
    }

    public int TotalE
    {
        get { return totalE; }
    }

    public int ActiveV
    {
        get { return activeV; }
    }

    public int ActiveE
    {
        get { return activeE; }
    }

    private List<NodeToEdges> tables;

    public NavGraph()
    {
        tables = new List<NodeToEdges>();
    }


    /// <summary>
    /// 添加节点
    /// </summary>
    public void AddNode(int id,float x,float y,float cost = 0)
    {
        if(id>tables.Count)
        {
            Debug.Log("AddNode中：要进行添加的节点的id号错误");
            return;
        }

        NavGraphNode node = new NavGraphNode(id,x,y,cost);

        NodeToEdges t = new NodeToEdges(node);

        tables.Add(t);

        totalV++;

        activeV++;
    }


    public void AddNode(NavGraphNode node)
    {
        if(HasNode(node.ID))
        {
            Debug.Log("要进行添加的顶点已经存在");
            return;
        }

        if(node.ID>tables.Count)
        {
            Debug.Log("要进行添加的节点的id号错误");
            return;
        }

        NodeToEdges t = new NodeToEdges(node);

        tables.Add(t);

        totalV++;

        activeV++;
    }

    /// <summary>
    /// 添加边
    /// </summary>
    public void AddEdge(int from,int to,float cost = 0)
    {
        
        if(!HasNode(from)||!HasNode(to))
        {
            Debug.Log("AddEdge中要进行添加的边对应的节点不存在");
            return;
        }
        if(tables[from].GetNode().ID!=from)
        {
            Debug.Log("AddEdge中：tables的id号和想要进行添加的节点的id号不同");

            return;
        }
        tables[from].AddEdge(from,to,Random.Range(0,cost));

        totalE++;

        activeE++;
    }


    /// <summary>
    /// 移除节点
    /// </summary>
    public void RemoveNode(int id)
    {
        if(!HasNode(id))
        {
            Debug.Log("RemoveNode中：要进行移除的节点的id号不存在");

            return;
        }

        if(tables[id].GetNode().ID != id)
        {
            Debug.Log("RemoveNode中table的id号和要进行移除的id号不一致");

            return;
        }

        tables[id].SetActive(false);

        activeV--;
    }


    /// <summary>
    /// 移除
    /// </summary>
    public void RemoveEdge(int from,int to)
    {
        if (!HasNode(from) || !HasNode(to))
        {
            Debug.Log("RemoveEdge中：要进行移除的边对应的节点不存在");
            return;
        }

        if (tables[from].GetNode().ID != from)
        {
            Debug.Log("RemoveEdge中table的id号和要进行移除的id号不一致");

            return;
        }

        tables[from].RemoveEdge(from,to);

        activeE--;
    }

    /// <summary>
    /// 获取节点，发生错误时返回null
    /// </summary>
    public NavGraphNode GetNode(int id)
    {
        if (!HasNode(id))
        {
            Debug.Log("GetNode中：要进行获取的节点的id号不存在");

            return null;
        }

        if (tables[id].GetNode().ID!=id)
        {
            Debug.Log("GetNode中：tables的id号和想要进行获取的id号不一致");
            return null;
        }

        return tables[id].GetNode();
    }

    /// <summary>
    /// 获取边当发生错误时返回null
    /// </summary>
    public GraphEdge GetEdge(int from,int to)
    {
        if (!HasNode(from) || !HasNode(to))
        {
            Debug.Log("GetEdge中：要进行获取的边对应的节点不存在");
            return null;
        }

        if (tables[from].GetNode().ID!=from)
        {
            Debug.Log("GetEdge中：tables的id号和id不对应");

            return null;
        }

        return tables[from].GetEdge(to);
    }

    /// <summary>
    /// 判断一个节点是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool HasNode(int id)
    {
        if(id>=tables.Count)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否存在一条边
    /// </summary>
    /// <returns></returns>
    public bool HasEdge(int from ,int to)
    {
        if(!HasNode(from) ||!HasNode(to))
        {
            Debug.Log("HasEdge:需要进行判断是否存在边时存入的顶点不存在");
            return false;
        }

        var edge = tables[from].GetEdge(to);

        if (edge != null) return true;
        return false;
    }

    /// <summary>
    /// 获取一个节点的所有邻接边
    /// </summary>
    /// <returns></returns>
    public List<GraphEdge> GetAdjoinEdges(int id)
    {
        if (!HasNode(id)) return new List<GraphEdge>();

        return tables[id].GetAdjoinEdges();
    }

}
