using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Rendering
{
    public class TextureManager
    {
        Dictionary<string, Texture> m_textureCache = new Dictionary<string, Texture>();

        public Texture GetTexture(string name)
        {
            if (m_textureCache.TryGetValue(name, out var texture))
                return texture;
            return null;
        }

        public Texture GetOrCreateTexture(string name, string textureFilePath)
        {
            if (m_textureCache.TryGetValue(name, out var texture))
                return texture;
            var newTexture = new Texture(textureFilePath);
            m_textureCache.Add(name, newTexture);
            return newTexture;
        }

        public void CreateTexture(string name, string textureFilePath)
        {
            m_textureCache[name] = new Texture(textureFilePath);
        }
    }
}
