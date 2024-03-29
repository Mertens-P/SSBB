﻿using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;

namespace ShootyShootyBangBangEngine.GameObjects
{
    public class Scene
    {
        Dictionary<Guid, GameObject> m_addQueue = new Dictionary<Guid, GameObject>();
        Dictionary<Guid, GameObject> m_gameObjects = new Dictionary<Guid, GameObject>();

        public void AddGameObject(GameObject gameObject)
        {
            m_addQueue[gameObject.GetId()] = gameObject;
        }

        public GameObject GetGameObject(Guid gameObject)
        {
            if (m_gameObjects.TryGetValue(gameObject, out var obj))
                return obj;
            if (m_addQueue.TryGetValue(gameObject, out var queuedObj))
                return queuedObj;
            return null;
        }

        public IEnumerable<GameObject> GetGameObjects()
        {
            foreach (var kv in m_gameObjects)
                yield return kv.Value;
            foreach (var kv in m_addQueue)
                yield return kv.Value;
        }

        public virtual void OnUpdate(double dt, BaseControllers controllers)
        {
            foreach (var gameObjectKv in m_gameObjects)
                gameObjectKv.Value.Update(dt, controllers);
            foreach (var kv in m_addQueue)
                m_gameObjects[kv.Value.GetId()] = kv.Value;
        }

        public void OnDelete()
        {
            foreach (var gameObjectKv in m_gameObjects)
                gameObjectKv.Value.OnDelete();
            m_gameObjects.Clear();
        }
    }
}
