using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace Editor.AssetBundle
{
    public class BundleBuilder
    {
        public static void BuildPackage(string packageName)
        {
            // 创建构建参数
            var buildParameters = new ScriptableBuildParameters
            {
                BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(),
                BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot(),
                BuildPipeline = nameof(EBuildPipeline.ScriptableBuildPipeline),
                BuildBundleType = (int)EBuildBundleType.AssetBundle,
                BuildTarget = EditorUserBuildSettings.activeBuildTarget,
                PackageName = packageName,
                PackageVersion = GetPackageVersion(),
                PackageNote = "",
                ClearBuildCacheFiles = true,
                UseAssetDependencyDB = true,
                EnableSharePackRule = true,
                SingleReferencedPackAlone = true,
                VerifyBuildingResult = true,
                FileNameStyle = EFileNameStyle.BundleName_HashName,
                BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyByTags,
                BuildinFileCopyParams = AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(packageName, nameof(EBuildPipeline.ScriptableBuildPipeline)),
                EncryptionServices = null,
                ManifestProcessServices = null,
                ManifestRestoreServices = null,
                CompressOption = ECompressOption.LZ4,
                StripUnityVersion = false,
                DisableWriteTypeTree = false,
                IgnoreTypeTreeChanges = true,
                TrackSpriteAtlasDependencies = false,
                WriteLinkXML = true, // 生成代码防裁剪是干什么的?
                CacheServerHost = String.Empty, // 补充服务器地址
                CacheServerPort = 0, // 补充服务器端口
                BuiltinShadersBundleName = GetBuiltinShaderBundleName(packageName), // 内置着色器资源包名称
                MonoScriptsBundleName = string.Empty // Mono脚本资源包名称
            };

            var builder = new AssetBundleBuilder();
            var pipeline = GetBuildPipeline(EBuildPipeline.ScriptableBuildPipeline);
            var buildResult = builder.Run(buildParameters, pipeline, true);

            if (buildResult.Success)
            {
                Debug.Log("Build Complete");
                // TODO Eddie 可以在这里将bundle上传到服务器上
            }
            else
            {
                Debug.LogError("Build Bundle Failed");
            }
        }

        private static List<IBuildTask> GetBuildPipeline(EBuildPipeline pipelineMode)
        {
            // 编程构建管线
            if (pipelineMode == EBuildPipeline.ScriptableBuildPipeline)
            {
                var pipeline = new List<IBuildTask>()
                {
                    new TaskPrepare_SBP(), // 前期准备工作
                    new TaskGetBuildMap_SBP(), // 获取构建列表
                    new TaskBuilding_SBP(), // 开始执行构建
                    // new TaskCopRaw // 拷贝原生文件
                    new TaskVerifyBuildResult_SBP(), // 验证构建结果
                    new TaskEncryption_SBP(), // 加密资源文件
                    new TaskUpdateBundleInfo_SBP(), // 更新资源包信息
                    new TaskCreateManifest_SBP(), // 创建清单文件
                    new TaskCreateReport_SBP(), // 创建报告文件
                    new TaskCreatePackage_SBP(), // 制作包裹
                    new TaskCopyBuildinFiles_SBP(), // 拷贝内置文件
                };
                
                return pipeline;
            }

            throw new Exception("Unsupported pipeline mode");
        }

        private static string GetPackageVersion()
        {
            return "1.0.0";
        }
        
        /// <summary>
        /// 内置着色器资源包名称
        /// 注意：和自动收集的着色器资源包名保持一致！
        /// </summary>
        private static string GetBuiltinShaderBundleName(string packageName)
        {
            var uniqueBundleName = AssetBundleCollectorSettingData.Setting.UniqueBundleName;
            var packRuleResult = DefaultPackRule.CreateShadersPackRuleResult();
            return packRuleResult.GetBundleName(packageName, uniqueBundleName);
        }
    }
}