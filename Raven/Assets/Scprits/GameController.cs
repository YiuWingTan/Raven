using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SmoothPathType
{
    None = 0,
    Quick,
    Best
}

public enum PathType
{
    DFS,
    BFS,
    DJ,
    ASTART
}

/// <summary>
/// 游戏控制器
/// </summary>
public class GameController : MonoBehaviour {

    [SerializeField]
    private Toggle timeSliced_Toggle;
    [SerializeField]
    private Dropdown smoothPath_DropDown;
    [SerializeField]
    private Dropdown searchType_DropDown;
    [SerializeField]
    private GameObject healthTrigger_Prefabe;
    [SerializeField]
    private GameObject mBulletTrigger_Prefabe;
    [SerializeField]
    private int mHealthTriggerNumber;
    [SerializeField]
    private int mBulletTriggerNumber;
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }
    private RaycastHit2D[] hits;
    private SearchType searchType = SearchType.AStar;
    [SerializeField]
    private GameObject textObject;
    [SerializeField]
    private LineRenderer render;
    /// <summary>
    /// 是否开启时间片
    /// </summary>
    private bool isTimeSliced = false;

    /// <summary>
    /// 游戏搜索方式
    /// </summary>
    public SearchType GameSearchType
    {
        get
        {
            return this.searchType;
        }

        set
        {
            this.searchType = value;
        }
    }

    public bool IsTimeSliced
    {
        get
        {
            return isTimeSliced;
        }
    }

    public SmoothPathType SmoothType
    {
        get
        {
            return smoothType;
        }

       
    }

    private SmoothPathType smoothType = SmoothPathType.None;

    private void Awake()
    {
        _instance = this;

        SetTimeSlicedIsOpen();
        SetSearchType();
        SetTimeSlicedIsOpen();
    }
    private void Start()
    {
        render = this.gameObject.GetComponent<LineRenderer>();
        int n = Mathf.Max(mBulletTriggerNumber,mHealthTriggerNumber);

        for(int i = 0;i<n;i++)
        {
            if(i<mBulletTriggerNumber)
            {
                GameObject.Instantiate(this.mBulletTrigger_Prefabe);
            }

            if(i<mHealthTriggerNumber)
            {
                GameObject.Instantiate(this.healthTrigger_Prefabe);
            }
        }
        GameObject.Instantiate(this.healthTrigger_Prefabe);
        if (Bullet.bulletGamePrefabe == null)
        {
            Bullet.bulletGamePrefabe = Resources.Load("Prefab/Bullet") as GameObject;

            if (Bullet.bulletGamePrefabe == null) Debug.LogError("加载失败");

            //Debug.LogError(Bullet.bulletGamePrefabe.name);
        }
        //GameObject.Instantiate(this.weaponTrigger_Prefabe);
    }
    
    private void FixedUpdate()
    {
        //检测玩家的鼠标输入
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 targetPos = new Vector3(pos.x, pos.y, 0);
            
            if((hits = Physics2D.LinecastAll(Camera.main.transform.position, targetPos, (1 << 10))).Length>0)
            {
                if((hits[hits.Length - 1].point - (Vector2)targetPos).sqrMagnitude<0.8f)
                {
                    int id = hits[hits.Length - 1].collider.GetComponent<RavenEntity>().ID;

                    MessageDispatcher.Instance.BroadCast(0, EntityMsgType.BeControl,0,id);
                }
            }
           
        }
    }

    /// <summary>
    /// 从lua文件读取lua文件
    /// </summary>
	private void ReadFromLUA()
    {
        //TODO lua
    }

    /// <summary>
    /// 将所需数据写入lua文件中
    /// </summary>
    private void WriteToLUA()
    {
        //TODO lua
    }

    /// <summary>
    /// 设置时间片是否开启
    /// </summary>
    public void SetTimeSlicedIsOpen()
    {
        if (timeSliced_Toggle.isOn)
            isTimeSliced = true;
        else
            isTimeSliced = false;
    }

    /// <summary>
    /// 设置路径平滑是否开启
    /// </summary>
    public void SetSmoothPath()
    {
        if(this.smoothPath_DropDown.value == 0)
        {
            this.smoothType = SmoothPathType.None;
        }else if(this.smoothPath_DropDown.value == 1)
        {
            this.smoothType = SmoothPathType.Quick;
        }else if(this.smoothPath_DropDown.value == 2)
        {
            this.smoothType = SmoothPathType.Best;
        }
    }

    /// <summary>
    /// 设置搜索的模式
    /// </summary>
    public void SetSearchType()
    {
        if(searchType_DropDown.value == 0)
        {
            this.searchType = SearchType.AStar;
        }else if(searchType_DropDown.value == 1)
        {
            this.searchType = SearchType.DFS;
        }else if(searchType_DropDown.value == 2)
        {
            this.searchType = SearchType.BFS;
        }else if(searchType_DropDown.value == 3)
        {
            this.searchType = SearchType.DJ;
        }
    }


}
