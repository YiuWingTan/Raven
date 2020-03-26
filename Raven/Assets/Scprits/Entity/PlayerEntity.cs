using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///// <summary>
///// 消息操作码
///// </summary>
//public enum RavenMessageOP
//{
//    /// <summary>
//    /// 路径已经准备好了
//    /// </summary>
//    PathReady = 0
//}

/// <summary>
/// 玩家的实体类
/// </summary>
public class PlayerEntity : RavenEntity {
    [SerializeField]
    private LayerMask mask;

    private RaycastHit hit;

    [SerializeField]
    private float orgX;
    [SerializeField]
    private float orgY;
    [SerializeField]
    private float perlineWidth;
    [SerializeField]
    private float perlinHeight;
    [SerializeField]
    private float scale;

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        //PerlineCaulator caulatorX = new PerlineCaulator(orgX,orgY,perlineWidth, perlinHeight,scale);
        //PerlineCaulator caulatorY = new PerlineCaulator(0,100.145f,56,89,30); ;

        //steer.SetWanderParameter(2,1,5,caulatorX, caulatorY);
        //steer.SetAvoidWallParameter(3,60,0.5f);
        //steer.SeekOff();
        //steer.ArriveOff();
        //steer.WallAvoidanceOn();
        ////steer.WanderOn();
        //steer.SetAstartWanderParameter(NavMap.Instance,this.PathPlanner);
        //steer.AstartWanderOn();

        
        //steer.SetTarget(Pos);

        mask = (1 >> 2);
    }

    private void Update()
    {
        //TODO
        //在update的更新频率内进行兑现的加速度，速度进行更新

        //Move();
    }

    /// <summary>
    /// 获取鼠标的输入
    /// </summary>
    protected  void Move()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //当鼠标右键点击抬起之后

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(!Physics2D.Raycast(ray.origin,ray.direction,~(1<<0))&&hit.collider == null)
            {
                DebugController.instance.CleanPathLine();
                //Debug.Log("可以开始寻路了"+hit.collider.tag);

                if(GameController.Instance.IsTimeSliced)
                {
                    //异步搜索
                    print("异步搜索");

                    this.PathPlanner.CreatePathToPositionByTimeSliced(mousePos,
                        delegate(List<PathEdge> path)
                        {

                            PathPlanner.SmoothPathEdge(ref path);
                            
                            if (!this.steer.SetPath(path))
                            {
                                Debug.Log("设置路径失败");


                            }
                            Debug.Log("时间片搜索算法找到了一条路径");
                            steer.ArriveOff();
                            
                            steer.SeekOn();
                        });
                }else
                {
                    //同步搜索
                    List<PathEdge> path = null;
                    print("同步搜索");
                 
                    if(this.PathPlanner.CreatePathToPositionNonTimeSliced(mousePos, ref  path))
                    {
                        PathPlanner.SmoothPathEdge(ref path);
                        //print("哈哈哈哈"+path.Count);
                        this.steer.SetPath(path);
                        
                    }
                }
            }

            if (hit.collider != null) Debug.Log(hit.collider.tag);
            
        }
    }

    public override bool OnMessage(Msg msg)
    {
        

        return false;
    }

}
