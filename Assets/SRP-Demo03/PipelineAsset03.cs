using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Demo03
{
    public class PipelineAsset03 : RenderPipelineAsset
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("SRP-Demo/03 - Create Asset Pipeline")]
        static void CreateBasicAssetPipeline()
        {
            //生成ScriptableObject
            var instance = ScriptableObject.CreateInstance<PipelineAsset03>();
            //将ScriptableObject保存为文件
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/PipelineAsset03.asset");
        }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            //应该是运行时被调用生成SRP
            return new BasicPipeline03();
        }
    }
}
