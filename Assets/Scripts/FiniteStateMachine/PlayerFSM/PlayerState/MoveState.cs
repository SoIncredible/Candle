using System.Collections;
using FiniteStateMachine.Core;

namespace FiniteStateMachine.PlayerFSM.PlayerState
{
    public class MoveParam : IBaseLocalStateParam
    {
    }

    public class MoveState : IBaseState
    {
        private readonly PlayerFiniteStateMachine _machine;

        public MoveState(PlayerFiniteStateMachine machine)
        {
            _machine = machine;
        }

        public void OnEnter(IBaseLocalStateParam param)
        {
            _machine.StartCoroutine(TickState());
        }

        public void OnExit()
        {
            _machine.StopCoroutine(TickState());
        }

        public IEnumerator TickState()
        {
            yield break;
        }
    }
}