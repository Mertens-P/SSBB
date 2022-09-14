using System;
using System.Threading;

namespace ShootyShootyBangBangEngine.Helpers.Threading
{
    public class ReaderWriterLockObject
    {
        public bool IsLocked { get; set; }
        public int ManagedThreadId { get; set; }
        public string ThreadName { get; set; }
        public ReaderWriterLockSlim Lock { get; set; } = new ReaderWriterLockSlim();
    }

    public struct ReadLock : IDisposable
    {
        public static ReadLock CreateLock(ReaderWriterLockObject lockObject)
        {
            return new ReadLock(lockObject);
        }

        private readonly ReaderWriterLockObject m_lockObject;

        private ReadLock(ReaderWriterLockObject lockObject)
        {
            m_lockObject = lockObject;
            m_lockObject.Lock.EnterReadLock();
            m_lockObject.IsLocked = true;
            m_lockObject.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            m_lockObject.ThreadName = Thread.CurrentThread.Name;
        }

        public void Dispose()
        {
            m_lockObject.IsLocked = false;
            m_lockObject.Lock.ExitReadLock();
        }
    }

    public struct UpgradeableReadLock : IDisposable
    {
        public static UpgradeableReadLock CreateLock(ReaderWriterLockObject lockObject)
        {
            return new UpgradeableReadLock(lockObject);
        }

        private readonly ReaderWriterLockObject m_lockObject;

        private UpgradeableReadLock(ReaderWriterLockObject lockObjet)
        {
            m_lockObject = lockObjet;
            m_lockObject.Lock.EnterUpgradeableReadLock();
            m_lockObject.IsLocked = true;
            m_lockObject.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            m_lockObject.ThreadName = Thread.CurrentThread.Name;
        }

        public void Dispose()
        {
            m_lockObject.IsLocked = false;
            m_lockObject.Lock.ExitUpgradeableReadLock();
        }
    }

    public struct WriteLock : IDisposable
    {
        public static WriteLock CreateLock(ReaderWriterLockObject lockObject)
        {
            return new WriteLock(lockObject);
        }

        private readonly ReaderWriterLockObject m_lockObject;

        private WriteLock(ReaderWriterLockObject lockObject)
        {
            m_lockObject = lockObject;
            m_lockObject.Lock.EnterWriteLock();
            m_lockObject.IsLocked = true;
            m_lockObject.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            m_lockObject.ThreadName = Thread.CurrentThread.Name;
        }

        public void Dispose()
        {
            m_lockObject.IsLocked = false;
            m_lockObject.Lock.ExitWriteLock();
        }
    }
}
