using UnityEngine;

namespace Human
{
    public class SyncHuman : BaseHuman
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        public void SyncAttack(float eulY)
        {
            transform.eulerAngles = new Vector3(0, eulY, 0);
            Attack();
        }
    }
}