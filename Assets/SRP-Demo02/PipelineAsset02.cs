using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Demo02
{
    public class PipelineAsset02 : RenderPipelineAsset
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("SRP-Demo/02 - Create Asset Pipeline")]
        static void CreateBasicAssetPipeline()
        {
            //生成ScriptableObject
            var instance = ScriptableObject.CreateInstance<PipelineAsset02>();
            //将ScriptableObject保存为文件
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/PipelineAsset02.asset");
        }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            //应该是运行时被调用生成SRP
            return new BasicPipeline02();
        }
    }
}
