using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine.Core
{
    /// <summary>
    /// 状态机基类
    /// </summary>
    public abstract class BaseFiniteStateMachine : MonoBehaviour
    {
        // 当前状态
        protected IBaseState CurState;

        // 状态字典
        protected Dictionary<StateType, IBaseState> StateTypeDic;

        // 状态机全局参数
        private Dictionary<GlobalParamType, IBaseGlobalStateParam> _globalParamDic;

        protected virtual void Init()
        {
            StateTypeDic = new Dictionary<StateType, IBaseState>();

            _globalParamDic = new Dictionary<GlobalParamType, IBaseGlobalStateParam>();
        }

        protected virtual void Release()
        {
            CurState = default;

            StateTypeDic.Clear();
            StateTypeDic = null;

            _globalParamDic.Clear();
            _globalParamDic = null;
        }

        protected virtual void TransitionState(IBaseState newState)
        {
            if (StateTypeDic.ContainsValue(CurState))
            {
                CurState.OnExit();
            }

            CurState = newState;
            CurState.OnEnter(default);
        }

        protected virtual void TransitionState(IBaseState newState, IBaseLocalStateParam param)
        {
            if (StateTypeDic.ContainsValue(CurState))
            {
                CurState.OnExit();
            }

            CurState = newState;
            CurState.OnEnter(param);
        }


        //--------------------------------------------------------------------------------
        // 状态机全局参数操作相关
        //--------------------------------------------------------------------------------

        protected virtual void SetGlobalParam(GlobalParamType globalParamType, IBaseGlobalStateParam param)
        {
            if (_globalParamDic.TryGetValue(globalParamType, out var value))
            {
                if (value == param)
                {
                    return;
                }

                _globalParamDic[globalParamType] = param;
                Debug.LogWarning(
                    $"[BaseFiniteStateMachine] the {globalParamType}'s value in GlobalParamDic has been changed! before:{value} now:{param}");
            }
            else
            {
                _globalParamDic.Add(globalParamType, param);
            }
        }

        protected virtual IBaseGlobalStateParam GetGlobalStateParam(GlobalParamType globalParamType)
        {
            if (_globalParamDic.TryGetValue(globalParamType, out var param))
            {
                return param;
            }
            else
            {
                Debug.LogError("[BaseFiniteStateMachine] ");
                return default;
            }
        }
    }
}