using System.Collections;
using FiniteStateMachine.Core;

namespace FiniteStateMachine.PlayerFSM.PlayerState
{
    public class CrouchParam : IBaseLocalStateParam
    {
    }

    public class CrouchState : IBaseState
    {
        private readonly PlayerFiniteStateMachine _machine;

        public CrouchState(PlayerFiniteStateMachine machine)
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