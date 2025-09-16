using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;

internal class FsmDownloadPackageOver : IStateNode
{
    private UniFramework.Machine.StateMachine _machine;

    void IStateNode.OnCreate(UniFramework.Machine.StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStepsChange.SendEventMessage("资源文件下载完毕！");
        _machine.ChangeState<FsmClearCacheBundle>();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}