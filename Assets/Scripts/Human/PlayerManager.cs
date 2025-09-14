using System.Collections.Generic;
using Network;
using Singletons;
using UnityEngine;

namespace Human
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private GameObject humanPrefab;
        private Dictionary<string, SyncHuman> otherHumans;

        protected override void Init()
        {
            base.Init();
            RegisterListener();
        }

        public override void Destroy()
        {
            
        }
        

        private void RegisterListener()
        {
            NetManager.AddListener("Enter", OnEnter);
            NetManager.AddListener("Move", OnMove);
            NetManager.AddListener("Leave", OnLeave);
            NetManager.AddListener("List", OnList);
            NetManager.AddListener("Attack", OnAttack);
            NetManager.AddListener("Hit", OnHit);
            NetManager.AddListener("Die", OnDie);
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
            
            var obj = Object.Instantiate(humanPrefab);
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
                
                var obj = Object.Instantiate(humanPrefab);
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
            Object.Destroy(human.gameObject);
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
    }
}