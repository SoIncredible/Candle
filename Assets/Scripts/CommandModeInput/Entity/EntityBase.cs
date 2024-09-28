using New.EntityComponent;
using UnityEngine;

namespace CommandModeInput.Entity
{
    public abstract class EntityBase : MonoBehaviour
    {
        public abstract void OnCreate(EntityComponentBase[] componentBases);

        public abstract void OnVanish();

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }
    }
}