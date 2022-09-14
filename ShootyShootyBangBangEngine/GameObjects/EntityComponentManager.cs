using ShootyShootyBangBangEngine.Helpers.Threading;
using System;
using System.Collections.Generic;
using ShootyShootyBangBangEngine.Helpers;

namespace ShootyShootyBangBangEngine.GameObjects
{
    public class EntityComponentManager
    {
        [Serializable]
        public class EntityComponentBase
        {
            public virtual void Init() { }
            public virtual void Tick(Controllers.BaseControllers controllers, GameObject owner, double dt) { }
            public virtual void CleanUp() { }

            public override string ToString()
            {
                return GetType().Name;
            }
        }

        private SortedDictionary<int, EntityComponentBase> m_componentsSorted = new SortedDictionary<int, EntityComponentBase>();
        [NonSerialized] private readonly ReaderWriterLockObject m_componentsSortedLock = new ReaderWriterLockObject();
        private List<EntityComponentBase> m_componentsAddQueue = new List<EntityComponentBase>();
        [NonSerialized] private readonly ReaderWriterLockObject m_componentsAddQueueLock = new ReaderWriterLockObject();
        private List<int> m_componentsRemoveQueue = new List<int>();
        [NonSerialized] private readonly ReaderWriterLockObject m_componentsRemoveQueueLock = new ReaderWriterLockObject();

        private IEnumerable<EntityComponentBase> Components { get { return m_componentsSorted.Values; } }

        public void Update(Controllers.BaseControllers controllers, GameObject ownerEnt, double dt)
        {
            using (UpgradeableReadLock.CreateLock(m_componentsAddQueueLock))
            {
                foreach (var comp in m_componentsAddQueue)
                    AddComponent(comp);

                using (WriteLock.CreateLock(m_componentsAddQueueLock))
                {
                    m_componentsAddQueue.Clear();
                }
            }

            using (WriteLock.CreateLock(m_componentsRemoveQueueLock))
            {
                foreach (var comp in m_componentsRemoveQueue)
                    m_componentsSorted.Remove(comp);

                m_componentsRemoveQueue.Clear();
            }

            using (ReadLock.CreateLock(m_componentsSortedLock))
            {
                foreach (var component in m_componentsSorted)
                    component.Value.Tick(controllers, ownerEnt, dt);
            }
        }

        public bool HasComponent(Type type)
        {
            return m_componentsSorted.ContainsKey(type.FullName.GetHashCode());
        }

        public bool HasComponent<T>() where T : EntityComponentBase
        {
            return m_componentsSorted.ContainsKey(typeof(T).FullName.GetHashCode());
        }

        public bool HasComponentOrIsComponentQueued(Type type)
        {
            if (m_componentsSorted.ContainsKey(type.FullName.GetHashCode()))
                return true;

            foreach (var comp in m_componentsAddQueue)
            {
                if (comp.GetType() == type)
                    return true;
            }

            return false;
        }

        public EntityComponentBase GetComponent(Type type)
        {
            return m_componentsSorted.GetValueOrDefault(type.FullName.GetHashCode());
        }

        public EntityComponentBase GetBaseComponent(Type type)
        {
            EntityComponentBase componentOut = null;
            foreach (var comp in m_componentsSorted)
            {
                if (comp.Value.GetType().IsSubclassOf(type))
                    componentOut = comp.Value;
            }
            return componentOut;
        }

        public T GetComponent<T>() where T : EntityComponentBase
        {
            return (T)m_componentsSorted.GetValueOrDefault(typeof(T).FullName.GetHashCode());
        }

        public T GetComponentAsType<T, U>()
            where T : EntityComponentBase, U
            where U : EntityComponentBase
        {
            return (T)m_componentsSorted.GetValueOrDefault(typeof(U).FullName.GetHashCode());
        }

        public IEnumerable<EntityComponentBase> GetComponents()
        {
            foreach (var comp in m_componentsSorted)
                yield return comp.Value;
        }

        public void AddComponent(EntityComponentBase component)
        {
            using (UpgradeableReadLock.CreateLock(m_componentsSortedLock))
            {
                if (m_componentsSorted.ContainsKey(component.GetType().FullName.GetHashCode()))
                    return;

                using (WriteLock.CreateLock(m_componentsSortedLock))
                {
                    m_componentsSorted[component.GetType().FullName.GetHashCode()] = component;
                }
            }
        }

        public void QueueAddComponent(EntityComponentBase component)
        {
            using (WriteLock.CreateLock(m_componentsAddQueueLock))
            {
                m_componentsAddQueue.Add(component);
            }
        }

        public void AddComponentAsType(EntityComponentBase component, Type type)
        {
            using (UpgradeableReadLock.CreateLock(m_componentsSortedLock))
            {
                if (m_componentsSorted.ContainsKey(type.FullName.GetHashCode()))
                    return;

                using (WriteLock.CreateLock(m_componentsSortedLock))
                {
                    m_componentsSorted[type.FullName.GetHashCode()] = component;
                }
            }
        }

        public void RemoveComponent(Type type)
        {
            using (WriteLock.CreateLock(m_componentsSortedLock))
            {
                m_componentsSorted.Remove(type.FullName.GetHashCode());
            }
        }

        public void RemoveComponent<T>() where T : EntityComponentBase
        {
            using (WriteLock.CreateLock(m_componentsSortedLock))
            {
                m_componentsSorted.Remove(typeof(T).FullName.GetHashCode());
            }
        }

        public void QueueRemoveComponent<T>() where T : EntityComponentBase
        {
            using (WriteLock.CreateLock(m_componentsRemoveQueueLock))
            {
                m_componentsRemoveQueue.Add(typeof(T).FullName.GetHashCode());
            }
        }
    }
}
