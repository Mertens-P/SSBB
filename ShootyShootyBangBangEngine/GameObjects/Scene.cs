using OpenTK.Input;
using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.GameObjects
{
    public class Scene
    {
        Dictionary<Guid, GameObject> m_gameObjects = new Dictionary<Guid, GameObject>();

        public void AddGameObject(GameObject gameObject)
        {
            m_gameObjects[gameObject.GetId()] = gameObject;
        }

        public GameObject GetGameObject(Guid gameObject)
        {
            if (m_gameObjects.TryGetValue(gameObject, out var obj))
                return obj;
            return null;
        }

        public void OnRender(RenderControllers controllers, SSBBE.RenderSettings renderSettings)
        {
            foreach (var gameObjectKv in m_gameObjects)
                gameObjectKv.Value.OnRender(controllers, renderSettings);
        }

        public virtual void OnUpdate(double dt, BaseControllers controllers)
        {
            foreach (var gameObjectKv in m_gameObjects)
                gameObjectKv.Value.OnUpdate(dt, controllers);
        }

        public void OnDelete()
        {
            foreach (var gameObjectKv in m_gameObjects)
                gameObjectKv.Value.OnDelete();
            m_gameObjects.Clear();
        }
    }
}
