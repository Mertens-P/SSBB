using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Rendering
{
    public class LayeredRenderPipeline : RenderPipelineBase
    {
        struct Layer
        {
            public SortedDictionary<int, HashSet<Renderable>> Objects;
        }
        SortedDictionary<int, Layer> m_layers = new SortedDictionary<int, Layer>();

        private Layer i_addLayer(int depth)
        {
            if (m_layers.ContainsKey(depth))
                throw new Exception("Adding a layer at the same depth as existing layer!!");
            var layer = new Layer() { Objects = new SortedDictionary<int, HashSet<Renderable>>() };
            m_layers.Add(depth, layer);
            return layer;
        }

        protected void i_addRenderable(Renderable obj, int layerDepth, int depthInLayer)
        {
            Layer layer;
            if (!m_layers.TryGetValue(layerDepth, out layer))
                layer = i_addLayer(layerDepth);
            if (layer.Objects.TryGetValue(depthInLayer, out var objects))
                objects.Add(obj);
            else
                layer.Objects.Add(depthInLayer, new HashSet<Renderable>() { obj });
        }

        protected void i_removeRenderable(Renderable obj, int layerDepth)
        {
            if (m_layers.TryGetValue(layerDepth, out var layer))
            {
                foreach(var depth in layer.Objects)
                {
                    if (depth.Value.Remove(obj))
                        return;
                }
            }
        }

        protected  void i_removeRenderable(Renderable obj, int layerDepth, int depthInLayer)
        {
            if (m_layers.TryGetValue(layerDepth, out var layer))
            {
                if (layer.Objects.TryGetValue(depthInLayer, out var objects))
                    objects.Remove(obj);
            }
        }

        public override void OnUpdate(RenderControllers controllers, double dt)
        {
            var camera = controllers.GetCamera();
            foreach (var layerKv in m_layers)
            {
                foreach (var depthKv in layerKv.Value.Objects)
                {
                    foreach (var obj in depthKv.Value)
                        obj.OnUpdate(controllers, dt);
                }
            }
        }

        public override void OnRender(RenderControllers controllers, SSBBE.RenderSettings renderSettings)
        {
            var camera = controllers.GetCamera();
            foreach(var layerKv in m_layers)
            {
                foreach(var depthKv in layerKv.Value.Objects)
                {
                    foreach (var obj in depthKv.Value)
                        obj.OnRender(renderSettings, camera);
                }
            }
        }
    }
}
