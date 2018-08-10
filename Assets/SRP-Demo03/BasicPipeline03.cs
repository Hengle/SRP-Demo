using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Demo03
{
    public class BasicPipeline03 : RenderPipeline
    {
        CommandBuffer _cb;
        //这个向量用于保存平行光方向。
        Vector3 _LightDir;

        //这个函数在管线被销毁的时候调用。
        public override void Dispose()
        {
            base.Dispose();
            if (_cb != null)
            {
                _cb.Release();
                _cb = null;
            }
        }

        //这个函数在需要绘制管线的时候调用。
        public override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            base.Render(context, cameras);
            if (_cb == null) _cb = new CommandBuffer();// new CommandBuffer
            //准备好光源名称
            var _LightDir = Shader.PropertyToID("_LightDir");
            var _LightColor = Shader.PropertyToID("_LightColor");
            var _CameraPos = Shader.PropertyToID("_CameraPos");


            //对于每一个相机执行操作。
            foreach (var camera in cameras)
            {
                //将上下文设置为当前相机的上下文。
                context.SetupCameraProperties(camera);
                _cb.name = "Setup";
                //显式将当前渲染目标设置为相机Backbuffer。
                _cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                //设置渲染目标的颜色为相机背景色。
                _cb.ClearRenderTarget(true, true, camera.backgroundColor);

                //设置相机的着色器全局变量_CameraPos
                Vector4 cameraPosition = new Vector4(camera.transform.localPosition.x, camera.transform.localPosition.y, camera.transform.localPosition.z, 1.0f);
                _cb.SetGlobalVector(_CameraPos, camera.transform.localToWorldMatrix * cameraPosition);
                context.ExecuteCommandBuffer(_cb);
                _cb.Clear();

                //绘制天空盒子，注意需要在ClearRenderTarget之后进行，不然颜色会被覆盖。
                context.DrawSkybox(camera);

                // 裁剪（Culling）
                var param = new ScriptableCullingParameters();
                CullResults.GetCullingParameters(camera, out param);    // 取出相机的裁剪信息
                param.isOrthographic = false;
                var culled = CullResults.Cull(ref param, context);

                //获取所有的灯光 All pass
                var lights = culled.visibleLights;
                _cb.name = "RenderLights";
                foreach (var light in lights)
                {
                    //我们只处理平行光
                    if (light.lightType != LightType.Directional) continue;
                    //获取光源方向
                    Vector4 pos = light.localToWorld.GetColumn(2);
                    Vector4 lightDirection = new Vector4(-pos.x, -pos.y, -pos.z, 0);
                    //获取光源颜色
                    Color LightColor = light.finalColor;
                    //构建shader常量缓存
                    _cb.SetGlobalVector(_LightDir, lightDirection);
                    _cb.SetGlobalColor(_LightColor, LightColor);
                    context.ExecuteCommandBuffer(_cb);
                    _cb.Clear();

                    // 过滤（Filtering）
                    var fs = new FilterRenderersSettings(true);
                    //设置只绘制不透明物体。
                    fs.renderQueueRange = RenderQueueRange.opaque;
                    //设置绘制所有层
                    fs.layerMask = ~0;

                    // 绘制设置（Renderer Settings）
                    //使用Shader中指定光照模式为BaseLit的pass
                    var drs = new DrawRendererSettings(camera, new ShaderPassName("BaseLit"));
                    //绘制物体
                    context.DrawRenderers(culled.visibleRenderers, ref drs, fs);

                    break;
                }
                //开始执行管线
                context.Submit();
            }
        }
    }
}

