using CommandModeInput;
using CommandModeInput.Entity;
using CommandModeInput.InputMapping;
using CommandModeInput.InputModule;
using New.EntityComponent;
using UnityEditor;
using UnityEngine;

namespace New
{
    [RequireComponent(typeof(PlayerEntity))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterDefine characterDefine;
        [SerializeField] private AnimatorComponent _animatorComponent;
        [SerializeField] private PlayerInputComponent _playerInputComponent;
        [SerializeField] private PhysicsComponent _physicsComponent;
        
        private InputMapping _playerMapping;
        
        private void Awake()
        {
            if (characterDefine == null)
            {
                characterDefine = AssetDatabase.LoadAssetAtPath<CharacterDefine>("Assets/AssetBundle/Config/Player/CharacterDefine.asset");
            }

            var animator = GetComponent<Animator>();
            var playerEntity = GetComponent<PlayerEntity>();
            var rigidBody = GetComponent<Rigidbody>();
            var inputMapping = characterDefine.PlayerInputMapping;
            
            _animatorComponent = new AnimatorComponent(animator, playerEntity);
            _playerInputComponent = new PlayerInputComponent(inputMapping, playerEntity);
            _physicsComponent = new PhysicsComponent(rigidBody, playerEntity);
        }

        private void Update()
        {
            _animatorComponent.OnUpdate();
            _playerInputComponent.OnUpdate();
            _physicsComponent.OnUpdate();
        }

        private void FixedUpdate()
        {
            _animatorComponent.OnFixedUpdate();
            _playerInputComponent.OnFixedUpdate();
            _physicsComponent.OnFixedUpdate();
        }
    }
}