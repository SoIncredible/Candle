using System;
using System.Collections.Generic;
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
        
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8888);
        
        var obj = Instantiate(humanPrefab);
        var x = Random.Range(-5, 5);
        var z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();
        
        var pos = myHuman.transform.position;
        var eul = myHuman.transform.eulerAngles;
        var sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + "," + pos.y + "," + pos.z + ",";
        sendStr += eul.y;
        NetManager.Send(sendStr);
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

    private void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg);
    }

    private void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg);
    }
}