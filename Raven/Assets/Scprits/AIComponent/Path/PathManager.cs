using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 路径搜索管理器
/// </summary>
public class PathManager : MonoBehaviour {

    private static PathManager _instance;

    public static PathManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private Dictionary<int,PathPlanner> pathPlanners;

    private List<int> removeID_List;

    private bool isClean;

    private void Awake()
    {
        _instance = this;

        pathPlanners = new Dictionary<int, PathPlanner>();

        removeID_List = new List<int>();

        isClean = false;
    }

   
    private void Update()
    {
        foreach(var p in pathPlanners.Values)
        {
            var state = p.CycleOne();

            if(state == SearchState.Search_Fail||state == SearchState.Search_Complete)
            {
                removeID_List.Add(p.Owner.ID);

                isClean = true;
            }
        }

        foreach(var p in removeID_List)
        {
            pathPlanners.Remove(p);

            //Debug.Log("移除一个时间片");
        }

        if (isClean)
        {
            removeID_List = new List<int>();

            isClean = false;
        }
    }

    /// <summary>
    /// 注册搜索请求
    /// </summary>
    /// <returns></returns>
    public bool RegistSearchRequest(PathPlanner planner )
    {
        pathPlanners[planner.Owner.ID] = planner;

        return true;
    }

    /// <summary>
    /// 取消寻路请求
    /// </summary>
    /// <returns></returns>
    public bool CancelSearchRequest()
    {
        //TODO
        return false;
    }
}
