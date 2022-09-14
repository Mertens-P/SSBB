using System;
using System.Threading;

namespace ShootyShootyBangBangEngine.Helpers.Threading
{
    public class LockObject
    {
        public bool IsLocked { get; set; }
        public int ManagedThreadId { get; set; }
        public string ThreadName { get; set; }
    }

    public struct MonitorLock : IDisposable
    {
        public static MonitorLock CreateLock(LockObject lockObject)
        {
            return new MonitorLock(lockObject);
        }

        private readonly LockObject m_lockObject;

        private MonitorLock(LockObject lockObject)
        {
            m_lockObject = lockObject;
            Monitor.Enter(m_lockObject);
            m_lockObject.IsLocked = true;
            m_lockObject.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            m_lockObject.ThreadName = Thread.CurrentThread.Name;
        }

        public void Dispose()
        {
            m_lockObject.IsLocked = false;
            Monitor.Exit(m_lockObject);
        }
    }
}
