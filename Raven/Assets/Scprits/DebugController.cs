using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueue;
using PathSearchAlgorithm;
using UnityEngine.UI;

/// <summary>
/// 测试线类
/// </summary>
public class DebugLine:MonoBehaviour
{
    private Vector3 p;

    private Vector3 l;

    public Vector2 P
    {
        get
        {
            return p;
        }

       
    }

    public Vector2 L
    {
        get
        {
            return l;
        }
    }

    private LineRenderer lineRender;

    public void SetDebugLine(Vector3 start,Vector3 end,LineRenderer lineRenderer)
    {
        this.lineRender = lineRenderer;

        
        this.lineRender.SetPosition(0,start);
        this.lineRender.SetPosition(1,end);
    }
}



/// <summary>
/// 测试点类
/// </summary>
public class DebugPoint:MonoBehaviour
{
    [SerializeField]
    private int id;

    public int ID
    {
        get
        {
            return id;
        }

        set {
            this.id = value;
            //TODO  测试
            this.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = this.id.ToString() ;
        }
    }
}


/// <summary>
/// 可视化控制器
/// </summary>
public class DebugController : MonoBehaviour {

    private static DebugController _instance;

    public static DebugController instance
    {
        get
        {

            return _instance;
        }
    }
    [SerializeField]
    private Transform pointsParent;//保存所有的点的父节点
    [SerializeField]
    private Transform lineParent;//保存所有线的父节点

    [SerializeField]
    private List<DebugLine> lines;
    [SerializeField]
    private List<DebugPoint> points;
    private List<DebugLine> pathLines;//路径线


    [SerializeField]
    private GameObject pointPrefab;//点的预制体
    [SerializeField]
    private GameObject linePrefab;//线的预制体
    [SerializeField]
    private GameObject pathLinePrefab;
    [SerializeField]
    private GameObject cellSpaceLine;//单元空间的预制体

    private void Awake()
    {
        _instance = this;
   
        points = new List<DebugPoint>();
        lines = new List<DebugLine>();
        pathLines = new List<DebugLine>();
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        //IndexPriorityQueue<int> pq = new IndexPriorityQueue<int>((i,j)=>( i - j),PriorityOrder.MinFirst);

        //for(int i = 0;i<200;i++)
        //{
        //    pq.Dequeue(Random.Range(0,1000));
        //}

        //print("优先队列中有"+pq.Count+"个元素");

        //while (!pq.isEmpty())
        //{
        //    Debug.Log(pq.Enqueue());
        //}

        //print("优先队列中有"+pq.Count+"个元素,空间大小为"+pq.Size);


        
        
    }

    /// <summary>
    /// 进行图形的渲染方法
    /// </summary>
    public void ToDebug()
    {
        NavMap.Instance.GetCellSpacePartition().ToDebug(cellSpaceLine);
    }

    public void AddPoint(NavGraphNode node)
    {
        var pointObject = GameObject.Instantiate(pointPrefab, new Vector3(node.Pos.x,node.Pos.y,-2), Quaternion.identity);

        pointObject.transform.parent = pointsParent;

        var component = pointObject.AddComponent(typeof(DebugPoint)) as DebugPoint;

        component.ID = node.ID;

        
        points.Add(component);
    }

    /// <summary>
    /// 用于第一个点的初始化
    /// </summary>
    /// <param name="go"></param>
    public void AddPoint(int id,GameObject go)
    {
        go.transform.parent = pointsParent;
        var component = go.AddComponent(typeof(DebugPoint)) as DebugPoint;

        component.ID = id;

        points.Add(component);
    }

    public void AddLine(Vector2 p1,Vector2 p2)
    {
        
        var line = GameObject.Instantiate(linePrefab, new Vector3(0,0,0),Quaternion.identity);

        line.transform.parent = lineParent;

        var lineComponent = line.AddComponent(typeof(DebugLine)) as DebugLine;

        lineComponent.SetDebugLine(new Vector3(p1.x,p1.y,-1),new Vector3(p2.x,p2.y,-1),line.GetComponent<LineRenderer>());

        lines.Add(lineComponent);
        
    }

    public void AddPathLine(Vector2 p1,Vector2 p2)
    {
        var line = GameObject.Instantiate(pathLinePrefab, new Vector3(0, 0, 0), Quaternion.identity);

        line.transform.parent = lineParent;

        var lineComponent = line.AddComponent(typeof(DebugLine)) as DebugLine;

        lineComponent.SetDebugLine(new Vector3(p1.x, p1.y, -2), new Vector3(p2.x, p2.y, -2), line.GetComponent<LineRenderer>());

        pathLines.Add(lineComponent);
    }

    /// <summary>
    /// 删除所有的路径线
    /// </summary>
    public void CleanPathLine()
    {
        foreach(var g in pathLines)
        {
            GameObject.Destroy(g.gameObject);
        }

        pathLines = new List<DebugLine>();
    }
}
