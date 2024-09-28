using System.Collections;

namespace FiniteStateMachine.Core
{
    /// <summary>
    /// 所有的状态都必须继承该接口
    /// </summary>
    public interface IBaseState
    {
        void OnEnter(IBaseLocalStateParam param);

        void OnExit();

        // TODO 接入UniTask后就不使用协程了
        IEnumerator TickState();
    }
}