using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// 操纵力行为的定义
/// </summary>
public class SteeringBehaviors {

    /// <summary>
    /// 当到达目标时进行的回调
    /// </summary>
    protected Action callBack;
    protected RaycastHit2D[] hits;
    /// <summary>
    /// 检测墙壁所用的触须之间的角度
    /// </summary>
    protected float dectedWallAngle;
    private float maxDectedDistance;
    /// <summary>
    /// 触须的数量
    /// </summary>
    private float dectedNumber;
    public enum Deceleration
    {
        Fast = 1,
        Normal = 2,
        Slow = 3
    }
    #region 操纵力开启和关闭的标志

    protected bool isSeek = false;

    protected bool isArrive = false;

    #endregion
    protected float arriveOffset = 0.1f;//d到达一个点时的距离偏差
    protected Deceleration deceleration;
    protected Vector2 targetPos;//目标
    protected MovingEntity owner;
    protected List<PathEdge> path;//路径
    protected bool isPathing;//是否使用路径
    protected int pathIndex;//当前的路径索引
    public SteeringBehaviors(MovingEntity entity)
    {
        this.owner = entity;

        deceleration = Deceleration.Normal;
    }
    protected bool isAvoidWall;
    protected bool isWanding;
    protected PerlineCaulator caulatorY;


    /// <summary>
    /// 计算操作力
    /// </summary>
    /// <returns></returns>
    public Vector2 Calculate()
    {
        Vector2 force = new Vector2();


        if (isAvoidWall)
        {
            //Debug.LogWarning("AvoidWall");
            force += AvoidObstacle();
        }

        if (isSeek)
        {
            //Debug.Log("seeking");
            force += Seek(targetPos);
        }

        if(isArrive)
        {
            //Debug.Log("arriving");
            force += Arrive(targetPos);
        }

        #region 是否开启寻路
        //if (isPathing)
        //{
        //    //是否开启了路径
        //    float dist = (owner.Pos- path[pathIndex].Destination()).sqrMagnitude;

        //    //Debug.Log("目标节点的位置是"+path[pathIndex]+"玩家位置是"+entity.Pos);
        //    //Debug.Log(dist);
        //    if (dist<arriveOffset)
        //    {
        //        //玩家已经到达一个点了

        //        pathIndex++;
        //        //Debug.Log("玩家到达一个点了");

        //        if (pathIndex == path.Count) {

        //            isPathing = false;
        //            //执行回调
        //            if (this.callBack != null) callBack();
        //            else
        //            {
        //                Debug.Log("callback是空的");
        //            }
        //        }
        //        else
        //        {
        //            SetTarget(path[pathIndex].Destination());

        //            if (pathIndex == path.Count - 1)
        //            {
                        
        //                SeekOff();

        //                ArriveOn();
                       
        //            }
        //        }
               
        //    }
        //}
        #endregion

        //如果AI当前正在移动中
        if(isPathing)
        {
            float sqrtDist = (owner.Pos - targetPos).sqrMagnitude;

            //AI到达了目标
            if(sqrtDist< arriveOffset)
            {
                isPathing = false;

                if (callBack != null) callBack();
            }
        }

        force = Vector2.ClampMagnitude(force,owner.MaxForce);

        return force;
    }
    public void SetTarget(Vector2 targetPos)
    {
        this.targetPos = targetPos;

        this.isPathing = true;
    }
    public void SetTarget(Vector2 targetPos,Action callBack)
    {
        this.callBack = callBack;

        SetTarget(targetPos);

        isPathing = true;
    }
    public bool SetPath(List<PathEdge> path)
    {
        if (path == null || path.Count <= 0) return false;

        this.path = path;

        isPathing = true;

        pathIndex = 0;

        SetTarget(path[pathIndex].Destination());

        if(path.Count!=1)
        {
            this.SeekOn();
            this.ArriveOff();
        }else
        {
            this.SeekOff();
            this.ArriveOn();
        }

        //Debug.Log("成功设置好路径了");

        return true;
    }
    /// <summary>
    /// 设置到达目标时的回调方法
    /// </summary>
    public void SetArriveTargetCallBack(Action callBack)
    {
        this.callBack = callBack;
    }
    /// <summary>
    /// 清空目标
    /// </summary>
    public void ClearTarget()
    {
        this.targetPos = this.owner.Pos;
        this.path = null;
        this.pathIndex = 0;
        isPathing = false;
        callBack = null;
    }

    #region 操作力开关的方法
    public void SeekOn()
    {
        if (!isSeek) isSeek = true;
    }

    public void SeekOff()
    {
        if (isSeek) isSeek = false;
    }

    public void FleeOn()
    {

    }

    public void FleeOff()
    {

    }

    public void ArriveOn()
    {
        if (!isArrive) isArrive = true;
    }

    public void ArriveOff()
    {
        if (isArrive) isArrive = false;
    }

    public void PursuitOn()
    {

    }

    public void PursuitOff()
    {

    }

    public void EvadeOff()
    {

    }

    public void EvadeOn()
    {

    }

    public void WanderOn()
    {
        if (!isWanding) isWanding = true;
    }

    public void WanderOff()
    {
        if (isWanding) isWanding = false;
    }

    public void ObstacleAvoidanceOn()
    {

    }

    public void ObstacleAvoidanceOff()
    {

    }

    public void WallAvoidanceOn()
    {
        if (!isAvoidWall) isAvoidWall = true;
    }

    public void WallAvoidanceOff()
    {
        if (isAvoidWall) isAvoidWall = false;
    }


    #endregion

    #region 操纵力方法组

    protected Vector2 Seek(Vector2 targetPos)
    {
        //Debug.Log("");
        Vector2 desiredVelocity = (targetPos - owner.Pos).normalized * owner.MaxSpeed;

        return desiredVelocity - owner.Velocity;
    }

    //protected float decelerationTweaker = 0.2f;

    protected Vector2 Arrive(Vector2 targetPos)
    {
        Vector3 toTarget = targetPos - owner.Pos;
        //Debug.Log("和目标的距离大于0.5");
        float dist = toTarget.magnitude;

        if (dist>0)
        {
            //Debug.Log("和目标的距离大于0.5");
             float decelerationTweaker = 0.2f;

            if (dist < 0.1f) {
                decelerationTweaker = 2000;
                //Debug.Log("和目标的距离小于0.5");
            }
            else
            {
                //Debug.Log("和目标的距离大于0.5");
            }

            float speed = dist / (decelerationTweaker * (float)deceleration);

            speed = Mathf.Min(speed, owner.MaxSpeed);

            Vector2 desiredVelocity = toTarget * speed / dist;

            return desiredVelocity - owner.Velocity;
        }

        return -owner.Velocity;
    }

    #region 随机漫游

    protected float circleRadius;

    protected float circleDistanceToEntity;

    protected float circleJitter;//随机向量的大小

    

    protected PerlineCaulator caulatorX;

    /// <summary>
    /// 随机漫游的方法
    /// </summary>
    /// <returns></returns>
    protected Vector2 Wander()
    {
        Vector2 randomVector = new Vector2(caulatorX.Caulator()*2*circleJitter-circleJitter,
           caulatorY.Caulator()*2* circleJitter-circleJitter);

        randomVector =  randomVector.normalized * circleRadius;

        

        Vector2 target = (Vector2)owner.transform.up * circleDistanceToEntity +owner.Pos + randomVector;

        return target - owner.Pos;
    }


    public void SetWanderParameter(float radius,
                                   float distance, 
                                   float jitter,
                                   PerlineCaulator caulatorX,
                                   PerlineCaulator caulatorY
                                  )
    {
        this.circleRadius = radius;
        this.circleDistanceToEntity = distance;
        this.circleJitter = jitter;
        this.caulatorX = caulatorX;
        this.caulatorY = caulatorY;
    }

    #endregion

    #region 避开障碍物

    /// <summary>
    /// 避开障碍物操纵行为方法
    /// </summary>
    /// <returns></returns>
   protected Vector2 ObstacleAvoiance()
    {
        return new Vector2();
    }

    /// <summary>
    ///判断前方是否存在一个障碍物
    /// </summary>
    /// <returns></returns>
    protected bool isObstacle()
    {
        hits = Physics2D.BoxCastAll(owner.Pos, new Vector2(owner.Radius,0), owner.transform.localEulerAngles.z,owner.transform.TransformDirection(owner.transform.up));
       
        

        return false;
    }

    #endregion


    #region 避开墙

    protected float DectedWallAngle
    {
        get
        {
            return dectedWallAngle;
        }

        set
        {
            dectedWallAngle = value;
        }
    }

    protected float DectedNumber
    {
        get
        {
            return dectedNumber;
        }

        set
        {
            dectedNumber = value;
        }
    }

    protected float MaxDectedDistance
    {
        get
        {
            return maxDectedDistance;
        }

        set
        {
            maxDectedDistance = value;
        }
    }

    //探测线的长度
    protected float detectedLineLength;

    public Vector2 AvoidObstacle()
    {
        //var hit = GetObstacle();

        //return hit.normal * (owner.Pos - hit.point).magnitude*100;

        //Bullet.CreateBullet

        return GetObstacle() * 10;
    }

    /// <summary>
    /// 获取当前和物体将要相交的最近的距离
    /// </summary>
    /// <returns></returns>
    public Vector2 GetObstacle()
    {
        float clostdistance = float.PositiveInfinity;

        //raycasthit2d center;
        //raycasthit2d left;
        //raycasthit2d right;
        //raycasthit2d best = default(raycasthit2d);

        //float offset =1f ;
        
        detectedLineLength = 0.2f+owner.Velocity.sqrMagnitude / (owner.MaxSpeed * owner.MaxSpeed);
        detectedLineLength *= maxDectedDistance;
        //Debug.Log("触须的长度为"+d);

        RaycastHit2D[] hits;

        hits = Physics2D.CircleCastAll(this.owner.Pos,this.owner.Radius+detectedLineLength,new Vector2(0,0),0,(1<<9)|(1<<10));
        


        //进行圆形探测
        Vector2 force = new Vector2();
        //Debug.LogWarning("检测到"+hits.Length+"个物体");
        
        for(int i = 1;i<hits.Length;i++)
        {
            force += (this.owner.Pos - hits[i].point);
            //Debug.LogWarning(hits[i].collider.gameObject.name);
        }

        return force;

        #region 重构
        //center= Physics2D.Raycast(
        //    owner.Pos+(Vector2)owner.transform.up*0.3f,owner.transform.TransformDirection(new Vector3(0,1,0)),
        //    detectedLineLength*offset,
        //    (1<<9)|(1<<10));

        //var leftRotation = Quaternion.AngleAxis(dectedWallAngle, new Vector3(0, 0, 1));

        //left = Physics2D.Raycast(
        //    owner.Pos+ (Vector2)(leftRotation * owner.transform.up)*0.3f , owner.transform.TransformDirection(
        //        leftRotation * new Vector3(0,1,0)),
        //    detectedLineLength*offset/1.2f, 
        //    (1 << 9)|(1<<10));

        //var rightRotation = Quaternion.AngleAxis(-dectedWallAngle, new Vector3(0, 0, 1));
        //right = Physics2D.Raycast(
        //    owner.Pos+(Vector2)(rightRotation*owner.transform.up)*0.3f, 
        //    owner.transform.TransformDirection(
        //         rightRotation * new Vector3(0, 1, 0)),
        //    detectedLineLength * offset / 1.2f, 
        //    (1 << 9)|(1<<10));
        //hits.Add(center);
        //hits.Add(left);
        //hits.Add(right);

        //foreach(var w in hits)
        //{
        //    if(w.collider!=null)
        //    {
        //        //Debug.Log("有");
        //        float sqrtDist = (w.point - owner.Pos).sqrMagnitude;
        //        if(sqrtDist< clostDistance)
        //        {
        //            clostDistance = sqrtDist;

        //            best = w;
        //        }
        //    }
        #endregion
    }

    /// <summary>
    /// 设置避免碰撞墙壁的方法
    /// </summary>
    /// <param name="dectedNumber"></param>
    /// <param name="angle"></param>
    /// <param name="maxDistance"></param>
    public void SetAvoidWallParameter(float dectedNumber,float angle,float maxDistance)
    {
        this.dectedWallAngle = angle;
        this.maxDectedDistance = maxDistance;
        this.dectedNumber = dectedNumber;
    }

    #endregion

    #region 基于AStar算法的随机游走
    protected NavMap navMap;

    protected bool isAstartWander = false;

    protected PathPlanner pathPlanner;

    public void AstartWander()
    {
        DebugController.instance.CleanPathLine();

        int targetIndex = UnityEngine.Random.Range(0,navMap.NavGraph.TotalV);

        Vector2 targetPos = navMap.NavGraph.GetNode(targetIndex).Pos;

        if(GameController.Instance.IsTimeSliced)
        {
            pathPlanner.CreatePathToPositionByTimeSliced(targetPos,PathFinderCallBack);
        }else
        {

            List<PathEdge> paths = new List<PathEdge>();
            pathPlanner.CreatePathToPositionNonTimeSliced(targetPos,ref paths);
            
            PathFinderCallBack(paths);
        }
    }

    /// <summary>
    /// 当搜索到一条路径时的回调方法
    /// </summary>
    private void PathFinderCallBack(List<PathEdge> paths)
    {
        pathPlanner.SmoothPathEdge(ref paths);
        var result = this.SetPath(paths);
        //Debug.Log("成功设置了路径"+result);
        //当寻路完成是重新进行寻路
        if(isAstartWander&&callBack == null)
            callBack = this.AstartWander;
        
    }

    public void AstartWanderOn()
    {
        if (!isAstartWander) isAstartWander = true;

        AstartWander();
    }

    public void AstartWaderOff()
    {
        if (isAstartWander) isAstartWander = false;
    }

    /// <summary>
    /// 设置Astart随机游走算法
    /// </summary>
    public void SetAstartWanderParameter(NavMap navmap,PathPlanner pathPlanner)
    {
        this.pathPlanner = pathPlanner;
        this.navMap = navmap;
    }

    #endregion

  
    #endregion

    public void DebugComponent()
    {
        Vector3 right = owner.transform.position + Quaternion.AngleAxis(dectedWallAngle,new Vector3(0,0,1))*owner.transform.up* (detectedLineLength /1.2f);
        Vector3 left = owner.transform.position + Quaternion.AngleAxis(-dectedWallAngle,new Vector3(0,0,1))*owner.transform.up*(detectedLineLength / 1.2f);
        Vector3 forward = owner.transform.position + owner.transform.up * detectedLineLength;

        Debug.DrawLine(owner.transform.position,forward);
        Debug.DrawLine(owner.transform.position,right);
        Debug.DrawLine(owner.transform.position,left);
    }
}

/// <summary>
/// 命令模式的柏林噪音
/// </summary>
public class PerlineCaulator
{

    protected float orgX;

    protected float orgY;

    protected float x;

    protected float y;

    protected float perlinWidth;

    protected float perlinHeight;

    protected float scale;

    public PerlineCaulator(float orgX,float orgY,float perlinWidth,float perlinHeight,float scale)
    {
        this.orgX = orgX;
        this.orgY = orgY;
        this.perlinHeight = perlinHeight;
        this.perlinWidth = perlinWidth;
        x = .0f;
        y = .0f;
        this.scale = scale;
    }

    /// <summary>
    /// 计算方法
    /// </summary>
    /// <returns></returns>
    public float Caulator()
    {

        x += 0.03f;
        y += 0.02f;

        if (x > 1000000) x = 0;
        if (y > 1000000) y = 100;

        float coordX = orgX + x / perlinWidth * scale;

        float coordY = orgY + y / perlinHeight * scale;

        float result = Mathf.PerlinNoise(coordX, coordY);

        //Debug.Log("Noise的结果是"+result);

        return result;
    }

   

}
