using UnityEngine.Rendering;
using UnityEngine;

#if HAS_URP
using UnityEngine.Rendering.Universal;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

#if HAS_URP
namespace ImGuiNET.Unity
{
    public class RenderImGuiFeature : ScriptableRendererFeature
    {
        class ExecuteCommandBufferPass : ScriptableRenderPass
        {
            public CommandBuffer cmd;

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
#if UNITY_EDITOR
                var sceneView = SceneView.currentDrawingSceneView;
                if (sceneView != null) return;
#endif
                ref CameraData cameraData = ref renderingData.cameraData;
                Matrix4x4 viewMatrix = cameraData.GetViewMatrix();
                Matrix4x4 projectionMatrix = cameraData.GetProjectionMatrix();

                cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
                context.ExecuteCommandBuffer(cmd);

            }
        }

        ExecuteCommandBufferPass _executeCommandBufferPass;

        public CommandBuffer commandBuffer;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        public override void Create()
        {
            _executeCommandBufferPass = new ExecuteCommandBufferPass()
            {
                cmd = commandBuffer,
                renderPassEvent = renderPassEvent,
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (commandBuffer == null) return;
            _executeCommandBufferPass.renderPassEvent = renderPassEvent;
            _executeCommandBufferPass.cmd = commandBuffer;
            renderer.EnqueuePass(_executeCommandBufferPass);
        }
    }
}
#else
namespace ImGuiNET.Unity
{
    public class RenderImGuiFeature : UnityEngine.ScriptableObject
    {
        public CommandBuffer commandBuffer;
    }
}
#endif
