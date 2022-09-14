using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Numerics;
using System.Collections.Generic;

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

        enum AnimationPlayType
        {
            APT_Paused,
            APT_Playing,
            APT_Looping,
        }

        float m_maxFrames;
        float m_maxAnimations;
        string m_currentPlayingAnimation = null;
        AnimationPlayType m_animationPlayType = AnimationPlayType.APT_Paused;
        double m_currentFrame = 0;
        double m_animationSpeed = 1.0f;
        Vector2 m_relFrameSize;
        float m_yCorrection = 0;

        public string GetAnimationName() { return m_currentPlayingAnimation; }
        public int GetCurrentFrame() { return (int)m_currentFrame; }

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
            m_yCorrection = 1.0f / dimensions.Y;
            SetUvScale(m_relFrameSize = new Vector2(1.0f / maxFrames, (1.0f / maxAnimations)));
            i_setCurrentFrame(0);
        }

        public override void OnUpdate(RenderControllers controllers, double dt)
        {
            base.OnUpdate(controllers, dt);
            if(!string.IsNullOrEmpty(m_currentPlayingAnimation) && m_animations.TryGetValue(m_currentPlayingAnimation, out var animation))
            {
                i_setCurrentFrame(animation.AnimationId);
                switch (m_animationPlayType)
                {
                    case AnimationPlayType.APT_Looping:
                        m_currentFrame = m_currentFrame + dt * m_animationSpeed;
                        if (m_currentFrame > m_maxFrames)
                            m_currentFrame -= m_maxFrames;
                        break;
                    case AnimationPlayType.APT_Playing:
                        m_currentFrame = Math.Min(animation.Frames, m_currentFrame += dt * m_animationSpeed);
                        break;
                }
            }
        }

        public void AddAnimation(string name, int animationId, int frames)
        {
            m_animations.Add(name, new AnimationStats() { AnimationId = animationId, Frames = frames });
        }

        public void PlayAnimation(string animationName, double animationSpeed, bool looping)
        {
            m_currentPlayingAnimation = animationName;
            m_animationPlayType = looping ? AnimationPlayType.APT_Looping : AnimationPlayType.APT_Playing;
            m_animationSpeed = animationSpeed;
        }

        public void SetAnimation(string animationName, int frame = 0)
        {
            m_currentPlayingAnimation = animationName;
            m_animationPlayType = AnimationPlayType.APT_Paused;
            if(m_animations.TryGetValue(m_currentPlayingAnimation, out var animation))
                m_currentFrame = Math.Min(animation.Frames, frame);
        }

        void i_setCurrentFrame(int animationId)
        {
            var yFac = 0.0f;
            if (m_maxAnimations > 1)
                yFac = ((float)animationId) / (m_maxAnimations - 1);
            var vec = new Vector2(
                Helpers.MathHelpers.Lerp(0, 1.0f, (int)m_currentFrame * m_relFrameSize.X),
                Helpers.MathHelpers.Lerp(1.0f - m_relFrameSize.Y, 0, yFac)- m_yCorrection);
            SetUvOffset(vec);
        }
    }
}
