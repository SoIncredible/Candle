using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Human;
using Network;
using UnityEngine;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    public GameObject humanPrefab;
    
    [SerializeField] private BaseHuman myHuman;
    
    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();
    
    private void Start()
    {
        // TODO Eddie 增加连接机制
        // TODO Eddie 增加心跳检测机制
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Attack", OnAttack);
        NetManager.AddListener("Hit", OnHit);
        NetManager.AddListener("Die", OnDie);
        NetManager.AddListener("Dialog", OnDialog);
        
        NetManager.Connect("127.0.0.1", 8888);
        
        var obj = Instantiate(humanPrefab);
        var x = Random.Range(-5, 5);
        var z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();
        
        var pos = myHuman.transform.position;
        var eul = myHuman.transform.eulerAngles;
        
        // 发送Enter协议
        var sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + "," + pos.y + "," + pos.z + ",";
        sendStr += eul.y;
        NetManager.Send(sendStr);

        // yield return WaitForSecondsMgr.Instance.GetWaitForSeconds(0.5f);
        // 发送List协议 请求玩家列表
        NetManager.Send("List|");
    }

    private void Update()
    {
        NetManager.Update();
    }

    private void OnDestroy()
    {
        NetManager.Destroy();
    }

    private void OnEnter(string msg)
    {
        Debug.Log("OnEnter" + msg);
        var split = msg.Split(',');
        var desc = split[0];
        
        // 如果是自己
        if (desc == NetManager.GetDesc())
        {
            return;
        }
        
        var x = float.Parse(split[1]);
        var y = float.Parse(split[2]);
        var z = float.Parse(split[3]);
        var eulY = float.Parse(split[4]);
        
        var obj = Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, eulY, 0);
        var human = obj.AddComponent<SyncHuman>();
        human.desc = desc;
        otherHumans.Add(desc, human);
    }

    private void OnList(string msg)
    {
        var split = msg.Split(',');
        var count = (split.Length - 1) / 6;
        for (var i = 0; i < count; i++)
        {
            var desc = split[i * 6];
            var x  = float.Parse(split[i * 6 + 1]);
            var y = float.Parse(split[i * 6 + 2]);
            var z = float.Parse(split[i * 6 + 3]);
            var eulY = float.Parse(split[i * 6 + 4]);
            var hp = int.Parse(split[i * 6 + 5]);

            // 是自己
            if (desc == NetManager.GetDesc())
            {
                continue;
            }
            
            var obj = Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            var h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }
    
    private void OnMove(string msg)
    {
        var split = msg.Split(',');
        var desc = split[0];
        var x = float.Parse(split[1]);
        var y = float.Parse(split[2]);
        var z = float.Parse(split[3]);

        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        
        var human = otherHumans[desc];
        var target = new Vector3(x, y, z);
        human.MoveTo(target);
    }

    private void OnLeave(string msg)
    {
        var split = msg.Split(',');
        var desc = split[0];

        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        
        var human = otherHumans[desc];
        Destroy(human.gameObject);
        otherHumans.Remove(desc);
    }

    private void OnAttack(string msg)
    {
        var split = msg.Split(',');
        var desc = split[0];
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        
        var eulY = float.Parse(split[1]);
        var human = otherHumans[desc] as SyncHuman;
        human.SyncAttack(eulY);
    }

    private void OnHit(string msg)
    {
        var split = msg.Split(',');
        var attackDesc = split[0]; // 攻击者
        var hitDesc = split[1]; // 被攻击者
        
        // 找到被攻击的人 扣他血
        if (hitDesc == NetManager.GetDesc()) // 如果是自己被扣血
        {
            // 血量HP就没在前端暴露 但是可以在这里做一个受击的动画 
            Debug.Log("Your Character is under attack");
        }
        
        // 如果是其他角色受击了 那么客户端这里要更新一下其他角色的血量显示.
    }

    private void OnDie(string msg)
    {
        // 删除掉死亡的玩家
        var split = msg.Split(',');
        var dieDesc = split[0]; // 死亡角色
        if (dieDesc == NetManager.GetDesc())
        {
            Debug.Log("You Die Game Over");
            return;
        }

        if (!otherHumans.ContainsKey(dieDesc))
        {
            return;
        }

        var h = otherHumans[dieDesc];
        h.gameObject.SetActive(false);
    }

    /// <summary>
    /// 聊天协议
    /// </summary>
    /// <param name="msg"></param>
    private void OnDialog(string msg)
    {
        
    }
}