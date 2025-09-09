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
        }
    }
}