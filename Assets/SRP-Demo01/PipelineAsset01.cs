using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Demo01
{
    public class PipelineAsset01 : RenderPipelineAsset
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("SRP-Demo/01 - Create Asset Pipeline")]
        static void CreateBasicAssetPipeline()
        {
            //生成ScriptableObject
            var instance = ScriptableObject.CreateInstance<PipelineAsset01>();
            //将ScriptableObject保存为文件
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/PipelineAsset01.asset");
        }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            //应该是运行时被调用生成SRP
            return new BasicPipeline01();
        }
    }
}
