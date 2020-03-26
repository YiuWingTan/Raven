using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 实体管理器
/// </summary>
public class EntityManager : MonoBehaviour {

    private static EntityManager _instance;

    public static EntityManager Instance
    {
        get
        {
            return _instance;
        }
    }


    private Dictionary<int, BaseGameEntity> entityContainer;

    public Dictionary<int, BaseGameEntity> EntityContainer
    {
        get
        {
            return entityContainer;
        }
    }

    private void Awake()
    {
        _instance = this;

        entityContainer = new Dictionary<int, BaseGameEntity>();
    }

    /// <summary>
    /// 向实体容器添加一个实体
    /// </summary>
    /// <param name="entity"></param>
    public void RegisterEntity(BaseGameEntity entity)
    {
        if(!entityContainer.ContainsKey(entity.ID))
        {
            entityContainer.Add(entity.ID,entity);
        }
    }

    /// <summary>
    /// 从实体容器中移除一个实体
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveEntity(BaseGameEntity entity)
    {
        if(entityContainer.ContainsKey(entity.ID))
        {
            entityContainer.Remove(entity.ID);
        }
    }

    public BaseGameEntity GetEntityByID(int id)
    {
        if(entityContainer.ContainsKey(id))
        {
            return entityContainer[id];
        }
        return null;
    }

    
}

