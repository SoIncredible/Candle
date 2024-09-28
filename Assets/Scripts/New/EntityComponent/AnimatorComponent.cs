using CommandModeInput.Entity;
using UnityEngine;

namespace New.EntityComponent
{
    public class AnimatorComponent : EntityComponentBase
    {
        private Animator _anim;
        private int _vertical;
        
        public AnimatorComponent(Animator animator, EntityBase entityBase) : base(entityBase)
        {
            _anim = animator;
            _vertical = Animator.StringToHash("Movement");
        }
        
        public override void OnUpdate()
        {
            _anim.SetFloat(_vertical, ((PlayerEntity)EntityBase)._smoothInputMovement.magnitude);
        }

        public override void OnFixedUpdate()
        {
         
        }

        public override void Receive()
        {
           
        }

   
    }
}