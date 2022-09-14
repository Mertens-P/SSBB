using ShootyShootyBangBangEngine.Controllers;
using System;

namespace ShootyShootyBangBangEngine.GameObjects
{
    public class GameObject
    {
        EntityComponentManager m_components = new EntityComponentManager();
        Guid m_id;

        public Guid GetId() { return m_id; }
        protected void SetId(Guid id) { m_id = id; }
        public EntityComponentManager GetComponents() { return m_components; }
        public GameObject() { m_id = Guid.NewGuid(); }
        public void Update(double dt, BaseControllers controllers)
        {
            m_components.Update(controllers, this, dt);
            i_onUpdate(dt, controllers);
        }
        protected virtual void i_onUpdate(double dt, BaseControllers controllers) { }
        public virtual void OnDelete() { }
    }
}
