using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器类型枚举
/// </summary>
public enum WeaponType
{
    Gun
}

/// <summary>
/// AI的武器系统
/// </summary>
public class WeaponSystem  {

    private RavenEntity owner;
    /// <summary>
    /// 当前武器
    /// </summary>
    private Weapon currentWeapon;

    /// <summary>
    /// 反应时间
    /// </summary>
    private float reactionTime;
    /// <summary>
    /// AI射击时的精度偏差
    /// </summary>
    private float aimAccuracy;
    /// <summary>
    /// AI的停留时间
    /// </summary>
    private float aimPersistance;

    protected int bulletNumber = 10;//AI的子弹的数量
    protected float shootForce;//子弹发射的力度

    public int BulletNumber
    {
        get { return this.bulletNumber; }
        set { this.bulletNumber = value; }
    }
    public float ShootForce
    {
        get { return shootForce; }
    }

    /// <summary>
    /// 武器
    /// </summary>
    private Dictionary<WeaponType, Weapon> weaponMap;

    public RavenEntity Owner
    {
        get
        {
            return owner; ;
        }
    }

    //TODO lua
    public float AimPersistance
    {
        get
        {
            return aimPersistance;
        }
    }
    //TODO lua
    public float ReactionTime
    {
        get
        {
            return reactionTime;
        }

       
    }
    //TODO lua
    public float AimAccuracy
    {
        get
        {
            return aimAccuracy;
        }
    }

    public WeaponSystem(RavenEntity entity,float reactionTime,float aimPersistance,float aimAccuracy)
    {
        this.owner = entity;

        this.reactionTime = reactionTime;

        this.aimPersistance = aimPersistance;

        this.aimAccuracy = aimAccuracy;
    }

    /// <summary>
    /// 给射击添加一次精度偏差
    /// </summary>
    private void AddNoistToAim(ref Vector2 aimPos)
    {

    }
    /// <summary>
    /// 获取预测位置
    /// </summary>
    /// <returns></returns>
    private Vector2 PredicFuturePositionOfTarget()
    {
        return new Vector2();
    }
    /// <summary>
    /// 初始化方法
    /// </summary>
    public void Init()
    {
        weaponMap = new Dictionary<WeaponType, Weapon>();
    }
    /// <summary>
    /// 瞄准并射击的方法
    /// </summary>
    public void TakeAimAndShoot(Vector2 pos)
    {
        //TODO 加入抖动
        Vector2 direction = (pos - owner.Pos).normalized;

        //Bullet.CreateBullet(pos, direction,shootForce);
    }
}
