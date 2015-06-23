using System.Reactive.Concurrency;

namespace RxTwitterSample
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
    }

    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler CurrentThread
        {
            get { return Scheduler.CurrentThread; }
        }
        public IScheduler Dispatcher
        {
            get { return DispatcherScheduler.Instance; }
        }
        public IScheduler Immediate
        {
            get { return Scheduler.Immediate; }
        }
        public IScheduler NewThread
        {
            get { return Scheduler.NewThread; }
        }
        public IScheduler ThreadPool
        {
            get { return Scheduler.ThreadPool; }
        }
    }
}