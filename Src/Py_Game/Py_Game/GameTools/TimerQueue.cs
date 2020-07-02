//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using static TimerQueue.TScheduler;

//namespace TimerQueue
//{
//    public struct TTimerInfo
//    {
//        public TCallback Proc;
//        public TRefCallback RefProc { get; set; }
//        public long Timer { get; set; }
//        public long Queue { get; set; }
//    } 

//    public class TScheduler
//    {

//        public enum TimerQueueTimerFlags : uint
//        {
//            ExecuteDefault = 0x0000,
//            ExecuteInTimerThread = 0x0020,
//            ExecuteInIoThread = 0x0001,
//            ExecuteInPersistentThread = 0x0080,
//            ExecuteLongFunction = 0x0010,
//            ExecuteOnlyOnce = 0x0008,
//            TransferImpersonation = 0x0100,
//        }
//        public delegate void Win32WaitOrTimerCallback(
//            IntPtr lpParam,
//            [MarshalAs(UnmanagedType.U1)]bool bTimedOut);

//        [DllImport("kernel32.dll", SetLastError = true)]
//        public extern static IntPtr CreateTimerQueue();
//        [DllImport("kernel32.dll", SetLastError = true)]
//        public extern static bool DeleteTimerQueue(IntPtr timerQueue);
//        [DllImport("kernel32.dll", SetLastError = true)]
//        public extern static bool DeleteTimerQueueEx(IntPtr timerQueue, IntPtr completionEvent);
//        [DllImport("kernel32.dll", SetLastError = true)]
//        public extern static bool CreateTimerQueueTimer(
//            out IntPtr newTimer,
//            IntPtr timerQueue,
//            Win32WaitOrTimerCallback callback,
//            IntPtr userState,
//            uint dueTime,
//            uint period,
//            TimerQueueTimerFlags flags);
//        [DllImport("kernel32.dll", SetLastError = true)]
//        public extern static bool ChangeTimerQueueTimer(
//            IntPtr timerQueue,
//            ref IntPtr timer,
//            uint dueTime,
//            uint period);
//        [DllImport("kernel32.dll", SetLastError = true)]
//        public extern static bool DeleteTimerQueueTimer(
//            IntPtr timerQueue,
//            IntPtr timer,
//            IntPtr completionEvent);

//        private long FQueue = 0;
//        private Dictionary<long, TTimerInfo> FTimers = null;
//        private CriticalSection FLock = null;

//        public TScheduler()
//        {
//            FQueue = CreateTimerQueue;

//            FTimers = new Dictionary<long, TTimerInfo>();
//            FLock = new TCriticalSection();
//        }
//        //@ Destructor  Destroy()
//        ~TScheduler()
//        {
//            //@ Undeclared identifier(3): 'INVALID_HANDLE_VALUE'
//            //@ Undeclared identifier(3): 'DeleteTimerQueueEx'
//            DeleteTimerQueueEx(FQueue, INVALID_HANDLE_VALUE);
//            //@ Unsupported property or method(C): 'Free'
//            FTimers.Free;
//            //@ Unsupported property or method(C): 'Free'
//            FLock.Free;
//            // base.Destroy();
//        }
//        public long AddSchedule(uint Milliseconds, TCallback Proc)
//        {
//            long result;
//            long Timer;
//            TTimerInfo Info;
//            Info = new TTimerInfo();
//            Info.Proc = Proc;
//            Info.RefProc = null;
//            Info.Queue = FQueue;
//            //@ Undeclared identifier(3): 'WT_EXECUTEONLYONCE'
//            //@ Undeclared identifier(3): 'CreateTimerQueueTimer'
//            if (!CreateTimerQueueTimer(Timer, FQueue, TimerQueueUnit.OnSchedule, Info, Milliseconds, 0, win32WaitOrTimerCallback))
//            {
//                throw new Exception("Creating a timer (schedule) failed!");
//            }
//            Info.Timer = Timer;
//            //@ Unsupported property or method(C): 'Acquire'
//            FLock.Acquire;
//            try
//            {
//                result = Timer;
//                //@ Unsupported property or method(A): 'Add'
//                FTimers.Add(result, Info);
//            }
//            finally
//            {
//                //@ Unsupported property or method(C): 'Release'
//                FLock.Release;
//            }
//            return result;
//        }

//        public long AddSchedule(uint Milliseconds, TRefCallback Proc)
//        {
//            long result;
//            long Timer = 0;
//            TTimerInfo Info;
//            Info = new TTimerInfo
//            {
//                Proc = null,
//                RefProc = Proc,
//                Queue = FQueue
//            };

//            if (!CreateTimerQueueTimer(Timer, FQueue, TimerQueueUnit.OnSchedule, Info, Milliseconds, 0, WT_EXECUTEONLYONCE))
//            {
//                throw new Exception("Creating a timer (schedule) failed!");
//            }
//            Info.Timer = Timer;

//            try
//            {
//                result = Timer;

//                FTimers.Add(result, Info);
//            }
//            finally
//            {

//            }
//            return result;
//        }

//        public void CancelSchedule(long Handle)
//        {
//            try
//            {
//                //@ Undeclared identifier(3): 'DeleteTimerQueueTimer'
//                if (!DeleteTimerQueueTimer((IntPtr)FQueue, (IntPtr)Handle, (IntPtr)0))
//                {
//                    //@ Unsupported function or procedure: 'GetLastError'
//                    //@ Undeclared identifier(3): 'ERROR_IO_PENDING'
//                    if (GetLastError != ERROR_IO_PENDING)
//                    {
//                        throw new Exception("Cancelling a timer failed!");
//                    }
//                }
//            }
//            finally
//            {
//                TimerDone(Handle);
//            }
//        }

//        public long AddRepeatedJob(uint Interval, TCallback Proc)
//        {
//            long result =0;
//            TTimerInfo Info;
//            Info = new TTimerInfo
//            {
//                Proc = Proc
//            };
//            //@ Undeclared identifier(3): 'WT_EXECUTEDEFAULT'
//            //@ Undeclared identifier(3): 'CreateTimerQueueTimer'
//            if (!CreateTimerQueueTimer(result, FQueue, TimerQueueUnit.OnRepeat, Info, Interval, Interval, Win32WaitOrTimerCallback))
//            {
//                throw new Exception("Creating a timer (repeat) failed!");
//            }

//            try
//            {
//                FTimers.Add(result, Info);
//            }
//            finally
//            {
//            }
//            return result;
//        }

//        public void RemoveRepeatedJob(long Handle)
//        {
//            if (!DeleteTimerQueueTimer((IntPtr)FQueue, (IntPtr)Handle, (IntPtr)0))
//            {
//                throw new Exception("Deleting a timer (repeat) failed!");
//            }
//            TimerDone(Handle);
//        }

//        public void TimerDone(long Handle)
//        {

//            try
//            {
//                if (!FTimers.ContainsKey(Handle))
//                {
//                    Console.Out.WriteLine("Could not dispose timer info");
//                    return;
//                }
//                FTimers.Remove(Handle);
//            }
//            finally
//            {
//            }
//        }

//    } // end TScheduler

//    public delegate void TCallback();
//    public delegate void TRefCallback();
//    public class TimerQueueUnit
//    {
//        public static TScheduler Sched = null;
//        public static void OnSchedule(TTimerInfo Context, bool Fired)
//        {
//            uint E;
//            try
//            {
//                if (Context.Proc != null)
//                {
//                    Context.Proc();
//                }
//                else
//                {
//                    Context.RefProc();
//                }

//                if (!DeleteTimerQueueTimer((IntPtr)Context.Queue, (IntPtr)Context.Timer, (IntPtr)0))
//                {
//                    E = GetLastError;
//                    //@ Undeclared identifier(3): 'ERROR_IO_PENDING'
//                    if (E != ERROR_IO_PENDING)
//                    {
//                        Console.Out.WriteLine(string.Format("Deleting a timer failed with %d", E));
//                    }
//                }
//            }
//            finally
//            {
//                Sched.TimerDone(Context.Timer);
//            }
//        }

//        public static void OnRepeat(TTimerInfo Context, bool Fired)
//        {
//            Context.Proc();
//        }

//        public void initialization()
//        {
//            // Singleton handling
//            Sched = new TScheduler();
//        }

//    } // end TimerQueue

//}

