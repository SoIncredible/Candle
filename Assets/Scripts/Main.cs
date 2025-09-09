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
    
    private IEnumerator Start()
    {
        // TODO Eddie 增加连接机制
        // TODO Eddie 增加心跳检测机制
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Attack", OnAttack);
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

        yield return WaitForSecondsMgr.Instance.GetWaitForSeconds(0.5f);
        
        // TODO Eddie 粘包问题解决
        // 尝试做一下分帧 目前存在粘包问题
        // 发送List协议 请求玩家列表
        NetManager.Send("List|");
    }

    private void Update()
    {
        NetManager.Update();
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
        
    }
}