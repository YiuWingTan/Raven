using PathSearchAlgorithm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class SearchFunctionSelect : MonoBehaviour {

    private Dropdown dropDown;

   
    private void Start()
    {
        dropDown = this.gameObject.GetComponent<Dropdown>();

        OnValueChange();
    }

    /// <summary>
    /// 当下拉列表改变时调用的方法
    /// </summary>
    public void OnValueChange()
    {
        if(dropDown.value == 0)
        {
            GameController.Instance.GameSearchType = SearchType.DFS;
            
        }
        else if(dropDown.value == 1)
        {
            GameController.Instance.GameSearchType = SearchType.BFS;

            
        }
        else if(dropDown.value == 2)
        {
            GameController.Instance.GameSearchType = SearchType.DJ;

            
        }else if(dropDown.value == 3)
        {
            GameController.Instance.GameSearchType = SearchType.AStar;

        }

    }
}
