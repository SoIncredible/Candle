using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

internal class FsmClearCacheBundle : IStateNode
{
    private UniFramework.Machine.StateMachine _machine;

    void IStateNode.OnCreate(UniFramework.Machine.StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStepsChange.SendEventMessage("清理未使用的缓存文件！");
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        operation.Completed += Operation_Completed;
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private void Operation_Completed(YooAsset.AsyncOperationBase obj)
    {
        _machine.ChangeState<FsmStartGame>();
    }
}