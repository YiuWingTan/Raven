using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 填充状态
/// </summary>
enum FillState
{
    Success,
    FailByOtherPoint,
    FailByWall
}

/// <summary>
/// 导航图的类
/// </summary>
public class NavMap : MonoBehaviour {

    [SerializeField]
    private float lowCost;
    [SerializeField]
    private float heightCost;

    private static NavMap _instance;

    public static NavMap Instance
    {
        get
        {
            return _instance;
        }
    }

    public NavGraph NavGraph
    {
        get
        {
            return navGraph;
        }
    }
    [SerializeField]
    private Transform upBorder;//上边界
    [SerializeField]
    private Transform downBorder;//下边界

    private CellSpacePartition<NavGraphNode> cellSpacePartition;//单元空间分割

    [SerializeField]
    private float distanceBetweenNode;//检查距离

    private float distanceBetweenDiagonal;//对角线的检查距离

    private NavGraph navGraph;//导航图的邻接表类

    private PathFinder pathFinder;//路径寻找者

    #region 生成单元的方向的单位向量组
    private Vector2 up = Vector2.up;

    private Vector2 down = Vector2.down;

    private Vector2 left = Vector2.left;

    private Vector2 right = Vector2.right;

    private Vector2 leftUp;//左上角

    private Vector2 rightUp;//右上角

    private Vector2 leftDown;//左下角

    private Vector2 rightDown;//右下角
    #endregion

    private void Awake()
    {
        _instance = this;
        navGraph = new NavGraph();

        //进行方向向量的初始化
        pathFinder = new PathFinder();

        leftUp = (Vector2.up + Vector2.left).normalized;

        rightUp = (Vector2.up + Vector2.right).normalized;

        rightDown = (Vector2.right + Vector2.down).normalized;

        leftDown = (Vector2.left + Vector2.down).normalized;

        cellSpacePartition = new CellSpacePartition<NavGraphNode>(Mathf.Abs(upBorder.position.x - downBorder.position.x)
            , Mathf.Abs(upBorder.position.y - downBorder.position.y)
            , 8, 8);

        CreateMap();

        Debug.Log("总共有" + NavGraph.TotalV + "个顶点");

        Debug.Log("总共有" + NavGraph.TotalE + "条边");

        ToDebug();
    }

    private void Start()
    {
        
    }

    #region 导航图生成的方法组
    /// <summary>
    /// 生成导航图的方法
    /// </summary>
    void CreateMap()
    {
        var startPoint =  GameObject.Find("StartPoint");

        distanceBetweenDiagonal = Mathf.Sqrt(distanceBetweenNode* distanceBetweenNode+ distanceBetweenNode* distanceBetweenNode);

        NavGraphNode startNode = new NavGraphNode(NavGraphNode.activeID++,startPoint.transform.position.x,startPoint.transform.position.y);

        NavGraph.AddNode(startNode);
        DebugController.instance.AddPoint(NavGraphNode.activeID-1, startPoint);

        FloodFill(startNode);
    }

    /// <summary>
    /// (八向)洪水填充算法
    /// </summary>
    private void FloodFill(NavGraphNode startPoint)
    {
        Queue<NavGraphNode> queue = new Queue<NavGraphNode>() ;

        queue.Enqueue(startPoint);

        FillState state;
        int  id = 0;
        while (queue.Count!=0)
        {
            var node = queue.Dequeue();

            #region 进行八个方向的填充


            //上方
            if ((state = isFill(node.Pos, Vector2.up, distanceBetweenNode, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + up * distanceBetweenNode,0,lowCost);
                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, lowCost);
                    //NavGraph.AddEdge(id, node.ID, lowCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }
            //下方
            if ((state = isFill(node.Pos, Vector2.down, distanceBetweenNode, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + down * distanceBetweenNode,0, lowCost);
                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, lowCost);
                    //NavGraph.AddEdge(id, node.ID, lowCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }

            //左边
            if ((state = isFill(node.Pos, Vector2.left, distanceBetweenNode, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + left * distanceBetweenNode,0, lowCost);
                queue.Enqueue(n);

            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, lowCost);
                    //NavGraph.AddEdge(id, node.ID, lowCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }

            //右边
            if ((state = isFill(node.Pos, Vector2.right, distanceBetweenNode, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + right * distanceBetweenNode,0, lowCost);

                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, lowCost);
                    //NavGraph.AddEdge(id, node.ID, lowCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }
            //左上角
            if ((state = isFill(node.Pos, Vector2.up + Vector2.left, distanceBetweenDiagonal, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + leftUp * distanceBetweenDiagonal, 0, heightCost);
                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, heightCost);
                    //NavGraph.AddEdge(id, node.ID, heightCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }
            //右上角
            if ((state = isFill(node.Pos, Vector2.up + Vector2.right, distanceBetweenDiagonal, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + rightUp * distanceBetweenDiagonal, 0, heightCost);
                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, heightCost);
                    //NavGraph.AddEdge(id, node.ID, heightCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }
            //左下角
            if ((state = isFill(node.Pos, Vector2.down + Vector2.left, distanceBetweenDiagonal, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + leftDown * distanceBetweenDiagonal, 0, heightCost);
                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, heightCost);
                    //NavGraph.AddEdge(id, node.ID, heightCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }
            //右下角
            if ((state = isFill(node.Pos, Vector2.down + Vector2.right, distanceBetweenDiagonal, ref id)) == FillState.Success)
            {
                var n = AddPointAndNode(node, node.Pos + rightDown * distanceBetweenDiagonal, 0, heightCost);
                queue.Enqueue(n);
            }
            else if (state == FillState.FailByOtherPoint)
            {
                if (id >= 0 && !navGraph.HasEdge(node.ID, id))
                {
                    NavGraph.AddEdge(node.ID, id, heightCost);
                    //NavGraph.AddEdge(id, node.ID, heightCost);
                    DebugController.instance.AddLine(node.Pos, navGraph.GetNode(id).Pos);
                }
            }
            #endregion
        }
    }


    /// <summary>
    /// 判断一个位置上的填充是否可行
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    private FillState isFill(Vector2 pos, Vector2 direction, float distance,ref int id)
    {
        RaycastHit2D[] hits = new RaycastHit2D[3];
    
       int num = Physics2D.RaycastNonAlloc(pos,direction,hits, distance,(1<<9)|(1<<0));

        if (num == 1)
        {
            id = -1;
            return FillState.Success;
        }
        else if (hits[1].collider.gameObject.tag == "Point")
        {
            id = hits[1].collider.GetComponent<DebugPoint>().ID;
            return FillState.FailByOtherPoint;
        }
        id = -1;
        return FillState.FailByWall;
    }

    private NavGraphNode AddPointAndNode(NavGraphNode start,Vector2 pos,float ncost = 0,float ecost = 0)
    {
        NavGraphNode node = new NavGraphNode(NavGraphNode.activeID++,pos,ncost);

        NavGraph.AddNode(node);

        //将这个点添加到对应的单元中
        cellSpacePartition.AddEntity(node,node.Pos,node.ID);

        //创建顶点时两个方向上都要有连线进行链接
        NavGraph.AddEdge(start.ID,node.ID,ecost);
        

        DebugController.instance.AddPoint(node);

        DebugController.instance.AddLine(start.Pos,node.Pos);

        return node;
    }

    #endregion

    public CellSpacePartition<NavGraphNode> GetCellSpacePartition()
    {
        return cellSpacePartition;
    }

    public List<PathEdge> GetPathByDfs(Vector3 start,Vector3 end)
    {
        return pathFinder.GetPathByDFS(start,end,this.navGraph);
    }

    public List<PathEdge> GetPathByBfs(Vector3 start,Vector3 end)
    {
        return pathFinder.GetPathByBFS(start,end,this.navGraph);
    }

    public List<PathEdge> GetPathByDijistra(Vector3 start,Vector3 end)
    {
        return pathFinder.GetPathByDijkstra(start,end,this.navGraph);
    }

    public List<PathEdge> GetPathByAStart(Vector3 start, Vector3 end)
    {
        return pathFinder.GetPathByAStart(start, end, this.navGraph);
    }


    /// <summary>
    /// 可视化
    /// </summary>
    public void ToDebug()
    {
        DebugController.instance.ToDebug();
    }
}
