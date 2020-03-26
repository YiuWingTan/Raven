using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本的图节点类
/// </summary>
public class GraphNode {

    protected int id;//ID号

    protected float cost;//该节点的花费

    public int ID
    {
        get { return id; }

        set { id = value; }
    }

    public GraphNode()
    {

    } 

    public GraphNode(int id)
    {
        this.id = id;
    }

    public GraphNode(int id,float cost)
    {
        this.id = id;

        this.cost = cost;
    }


    public void SetCost(float cost)
    {
        this.cost = cost;
    }
}