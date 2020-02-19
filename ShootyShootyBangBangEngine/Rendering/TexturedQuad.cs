using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Rendering
{
    public class TexturedQuad : Renderable
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

        Vector2 m_position;
        Vector2 m_dimensions;
        Vector2 m_offset;
        Vector2 m_uvScale = new Vector2(1, 1);
        Vector2 m_uvOffset = new Vector2();

        static int s_vertexStride = 5;
        
        public void SetPosition(Vector2 position)           {   m_position = position; }
        public void SetDimensions(Vector2 dimensions)       {   m_dimensions = dimensions;  }
        public void SetRepeating(float textureRepeatScale)  { m_uvScale = new Vector2(textureRepeatScale, textureRepeatScale);  }
        public void SetUvScale(Vector2 uvScale)             { m_uvScale = uvScale; }
        public void SetUvOffset(Vector2 uvOffset)           { m_uvOffset = uvOffset; }

        public TexturedQuad(Vector2 dimensions, Texture texture, Shader shader)
        {
            i_init(dimensions, new Vector2(), texture, shader);
        }

        public TexturedQuad(Vector2 dimensions, Vector2 offset, Texture texture, Shader shader)
        {
            i_init(dimensions, offset, texture, shader);
        }

        void i_init(Vector2 dimensions, Vector2 offset, Texture texture, Shader shader)
        {
            m_texture = texture;
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
            m_offset = offset;
        }
        
        public override void OnRender(SSBBE.RenderSettings renderSettings, GameObjects.Cameras.Camera camera)
        {
            m_texture.Use();
            m_shader.Use();
            i_updateVertices(m_position, renderSettings, camera);

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

        void i_updateVertices(Vector2 position, SSBBE.RenderSettings renderSettings, GameObjects.Cameras.Camera camera)
        {
            Vector2 cameraPos = new Vector2();
            if (camera != null)
                cameraPos = camera.GetComponents().GetComponent<GameObjects.Components.ComponentTransform>().GetPosition();

            position += m_offset;
            var halfDim = m_dimensions * 0.5f;
            m_vertices[i_getVertexStartId(0) + 0] = (position.X + halfDim.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(0) + 1] = (position.Y + halfDim.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(0) + 3] = m_uvOffset.X + m_uvScale.X;
            m_vertices[i_getVertexStartId(0) + 4] = m_uvOffset.Y + m_uvScale.Y;

            m_vertices[i_getVertexStartId(1) + 0] = (position.X + halfDim.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(1) + 1] = (position.Y - halfDim.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(1) + 3] = m_uvOffset.X + m_uvScale.X;
            m_vertices[i_getVertexStartId(1) + 4] = m_uvOffset.Y;

            m_vertices[i_getVertexStartId(2) + 0] = (position.X - halfDim.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(2) + 1] = (position.Y - halfDim.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(2) + 3] = m_uvOffset.X;
            m_vertices[i_getVertexStartId(2) + 4] = m_uvOffset.Y;

            m_vertices[i_getVertexStartId(3) + 0] = (position.X - halfDim.X - cameraPos.X) * renderSettings.InvWidth;
            m_vertices[i_getVertexStartId(3) + 1] = (position.Y + halfDim.Y - cameraPos.Y) * renderSettings.InvHeight;
            m_vertices[i_getVertexStartId(3) + 3] = m_uvOffset.X;
            m_vertices[i_getVertexStartId(3) + 4] = m_uvOffset.Y + m_uvScale.Y;
        }
    }
}
