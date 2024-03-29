﻿using ShootyShootyBangBangEngine.Rendering;

namespace ShootyShootyBangBang.Implementations
{
    class SSBBRenderPipeline : LayeredRenderPipeline
    {
        public enum LayerIdentifiers
        {
            LI_Background,
            LI_Characters
        }

        public void AddRenderable(Renderable obj, LayerIdentifiers layerId, int depthInLayer)
        {
            i_addRenderable(obj, (int)layerId, depthInLayer);
        }

        public void RemoveRenderable(Renderable obj, LayerIdentifiers layerId)
        {
            i_removeRenderable(obj, (int)layerId);
        }

        public void RemoveRenderable(Renderable obj, LayerIdentifiers layerId, int depthInLayer)
        {
            i_removeRenderable(obj, (int)layerId);
        }
    }
}
