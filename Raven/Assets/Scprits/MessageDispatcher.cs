using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct  Msg
{
    private MsgKey key;

    public MsgKey Key
    {
        get
        {
            return key;
        }
    }

    public int SenderID
    {
        get
        {
            return key.SenderID;
        }

    }

    public int ReceiverID
    {
        get
        {
            return key.ReceiverID;
        }
    }

    //TODO 操作码的编写
    public int Op
    {
        get
        {
            return key.Op;
        }
    }

    public float DelayTime
    {
        get
        {
            return delayTime;
        }

      
    }

    public object Value
    {
        get
        {
            return value;
        }

        
    }

    /// <summary>
    /// 消息延时时间
    /// </summary>
    private float delayTime;

    /// <summary>
    /// 消息传递的值
    /// </summary>
    private object value;

    public Msg(int sender,int receiver,int op,float delayTime,object value = null)
    {

        key = new MsgKey(sender, receiver, op);

        this.delayTime = delayTime;

        this.value = value;
    }

    public float UpdateDelayTime(float detalTime)
    {
        delayTime -= detalTime;

        return delayTime;
    }
}

/// <summary>
/// 逻辑同等性
/// </summary>
public struct MsgKey
{
    /// <summary>
    /// 发送者
    /// </summary>
    private int senderID;
    /// <summary>
    /// 接收者
    /// </summary>
    private int receiverID;

    /// <summary>
    /// 消息类型(操作类型)
    /// </summary>
    private int op;

    public int SenderID
    {
        get
        {
            return senderID;
        }

       
    }

    public int ReceiverID
    {
        get
        {
            return receiverID;
        }
    }

    public int Op
    {
        get
        {
            return op;
        }
    }

    public MsgKey(int senderID,int reciverID,int op)
    {
        this.senderID = senderID;

        this.receiverID = reciverID;

        this.op = op;
    }

    public override bool Equals(object obj)
    {
        if(obj is MsgKey)
        {
            var msg = (MsgKey)obj;

            if (msg.SenderID == this.SenderID && msg.ReceiverID == this.ReceiverID && msg.Op == this.Op)
                return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return SenderID.GetHashCode() + ReceiverID.GetHashCode() + Op.GetHashCode();
    }
}


/// <summary>
/// 消息发送控制器
/// </summary>
public class MessageDispatcher : MonoBehaviour {

    private static MessageDispatcher _instance;

    public static MessageDispatcher Instance
    {
        get
        {
            return _instance;
        }
    }

    private Dictionary<MsgKey, Msg> msgContainer;

    private List<MsgKey> deletedKey;//要进行移除的消息

    private void Awake()
    {
        _instance = this;
        msgContainer = new Dictionary<MsgKey, Msg>();
        deletedKey = new List<MsgKey>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        bool isClean = false;

        foreach(var msg in msgContainer.Values)
        {
            if(msg.UpdateDelayTime(Time.deltaTime)<0)
            {
                //进行消息的发送

                var receiver =  EntityManager.Instance.GetEntityByID(msg.ReceiverID);

                //如果消息传递失败
                if(!receiver.OnMessage(msg))
                {
                    var sender = EntityManager.Instance.GetEntityByID(msg.SenderID);

                    if(sender!=null)
                    sender.OnMessage(msg);
                }

                deletedKey.Add(msg.Key);
            }
        }

        foreach(var key in deletedKey)
        {
            if(msgContainer.ContainsKey(key))
            {
                msgContainer.Remove(key);

                isClean = true;
            }
        }

        if (isClean) deletedKey.Clear();
    }

    public void AddMsg(int senderID,int receiverID,int op,float delayTime,object value)
    {
        Msg msg = new Msg(senderID,receiverID,op,delayTime,value);

        msgContainer.Add(msg.Key,msg);
    }

    /// <summary>
    /// 将消息传递给所有的AI
    /// </summary>
    /// <param name="id"></param>
    /// <param name="msgType"></param>
    public void BroadCast(int id, EntityMsgType msgType,float delayTime = 0,object value = null)
    {
        foreach (var e in EntityManager.Instance.EntityContainer.Values)
        {
            if (e.ID != id)
            {
                //判断是不是一个正确的发送对象
                Msg msg = new Msg(id, e.ID, (int)msgType, delayTime,value);
                //发送一个消息
                e.OnMessage(msg);
            }
        }
    }
}
