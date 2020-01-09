using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.GameObjects.Cameras
{
    public class FollowCamera: Camera
    {
        Guid m_followObj;
        Vector2? m_topLeftExtend;
        Vector2? m_botRightExtend;

        public FollowCamera(Guid followObjId): base(new Vector2()) { m_followObj = followObjId; }

        public void SetExtends(Vector2? topLeft, Vector2? BotRight)
        {
            m_topLeftExtend = topLeft;
            m_botRightExtend = BotRight;
        }

        public override void OnUpdate(double dt, BaseControllers controllers)
        {
            base.OnUpdate(dt, controllers);
            var followObj = controllers.GetRootScene().GetGameObject(m_followObj);
            if (followObj != null)
            {
                var tarTransformComp = followObj.GetComponents().GetComponent<Components.ComponentTransform>();
                if (tarTransformComp != null)
                {
                    var tarPos = tarTransformComp.GetPosition();
                    if (m_topLeftExtend != null)
                    {
                        tarPos.X = Math.Max(tarPos.X, m_topLeftExtend.Value.X);
                        tarPos.Y = Math.Max(tarPos.Y, m_topLeftExtend.Value.Y);
                    }
                    if (m_botRightExtend != null)
                    {
                        tarPos.X = Math.Min(tarPos.X, m_botRightExtend.Value.X);
                        tarPos.Y = Math.Min(tarPos.Y, m_botRightExtend.Value.Y);
                    }
                    var transformComp = GetComponents().GetComponent<Components.ComponentTransform>();
                    if(transformComp != null)
                        transformComp.SetPosition(tarPos);
                }
            }
        }
    }
}
