using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Demo02
{
    public class BasicPipeline02 : RenderPipeline
    {
        CommandBuffer _cb;

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
                context.ExecuteCommandBuffer(_cb);
                _cb.Clear();

                //绘制天空盒子，注意需要在ClearRenderTarget之后进行，不然颜色会被覆盖。
                context.DrawSkybox(camera);

                // 裁剪（Culling）
                var param = new ScriptableCullingParameters();
                CullResults.GetCullingParameters(camera, out param);    // 取出相机的裁剪信息
                param.isOrthographic = false;
                var culled = CullResults.Cull(ref param, context);

                // 过滤（Filtering）
                var fs = new FilterRenderersSettings(true);
                //设置只绘制不透明物体。
                fs.renderQueueRange = RenderQueueRange.opaque;
                //设置绘制所有层
                fs.layerMask = ~0;

                // 绘制设置（Renderer Settings）
                //注意在构造的时候就需要传入Lightmode参数
                var rs = new DrawRendererSettings(camera, new ShaderPassName("Unlit"));
                //由于绘制不透明物体可以借助Z-Buffer，因此不需要额外的排序。
                rs.sorting.flags = SortFlags.None;

                context.DrawRenderers(culled.visibleRenderers, ref rs, fs);

                context.Submit();                    // submit

                //开始执行管线
                context.Submit();
            }
        }
    }
}