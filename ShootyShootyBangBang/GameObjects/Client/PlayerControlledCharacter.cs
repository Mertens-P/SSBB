using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.GameObjects.Client
{
    class PlayerControlledCharacter : ClientCharacter
    {
        public PlayerControlledCharacter(Guid id, RenderControllers controllers, Vector2 position, Vector2 dimensions, Texture texture, Shader shader)
            : base(controllers, position, dimensions, texture, shader)
        {
            SetId(id);
        }

        public override void OnUpdate(double dt, BaseControllers controllers)
        {
            var renderControllers = controllers as RenderControllers;
            Vector2 dir = new Vector2();
            if (renderControllers.GetInput().IsKeyDown(Key.A)) dir.X = -1;
            if (renderControllers.GetInput().IsKeyDown(Key.D)) dir.X = 1;
            if (renderControllers.GetInput().IsKeyDown(Key.W)) dir.Y = 1;
            if (renderControllers.GetInput().IsKeyDown(Key.S)) dir.Y = -1;
            if (dir.LengthSquared > 0)
            {
                dir.Normalize();
                var transform = GetComponents().GetComponent<ComponentTransform>();
                if (transform != null)
                    transform.SetPosition(transform.GetPosition() + dir * m_movementSpeed * (float)dt);
            }
            base.OnUpdate(dt, controllers);
        }
    }
}
