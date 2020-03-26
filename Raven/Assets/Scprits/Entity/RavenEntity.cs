using GoalPack;
using GoalPack.RavenGoals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityMsgType
{
    None = 0,
    HasBeenKilled,//一个AI被杀的消息
    HealthPackIsGone,//当一个健康包被别人抢走的时候，这个消息必须要是一个延迟的消息
    BulletPackIsGone,
    BeControl//当一个AI被控制时的消息
}

/// <summary>
/// AI的实体类
/// </summary>
public class RavenEntity : MovingEntity {

    [SerializeField]
    public EvaluatorType state;
    [SerializeField]
    protected bool mIsPlayerControl = false;//表示当前是否在玩家的控制之下


    /// <summary>
    /// AI的寻路组件
    /// </summary>
    private PathPlanner pathPlanner;
    /// <summary>
    /// AI的感知组件
    /// </summary>
    protected RavenSensoryComponent sensoryComponent;
    /// <summary>
    /// AI的目标选择组件
    /// </summary>
    protected TargetSelectSystem targetSelectionSystem;
    /// <summary>
    /// AI的武器系统
    /// </summary>
    protected WeaponSystem weaponSystem;
    /// <summary>
    /// AI的思考组件
    /// </summary>
    protected Goal_Think thinkComponent;

    public Goal_Think ThinkGomponent
    {
        get { return thinkComponent; }
    }
    public PathPlanner PathPlanner
    {
        get
        {
            return pathPlanner;
        }
    }
    public TargetSelectSystem Raven_TargetSelectionSystem
    {
        get
        {
            return targetSelectionSystem;
        }
    }
    public WeaponSystem Raven_WeaponSystem
    {
        get
        {
            return weaponSystem;
        }
    }
    public RavenSensoryComponent SensoryComponent
    {
        get { return sensoryComponent; }
    }

    protected float health = 80;
    protected static float maxHealth = 100;//AI的最大生命值
    protected static int maxBulletNumber= 15;//AI可携带的最大弹药量
    protected float attackDistance = 1.5f;//攻击的范围

    public static int MaxBulletNumber
    {
        get { return maxBulletNumber; }
    }
    public static float MaxHealth
    {
        get { return maxHealth; }
    }
    public float Health
    {
        get
        {
            return health;
        }
    }
    public float AttackDistance
    {
        get { return attackDistance; }
    }


    private void Start()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();

        this.state = EvaluatorType.None;
        sensoryComponent = new RavenSensoryComponent(this);

        //TODO 正确的传入参数
        weaponSystem = new WeaponSystem(this,0,0,0);

        pathPlanner = new PathPlanner(this);

        targetSelectionSystem = new TargetSelectSystem(this);

        thinkComponent = new Goal_Think(this);

        thinkComponent.Activate();

        this.SteerComponent.SetAvoidWallParameter(3, 70, 0.15f);

        this.SteerComponent.WallAvoidanceOn();
    }
    private void Update()
    {
        if(mIsPlayerControl)
        {
            //进行玩家模拟行为
            PlayerControlFunction();
        }else
        {
            //进行AI模拟行为
            AIControlFunction();
        }

        if(health<=0)
        {
            //将死亡消息进行广播
            MessageDispatcher.Instance.BroadCast(this.id, EntityMsgType.HasBeenKilled);
            //AI已经死亡
            EntityManager.Instance.RemoveEntity(this);
            //Debug.LogError("成功将消息进行广播了");
            GameObject.Destroy(this.gameObject);
        }
    }
    private void FixedUpdate()
    {
        EntityUpdate();
    }
    /// <summary>
    /// 玩家控制方法
    /// </summary>
    private void PlayerControlFunction()
    {

    }
    /// <summary>
    /// AI控制方法
    /// </summary>
    private void AIControlFunction()
    {
        sensoryComponent.UpdateComponent();
        targetSelectionSystem.Update(Time.deltaTime);
        thinkComponent.UpdateThink();

        //测试的更新
        sensoryComponent.DebugComponent();
        steer.DebugComponent();
    }
    /// <summary>
    /// 增加生命值的方法
    /// </summary>
    public void IncreaseHealth(float increaseHealth)
    {
        float result = this.health + increaseHealth;

        if(result <= maxHealth)
        {
            this.health = result;
        }
    }
    /// <summary>
    /// 增加子弹数量
    /// </summary>
    public void IncreaseBullet(int increaseBullet)
    {
        int result = increaseBullet + this.weaponSystem.BulletNumber;

        if(result<=maxBulletNumber)
        {
            this.weaponSystem.BulletNumber = result;
        }
    }
    /// <summary>
    /// AI受到伤害的方法
    /// </summary>
    public void Hurt(int damage)
    {
        this.health += damage;

        if(health<=0)
        {
            this.health = 0;
        }
    }
    /// <summary>
    /// AI的消息处理函数
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public override bool OnMessage(Msg msg)
    {
        switch((EntityMsgType)msg.Op)
        {
            case EntityMsgType.HasBeenKilled:
                //当一个AI接收到另一个AI的死亡消息的时候
                //将消息传递给AI的感知部分
                this.sensoryComponent.OnMessage(msg);
                //将消息传递给目标选择组件
                this.targetSelectionSystem.OnMessage(msg);
                Debug.LogWarning("死亡信息被成功的处理了");
                return true;

            case EntityMsgType.BeControl:
                //Debug.LogError("接收到控制消息");
                int id = (int)msg.Value;
                if(this.mIsPlayerControl)
                {
                    //当前正在被控制,取消控制
                    this.mIsPlayerControl = false;
                }
                else
                {
                    //检测位置是否一样
                    if(this.id == id)
                    {
                        thinkComponent.ClearGoal();
                        this.mIsPlayerControl = true;
                        steer.ClearTarget();
                        this.velocity = new Vector2();
                        this.acceleration = new Vector2();
                    }
                }

                return true;
        }

        return thinkComponent.OnMessage(msg);
    }

    
}
