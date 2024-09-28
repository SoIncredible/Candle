using CommandModeInput.InputModule;
using New.EntityComponent;
using UnityEngine;

namespace CommandModeInput.Entity
{
    // 这只是个容器 容纳各种组件
    [RequireComponent(typeof(InputHandler))]
    public class PlayerEntity : CharacterEntityBase
    {
        // camera的父级组件
        public Transform cameraParentTrans
        {
            get
            {
                if (_cameraParentTransform == null)
                {
                    _cameraParentTransform = GameObject.Find("CameraParent").transform;
                }

                return _cameraParentTransform;
            }
        }
        
        private Transform _cameraParentTransform;

        // camera的本身
        public Transform cameraTransform;
        
        public float turnSpeed = 0.1f;
        public float movementSmoothingSpeed = 5f;
        
        public Vector3 _rawInputMovement;
        public Vector3 _smoothInputMovement;

        public Vector3 moveDir;
        
        private PlayerInputComponent _playerInputComponent;
        private AnimatorComponent _animatorComponent;
        private PhysicsComponent _physicsComponent;
        
        private Vector2 _movementInput;
        private Vector2 _cameraInput;
        
        public override void OnCreate(EntityComponentBase[] componentBases)
        {
            cameraTransform = GameObject.Find("Main Camera").transform;
         
            // TODO 用更好的方式获得inputComponent组件
            _playerInputComponent = componentBases[0] as PlayerInputComponent;
            _animatorComponent = componentBases[1] as AnimatorComponent;
            _physicsComponent = componentBases[2] as PhysicsComponent;
        }

        public override void OnVanish()
        {
           
        }

        public override void OnUpdate()
        {
            _playerInputComponent.OnUpdate();
            _animatorComponent.OnUpdate();
        }

        public override void OnFixedUpdate()
        {
            _physicsComponent.OnFixedUpdate();
        }
    }
}