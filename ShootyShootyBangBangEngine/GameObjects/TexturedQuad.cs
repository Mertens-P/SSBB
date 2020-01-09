using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.GameObjects
{
    public class TexturedQuad
    {
        int m_vertexBufferObjectHandle;
        int m_vertexArrayObjectHandle;
        float[] m_vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };
        uint[] m_indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };
        Shader m_shader;
        Texture m_texture;
        
        Vector2 m_dimensions;
        float m_textureRepeatScale = 1;

        static int s_vertexStride = 5;
        
        public void SetDimensions(Vector2 dimensions)       {   m_dimensions = dimensions;  }
        public void SetRepeating(float textureRepeatScale)  {   m_textureRepeatScale = textureRepeatScale;  }

        public TexturedQuad(Vector2 pos, Vector2 dimensions, string textureFilePath, Shader shader)
        {
            m_texture = new Texture(textureFilePath);
            m_vertexBufferObjectHandle = GL.GenBuffer();
            m_shader = shader;
            m_vertexArrayObjectHandle = GL.GenVertexArray();
            // ..:: Initialization code (done once (unless your object frequently changes)) :: ..
            // 1. bind Vertex Array Object
            GL.BindVertexArray(m_vertexArrayObjectHandle);
            // 2. copy our vertices array in a buffer for OpenGL to use
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBufferObjectHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vertices.Length * sizeof(float), m_vertices, BufferUsageHint.DynamicDraw);
            // 3. then set our vertex attributes pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(0);

            var ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, m_indices.Length * sizeof(uint), m_indices, BufferUsageHint.StaticDraw);
            
            m_dimensions = dimensions;
        }

        public void OnRender(Vector2 position, SSBBE.RenderSettings renderSettings, Cameras.Camera camera)
        {
            m_texture.Use();
            m_shader.Use();
            i_updateVertices(position, renderSettings, camera);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            GL.BindVertexArray(m_vertexArrayObjectHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBufferObjectHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vertices.Length * sizeof(float), m_vertices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(BeginMode.Triangles, m_indices.Length, DrawElementsType.UnsignedInt, 0);
            
            GL.Disable(EnableCap.Blend);
        }

        public void OnDelete()
        {
            GL.DeleteBuffer(m_vertexBufferObjectHandle);
        }

        int i_getVertexStartId(int vertexId)
        {
            return vertexId * s_vertexStride;
        }

        void i_updateVertices(Vector2 position, SSBBE.RenderSettings renderSettings, Cameras.Camera camera)
        {
            Vector2 cameraPos = new Vector2();
            if (camera != null)
                cameraPos = camera.GetComponents().GetComponent<Components.ComponentTransform>().GetPosition();

            var halfDim = m_dimensions * 0.5f;
            m_vertices[i_getVertexStartId(0) + 0] = (position.X + m_dimensions.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(0) + 1] = (position.Y + m_dimensions.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(0) + 3] = m_textureRepeatScale;
            m_vertices[i_getVertexStartId(0) + 4] = m_textureRepeatScale;

            m_vertices[i_getVertexStartId(1) + 0] = (position.X + m_dimensions.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(1) + 1] = (position.Y - m_dimensions.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(1) + 3] = m_textureRepeatScale;

            m_vertices[i_getVertexStartId(2) + 0] = (position.X - m_dimensions.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(2) + 1] = (position.Y - m_dimensions.Y - cameraPos.Y) * renderSettings.InvHeight;

            m_vertices[i_getVertexStartId(3) + 0] = (position.X - m_dimensions.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(3) + 1] = (position.Y + m_dimensions.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(3) + 4] = m_textureRepeatScale;
        }
    }
}
