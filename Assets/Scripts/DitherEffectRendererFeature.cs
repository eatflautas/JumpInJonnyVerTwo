using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class DitherEffectRendererFeature : ScriptableRendererFeature
{
    [SerializeField] DitherEffectRendererFeatureSettings settings;

    

    // Use this class to pass around settings from the feature to the pass
    [Serializable]
    public class DitherEffectRendererFeatureSettings
    {
        
    }

    class DitherEffectPass : ScriptableRenderPass
    {
        const string m_PassName = "DitherEffectPass";
        Material m_BlitMaterial;
        
        readonly DitherEffectRendererFeatureSettings settings;

        public DitherEffectPass(DitherEffectRendererFeatureSettings settings)
        {
            this.settings = settings;
        }

        public void Setup(Material mat)
        {
            m_BlitMaterial = mat;
            // we want to apply our effect directly to the final output
            requiresIntermediateTexture = true;
        }


        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<SphereVolumeComponent>();

            if (!customEffect.IsActive())
                return;
            
            //creates a temporary texture while we apply our effect material
            var resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                // we need an intermediate texture to work with.
                Debug.LogError($"Skipping render pass. ditherEffectRendererFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input");
                return;
            }

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            // we want the name of the destination texture to match the pass
            destinationDesc.name = $"CameraColor-{m_PassName}";
            // we don't want the buffer cleared, our goal is to modify
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, m_BlitMaterial, 0);
            renderGraph.AddBlitPass(para, passName: m_PassName);
            
            //swaps the camera color buffer with our new modified texture
            resourceData.cameraColor = destination;
            
        }
        
        
        
    }
    
    // sets our injection point
    public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    public Material material;
    
    DitherEffectPass m_ScriptablePass;
    
    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new DitherEffectPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = injectionPoint;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
        {
            Debug.LogWarning("DitherEffectRendererFeature material is null and will be skipped.");
            return;
        }

        m_ScriptablePass.Setup(material);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
