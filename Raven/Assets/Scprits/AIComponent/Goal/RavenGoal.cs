using GoalPack.RavenGoals;
using GoalPack.RavenGoalsEvaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
namespace GoalPack
{


    /// <summary>
    /// 目标的状态枚举
    /// </summary>
    public enum GoalState
    {
        Wait = 0,
        Fail,
        Complete,
        Runing
    }

    /// <summary>
    /// 目标类型的枚举
    /// </summary>
    public enum GoalType
    {
        /// <summary>
        /// 原子目标
        /// </summary>
        Singal,
        /// <summary>
        /// 组合目标
        /// </summary>
        Composite
    }

    /// <summary>
    /// 评估器的类型
    /// </summary>
    public enum EvaluatorType
    {
        HeathEvaluator,
        WeaponEvaluator,
        WanderEvaluator,
        AttackEvaluator,
        HunterEvaluator,
        None,
        SeekMemory
    }

    /// <summary>
    /// 目标基类
    /// </summary>
    public class RavenGoal
    {

        protected bool active;
        protected GoalType type;
        public GoalType Type
        {
            get
            {
                return type;
            }
        }
        protected GoalState state;
        public GoalState State
        {
            get { return state; }
        }
        protected RavenEntity owner;
        public RavenEntity Owner
        {
            get
            {
                return owner;
            }
        }

        public RavenGoal(RavenEntity owner)
        {
            this.owner = owner;

            this.state = GoalState.Wait;
        }

        /// <summary>
        /// 激活这个目标的方法
        /// </summary>
        public virtual void Activate()
        {

        }
        /// <summary>
        /// 执行这个目标的方法
        /// </summary>
        public virtual GoalState Process()
        {
            return GoalState.Complete;
        }
        /// <summary>
        /// 消除这个目标的方法
        /// </summary>
        public virtual void Terminate()
        {

        }
        /// <summary>
        /// 激活这个目标
        /// </summary>
        public void MakeActive()
        {
            if (!active)
            {
                active = true;
            }
        }
        /// <summary>
        /// 该目标是否完成
        /// </summary>
        /// <returns></returns>
        public bool isComplete()
        {
            return state == GoalState.Complete;
            
        }
        /// <summary>
        /// 返回该目标是否执行失败
        /// </summary>
        /// <returns></returns>
        public bool HasFail()
        {
            return state == GoalState.Fail;
            
        }
        /// <summary>
        /// 当前目标是否被激活
        /// </summary>
        /// <returns></returns>
        public bool isActive()
        {
            return active;
        }
        /// <summary>
        /// 返回目标是否被禁用
        /// </summary>
        /// <returns></returns>
        public bool isInactive()
        {
            return !active;
        }
        /// <summary>
        /// 消息处理函数
        /// </summary>
        /// <returns></returns>
        public virtual bool HandleMessage()
        {
            return false;
        }
        /// <summary>
        /// 添加一个子目标
        /// </summary>
        /// <returns></returns>
        public virtual bool AddSubGoal(RavenGoal goal)
        {
            return false;
        }
        /// <summary>
        /// 禁用目标
        /// </summary>
        public virtual void InActive()
        {

        }
        /// <summary>
        /// 消息处理函数
        /// </summary>
        /// <returns></returns>
        public virtual bool OnMessage(Msg msg)
        {
            return false;
        }

    }
    /// <summary>
    /// 组合目标的名称空间
    /// </summary>
    namespace RavenGoals
    {
        #region 基本类型

        /// <summary>
        /// AI的思考类，用来处理AI的思考逻辑
        /// </summary>
        public class Goal_Think : CompositeGoal
        {
            protected List<Evaluator> evaluators;

            protected float thinkRate = 0.5f;//AI思考的速率

            protected float thinkTimer = 1;//计时器

            //当前最优的目标
            protected Evaluator bestGoal;

            protected Evaluator prevGoal;

            public Goal_Think(RavenEntity owner) : base(owner)
            {
                evaluators = new List<Evaluator>();

                bestGoal = null;
                prevGoal = null;
                thinkRate = UnityEngine.Random.Range(0.5f,0.8f);
                evaluators.Add(new GetHealthGoal_Evaluator());
                evaluators.Add(new GetBulletGoal_Evaluator());
                evaluators.Add(new WanderGoal_Evaluator());
                evaluators.Add(new AttackAndHunterGoal_Evaluator());
            }

            public override void Activate()
            {
                base.Activate();

                this.state = GoalState.Runing;
            }

            public override GoalState Process()
            {
                this.state = ProcessSubGoal();

                return GoalState.Runing;
            }

            public override void Terminate()
            {
                base.Terminate();
            }

            /// <summary>
            /// 判断前一个目标和当前目标是否是同一个
            /// </summary>
            /// <returns></returns>
            private bool isSameGoal()
            {
                if (prevGoal == null) return false;

                if (prevGoal.GetEvaluatorType() == bestGoal.GetEvaluatorType()) return true;

                return false;
            }

            /// <summary>
            /// AI 用于更新思考的方法
            /// </summary>
            public void SelectGoal()
            {
                prevGoal = bestGoal;

                float evaluation = 0;

                foreach (var evaluator in evaluators)
                {
                    float value = evaluator.Calculate(this.owner);

                    if (value > evaluation)
                    {
                        evaluation = value;

                        this.bestGoal = evaluator;

                    }
                }

                if (!isSameGoal())
                {
                   
                    this.Terminate();
                    this.bestGoal.SetGoal(this.owner);
                    this.owner.state = bestGoal.GetEvaluatorType();
                }
                    
            }

            /// <summary>
            /// 更新思考
            /// </summary>
            public void UpdateThink()
            {
                thinkTimer += Time.deltaTime;

                if (thinkTimer > thinkRate)
                {
                    //Debug.Log("AI进行了一个思考");
                    this.SelectGoal();

                    thinkTimer = 0;
                }

                this.Process();
            }

            public override GoalState ProcessSubGoal()
            {
                var goal = this.subGoalStack.Peek();

                if (!goal.isActive())
                {
                    goal.Activate();
                }

                
                goal.Process();

                return GoalState.Runing;
            }
            public void ClearGoal()
            {
                prevGoal = null;
                bestGoal = null;
            }
            public override bool OnMessage(Msg msg)
            {
                bool marked = false;
                foreach(var g in subGoalStack)
                {
                    marked = g.OnMessage(msg);
                }

                return marked;
            }
        }

        /// <summary>
        /// 组合目标的基类
        /// </summary>
        public class CompositeGoal : RavenGoal
        {
            /// <summary>
            /// 保存着子目标的栈
            /// </summary>
            protected Stack<RavenGoal> subGoalStack;

            public CompositeGoal(RavenEntity entity) : base(entity)
            {
                this.type = GoalType.Composite;
                subGoalStack = new Stack<RavenGoal>();
                this.state = GoalState.Wait;
            }

            public override bool AddSubGoal(RavenGoal goal)
            {
                this.subGoalStack.Push(goal);

                return true;
            }

            /// <summary>
            /// 处理所有的子目标
            /// </summary>
            /// <returns></returns>
            public virtual GoalState ProcessSubGoal()
            {

                //在开始处理前进行先将已经完成的和已经失败的子目标进行移除
                while (subGoalStack.Count != 0 && (subGoalStack.Peek().isComplete() || subGoalStack.Peek().HasFail()))
                {
                    var goal = subGoalStack.Peek();

                    goal.Terminate();

                    subGoalStack.Pop();

                    //Debug.Log("移除了"+goal.GetType()+"目标"+"移除的状态为"+goal.State);
                }

                if (subGoalStack.Count != 0)
                {
                    var subGoal = subGoalStack.Peek();

                    if (!subGoal.isActive()) subGoal.Activate();

                    var subGoalState = subGoal.Process();

                    //Debug.Log("更新了" + subGoal.GetType()+"目标");

                    if (subGoalState == GoalState.Complete)
                    {
                        this.subGoalStack.Pop().Terminate();
                        return GoalState.Runing;
                    }

                    return subGoalState;

                }

                return GoalState.Complete;
            }

            /// <summary>
            /// 移除所有的子目标
            /// </summary>
            public virtual void RemoveAllSubGoal()
            {
                foreach(var goal in subGoalStack)
                {
                    goal.Terminate();
                }

                while (subGoalStack.Count != 0)
                    subGoalStack.Pop();

            }

            /// <summary>
            /// 组合目标类的激活方法
            /// </summary>
            public override void Activate()
            {
                if (!active) MakeActive();
            }

            public override GoalState Process()
            {
                this.state = ProcessSubGoal();

                if (isActive()) return state;

                
                return GoalState.Fail;
            }

            public override void Terminate()
            {
                this.InActive();

                RemoveAllSubGoal();
            }

            
        }
        #endregion

        #region 移动相关


        /// <summary>
        /// AI的随机游走类
        /// </summary>
        public class Goal_Wander : CompositeGoal
        {
            private Vector2 target;

            protected NavMap map;

            public Goal_Wander(RavenEntity owner, NavMap map) : base(owner)
            {
                this.map = map;
            }

            public override void Activate()
            {
                base.Activate();
            }

            public override GoalState Process()
            {
                this.state = base.Process();

                //当AI到达了目标的时候返回Complete
                if (state == GoalState.Complete || state == GoalState.Fail)
                {
                    Vector2 target = map.NavGraph.GetNode(Random.Range(0, map.NavGraph.TotalV)).Pos;

                    AddSubGoal(new Goal_MoveToPosition(this.owner, target));
                    //Debug.Log("Wander目标添加了一个MoveToPosition目标");
                    this.state = GoalState.Runing;
                }

                return this.state;
            }

            public override void Terminate()
            {
                base.Terminate();

                map = null;
            }

            public override bool HandleMessage()
            {
                return base.HandleMessage();
            }

        }


        /// <summary>
        /// 将角色移动到目标
        /// </summary>
        public class Goal_MoveToPosition : CompositeGoal
        {
            protected Vector2 targetPos;

            //用于寻路的路径
            protected List<PathEdge> path;

            protected bool isHasPath;

            public Goal_MoveToPosition(RavenEntity owner, Vector2 pos) : base(owner)
            {
                this.targetPos = pos;

                path = new List<PathEdge>();

                isHasPath = false;
            }

            public override void Activate()
            {
                base.Activate();
                DebugController.instance.CleanPathLine();
                //判断是否要使用时间片搜索算法
                if (GameController.Instance.IsTimeSliced)
                {
                    owner.PathPlanner.CreatePathToPositionByTimeSliced(targetPos,
                        delegate (List<PathEdge> p) {
                            this.path = p;
                            if (path.Count != 0)
                            {
                                isHasPath = true;

                                //进行路径平滑如果有的话
                                owner.PathPlanner.SmoothPathEdge(ref this.path);
                                Debug.Log("找到路了");
                                AddSubGoal(new Goal_FollowPath(owner, this.path));

                                //Debug.Log("MoveToPosition目标添加了一个FollowPath目标 " + subGoalStack.Count);
                            }
                            else
                            {
                                //Debug.Log("目标MoveToPosition 没有为targetPos找到一条合适的路径");
                                this.state = GoalState.Fail;
                                return;
                            }
                        });
                }
                else
                {
                    owner.PathPlanner.CreatePathToPositionNonTimeSliced(targetPos, ref path);

                    if (path.Count != 0)
                    {
                        isHasPath = true;
                        //进行路径平滑如果有的话
                        owner.PathPlanner.SmoothPathEdge(ref path);

                        AddSubGoal(new Goal_FollowPath(owner, path));

                        Debug.Log("MoveToPosition目标添加了一个FollowPath目标");
                    }
                    else
                    {
                        Debug.Log("目标MoveToPosition 没有为targetPos找到一条合适的路径");

                        this.state = GoalState.Fail;

                        return;
                    }
                }


                this.state = GoalState.Runing;
            }

            public override GoalState Process()
            {
                if (isActive())
                {
                    if (isHasPath)
                    {
                        this.state = ProcessSubGoal();
                    }
                    else
                    {
                        this.state = GoalState.Runing;
                    }
                }
                else
                {
                    this.state = GoalState.Fail;
                }

                return this.state;
            }

            public override void Terminate()
            {
                base.Terminate();
            }
        }

        /// <summary>
        /// 目标的路径跟随目标
        /// </summary>
        public class Goal_FollowPath : CompositeGoal
        {
            protected List<PathEdge> path;

            protected int index;

            public Goal_FollowPath(RavenEntity owner, List<PathEdge> path) : base(owner)
            {
                this.path = path;
            }

            public override void Activate()
            {
                base.Activate();

                this.state = GoalState.Runing;

                index = 0;
            }

            public override GoalState Process()
            {


                this.state = ProcessSubGoal();

                if (state == GoalState.Complete)
                {
                    if (index == path.Count)
                    {
                        this.state = GoalState.Complete;
                    }
                    else
                    {
                        //如果AI到达一个节点
                        var goal = new Goal_TraverseEdge(owner, path[index]);
                        if (index == path.Count - 1) goal.SetArrive();
                        else goal.SetSeek();
                        this.AddSubGoal(goal);
                        //Debug.Log("FollowPath目标添加了一个TraverseEdge目标");
                        this.state = GoalState.Runing;
                        index++;
                    }

                }
                return state;
            }

            public override void Terminate()
            {
                base.Terminate();
                owner.SteerComponent.ArriveOff();
                owner.SteerComponent.SeekOff();

                //Debug.Log("路径跟随终止了seek行为和arrive行为");
            }
        }

        /// <summary>
        /// 穿越一条边的目标
        /// </summary>
        public class Goal_TraverseEdge : RavenGoal
        {
            protected PathEdge edge;

            protected bool isArrive = false;

            //LUA
            protected static float overTime = 2;

            //一个计数器当计算器
            protected float timer;

            public Goal_TraverseEdge(RavenEntity owner, PathEdge edge) : base(owner)
            {
                this.edge = edge;

            }

            public override void Activate()
            {
                base.Activate();
                owner.SteerComponent.SetTarget(edge.Destination(), () => { this.isArrive = true; });
                this.state = GoalState.Runing;
                this.timer = 0;
            }

            public override GoalState Process()
            {

                //if(this.owner.Raven_TargetSelectionSystem.isHaveTarget()&&this.owner.Raven_TargetSelectionSystem.isTargetWithinFOV())
                //{
                //    this.owner.UpdateOrientationOff();
                //    this.owner.SetOrientationTarget(this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition() - this.owner.Pos);
                //}

                if (isArrive)
                    this.state = GoalState.Complete;
                else if (timer > overTime)
                {
                    //到达目标失败
                    this.state = GoalState.Fail;

                    Debug.Log("AI到达目标超时");
                }
                else
                {
                    this.state = GoalState.Runing;
                }
                timer += Time.deltaTime;

                return state;
            }
            
            public override void Terminate()
            {
                base.Terminate();
                this.owner.SteerComponent.SeekOff();
                this.owner.SteerComponent.ArriveOff();
                this.owner.UpdateOrientationOn();
            }

            //让AI以seek行为进行操作
            public void SetSeek()
            {
                owner.SteerComponent.SeekOn();
                owner.SteerComponent.ArriveOff();
            }

            /// <summary>
            /// 让AI以arrive行为进行操作
            /// </summary>
            public void SetArrive()
            {
                owner.SteerComponent.SeekOff();
                owner.SteerComponent.ArriveOn();
            }
        }

        /// <summary>
        /// 跟随的行为的目标
        /// </summary>
        public class Goal_FollowTarget:RavenGoal
        {
            public Goal_FollowTarget(RavenEntity entity):base(entity)
            {
                this.state = GoalState.Wait;
            }

            public override void Activate()
            {
                base.Activate();
                this.state = GoalState.Runing;
                this.owner.SteerComponent.ArriveOff();
                this.owner.SteerComponent.SeekOn();
            }

            public override GoalState Process()
            {
                if(owner.Raven_TargetSelectionSystem.isHaveTarget())
                {
                    owner.SteerComponent.SetTarget(owner.Raven_TargetSelectionSystem.GetLastRecoredPosition());
                    this.state = GoalState.Runing;
                }else
                {
                    this.state = GoalState.Fail;
                }

                return state;
            }

            public override void Terminate()
            {
                base.Terminate();
                this.owner.SteerComponent.SeekOff();
                this.owner.SteerComponent.ArriveOff();
            }

        }


        #endregion

        #region 其他类型

        /// <summary>
        /// 获取健康包目标
        /// </summary>
        public class Goal_GetHealth:CompositeGoal
        {
            private int mTargetHealthPackID;//目标健康包的ID号

            public Goal_GetHealth(RavenEntity owner):base(owner)
            {
                this.state = GoalState.Wait;
                
            }

            public override void Activate()
            {
                //Debug.LogWarning("AI开始寻找健康包"+this.owner.gameObject.name);
                base.Activate();
                this.state = GoalState.Runing;
                
                //寻找一个健康包的位置进行寻路
                var trigger = TriggerSystem.Instance.GetTriggerByType( TriggerType.HealthGiverTrigger,this.owner);

                if(trigger == null)
                {
                    
                    this.state = GoalState.Fail;
                    return;
                }
                //目标健康包的时候
                this.mTargetHealthPackID = trigger.ID;
                //添加一个移动到指定目标的
                if(trigger!=null)
                {
                    AddSubGoal(new Goal_MoveToPosition(this.owner, trigger.Pos));

                    Debug.Log("成功找到一个健康包的位置，并且成功添加到子目标当中");
                   
                }
                else
                {
                    Debug.Log("没有找到健康包的位置");
                    
                }
            }
            public override GoalState Process()
            {
                if(this.state != GoalState.Fail)
                    this.state = ProcessSubGoal();
                
                return this.state;
            }
            public override void Terminate()
            {
                base.Terminate();
                //Debug.LogWarning("从GetHealth目标中退出了"+this.owner.gameObject.name);
            }
            public override bool OnMessage(Msg msg)
            {
                switch((EntityMsgType)msg.Op)
                {
                    case EntityMsgType.HealthPackIsGone:
                       if(msg.SenderID == this.mTargetHealthPackID)
                        {
                            //当健康包被别人抢走的时候,并且状态没有发生转换
                            RemoveAllSubGoal();
                            Activate();//重新选择一遍路径
                            //Debug.LogError("受到一条目标健康包被其他人抢走的消息了");
                        }
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 获取武器的方法
        /// </summary>
        public class Goal_GetBullet : CompositeGoal
        {
            private int targetBulletID;//当前子弹的ID

            public Goal_GetBullet(RavenEntity owner):base(owner)
            {
                this.state = GoalState.Wait;
            }

            public override void Activate()
            {
                base.Activate();
                this.state = GoalState.Runing;

                //寻找一个子弹包的位置并进行寻路
                var trigger = TriggerSystem.Instance.GetTriggerByType(TriggerType.BulletGiverTrigger,this.owner);

                if(trigger == null)
                {
                    this.state = GoalState.Fail;
                    return;
                }

                this.targetBulletID = trigger.ID;
                //添加一个移动到指定目标的
                if (trigger != null)
                {
                    AddSubGoal(new Goal_MoveToPosition(this.owner, trigger.Pos));
                    
                    Debug.Log("成功找到一个武器包的位置，并且成功添加到子目标当中");
                }
                else
                {
                    Debug.Log("没有找到武器包的位置");
                }
            }
            public override GoalState Process()
            {
                if(this.state != GoalState.Fail)
                    this.state = ProcessSubGoal();

                return this.state;
            }
            public override void Terminate()
            {
                base.Terminate();
            }
            public override bool OnMessage(Msg msg)
            {
                switch((EntityMsgType)msg.Op)
                {
                    case EntityMsgType.BulletPackIsGone:
                        if(msg.SenderID == targetBulletID)
                        {
                            //当一个弹药包被抢走的时候触发的方法
                            RemoveAllSubGoal();
                            Activate();//重新选择一个弹药包进行寻路
                            //Debug.LogError("受到了目标子弹包被其他人抢走的消息了");
                        }
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 猎杀目标
        /// </summary>
        public class Goal_AttackAndHunterTarget:CompositeGoal
        {
            protected float attackSpan;
            protected bool isCanShoot;//是否可以进行射击
            protected float timer;
            
           
            protected Vector2 targetPos;

            protected bool isCatch = false;

            protected int targetID;

            public Goal_AttackAndHunterTarget(RavenEntity owner):base(owner)
            {
                this.owner.state = EvaluatorType.None;
                this.state = GoalState.Wait;
                attackSpan = 0.8f;
                timer = attackSpan;

            }

            public override void Activate()
            {
                base.Activate();

                this.state = GoalState.Runing;

            }

            public override GoalState Process()
            {
                base.Process();

                if (this.owner.Raven_TargetSelectionSystem.isHaveTarget())
                {
                    if(this.owner.Raven_TargetSelectionSystem.isTargetShootable())
                    {
                        //当目标在攻击范围内的时候
                        //Debug.LogError("目标在攻击范围内"+ this.subGoalStack.Count);
                        if(this.subGoalStack.Count == 0)
                        {
                            //Debug.LogError("成功添加一个attack目标了" + this.owner.gameObject.name);
                            this.AddSubGoal(new Goal_AttackTarget(this.owner));
                            isCanShoot = true;
                        }

                    }
                    else if(this.owner.Raven_TargetSelectionSystem.isTargetWithinFOV())
                    {
                        //不在攻击范围内但是在视野范围内的时候,追踪目标
                        //Debug.LogError("目标不在攻击范围内,但在视野范围内"+ this.subGoalStack.Count);
                        if (this.subGoalStack.Count == 0)
                        {
                            //Debug.LogError("成功添加一个hunter目标了" + this.owner.gameObject.name);
                            this.AddSubGoal(new Goal_SeekTarger(this.owner));
                            isCanShoot = true;
                        }
                    }
                    else 
                    {
                        //在视野范围外的时候,寻找记忆点
                        //Debug.LogError("目标既不在攻击范围内,也在视野范围内"+ this.subGoalStack.Count);
                        if(this.subGoalStack.Count == 0)
                        {
                            //Debug.LogWarning("成功添加了一个seekmemory目标了" + this.owner.gameObject.name);
                            this.AddSubGoal(new Goal_SeekMemoryTarget(this.owner));
                            isCanShoot = false;
                        }
                        
                    }
                }
                
                if(owner.Raven_WeaponSystem.BulletNumber>0&&isCanShoot&&(timer+=Time.deltaTime)>=attackSpan)
                {
                    Vector2 targetPos = new Vector2();

                    if(this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition(ref targetPos))
                    {
                        Bullet.CreateBullet(
                        this.owner.Pos + (Vector2)this.owner.transform.up * 0.3f,
                        (targetPos - this.owner.Pos).normalized,
                        0.8f);
                        timer = 0;
                    }
                    owner.Raven_WeaponSystem.BulletNumber--;
                }

                return  GoalState.Runing;
            }

            public override void Terminate()
            {
                base.Terminate();
                //Debug.LogError("AI退出了Goal_Hunter目标");
            }

        }

        /// <summary>
        /// 主要实现AI在攻击时候的左右移动的行为
        /// </summary>
        public class Goal_AttackTarget:RavenGoal
        {
            protected RaycastHit2D[] hits;

            protected float radius;//位置选择的半径

            /// <summary>
            /// 寻找一个目标点的间隔时间
            /// </summary>
            protected float findPositionSpan;

            /// <summary>
            /// 计时器
            /// </summary>
            protected float timer;

            public Goal_AttackTarget(RavenEntity owner):base(owner)
            {
                this.state = GoalState.Wait;
                findPositionSpan = 0.5f;
                timer = findPositionSpan;
                this.radius = 1.2f;
                this.owner.state = EvaluatorType.AttackEvaluator;
            }

            public override void Activate()
            {
                base.Activate();
                this.state = GoalState.Runing;
                this.owner.SteerComponent.ArriveOff();
                this.owner.SteerComponent.SeekOn();
                this.owner.UpdateOrientationOff();
            }

            public override GoalState Process()
            {
                if(this.owner.Raven_TargetSelectionSystem.isHaveTarget())
                {
                    if(this.owner.Raven_TargetSelectionSystem.isTargetShootable())
                    {
                        //在范围内
                        this.state = GoalState.Runing;
                        //进行计时
                        if ((timer += Time.deltaTime) >= findPositionSpan)
                        {
                            this.owner.SteerComponent.SetTarget(GetRandomPositin());
                            timer = 0;
                        }
                    }
                    else
                    {
                        //不在范围内
                        this.state = GoalState.Fail;
                    }

                    this.owner.SetOrientationTarget(this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition()-this.owner.Pos);

                }else
                {
                    //目标已经死亡
                    //Debug.LogError("目标已经死亡了");
                    this.state = GoalState.Complete;
                    this.owner.Raven_TargetSelectionSystem.ClearTarget();
                }

                return this.state;
            }

            public override void Terminate()
            {
                base.Terminate();
                this.owner.SteerComponent.SeekOff();
                this.owner.UpdateOrientationOn();
            }

            /// <summary>
            /// 检测是否可以移动到某一个位置
            /// </summary>
            /// <returns></returns>
            private bool isCanMoveToPosition(Vector2 pos)
            {
                //进行射线探测
                //不对路点进行探测
                hits = Physics2D.LinecastAll(this.owner.Pos, pos, (1 << 10) | (1 << 9));

                if (hits.Length > 1)
                {
                    //Debug.LogWarning("不行");
                    
                    return false;
                }
                //Debug.LogWarning("行");
                return true;
            }
            /// <summary>
            /// 获取一个随即位置
            /// </summary>
            /// <returns></returns>
            private Vector2 GetRandomPositin()
            {
                Vector2 pos = new Vector2();
                float radius = this.radius;
                int i = 0;
                do
                {
                    pos = UnityEngine.Random.insideUnitCircle * radius;

                    pos = pos + this.owner.Pos;

                    if (i++ % 10 == 0)
                    {
                        radius /= 2;
                    }

                    if (i >= 20) break;

                } while (!isCanMoveToPosition(pos));

                return pos;
            }
        }
        
        /// <summary>
        /// 主要实现追踪一个目标的行为
        /// </summary>
        public class Goal_SeekTarger:CompositeGoal
        {
            

            public Goal_SeekTarger(RavenEntity owner):base(owner)
            {
                this.state = GoalState.Wait;
                this.owner.state = EvaluatorType.HunterEvaluator;
            }

            public override void Activate()
            {
                base.Activate();
                this.state = GoalState.Runing;
                this.owner.SteerComponent.SeekOn();
                this.owner.UpdateOrientationOff();
            }

            public override GoalState Process()
            {
                //Debug.LogWarning("hunting");

                if(this.owner.Raven_TargetSelectionSystem.isHaveTarget())
                {
                    if(this.owner.Raven_TargetSelectionSystem.isTargetShootable())
                    {
                        Debug.LogWarning("hunter");
                        this.state = GoalState.Complete;

                    }else if(this.owner.Raven_TargetSelectionSystem.isTargetWithinFOV())
                    {
                        if (subGoalStack.Count != 0) RemoveAllSubGoal();//将子目标全部进行清除
                        this.state = GoalState.Runing;
                        //直接靠近
                        this.owner.SteerComponent.SetTarget(this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition());
                    }
                    this.owner.SetOrientationTarget(this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition() - this.owner.Pos);
                }
                else
                {
                    //目标已经死亡
                    this.state = GoalState.Fail;
                }

                return this.state;
            }

            public override void Terminate()
            {
                base.Terminate();
                this.owner.SteerComponent.SeekOff();
                this.RemoveAllSubGoal();
                this.owner.UpdateOrientationOn();
                //Debug.LogError("hunter被移除");
            }
        }

        public class Goal_SeekMemoryTarget:CompositeGoal
        {
            protected float activeTime;//有效时间

            public Goal_SeekMemoryTarget(RavenEntity owner):base(owner)
            {
                this.state = GoalState.Wait;
                this.activeTime = 0.5f;
                this.owner.state = EvaluatorType.SeekMemory;
            }

            public override void Activate()
            {
                base.Activate();
                this.state = GoalState.Runing;
                this.owner.SteerComponent.SeekOn();
            }

            public override GoalState Process()
            {

                if(this.owner.Raven_TargetSelectionSystem.isHaveTarget())
                {
                    if(this.owner.Raven_TargetSelectionSystem.GetTimeTargetHasbeenOutOfView()>activeTime)
                    {
                        //超过有效时间,进行寻路
                        if(this.subGoalStack.Count == 0)
                        {
                            AddSubGoal(new Goal_MoveToPosition(this.owner,this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition()));

                        }

                        this.state = base.Process();

                        if(state == GoalState.Complete)
                        {
                            //寻路已经完成
                            this.state = GoalState.Fail;
                            this.owner.Raven_TargetSelectionSystem.ClearTarget();//移除目标防止AI一直呆在一个地方
                        }else
                        {
                            this.state = GoalState.Runing;
                        }

                    }else
                    {
                        //没有超过有效时间,进行seek
                        Vector2 targetPos = this.owner.Raven_TargetSelectionSystem.GetLastRecoredPosition();
                        this.owner.SteerComponent.SetTarget(targetPos);

                        if ((this.owner.Pos - targetPos).sqrMagnitude < 0.02f)
                        {
                            //到达位置
                            this.state = GoalState.Fail;
                        }
                        else
                        {
                            this.state = GoalState.Runing;
                        }
                    }
                }


                return this.state;
            }

            public override void Terminate()
            {
                base.Terminate();
                RemoveAllSubGoal();
                this.owner.SteerComponent.SeekOff();
            }
        }

        #endregion
    }
    /// <summary>
    /// 进行目标仲裁的名称空间（所有类都基于命令模式进行编写）
    /// </summary>
    namespace RavenGoalsEvaluator
    {
        /// <summary>
        /// 基于目标评估的基类
        /// </summary>
        public class Evaluator
        {
            protected EvaluatorType type;

            protected float k = 10;//目标价值的调整因子

            public virtual float Calculate(RavenEntity entity)
            {
                return 0;
            }

            /// <summary>
            /// 让AI设置完成这个目标的子目标
            /// </summary>
            public virtual void SetGoal(RavenEntity entity)
            {
                
            }

            public virtual EvaluatorType GetEvaluatorType()
            {
                return this.type;
            }
        }

        /// <summary>
        /// 获取健康包的目标评估
        /// </summary>
        public class GetHealthGoal_Evaluator:Evaluator
        {
            public GetHealthGoal_Evaluator()
            {
                this.type = EvaluatorType.HeathEvaluator;
            }

            public override float Calculate(RavenEntity entity)
            {
                //当没有这样一个触发器的时候dist是一个无穷大的数
                float desirability = 0;
                float dist = TriggerSystem.Instance.GetClostTriggerByRaven(TriggerType.HealthGiverTrigger,entity);

                if (dist == float.PositiveInfinity) desirability =  0;
                else
                desirability = k * (1 - entity.Health / RavenEntity.MaxHealth)/dist;

                //Debug.Log("Health目标的评估是"+desirability+","+dist+"Health"+ entity.Health);
                return desirability;
            }

            public override void SetGoal(RavenEntity entity)
            {
                //添加一个获取健康包的目标
                entity.ThinkGomponent.AddSubGoal(new Goal_GetHealth(entity));
                Debug.Log(entity.gameObject.name+" 当前的最上层的目标是GetHealth");
                
            }
        }

        /// <summary>
        /// 获取弹药的目标评估
        /// </summary>
        public class GetBulletGoal_Evaluator:Evaluator
        {
            public GetBulletGoal_Evaluator()
            {
                this.type = EvaluatorType.WeaponEvaluator;
                k = 0.1f;
            }

            public override float Calculate(RavenEntity entity)
            {
                float desirability = 0;
                float dist = TriggerSystem.Instance.GetClostTriggerByRaven( TriggerType.BulletGiverTrigger,entity);

                if (dist != float.PositiveInfinity)
                {
                    desirability = k * entity.Health * (1 - (float)entity.Raven_WeaponSystem.BulletNumber / (float)RavenEntity.MaxBulletNumber) / dist;
                    Debug.LogWarning("GetWeapon目标的评估是" + desirability);
                }
                
                return desirability;
            }

            public override void SetGoal(RavenEntity entity)
            {
                entity.ThinkGomponent.AddSubGoal(new Goal_GetBullet(entity));

                Debug.Log(entity.gameObject.name+"当前最上层的目标是GetWeapon");
               
            }
        }

        /// <summary>
        /// 随机游走目标评估
        /// </summary>
        public class WanderGoal_Evaluator:Evaluator
        {
            public WanderGoal_Evaluator()
            {
                this.type = EvaluatorType.WanderEvaluator;
            }

            public override float Calculate(RavenEntity entity)
            {

                //一直返回一个比较小的的期望值
                return 0.2f;
            }

            public override void SetGoal(RavenEntity entity)
            {

                //设置目标
                entity.ThinkGomponent.AddSubGoal(new Goal_Wander(entity, NavMap.Instance));

                Debug.Log(entity.gameObject.name+"当前最上层的目标是Wander");

                //Debug.Break();
            }
        }

        /// <summary>
        /// 追踪目标的目标评估
        /// </summary>
        public class AttackAndHunterGoal_Evaluator:Evaluator
        {
            public AttackAndHunterGoal_Evaluator()
            {
                this.type = EvaluatorType.HunterEvaluator;
            }

            public override float Calculate(RavenEntity entity)
            {
                float desirability = 0;
                if (entity.Raven_TargetSelectionSystem.isHaveTarget())
                {
                    //float dist = (entity.Raven_TargetSelectionSystem.GetLastRecoredPosition() - entity.Pos).magnitude;

                    desirability = k * (entity.Health / RavenEntity.MaxHealth) * ((float)entity.Raven_WeaponSystem.BulletNumber / RavenEntity.MaxBulletNumber);

                    Debug.Log(entity.gameObject.name+"HunterGoal目标的评估是" + desirability);
                }
                
                return desirability;
            }

            public override void SetGoal(RavenEntity entity)
            {
                entity.ThinkGomponent.AddSubGoal(new Goal_AttackAndHunterTarget(entity));
                Debug.Log(entity.gameObject.name+"的当前最上层的目标是hunter");
                //Debug.Break();
            }
        }

    }
}
