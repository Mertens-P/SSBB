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
            public string LayerName;
            public SortedDictionary<int, HashSet<Renderable>> Objects;
        }
        SortedDictionary<int, Layer> m_layers = new SortedDictionary<int, Layer>();
        Dictionary<string, int> m_layerNameLookup = new Dictionary<string, int>();

        public void AddLayer(string name, int depth)
        {
            if (m_layers.ContainsKey(depth))
                throw new Exception("Adding a layer at the same depth as existing layer!!");
            if(m_layerNameLookup.ContainsKey(name))
                throw new Exception($"Layer {name} allready exists!!");
            m_layers.Add(depth, new Layer() { LayerName = name, Objects = new SortedDictionary<int, HashSet<Renderable>>() });
            m_layerNameLookup.Add(name, depth);
        }

        public void AddRenderable(Renderable obj, string layerName, int depthInLayer)
        {
            if (m_layerNameLookup.TryGetValue(layerName, out int layerDepth))
            {
                if (m_layers.TryGetValue(layerDepth, out var layer))
                {
                    if (layer.Objects.TryGetValue(depthInLayer, out var objects))
                        objects.Add(obj);
                    else
                        layer.Objects.Add(depthInLayer, new HashSet<Renderable>() { obj });
                }
                else
                    throw new Exception($"Layer at depth {layerDepth} does not exists!!");
            }
            else
                throw new Exception($"Layer {layerName} does not exists!!");
        }

        public void RemoveRenderable(Renderable obj, string layerName)
        {
            if (m_layerNameLookup.TryGetValue(layerName, out int layerDepth))
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
        }

        public void RemoveRenderable(Renderable obj, string layerName, int depthInLayer)
        {
            if (m_layerNameLookup.TryGetValue(layerName, out int layerDepth))
            {
                if (m_layers.TryGetValue(layerDepth, out var layer))
                {
                    if (layer.Objects.TryGetValue(depthInLayer, out var objects))
                        objects.Remove(obj);
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
