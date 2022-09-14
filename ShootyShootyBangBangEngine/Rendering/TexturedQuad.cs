using OpenTK.Graphics.OpenGL;
using System;
using System.Numerics;

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
        Vector2 m_scale = new Vector2(1, 1);
        Vector2 m_offset;
        float m_angleInRad;
        Vector2 m_uvScale = new Vector2(1, 1);
        Vector2 m_uvOffset = new Vector2();
        bool m_visible = true;

        static int s_vertexStride = 5;
        
        public void SetPosition(Vector2 position)           {   m_position = position; }
        public void SetDimensions(Vector2 dimensions)       {   m_dimensions = dimensions;  }
        public void SetScale(Vector2 scale)                 {   m_scale = scale; }
        public Vector2 GetScale()                           {   return m_scale; }
        public Vector2 GetOffset()                          {   return m_offset; }
        public void SetAngle(float angleInRad)              {   m_angleInRad = angleInRad; }
        public void SetRepeating(float textureRepeatScale)  {   m_uvScale = new Vector2(textureRepeatScale, textureRepeatScale);  }
        public void SetUvScale(Vector2 uvScale)             {   m_uvScale = uvScale; }
        public void SetUvOffset(Vector2 uvOffset)           {   m_uvOffset = uvOffset; }
        public void SetVisible(bool visible)                {   m_visible = visible; }
        public bool GetVisible()                            {   return m_visible; }

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
            if (!m_visible) return;
            m_texture.Use();
            m_shader.Use();
            i_updateVertices(m_position, m_angleInRad, renderSettings, camera);

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

        void i_updateVertices(Vector2 position, float angleInRad, SSBBE.RenderSettings renderSettings, GameObjects.Cameras.Camera camera)
        {
            Vector2 cameraPos = new Vector2();
            if (camera != null)
                cameraPos = camera.GetComponents().GetComponent<GameObjects.Components.ComponentTransform>().GetPosition();

            var halfDim = m_dimensions * 0.5f;
            var cosAngle = (float)Math.Cos(angleInRad);
            var sinAngle = (float)Math.Sin(angleInRad);
            var locX = new Vector2(cosAngle, sinAngle);
            var locY = new Vector2(-sinAngle, cosAngle);
            var topRight = locX * (halfDim.X - m_offset.X) + locY * (halfDim.Y - m_offset.Y);
            var botRight = locX *( halfDim.X  - m_offset.X) + locY * (-halfDim.Y - m_offset.Y);
            var topLeft = locX * (-halfDim.X  - m_offset.X) + locY * (halfDim.Y - m_offset.Y);
            var botLeft = locX * (-halfDim.X - m_offset.X) + locY * (-halfDim.Y - m_offset.Y);
            var zoomFac = 1.0f;
            if(camera != null)
                zoomFac = camera.GetZoomFactor();
            m_vertices[i_getVertexStartId(0) + 0] = (position.X + topRight.X * m_scale.X - cameraPos.X) * (renderSettings.InvWidth * zoomFac);
            m_vertices[i_getVertexStartId(0) + 1] = (position.Y + topRight.Y * m_scale.Y - cameraPos.Y) * (renderSettings.InvHeight * zoomFac);
            m_vertices[i_getVertexStartId(0) + 3] = m_uvOffset.X + m_uvScale.X;
            m_vertices[i_getVertexStartId(0) + 4] = m_uvOffset.Y + m_uvScale.Y;

            m_vertices[i_getVertexStartId(1) + 0] = (position.X + botRight.X * m_scale.X - cameraPos.X) * (renderSettings.InvWidth * zoomFac);
            m_vertices[i_getVertexStartId(1) + 1] = (position.Y + botRight.Y * m_scale.Y - cameraPos.Y) * (renderSettings.InvHeight * zoomFac);
            m_vertices[i_getVertexStartId(1) + 3] = m_uvOffset.X + m_uvScale.X;
            m_vertices[i_getVertexStartId(1) + 4] = m_uvOffset.Y;

            m_vertices[i_getVertexStartId(2) + 0] = (position.X + botLeft.X * m_scale.X - cameraPos.X) * (renderSettings.InvWidth * zoomFac);
            m_vertices[i_getVertexStartId(2) + 1] = (position.Y + botLeft.Y * m_scale.Y - cameraPos.Y) * (renderSettings.InvHeight * zoomFac);
            m_vertices[i_getVertexStartId(2) + 3] = m_uvOffset.X;
            m_vertices[i_getVertexStartId(2) + 4] = m_uvOffset.Y;

            m_vertices[i_getVertexStartId(3) + 0] = (position.X + topLeft.X * m_scale.X - cameraPos.X) * (renderSettings.InvWidth * zoomFac);
            m_vertices[i_getVertexStartId(3) + 1] = (position.Y + topLeft.Y * m_scale.Y - cameraPos.Y) * (renderSettings.InvHeight * zoomFac);
            m_vertices[i_getVertexStartId(3) + 3] = m_uvOffset.X;
            m_vertices[i_getVertexStartId(3) + 4] = m_uvOffset.Y + m_uvScale.Y;
        }
    }
}
