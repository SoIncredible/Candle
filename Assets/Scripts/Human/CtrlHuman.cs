using Network;
using UnityEngine;

namespace Human
{
    public class CtrlHuman : BaseHuman
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider != null && hit.collider.CompareTag("Terrain"))
                {
                    MoveTo(hit.point);
                    var sendStr = "Move|";
                    sendStr += NetManager.GetDesc() + ",";
                    sendStr += hit.point.x + ",";
                    sendStr += hit.point.y + ",";
                    sendStr += hit.point.z + ",";
                    NetManager.Send(sendStr);
                }
            }

            if (Input.GetMouseButtonUp(1)) // 按下鼠标右键
            {
                if (isAttacking) return; // 正在攻击, 不能反复攻击
                if(isMoving) return; // 移动过程中不能攻击
                
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out var hit);
                
                transform.LookAt(hit.point);
                Attack();
                
                var sendStr = "Attack|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += transform.eulerAngles.x + ",";
                NetManager.Send(sendStr);
                
                var lineEnd = transform.position + 0.5f * Vector3.up;
                var lineStart = lineEnd + 20 * transform.forward;
                if (Physics.Linecast(lineStart, lineEnd, out hit))
                {
                    var hitObj = hit.collider.gameObject;
                    if (hitObj == gameObject)
                    {
                        return;
                    }
                    
                    var h = hitObj.GetComponent<SyncHuman>();
                    if (h == null)
                    {
                        return;
                    }

                    sendStr = "Hit|";
                    sendStr += NetManager.GetDesc() + ","; // 攻击者信息
                    sendStr += h.desc + ","; // 被攻击者信息
                    NetManager.Send(sendStr);
                }
                
            }
        }
    }
}