using FiniteStateMachine.Core;
using FiniteStateMachine.PlayerFSM.PlayerState;
using UnityEngine;

namespace FiniteStateMachine.PlayerFSM
{
    /// <summary>
    /// 控制玩家状态的状态机
    /// </summary>
    public class PlayerFiniteStateMachine : BaseFiniteStateMachine
    {
        protected override void Init()
        {
            base.Init();
            StateTypeDic.Add(StateType.Idle, new IdleState(this));
            StateTypeDic.Add(StateType.Move, new MoveState(this));
            StateTypeDic.Add(StateType.Jump, new JumpState(this));
            StateTypeDic.Add(StateType.Crouch, new CrouchState(this));
            StateTypeDic.Add(StateType.Attack, new AttackState(this));

            // 初始化状态
            StateTypeDic.TryGetValue(StateType.Idle, out CurState);

            Debug.Log("111");
            Debug.Log("222");
        }
    }
}