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
                }
            }
        }
    }
}