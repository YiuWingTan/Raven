using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///子弹类
/// </summary>
public class Bullet : MonoBehaviour {

    public static GameObject bulletGamePrefabe = null;//子弹的游戏对象

    protected Vector2 velocity;
    
    protected Vector2 accerlationDirection;//加速度方向
    protected float accerlation;//加速度大小

    
    protected float firction = 0.8f;//摩檫力的大小
   

    private void Update()
    {
        this.accerlation *= firction;

        this.velocity += accerlationDirection * (accerlation);
        
        this.transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Raven")
        {
            //击中AI了
            //TODO 进行处理
            var entity = collision.GetComponent<RavenEntity>();
            entity.Hurt(-10);
            Debug.LogWarning("子弹成功击中一个目标");
        }

        //删除这个子弹
        GameObject.Destroy(this.gameObject);


    }

    /// <summary>
    /// 发射速度
    /// </summary>
    public static void CreateBullet(Vector2 pos,Vector2 forceDirction,float force)
    {
        Vector3 position = new Vector3(pos.x, pos.y, -8);

        var bullet = GameObject.Instantiate(
                                            Bullet.bulletGamePrefabe,position,
                                            Quaternion.LookRotation(new Vector3(0,0,1), 
                                            new Vector3(forceDirction.x, forceDirction.y,-8))).GetComponent<Bullet>();

        bullet.SetForce( forceDirction,  force);
    }

    /// <summary>
    /// 设置力的方向和大小
    /// </summary>
    /// <param name="forceDirction"></param>
    /// <param name="force"></param>
    public void SetForce(Vector2 forceDirction, float force)
    {
        this.accerlationDirection = forceDirction;

        this.accerlation = force;
    }
}
