using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动实体类
/// </summary>
public class MovingEntity : BaseGameEntity {

    [SerializeField]
    protected float radius = 0.16f;
    protected bool isUpdateOrientation;
    /// <summary>
    /// 朝向对齐的目标
    /// </summary>
    protected Vector2 orientationTarget;
    public float Radius
    {
        get
        {
            return radius;
        }
    }
    public Vector2 Pos
    {
        get
        {
            return this.transform.position;
        }

        set
        {
            this.transform.position = value;
        }
    }
    protected Vector2 velocity = new Vector3();//速度
    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }
        set
        {
            //TODO 要进行移除这个Set
            this.velocity = value;
        }
    }
    protected Vector2 heading = new Vector2();//朝向向量
    public Vector2 Heading
    {
        get
        {
            return heading;
        }
    }
    protected Vector2 side = new Vector2();//垂直于朝向向量的向量
    public Vector2 Side
    {
        get
        {
            return side;
        }
    }
    protected Vector2 acceleration = new Vector2();//加速度
    public Vector3 Acceleration
    {
        get
        {
            return acceleration;
        }
    }
    [SerializeField]
    protected float mass;
    public float Mass
    {
        get
        {
            return mass;
        }
    }
    //TODO lua设置
    [SerializeField]
    protected float maxSpeed;//最大速度
    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }
    //TODO lua设置
    [SerializeField]
    protected float maxForce;//最大力
    public float MaxForce
    {
        get
        {
            return maxForce;
        }
    }
    //TODO lua设置
    [SerializeField]
    protected float maxTurnRate;//最大转向速度
    public float MaxTurnRate
    {
        get
        {
            return maxTurnRate;
        }
    }
    protected SteeringBehaviors steer;//操纵控制类的引用
    public SteeringBehaviors SteerComponent
    {
        get { return steer; }
    }
    
    

    protected override void Init()
    {
        base.Init();

        steer = new SteeringBehaviors(this);

        heading = this.transform.up;
        this.isUpdateOrientation = true;
    }
    private void FixedUpdate()
    {
        EntityUpdate();
    }
    protected override void EntityUpdate()
    {
        Vector2 force = steer.Calculate();
        //Debug.Log("操纵力是"+force);
        acceleration = force / mass;

        velocity += acceleration ;

        velocity = Vector2.ClampMagnitude(velocity,maxSpeed);

        

        if(velocity.sqrMagnitude>0.01f)
        {
            //当速度大于某个值的时候进行转向
            if (isUpdateOrientation) orientationTarget = velocity;
            UpdateOrientation();
        }


        //TODO 注意引用或值传递
        //print(velocity);
        //velocity.z = -8;
        Pos = Pos+ velocity * Time.fixedDeltaTime;
        this.transform.position = new Vector3(Pos.x,Pos.y,-8);

    }
    /// <summary>
    /// 更新朝向
    /// </summary>
    protected virtual void UpdateOrientation()
    {
        Quaternion turn = Quaternion.LookRotation(Vector3.forward,(orientationTarget + Pos) - Pos);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, turn, maxTurnRate);
    }
    /// <summary>
    /// 获取一个向量的垂直单位向量
    /// </summary>
    /// <returns></returns>
    protected Vector3 GetVertical(Vector3 target)
    {
        float y = -target.y / target.x;

        return new Vector3(1,y,0).normalized;
    }
    public void SetOrientationTarget(Vector2 target)
    {
        this.orientationTarget = target;
    }
    public void UpdateOrientationOn()
    {
        if (!isUpdateOrientation) isUpdateOrientation = true;
    }
    public void UpdateOrientationOff()
    {
        if (isUpdateOrientation) isUpdateOrientation = false;
    }



   
}
