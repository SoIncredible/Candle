using System;
using System.Collections.Generic;
using CommandModeInput;
using CommandModeInput.Entity;
using CommandModeInput.InputModule;
using UnityEngine;

namespace New.EntityComponent
{
    public sealed class PlayerInputComponent : EntityComponentBase
    {
        private readonly CharacterInputModuleBase _inputModule;

        private readonly Dictionary<CommandType, Action> _commandActionDic;
        
        public PlayerInputComponent(CommandModeInput.InputMapping.InputMapping inputMapping, EntityBase entityBase) : base(entityBase)
        {
            // CommandActionDic的创建和InputModule初始化的顺序是严格的 
            _commandActionDic = new Dictionary<CommandType, Action>()
            {
                { CommandType.MoveForward, MovementForwardMark},
                { CommandType.MoveBackward, MovementBackwardMark},
                { CommandType.MoveLeft, MovementLeftMark},
                { CommandType.MoveRight, MovementRightMark},
                { CommandType.Jump , JumpMark},
                { CommandType.Attack, AttackMark},
                { CommandType.Crunch, CrunchMark}
            };
            _inputModule = new PlayerInputModule(inputMapping, this);
            _inputModule.InitInputModule();
        }
        
        public override void OnUpdate()
        {
            _inputModule.GatherInput();
            
            var tempMoveDir = Vector2.zero;
            
            if (_isMoveForward)
            {
                tempMoveDir.y += 1;
            }

            if (_isMoveBackward)
            {
                tempMoveDir.y -= 1;
            }

            if (_isMoveLeft)
            {
                tempMoveDir.x -= 1;
            }

            if (_isMoveRight)
            {
                tempMoveDir.x += 1;
            }
            
            tempMoveDir.Normalize();
            ((PlayerEntity)EntityBase)._rawInputMovement = new Vector3(tempMoveDir.x, 0, tempMoveDir.y);
           
            ((PlayerEntity)EntityBase)._smoothInputMovement = Vector3.Lerp(
                ((PlayerEntity)EntityBase)._smoothInputMovement, ((PlayerEntity)EntityBase)._rawInputMovement,
                Time.deltaTime * ((PlayerEntity)EntityBase).movementSmoothingSpeed);
            
            ((PlayerEntity)EntityBase).moveDir = ((PlayerEntity)EntityBase)._smoothInputMovement;

            _isMoveForward = false;
            _isMoveBackward = false;
            _isMoveLeft = false;
            _isMoveRight = false;
        }

        public override void Receive()
        {
        }


        private bool _isMoveForward;

        /// <summary>
        /// 这个方法只是给这个Entity标记一下 说明输入了一个向前走的指令
        /// 标记了一个待向前走的状态
        /// 调用这个方法不会真的让Entity向前走
        /// </summary>
        private void MovementForwardMark()
        {
            _isMoveForward = true;
            // Debug.Log("向前走");
        }

        private bool _isMoveBackward;
        /// <summary>
        /// 这个方法只是给这个Entity标记一下 说明输入了一个向前走的指令
        /// 标记了一个待向前走的状态
        /// 调用这个方法不会真的让Entity向前走
        /// </summary>
        private void MovementBackwardMark()
        {
            _isMoveBackward = true;
            // Debug.Log("向后走");
        }

        private bool _isMoveLeft;

        /// <summary>
        /// 这个方法只是给这个Entity标记一下 说明输入了一个向前走的指令
        /// 标记了一个待向前走的状态
        /// 调用这个方法不会真的让Entity向前走
        /// </summary>
        private void MovementLeftMark()
        {
            _isMoveLeft = true;
            // Debug.Log("向左走");
        }

        private bool _isMoveRight;

        /// <summary>
        /// 这个方法只是给这个Entity标记一下 说明输入了一个向前走的指令
        /// 标记了一个待向前走的状态
        /// 调用这个方法不会真的让Entity向前走
        /// </summary>
        private void MovementRightMark()
        {
            _isMoveRight = true;
            // Debug.Log("向右走");
        }

        private bool _isJump;

        private void JumpMark()
        {
            _isJump = true;
        }

        private void CrunchMark()
        {
            
        }

        private void AttackMark()
        {
            
        }
        
        public Action GetCommandMarkAction(CommandType type)
        {
            if(_commandActionDic.TryGetValue(type, out var action))
            {
                return action;
            }
            
            Debug.LogError($"Can not find Command Type {type} correspond action");
            return null;
        }
    }
}