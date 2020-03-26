using PathSearchAlgorithm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI的寻路组件
/// </summary>
public class PathPlanner 
{
    /// <summary>
    /// 对所有寻路算法的封装
    /// </summary>
    private PathFinder pathFinder;

    /// <summary>
    /// 时间片的回调方法
    /// </summary>
    private Action<List<PathEdge>> callBack;

    private RavenEntity owner;

    public RavenEntity Owner
    {
        get
        {
            return owner;
        }
    }

    public Vector2 TargetPos
    {
        get
        {
            return targetPos;
        }
    }

    private Vector2 targetPos;

    private NavMap map;

    private RaycastHit2D hit;

    private SearchTimeSliced searchTimeSliced;

    /// <summary>
    /// 获取最近节点的一个ID
    /// </summary>
    /// <returns></returns>
    private int GetCloseNodeID(Vector2 pos)
    {
        float dist = float.PositiveInfinity;

        int clostNodeID = -1;

        var cell = map.GetCellSpacePartition().GetNeighbors(pos);

        if (cell == null) return -1;

        foreach(var n in cell)
        {
            var p = (n.Pos - pos);
            float sqrtDist = Mathf.Abs(p.x) + Mathf.Abs(p.y);

            if (dist>sqrtDist)
            {
                dist = sqrtDist;

                clostNodeID = n.ID;
            }
        }

        return clostNodeID;
    }

    public PathPlanner(RavenEntity entity)
    {
        this.owner = entity;

        map = NavMap.Instance;

        pathFinder = new PathFinder();

        searchTimeSliced = new AStar_TimeSliced();
    }

    /// <summary>
    /// 创建一个到达可达位置的一条最短路径
    /// </summary>
    /// <returns></returns>
    public bool CreatePathToPositionByTimeSliced(Vector2 targetPos,Action<List<PathEdge>> callBack)
    {
        //使用A*
        int source = GetCloseNodeID(owner.Pos);

        int target = GetCloseNodeID(targetPos);
        //Debug.Log("最近的点的编号是"+target);
        if (source == -1 || target == -1)
        {
            //Debug.Log("节点无效");

            return false;
        }

        if(target == source)
        {
            Debug.Log("目标点和起点是一样的");
        }
        searchTimeSliced.SetSearch(source,target,owner.Pos, targetPos, NavMap.Instance.NavGraph);

        PathManager.Instance.RegistSearchRequest(this);

        this.callBack = callBack;

        return true;
    }

    /// <summary>
    /// 非时间片的寻路
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool CreatePathToPositionNonTimeSliced(Vector2 targetPos,ref List<PathEdge> path)
    {
        switch(GameController.Instance.GameSearchType)
        {
            case SearchType.AStar:
                path = pathFinder.GetPathByAStart(this.owner.Pos, targetPos,NavMap.Instance.NavGraph);
                //Debug.Log(path.Count);
                return true;
               
            case SearchType.BFS:
                path = pathFinder.GetPathByBFS(this.owner.Pos, targetPos, NavMap.Instance.NavGraph);
                return true;

            case SearchType.DFS:
                path = pathFinder.GetPathByDFS(this.owner.Pos, targetPos, NavMap.Instance.NavGraph);
                return true;

            case SearchType.DJ:
                path = pathFinder.GetPathByDijkstra(this.owner.Pos, targetPos, NavMap.Instance.NavGraph);
                return true;
        }
       
        return false;
    }

    /// <summary>
    /// 创建一个到达物品的一条最短路径
    /// </summary>
    /// <returns></returns>
    public bool CreatePathToItem(Vector2 targetPos)
    {
        return false;
    }

    /// <summary>
    /// 精确的路径平滑
    /// </summary>
    protected void SmoothPathEdgePrecise(ref List<PathEdge> path)
    {
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            for (int j = path.Count-1; j >i;j--)
            {

                if(isPass(path[i].Source(), path[j].Destination()))
                {
                    path[i].SetDestination(path[j].Destination());
                    path[i].SetTarget(path[j].to);

                    for (int k = i + 1; k <= j && k < path.Count; )
                   {
                        path.RemoveAt(k);
                        j--;
                   }

                    break;
                }

                //if (isPass(path[i].Source(), path[j].Destination()))
                //{
                //    path[i].SetDestination(path[j].Destination());
                //    path[i].SetTarget(path[j].to);

                //    for(int k = i+1;k<=j&&k<path.Count;k++)
                //    {
                //        path.RemoveAt(k);
                //    }

                //    j = i + 1;
                //}
                //else
                //{
                //    j++;
                //}
            }
        }
    }

    /// <summary>
    /// 快速的路径平滑
    /// </summary>
    protected void SmootPathEdgeQuick(ref List<PathEdge> path)
    {
        
        int i = 0, j = 1;

        while (i<path.Count-1)
        {
            if(isPass(path[i].Source(),path[j].Destination()))
            {
                path[i].SetDestination(path[j].Destination());

                path.RemoveAt(j);
                //j--;
                Debug.Log("没有阻碍了");

            }else
            {
                i++;
                j++;
                Debug.Log("存在阻碍");
            }
        }
    }

    /// <summary>
    /// 一个搜索周期
    /// </summary>
    public SearchState CycleOne()
    {
        SearchState state = SearchState.Searching ;
        for(int i = 0;i<5;i++)
         state =  searchTimeSliced.CycleOne();
        //Debug.Log("搜索结果是："+state.ToString()+" "+owner.gameObject.name);
        if(state == SearchState.Search_Complete)
        {
            //TODO 将当前的路径设置到path上
            //Debug.Log("找到一条路径");
            var path =  searchTimeSliced.GetPathAsPathEdges();

            //进行回调
            callBack(path);

        }

        return state;
    }

    private bool isPass(Vector2 p1,Vector2 p2)
    {
        hit = Physics2D.Linecast(p1, p2,(1<<9)|~(1<<0));
        if (hit.collider == null ) return true;
        return false;
    }

    public void SmoothPathEdge(ref List<PathEdge> p )
    {
        if(GameController.Instance.SmoothType == SmoothPathType.Best)
        {
            SmoothPathEdgePrecise(ref p);
            Debug.Log("精确");
        }
        else if(GameController.Instance.SmoothType == SmoothPathType.Quick)
        {
            SmootPathEdgeQuick(ref p);
            Debug.Log("快速");
        }
        //Debug.Log("路径平滑了");
    }
}

/// <summary>
/// 搜索状态枚举
/// </summary>
public enum SearchState
{
    Search_Fail,//搜索失败
    Search_Complete,//搜索完成
    Searching//搜索进行中
}

public enum SearchType
{
    AStar,
    DJ,
    DFS,
    BFS
}

/// <summary>
/// 搜索时间片类
/// </summary>
public class SearchTimeSliced
{

    protected bool isHavePath;

    protected NavGraph graph;

    protected int target;

    protected int source;

    protected Vector2 resourcePos;

    protected Vector2 targetPos;

    protected SearchType type;

    public SearchType Type
    {
        get
        {
            return this.type;
        }
    }

    /// <summary>
    /// 进行一次搜索周期
    /// </summary>
    /// <returns></returns>
     public virtual SearchState CycleOne()
    {
        //TODO 寻路的一个周期

        return SearchState.Searching;
    }

    /// <summary>
    /// 获取最短路径
    /// </summary>
    /// <returns></returns>
    public virtual List<GraphEdge> GetSPT()
    {
        return null;
    }

    public virtual List<PathEdge> GetPathAsPathEdges()
    {
        return null;
    }

    /// <summary>
    /// 获取到达目标点的花费
    /// </summary>
    /// <returns></returns>
    public virtual float GetCostToTarget()
    {
        return 90;
    }

    public virtual void SetSearch(int resourceID,int targetID,Vector2  resource, Vector2 target,NavGraph graph)
    {
        
    }

    public virtual void SetSearch(int resouce,int target)
    {

    }

    protected virtual void InitSearch(int source, int target, NavGraph graph)
    {

    }



    public virtual bool IsHavePath()
    {
        return false;
    }
}
