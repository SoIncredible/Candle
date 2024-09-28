using CommandModeInput.Entity;

namespace New.EntityComponent
{
    public abstract class EntityComponentBase
    {
        protected readonly EntityBase EntityBase;
        
        protected EntityComponentBase(EntityBase entityBase)
        {
            EntityBase = entityBase;
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public abstract void Receive();
    }
}