using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Cell<T>
{
    public static int activeID = 0;

    private List<DebugLine> debugLines;

    private Vector2 pos;

    private List<T> objects;

    private int id;

    public Vector2 Pos
    {
        get
        {
            return pos;
        }
    }

    public Cell(int id,float x,float y)
    {
        this.id = id;

        pos = new Vector2(x,y);

        objects = new List<T>();

        debugLines = new List<DebugLine>();
    }

    public void AddEntity(T o)
    {
        objects.Add(o);
    }

    public List<T> GetContainer()
    {
        return objects;
    }
}

/// <summary>
/// 单元空间分割(每个空间内只包含路点)
/// </summary>
public class CellSpacePartition<T>  {

    private float worldW;//游戏世界的宽度

    private float worldH;//游戏世界的高度

    private float cellW;//单元的宽度

    private float cellH;//单元的高度

    private int cellNumX;//x轴上有几个单元

    private int cellNumY;//y轴上有几个单元 

    private Vector2 offset;

    private Cell<T>[,] cells;

    public CellSpacePartition(float ww,float wh,int xn,int yn)
    {
        cells = new Cell<T>[xn,yn];

        this.worldW = ww;

        this.worldH = wh;

        Debug.Log("世界的宽度是"+worldW);
        Debug.Log("世界的高度是"+worldH);

        this.offset = new Vector2(-worldW/2,-worldH/2);

        this.cellNumX = xn;

        this.cellNumY = yn;

        this.cellW = worldW / xn;

        this.cellH = worldH / yn;

        for(int i = 0;i<xn;i++)
        {
            for(int j = 0;j<yn;j++)
            {
                float x = i * cellW+offset.x;

                float y = j * cellH + offset.y;

                cells[i, j] = new Cell<T>(Cell<T>.activeID++,x,y);
            }
        }
    }

    public void AddEntity(T entity,Vector2 pos,int id = 0)
    {
        Vector2 reflection = pos - offset;

        int i = (int)(reflection.x / cellW);

        int j = (int)(reflection.y / cellH);


        //Debug.Log("要加入到"+i+","+j+"单元"+",点的编号是"+id);

        if (i < 0 || i >= cellNumX) return;

        if (j < 0 || j >= cellNumY) return; 

        cells[i, j].AddEntity(entity);
    }

    public List<T> GetNeighbors(Vector2 pos)
    {
        Vector2 reflection = pos - offset;

        int i = (int)(reflection.x / cellW);

        int j = (int)(reflection.y / cellH);

        //Debug.Log("要进行单元格获取的是"+i+","+j);

        if (i < 0 || i >= cellNumX) return null;

        if (j < 0 || j >= cellNumY) return null;

        return cells[i,j].GetContainer();
    }

   

    /// <summary>
    /// 可视化方法
    /// </summary>
    public void ToDebug(GameObject line)
    {
        //TODO 进行单元格的可视化

        for(int i = 0;i<cellNumX;i++)
        {
            for(int j = 0;j<cellNumY;j++)
            {
                Vector3 upLeft = new Vector3(cells[i, j].Pos.x, cells[i, j].Pos.y+cellH, -6);
                Vector3 upRight = new Vector3(cells[i,j].Pos.x+cellW,cells[i,j].Pos.y+cellH,-6);

                Vector3 downLeft = new Vector3(cells[i, j].Pos.x, cells[i, j].Pos.y, -6);
                Vector3 downRight= new Vector3(cells[i, j].Pos.x+cellW, cells[i, j].Pos.y, -6);

                GameObject go = GameObject.Instantiate(line,new Vector3(0,0,0),Quaternion.identity);
                go.name = "up";

                DebugLine up = go.AddComponent(typeof(DebugLine)) as DebugLine;

                up.SetDebugLine(upLeft,upRight,go.GetComponent<LineRenderer>());

                go = GameObject.Instantiate(line,new Vector3(0,0,0),Quaternion.identity);
                go.name = "down";
                DebugLine down = go.AddComponent(typeof(DebugLine)) as DebugLine;

                down.SetDebugLine(downLeft,downRight, go.GetComponent<LineRenderer>());

                go = GameObject.Instantiate(line,new Vector3(),Quaternion.identity);
                go.name = "left";
                DebugLine left = go.AddComponent(typeof(DebugLine)) as DebugLine;

                left.SetDebugLine(upLeft,downLeft, go.GetComponent<LineRenderer>());

                go = GameObject.Instantiate(line,new Vector3(),Quaternion.identity);
                go.name = "right";
                DebugLine right = go.AddComponent(typeof(DebugLine)) as DebugLine;

                right.SetDebugLine(upRight,downRight,go.GetComponent<LineRenderer>());

                //Debug.Log(""+i+","+j+"  "+cells[i,j].GetContainer().Count);
            }
        }
    }

}
