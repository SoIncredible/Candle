using CommandModeInput.CharacterProperties;
using CommandModeInput.InputModule;
using CommandModeInput.Manager;

namespace CommandModeInput.Entity
{
    public abstract class CharacterEntityBase : EntityBase
    {
        private CharacterProperty _characterProperty;

        private CharacterInputModuleBase _inputModule;
        
        public PlayerType playerType;
        
        public float _speed;
        protected float _health;
        protected float _jumpForce;
        
        public virtual void Init(CharacterProperty property, PlayerType type)
        {
            _characterProperty = property;
            
            _speed = property.MoveSpeed;
            _health = property.Health;
            _jumpForce = property.JumpForce;

            playerType = type;
        }

        public virtual void Release()
        {
            _characterProperty = null;
        }

        public abstract override void OnUpdate();
    }
}