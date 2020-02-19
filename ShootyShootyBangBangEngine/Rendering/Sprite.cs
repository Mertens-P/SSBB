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
    public class Sprite : TexturedQuad
    {
        struct AnimationStats
        {
            public int AnimationId;
            public int Frames;
        }

        Dictionary<string, AnimationStats> m_animations = new Dictionary<string, AnimationStats>();

        float m_maxFrames;
        float m_maxAnimations;
        string m_currentPlayingAnimation = null;
        bool m_looping = false;
        double m_currentFrame = 0;
        double m_animationSpeed = 1.0f;
        Vector2 m_relFrameSize;

        public Sprite(Vector2 dimensions, Texture texture, Shader shader, int maxAnimations, int maxFrames)
            : base(dimensions, texture, shader)
        {
            i_initSprite(dimensions, maxAnimations, maxFrames);
        }

        public Sprite(Vector2 dimensions, Vector2 offset, Texture texture, Shader shader, int maxAnimations, int maxFrames)
            : base(dimensions, offset, texture, shader)
        {
            i_initSprite(dimensions, maxAnimations, maxFrames);
        }

        void i_initSprite(Vector2 dimensions, float maxAnimations, float maxFrames)
        {
            m_maxAnimations = maxAnimations;
            m_maxFrames = maxFrames;
            SetUvScale(m_relFrameSize = new Vector2(1.0f / maxFrames, 1.0f / maxAnimations));
            i_setCurrentFrame(0);
        }

        public override void OnUpdate(RenderControllers controllers, double dt)
        {
            base.OnUpdate(controllers, dt);
            if(!string.IsNullOrEmpty(m_currentPlayingAnimation) && m_animations.TryGetValue(m_currentPlayingAnimation, out var animation))
            {
                i_setCurrentFrame(animation.AnimationId);
                if (m_looping)
                {
                    m_currentFrame = m_currentFrame + dt* m_animationSpeed;
                    if (m_currentFrame > m_maxFrames)
                        m_currentFrame -= m_maxFrames;
                }
                else
                    m_currentFrame = Math.Min(animation.Frames, m_currentFrame += dt* m_animationSpeed);
            }
        }

        public void AddAnimation(string name, int animationId, int frames)
        {
            m_animations.Add(name, new AnimationStats() { AnimationId = animationId, Frames = frames });
        }

        public void PlayAnimation(string animationName, double animationSpeed, bool looping)
        {
            m_currentPlayingAnimation = animationName;
            m_looping = looping;
            m_animationSpeed = animationSpeed;
        }

        void i_setCurrentFrame(int animationId)
        {
            var vec = new Vector2(
                Helpers.MathHelpers.Lerp(0, 1.0f, (int)m_currentFrame * m_relFrameSize.X),
                Helpers.MathHelpers.Lerp(1.0f - m_relFrameSize.Y, 0, (float)animationId * m_relFrameSize.Y));
            SetUvOffset(vec);
        }
    }
}
