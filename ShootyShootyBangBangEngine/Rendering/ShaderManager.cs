using System.Collections.Generic;

namespace ShootyShootyBangBangEngine.Rendering
{
    public class ShaderManager
    {
        Dictionary<string, Shader> m_shaderCache = new Dictionary<string, Shader>();

        public void Init()
        {
            CreateShader("DefaultShader", "Shaders/shader.vert", "Shaders/shader.frag");
        }

        public Shader GetDefaultShader() { return GetShader("DefaultShader"); }

        public Shader GetShader(string name)
        {
            if (m_shaderCache.TryGetValue(name, out var shader))
                return shader;
            return null;
        }

        public Shader GetOrCreateShader(string name, string vertexShaderFilePath, string fragmentShaderFilePath)
        {
            if (m_shaderCache.TryGetValue(name, out var shader))
                return shader;
            var newShader = new Shader(vertexShaderFilePath, fragmentShaderFilePath);
            m_shaderCache.Add(name, newShader);
            return newShader;
        }

        public void CreateShader(string name, string vertexShaderFilePath, string fragmentShaderFilePath)
        {
            m_shaderCache[name] = new Shader(vertexShaderFilePath, fragmentShaderFilePath);
        }

        public void OnUnload()
        {
            foreach (var shaderKv in m_shaderCache)
                shaderKv.Value.Dispose();
            m_shaderCache.Clear();
        }
    }
}
