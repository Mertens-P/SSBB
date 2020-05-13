using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.GameObjects.ClientServer
{
    class SSBBCharacter : GameObject
    {
        protected float m_movementSpeed = 500.0f;

        public float GetMovementSpeed() { return m_movementSpeed; }

        public SSBBCharacter(Vector2 position, ComponentReplicator.PeerType peerType)
        {
            GetComponents().AddComponent(new ComponentTransform(position));
            GetComponents().AddComponentAsType(new Components.CharacterReplicator(peerType), typeof(ComponentReplicator));
        }
    }
}
