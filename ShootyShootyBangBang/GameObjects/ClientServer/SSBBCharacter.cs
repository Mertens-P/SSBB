using OpenTK;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.GameObjects.ClientServer
{
    class SSBBCharacter : GameObject
    {
        protected float m_movementSpeed = 1000.0f;

        public SSBBCharacter(Vector2 position, ComponentReplicator.PeerType peerType)
        {
            GetComponents().AddComponent(new ComponentTransform(position));
            GetComponents().AddComponentAsType(new Components.CharacterReplicator(peerType), typeof(ComponentReplicator));
        }
    }
}
