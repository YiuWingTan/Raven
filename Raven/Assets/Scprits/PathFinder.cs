using PathSearchAlgorithm;
using PriorityQueue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 路径寻找者类,封装了各种寻路算法的引用（非时间片）
/// </summary>
public class PathFinder {

    private BFS bfs;

    private DFS dfs;

    private Dijkstra dj;

    private AStart aStart;

    public PathFinder()
    {
        bfs = new BFS();

        dfs = new DFS();

        dj = new Dijkstra();

        aStart = new AStart(AStartHeuristic.Euclid);
    }

    /// <summary>
    /// 获取离目标位置最近的那个节点的ID
    /// </summary>
    /// <returns></returns>
    private int GetClostNodeID(Vector2 target,List<NavGraphNode> cell)
    {
        int id = -1 ;

        float minDist = float.PositiveInfinity;

        foreach(var n in cell)
        {
            float dist = (target - n.Pos).sqrMagnitude;

            if(minDist>dist)
            {
                id = n.ID;

                minDist = dist;
            }
        }

        return id;
    }

    /// <summary>
    /// 路径的ID列表转化为位置向量列表
    /// </summary>
    /// <param name="idPath"></param>
    /// <param name="path"></param>
    private void ConvertIDToVector2(List<int> idPath,List<Vector2> path)
    {
        foreach(var id in idPath)
        {
            var node = NavMap.Instance.NavGraph.GetNode(id);

            path.Add(new Vector2(node.Pos.x,node.Pos.y));
        }
    }

    /// <summary>
    /// 将Vectro2数据转换为PathEdge
    /// </summary>
    private void ConverVector2ToPathEdge(List<Vector2> idPath, List<PathEdge> path)
    {
        for(int i = 0;i<idPath.Count-1;i++)
        {
            path.Add(new PathEdge(idPath[i],idPath[i+1], EdgeBehavious.Normal));
        }
    }
    
    public List<PathEdge> GetPathByDFS(Vector2 start, Vector2 end,NavGraph graph)
    {
        

        List<Vector2> path = new List<Vector2>();

        List<PathEdge> pathEdges = new List<PathEdge>();

        var startCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(start);

        var endCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(end);

        int startID = GetClostNodeID(start,startCell);

        int endID = GetClostNodeID(end,endCell);

        if(startID !=-1&&endID!=-1)
        {
            dfs.Search(startID, endID, NavMap.Instance.NavGraph);

            if (dfs.IsHavePath())
            {
                path.Add(start);

                //如果有路径的话就进行路径的转换
                ConvertIDToVector2(dfs.GetPath(), path);

                path.Add(end);

                ConverVector2ToPathEdge(path,pathEdges);
            }
        }

        return pathEdges;
    }

    public List<PathEdge> GetPathByBFS(Vector2 start, Vector2 end, NavGraph graph)
    {
        List<Vector2> path = new List<Vector2>();
        List<PathEdge> pathEdges = new List<PathEdge>();
        var startCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(start);

        var endCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(end);

        int startID = GetClostNodeID(start, startCell);

        int endID = GetClostNodeID(end, endCell);

       if(startID!=-1&&endID!=-1)
        {
            bfs.Search(startID, endID, NavMap.Instance.NavGraph);


            if (bfs.IsHavePath())
            {
                path.Add(start);

                //如果有路径的话就进行路径的转换
                ConvertIDToVector2(bfs.GetPath(), path);

                path.Add(end);

                ConverVector2ToPathEdge(path, pathEdges);
            }
        }


        return pathEdges;
    }

    public List<PathEdge> GetPathByDijkstra(Vector2 start, Vector2 end, NavGraph graph)
    {
        List<Vector2> path = new List<Vector2>();
        List<PathEdge> pathEdges = new List<PathEdge>();
        var startCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(start);

        var endCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(end);

        int startID = GetClostNodeID(start, startCell);

        int endID = GetClostNodeID(end, endCell);

        if(startID!=-1&&endID!=-1)
        {
            dj.Search(startID, endID, NavMap.Instance.NavGraph);

            if (dj.IsHavePath())
            {
                path.Add(start);

                //如果有路径的话就进行路径的转换
                ConvertIDToVector2(dj.GetPath(), path);

                path.Add(end);

                ConverVector2ToPathEdge(path, pathEdges);
            }

        }

        return pathEdges;
    }
    
    public List<PathEdge> GetPathByAStart(Vector3 start, Vector3 end, NavGraph navGraph)
    {
        List<Vector2> path = new List<Vector2>();

        List<PathEdge> pathEdges = new List<PathEdge>();

        var startCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(start);

        var endCell = NavMap.Instance.GetCellSpacePartition().GetNeighbors(end);

        int startID = GetClostNodeID(start, startCell);

        int endID = GetClostNodeID(end, endCell);

        if (startID != -1 && endID != -1)
        {
            aStart.Search(startID, endID, NavMap.Instance.NavGraph);

            if (aStart.IsHavePath())
            {
                path.Add(start);

                //如果有路径的话就进行路径的转换
                ConvertIDToVector2(aStart.GetPath(), path);

                path.Add(end);

                ConverVector2ToPathEdge(path, pathEdges);
            }
        }
      
        return pathEdges;
    }
}

/// <summary>
/// 搜索终止条件的基类
/// </summary>
public class SearchTerminationCondition
{
    /// <summary>
    /// 搜索条件是否被满足
    /// </summary>
    /// <returns></returns>
    public virtual bool isSatisfied(int targetNodeID,int currentNodeID)
    {
        bool satisfied = false;

        

        return satisfied;
    }
}


#region 各种寻路算法
namespace PriorityQueue
{
    /// <summary>
    /// 表明队列的优先顺序
    /// </summary>
    public enum PriorityOrder
    {
        /// <summary>
        /// 以大的为优先级
        /// </summary>
        MaxFirst,
        /// <summary>
        /// 以小的为优先级
        /// </summary>
        MinFirst
    }

    #region 优先队列
    /// <summary>
    /// 优先队列(默认元素越大优先级越大)
    /// </summary>
    public class PQueue<T>
    {
        protected T[] values;

        /// <summary>
        /// 当前优先队列中的元素的个数
        /// </summary>
        protected int count;

        /// <summary>
        /// 优先队列的分配的空间大小
        /// </summary>
        protected int size;

        public int Count
        {
            get { return count; }
        }

        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// 比较大小的泛型委托
        /// </summary>
        protected Func<T, T, float> compare;

        protected PriorityOrder order;

        protected PQueue(int size, Func<T, T, float> compare)
        {
            values = new T[size];
            this.size = size;
            count = 0;
            this.compare = compare;
        }

        public PQueue(Func<T, T, float> compare, PriorityOrder order = PriorityOrder.MaxFirst) : this(100, compare)
        {
            this.order = order;
        }

        public  PQueue(int size, Func<T, T, float> compare, PriorityOrder order = PriorityOrder.MaxFirst) : this(size, compare)
        {
            this.order = order;
        }

        public PQueue(T[] values , Func<T, T, float> compare, PriorityOrder order = PriorityOrder.MaxFirst)
        {
            this.values = new T[values.Length];

            this.compare = compare;

            this.order = order;

            this.count = 0;

            size = this.count;

            for(int i = 0;i<values.Length;i++)
            {
                Enqueue(values[i]);
            }
            
        }

        protected PQueue()
        {

        }

        /// <summary>
        /// 扩大数组的大小
        /// </summary>
        protected virtual void extendSize()
        {
            T[] newvalues = new T[size*2];

            ///进行数组的拷贝
            Array.Copy(values,0,newvalues,0,values.Length);

            values = newvalues;

            size *= 2;

            Debug.Log("扩大了一次数组");
        }

        protected virtual void minSize()
        {
            //TODO
        }

        /// <summary>
        /// 上浮
        /// </summary>
        /// <param name="k"></param>
        protected virtual void swim(int k)
        {
            int n = k/2;

            while(n>0)
            {
                n = k / 2;
                if (bigPriority(k, n))
                {
                    change(k, n);
                    k = n;
                }
                else break;
            }
        }

        /// <summary>
        /// 下沉
        /// </summary>
        /// <param name="k"></param>
        protected virtual void sink(int k)
        {
            int n = k*2;

            while(n<count && count != 1)
            {
                if (n + 1 < count && bigPriority(n + 1, n)) n++;

                if (bigPriority(n, k))
                {
                    change(n, k);
                    k = n;
                }
                else break;

                n = k * 2;
            }
        }

        protected virtual void change(int k,int j)
        {
            T middle = values[k];

            values[k] = values[j];

            values[j] = middle;
        }

        /// <summary>
        /// 判断两个元素哪一个的优先级更高
        /// </summary>
        /// <returns></returns>
        protected virtual bool bigPriority(T a,T b)
        {
            if(order == PriorityOrder.MaxFirst)
            {
                if (compare(a, b) > 0) return true;

                return false;
            }else
            {
                if (compare(a, b) > 0) return false;
                return true;
            }
        }

        protected virtual bool bigPriority(int i,int j)
        {
            return bigPriority(values[i],values[j]);
        }

        /// <summary>
        /// 入队
        /// </summary>
        public virtual void Enqueue(T v)
        {
            if (count >= size) extendSize(); 

            values[count++] = v;

            swim(count-1);
        }
       
        
        /// <summary>
        /// 获取顶端元素
        /// </summary>
        public virtual T Top()
        {
            return values[count-1];
        }

        /// <summary>
        /// 出对方法
        /// </summary>
        /// <returns></returns>
        public virtual T Dequeue()
        {
            T v = values[0];

            count--;

            values[0] = values[count];

            sink(0);

            return v;
        }
        /// <summary>
        /// 判断优先队列大小是否为空
        /// </summary>
        /// <returns></returns>
        public virtual bool isEmpty()
        {
            if (count == 0) return true;
            return false;
        }


    }



    #endregion

    #region 索引优先队列

    /// <summary>
    /// 索引优先队列
    /// </summary>
    class IndexPriorityQueue<T>
    {
        private T[] values;

        private int count;

        private int size;

        private PriorityOrder order;

        private int[] indexs;

        private int[] qp;//方向索引

        private Func<T, T, float> compare;

        public IndexPriorityQueue(int size, Func<T, T, float> compare, PriorityOrder order)
        {
            this.order = order;

            indexs = new int[size];

            values = new T[size];

            qp = new int[size];

            this.size = size;

            count = 0;

            this.compare = compare;
        }

        public IndexPriorityQueue(Func<T, T, float> compare, PriorityOrder order)
        {
            this.order = order;

            this.compare = compare;

            indexs = new int[100];

            qp = new int[100];

            count = 0;

            this.size = 100;

            this.values = new T[100];


        }

        public IndexPriorityQueue(T[] _values, Func<T, T, float> compare, PriorityOrder order)
        {
            this.size = _values.Length + 1;

            values = new T[size];

            indexs = new int[size];

            qp = new int[size];

            this.order = order;

            this.compare = compare;

            count = 0;



            for (int i = 0; i < _values.Length; i++)
            {
                Enqueue(_values[i]);
            }

            
        }

        protected bool bigPriority(int i, int j)
        {
            return bigPriority(values[indexs[i]], values[indexs[j]]);
        }

        protected bool bigPriority(T a, T b)
        {
            if (order == PriorityOrder.MaxFirst)
            {
                if (compare(a, b) > 0) return true;

                return false;
            }
            else
            {
                if (compare(a, b) > 0) return false;
                return true;
            }
        }

        protected void change(int k, int j)
        {
            int middle = indexs[k];

            indexs[k] = indexs[j];

            indexs[j] = middle;

            qp[indexs[k]] = k;

            qp[indexs[j]] = j;
        }

        protected void extendSize()
        {

            size *= 2;

            T[] newvalues = new T[size];
            int[] newindexs = new int[size];
            int[] newqp = new int[size];
            Array.Copy(values, 0, newvalues, 0, values.Length);
            Array.Copy(indexs, 0, newindexs, 0, indexs.Length);
            Array.Copy(qp, 0, newqp, 0, qp.Length);

            values = newvalues;
            indexs = newindexs;
            qp = newqp;

        }

        protected void minSize()
        {
            //TODO 要不要实现再说
        }

        protected void sink(int k)
        {
            while (k * 2 <= count)
            {
                int j = k * 2;
                if (j + 1 <= count && !bigPriority(j, j + 1)) j++;
                if (!bigPriority(j, k)) break;
                change(j, k);
                k = j;
            }

        }

        protected void swim(int k)
        {
            int n = k;
            while (k > 1 && bigPriority(k, k / 2))
            {
                change(k, k / 2);
                k /= 2;
            }
        }

        /// <summary>
        /// 直接在数组的末尾插入元素
        /// </summary>
        /// <param name="v"></param>
        public void Enqueue(T v)
        {
            count++;
            if (count >= size) extendSize();

            values[count] = v;

            indexs[count] = count;

            qp[count] = count;

            swim(count);

            //Console.WriteLine("哈哈哈");
        }

        public bool isEmpty()
        {
            if (count == 0) return true;
            return false;
        }

        public T Top()
        {
            return values[indexs[1]];
        }

        public int TopIndex()
        {
            return indexs[1] - 1;
        }

        public T DequeueValue()
        {
            T v = values[indexs[1]];

            change(1, count);

            count--;

            sink(1);

            return v;
        }

        /// <summary>
        /// 返回最小的那个的索引,不会弹出元素
        /// </summary>
        /// <returns></returns>
        public int DequeueIndex()
        {
            int index = indexs[1];

            change(1, count);

            count--;

            sink(1);

            return index - 1;
        }

        /// <summary>
        /// 根据索引来改变一个元素的值
        /// </summary>
        public void ChangePriority(int index, T v)
        {
            index++;//因为索引优先队列中0索引的位置不存储任何东西
            if (index > size) { Debug.LogWarning("超过了最大值"+index+"当前的Size是"+this.count);}

            values[index] = v;
            //Console.WriteLine(qp[index]);
            //Console.Read();

            sink(qp[index]);

            swim(qp[index]);
        }

        public void Text()
        {
        }
    }

    #endregion

}

namespace PathSearchAlgorithm
{

    #region DFS

    public class DFS
    {
        private int Target;//目标节点

        private int source;//源节点

        private int[] pathIndex;

        private bool[] hasVisited;

        private NavGraph graph;

        private List<int> path;//路劲列表

        private bool isHavePath;//是否存在一条路径

        public DFS(int source,int target,NavGraph graph)
        {
            
        }

        public DFS()
        {

        }

        public void Search(int source, int target, NavGraph graph)
        {
            this.source = source;

            this.Target = target;

            this.graph = graph;

            path = new List<int>();

            pathIndex = new int[graph.ActiveV];

            hasVisited = new bool[graph.ActiveV];

            isHavePath = search(source, target);

            if (isHavePath)
            {
                Stack<int> p = new Stack<int>();

                p.Push(target);

                int n = target;
                while (pathIndex[n] != source)
                {
                    p.Push(pathIndex[n]);
                    n = pathIndex[n];
                }

                p.Push(source);

                while (p.Count != 0)
                {
                    path.Add(p.Pop());
                }
            }
        }

        private bool search(int source,int target)
        {
            Stack<GraphEdge> stack = new Stack<GraphEdge>();

            foreach(var e in graph.GetAdjoinEdges(source))
            {
                stack.Push(e);
            }
            
            hasVisited[source] = true;

            while(stack.Count!=0)
            {
                var edge = stack.Pop();
                DebugController.instance.AddPathLine(graph.GetNode(edge.From).Pos, graph.GetNode(edge.To).Pos);
                hasVisited[edge.From] = true;

                pathIndex[edge.To] = edge.From;

                if (edge.To == target) return true;

                foreach(var e in graph.GetAdjoinEdges(edge.To))
                {
                    if(!hasVisited[e.To])
                    {
                        stack.Push(e);
                    }
                }
               
            }

            return false;
        }

        public List<int> GetPath()
        {
            return path;
        }

        public bool IsHavePath()
        {
            return isHavePath;
        }

    }

    #endregion


    #region BFS

    public class BFS
    {
        private int Target;//目标节点

        private int source;//源节点

        private int[] pathIndex;

        private bool[] hasVisited;

        private NavGraph graph;

        private List<int> path;//路劲列表

        private bool isHavePath;//是否存在一条路径

        public BFS()
        {

        }

        public BFS(int source ,int target,NavGraph graph)
        {
            
        }

        public void Search(int source, int target, NavGraph graph)
        {
            this.source = source;

            this.Target = target;

            this.graph = graph;

            path = new List<int>();

            pathIndex = new int[graph.ActiveV];

            hasVisited = new bool[graph.ActiveV];

            isHavePath = search(source, target);

            if (isHavePath)
            {
                Stack<int> p = new Stack<int>();

                p.Push(target);

                int n = target;
                while (pathIndex[n] != source)
                {
                    p.Push(pathIndex[n]);
                    n = pathIndex[n];
                }

                p.Push(source);

                while (p.Count != 0)
                {
                    path.Add(p.Pop());
                }
            }
        }

        private bool search(int source, int target)
        {
            if(source == target)
            {
                pathIndex[target] = source;

                return true;
            }

            Queue<GraphEdge> queue = new Queue<GraphEdge>();

            hasVisited[source] = true;

            pathIndex[source] = source;

            foreach(var e in graph.GetAdjoinEdges(source))
            {
                queue.Enqueue(e);

                hasVisited[e.To] = true;

                pathIndex[e.To] = e.From;

                if (e.To == target) return true;
            }
            
            while(queue.Count!=0)
            {
                var edge = queue.Dequeue();
                DebugController.instance.AddPathLine(graph.GetNode(edge.From).Pos, graph.GetNode(edge.To).Pos);
                foreach (var e in graph.GetAdjoinEdges(edge.To))
                {
                    if(!hasVisited[e.To])
                    {
                        queue.Enqueue(e);

                        hasVisited[e.To] = true;

                        pathIndex[e.To] = e.From;

                        if (e.To == target) return true;
                    }
                }
            }

            return false;
        }

        public List<int> GetPath()
        {
            return path;
        }

        public bool IsHavePath()
        {
            return isHavePath;
        }

    }

    #endregion


    #region 迪杰斯特拉算法
    public class Dijkstra
    {

        private float[] costToNode;

        private GraphEdge[] spt;

        private GraphEdge[] searchFrontier;//搜索边界

        private bool isHavePath;

        private NavGraph graph;

        private List<int> path;

        public Dijkstra(int source ,int target,NavGraph graph)
        {
            

        }

        public Dijkstra()
        {

        }

        public void Search(int source, int target, NavGraph graph)
        {
            this.graph = graph;

            searchFrontier = new GraphEdge[graph.TotalV];

            spt = new GraphEdge[graph.TotalV];

            costToNode = new float[graph.TotalV];
            //Debug.Log("总点数是:"+graph.TotalV);
            //Debug.Log("");
            path = new List<int>();

            for (int i = 0; i < graph.TotalV; i++)
            {
                costToNode[i] = float.PositiveInfinity;
                //spt[i] = null;
                if (i == source) costToNode[i] = 0;
            }

            isHavePath = search(source, target);

            if (isHavePath)
            {
                Stack<int> p = new Stack<int>();

                p.Push(target);

                int n = target;
                if (spt[n] == null)
                {
                    Debug.Log("是空的");
                }
                try
                {
                    while (spt[n].From != source)
                    {
                        p.Push(spt[n].From);
                        n = spt[n].From;
                    }
                }
                catch
                {
                    if(spt[n] == null)
                    Debug.LogErrorFormat("当前的目标点是:"+target+",起点是"+source+",空的点是"+n);
                }
                

                p.Push(source);

                while (p.Count != 0)
                {
                    path.Add(p.Pop());
                }
            }
            else
            {
                Debug.LogWarningFormat("没有路径");
            }
        }

        private bool search(int source,int target)
        {
            int num = 0;

            if(source == target)
            {
                spt[target] = new GraphEdge(target,target) ;
                Debug.Log("一样");
                return true;
            }

            IndexPriorityQueue<float> queue = new IndexPriorityQueue<float>(costToNode,(i,j)=>(i-j),PriorityOrder.MinFirst);

            
            searchFrontier[source] = new GraphEdge(source, source);

            //searchFrontier[]
            //int t = queue.TopIndex();
            //Debug.Log("起点处理完后最小的点是" + t+"花费是"+costToNode[t]);
            while(!queue.isEmpty())
            {
                var index = queue.DequeueIndex();

                spt[index] = searchFrontier[index];
                DebugController.instance.AddPathLine(graph.GetNode(spt[index].From).Pos, graph.GetNode(spt[index].To).Pos);
                if (index == target)
                {
                    //Debug.Log("DJ总的搜索次数是"+num);
                    return true;
                }

                //queue.ChangePriority(index,float.PositiveInfinity);
                #region 邻接边的寻找
                foreach (var e in graph.GetAdjoinEdges(index))
                {
                    num++;
                    float costNode = costToNode[e.From] + e.Cost;

                    //float costEnd = costNode + heuristic(e.To, target);
                    if(searchFrontier[e.To] == null)
                    {
                        costToNode[e.To] = costNode;

                        searchFrontier[e.To] = e;

                        queue.ChangePriority(e.To, costNode);
                    }else
                    if (costNode < costToNode[e.To] && spt[e.To] == null)
                    {
                        //否则只有当spt上没有这个节点
                        Debug.Log("lalalal");
                        costToNode[e.To] = costNode;

                        searchFrontier[e.To] = e;

                        queue.ChangePriority(e.To, costNode);
                    }
                }
                #endregion
            }

            return false;
        }

        public List<int> GetPath()
        {
            return path;
        } 

        public bool IsHavePath()
        {
            return isHavePath;
        }

    }
    #endregion


    #region A*

    /// <summary>
    /// A*启发因子枚举
    /// </summary>
    public enum AStartHeuristic
    {
        Manhattan,
        Euclid
    }

    /// <summary>
    /// A*寻路算法
    /// </summary>
    public class AStart
    {
        private Func<int, int, double> heuristic;

        private double[] costToNode;

        private double[] costToEnd;

        private GraphEdge[] spt;

        private GraphEdge[] searchFrontier;//搜索边界

        private bool isHavePath;

        private NavGraph graph;

        private List<int> path;

        public AStart(Func<int, int, double> _heuristic)
        {
            this.heuristic = _heuristic;
        }

        public AStart(AStartHeuristic type)
        {
            if(type == AStartHeuristic.Euclid)
            {
                heuristic = this.Euclid;
            }else
            {
                heuristic = this.Manhattan;
            }
        }

        public void Search(int source, int target, NavGraph graph )
        {
            this.graph = graph;
            
            searchFrontier = new GraphEdge[graph.TotalV];

            costToEnd = new double[graph.TotalV];

            spt = new GraphEdge[graph.TotalV];

            costToNode = new double[graph.TotalV];
            //Debug.Log("总点数是:"+graph.TotalV);
            //Debug.Log("");
            path = new List<int>();

            for (int i = 0; i < graph.TotalV; i++)
            {
                costToNode[i] = float.PositiveInfinity;
                costToEnd[i] = float.PositiveInfinity;
                //spt[i] = null;
                if (i == source)
                {
                    costToNode[i] = 0;

                    costToEnd[i] = 0;
                }
            }

            isHavePath = search(source, target);

            if (isHavePath)
            {
                Stack<int> p = new Stack<int>();

                p.Push(target);

                int n = target;

                while (spt[n]!=null&&spt[n].From != source)
                {
                    p.Push(spt[n].From);
                    n = spt[n].From;
                }

                p.Push(source);

                while (p.Count != 0) path.Add(p.Pop());
            }
            else
            {
                Debug.LogWarningFormat("没有路径");
            }
        }

        private bool search(int source, int target)
        {
            //int num = 0;

            if(source == target)
            {
                //当起点和终点相同时
                Debug.Log("目标和起点节点相同"+target);

                

                return true;
            }
            //Debug.Log("起始点是"+source+"终点是"+target);

            costToNode[source] = 0;

            costToEnd[source] = 0;
            IndexPriorityQueue<double> pq = new IndexPriorityQueue<double>(costToEnd, (i, j) => (float)(i - j),PriorityOrder.MinFirst);
            
            searchFrontier[source] = new GraphEdge(source,source);

            while(!pq.isEmpty())
            {
                //Debug.Log("A*");
                int index = pq.DequeueIndex();

                spt[index] = searchFrontier[index];
                //Debug.Log("最小花费的点是"+index+"花费是"+costToEnd[index]);
                DebugController.instance.AddPathLine(graph.GetNode(spt[index].From).Pos, graph.GetNode(spt[index].To).Pos);

                if (index == target)
                {
                    //Debug.Log("A*总的搜索次数是"+num);
                    return true;
                }
                //Debug.Log("" + graph.GetAdjoinEdges(index).Count);
                foreach (var e in graph.GetAdjoinEdges(index))
                {
                    //num++;

                    double costNode = costToNode[e.From] + e.Cost;

                    double costEnd = costNode + heuristic(e.To,target);

                    //Debug.Log("边的总花费是" + costEnd+"边的花费是"+e.Cost+"上一个节点的花费是"+costToNode[e.From]+"启发因子花费是"+ heuristic(e.To, target));

                    if (searchFrontier[e.To]==null)
                    {
                        costToNode[e.To] = costNode;

                        costToEnd[e.To] = costEnd;

                        searchFrontier[e.To] = e;

                        pq.ChangePriority(e.To,costEnd);
                    }
                     else
                    {
                        //Debug.Log("哈哈哈哈"+costNode+",,"+costToNode[e.To]);


                        if (costNode < costToNode[e.To] && spt[e.To] == null)
                        {
                            //否则只有当spt上没有这个节点
                            //Debug.Log("啦啦啦啦");
                            costToEnd[e.To] = costEnd;

                            costToNode[e.To] = costNode;

                            searchFrontier[e.To] = e;

                            pq.ChangePriority(e.To, costEnd);
                        }
                    }
                }
            }
            return false;
        }

        public List<int> GetPath()
        {
            return path;
        }

        public bool IsHavePath()
        {
            return isHavePath;
        }

        /// <summary>
        /// 曼哈顿启发因子
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private double Manhattan(int s,int e)
        {
            Vector2 sp = graph.GetNode(s).Pos;

            Vector2 ep = graph.GetNode(e).Pos;

            return Vector2.Distance(sp , ep)*20;
        }

        /// <summary>
        /// 欧几里得启发因子
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private double Euclid(int s,int e)
        {
            Vector2 sp = graph.GetNode(s).Pos;

            Vector2 ep = graph.GetNode(e).Pos;
            double dist = Mathf.Abs(sp.x - ep.x) + Mathf.Abs(sp.y - ep.y);

            dist *= 20;//乘以一个因子
           
            return dist;
        }
    }

    #endregion

    #region B*

    #endregion


    #region 基于时间片的搜索算法

    /// <summary>
    /// 时间片A*搜索算法
    /// </summary>
    public class AStar_TimeSliced : SearchTimeSliced
    {
        private Func<int, int, double> heuristic;
        
        private double[] costToNode;

        private double[] costToEnd;

        private GraphEdge[] spt;

        private GraphEdge[] searchFrontier;//搜索边界

        private IndexPriorityQueue<double> pq;

        
        public AStar_TimeSliced()
        {
            heuristic = Euclid;
        }

        /// <summary>
        /// 初始化搜索状态
        /// </summary>
        protected override void InitSearch(int source,int target,NavGraph graph)
        {
            isHavePath = false;

            this.graph = graph;

            this.source = source;

            this.target = target;

            if(spt == null)
            {
                costToNode = new double[graph.TotalV];

                costToEnd = new double[graph.TotalV];

                spt = new GraphEdge[graph.TotalV];

                searchFrontier = new GraphEdge[graph.TotalV];
            }

            for(int i = 0;i<graph.TotalV;i++)
            {
                costToNode[i] = double.PositiveInfinity;

                costToEnd[i] = double.PositiveInfinity;

                spt[i] = null;

                searchFrontier[i] = null;

                
            }

            spt[source] = new GraphEdge(source,source,0);
            searchFrontier[source] = spt[source]; 
            costToNode[source] = 0;

            costToEnd[source] = 0;

            pq = new IndexPriorityQueue<double>(costToEnd, (i, j) => (float)(i - j), PriorityOrder.MinFirst);
        }

        public override bool IsHavePath()
        {
            return isHavePath;
        }

        public override SearchState CycleOne()
        {
            if (isHavePath) return SearchState.Search_Complete;
            if (pq.isEmpty()) return SearchState.Search_Fail;

            int index = pq.DequeueIndex();

            spt[index] = searchFrontier[index];
            
            DebugController.instance.AddPathLine(graph.GetNode(spt[index].From).Pos, graph.GetNode(spt[index].To).Pos);

            if (index == target)
            {
                isHavePath = true;

                return SearchState.Search_Complete;
            }
            
            foreach (var e in graph.GetAdjoinEdges(index))
            {
                double costNode = costToNode[e.From] + e.Cost;

                double costEnd = costNode + heuristic(e.To, target);

                if (searchFrontier[e.To] == null)
                {
                    costToNode[e.To] = costNode;

                    costToEnd[e.To] = costEnd;

                    searchFrontier[e.To] = e;

                    pq.ChangePriority(e.To, costEnd);
                }
                else
                {
                    if (costNode < costToNode[e.To] && spt[e.To] == null)
                    {
                        
                        costToEnd[e.To] = costEnd;

                        costToNode[e.To] = costNode;

                        searchFrontier[e.To] = e;

                        pq.ChangePriority(e.To, costEnd);
                    }
                }
            }

                return SearchState.Searching;
        }

        /// <summary>
        /// 返回到目标点的花费
        /// </summary>
        /// <returns></returns>
        public override float GetCostToTarget()
        {
            if (isHavePath) return (float)costToNode[target];

            return float.PositiveInfinity;
        }

        /// <summary>
        /// 返回到目标点的路径边
        /// </summary>
        /// <returns></returns>
        public override List<PathEdge> GetPathAsPathEdges()
        {
            if (!isHavePath) return null;

            List<PathEdge> pathEdge = new List<PathEdge>();
            pathEdge.Add(new PathEdge(graph.GetNode(target).Pos, targetPos, EdgeBehavious.Normal));
            int index = target;

            int preIndex = spt[target].From;

            while(index != source)
            {
                pathEdge.Add(new PathEdge(graph.GetEdge(preIndex,index),graph.GetNode(preIndex).Pos,graph.GetNode(index).Pos, EdgeBehavious.Normal));
                index = preIndex;
                preIndex = spt[index].From;
            }

            
            //反转数组
            pathEdge.Reverse();

            return pathEdge;
        }

        /// <summary>
        /// 获取最短路径树
        /// </summary>
        /// <returns></returns>
        public override List<GraphEdge> GetSPT()
        {
            if (isHavePath) return new List<GraphEdge>(spt);

            return null;
        }

        /// <summary>
        /// 使用当前的导航图进行寻路
        /// </summary>
        /// <param name="resouce"></param>
        /// <param name="target"></param>
        public override void SetSearch(int resouce, int target)
        {
            InitSearch(resouce,target,this.graph);


        }
        /// <summary>
        /// 使用一个新的导航图进行寻路
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="target"></param>
        /// <param name="graph"></param>
        public override void SetSearch(int resourceID,int targetID,Vector2 resource, Vector2 target, NavGraph graph)
        {
            this.spt = null;
            this.resourcePos = resource;
            this.targetPos = target;

            InitSearch(resourceID,targetID,graph);
        }

        /// <summary>
        /// 曼哈顿启发因子
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private double Manhattan(int s, int e)
        {
            Vector2 sp = graph.GetNode(s).Pos;

            Vector2 ep = graph.GetNode(e).Pos;

            return Vector2.Distance(sp, ep) * 20;
        }

        /// <summary>
        /// 欧几里得启发因子
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private double Euclid(int s, int e)
        {
            Vector2 sp = graph.GetNode(s).Pos;

            Vector2 ep = graph.GetNode(e).Pos;
            double dist = Mathf.Abs(sp.x - ep.x) + Mathf.Abs(sp.y - ep.y);

            dist *= 20;//乘以一个因子

            return dist;
        }
    }

    /// <summary>
    /// 基于时间片的BFS
    /// </summary>
    public class BFS_TimeSliced:SearchTimeSliced
    {
        public BFS_TimeSliced()
        {

        }

        public override SearchState CycleOne()
        {
            return base.CycleOne();
        }

        public override float GetCostToTarget()
        {
            return base.GetCostToTarget();
        }

        public override List<PathEdge> GetPathAsPathEdges()
        {
            return base.GetPathAsPathEdges();
        }

        public override List<GraphEdge> GetSPT()
        {
            return base.GetSPT();
        }

        public override bool IsHavePath()
        {
            return base.IsHavePath();
        }
    }

    #endregion
}

#endregion