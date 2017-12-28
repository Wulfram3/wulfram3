using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;
using System.Collections;
using B83.JobQueue;

namespace socket.io {

    /// <summary>
    /// Streams out received packets as observable
    /// </summary>
    public class WebSocketTrigger : ObservableTriggerBase {

        /// <summary>
        /// Observes received packets and also starts Ping-Pong routine
        /// </summary>
        /// <returns></returns>
        public UniRx.IObservable<string> OnRecvAsObservable() {
            if (_cancelPingPong == null) {
                _cancelPingPong = gameObject.UpdateAsObservable()
                    .Sample(TimeSpan.FromSeconds(10f))
                    .Subscribe(_ => {
                        WebSocket.Send(Packet.Ping);
                        Debug.LogFormat("socket.io => {0} ping~", WebSocket.Url.ToString());
                    });
            }

            Debug.Log("OnRecvAsObservable");
            if (_onRecv == null)
                _onRecv = new Subject<string>();

            WebSocket.OnDataRecieved += WebSocketOnDataRecieved;
            eventHandlerAttached = true;
            return _onRecv;
        }



        protected override void RaiseOnCompletedOnDestroy() {
            if (_cancelPingPong != null) {
                _cancelPingPong.Dispose();
                _cancelPingPong = null;
            }

            if (_onRecv != null) {
                _onRecv.OnCompleted();
                _onRecv = null;
            }

            if (!IsConnected)
                WebSocket.Close();
        }

        IDisposable _cancelPingPong;
        Subject<string> _onRecv;
        bool eventHandlerAttached = false;

        /// <summary>
        /// WebSocket object ref
        /// </summary>
        public WebSocketWrapper WebSocket { get; set; }

        /// <summary>
        /// Holds the last error on WebSocket
        /// </summary>
        public string LastWebSocketError { get; private set; }

        public bool IsConnected {
            get { return WebSocket != null && WebSocket.IsConnected; }
        }

        public bool IsProbed { get; set; }

        public bool IsUpgraded { get; set; }

        private float nextActionTime = 0.0f;
        public float period = 0.8f;

        private void WebSocketOnDataRecieved(string obj)
        {
            Debug.Log("WebSocketOnDataRecieved:" + obj);

            if (IsConnected)
                ReceiveWebSocketData();
        }


        void Update() {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;

                LastWebSocketError = WebSocket.GetLastError();

                if (!string.IsNullOrEmpty(LastWebSocketError))
                {
                    CheckAndHandleWebSocketDisconnect();
                    Debug.LogError(LastWebSocketError);
                }

                if (IsConnected && !eventHandlerAttached)
                {
                    ReceiveWebSocketData();
                }
                    
            }
        }

        void ReceiveWebSocketData() {
            var recvData = WebSocket.Recv();
            Debug.Log("ReceiveWebSocketData:" + recvData);
            if (string.IsNullOrEmpty(recvData))
                return;

            if (recvData == Packet.ProbeAnswer) {
                IsProbed = true;
                Debug.LogFormat("socket.io => {0} probed~", WebSocket.Url.ToString());
            }
            else if (recvData == Packet.Pong) {
                Debug.LogFormat("socket.io => {0} pong~", WebSocket.Url.ToString());
            }
            else {
                if (_onRecv != null)
                    _onRecv.OnNext(recvData);
            }
        }
        
        void CheckAndHandleWebSocketDisconnect() {
            if (IsConnected)
                return;

            if (_onRecv != null) {
                _cancelPingPong.Dispose();
                _cancelPingPong = null;
                _onRecv.Dispose();
                _onRecv = null;
                IsProbed = false;
                IsUpgraded = false;

                var sockets = gameObject.GetComponentsInChildren<Socket>();
                foreach (var s in sockets) {
                    if (s.OnDisconnect != null)
                        s.OnDisconnect();
                }
            }
            
            if (SocketManager.Instance.Reconnection) {
                var sockets = gameObject.GetComponentsInChildren<Socket>();
                foreach (var s in sockets)
                    SocketManager.Instance.Reconnect(s, 1);
            }
        }
        
    }

    // The example job class:
    public class SocketDataRetreval : JobItem
    {
        // some identifying name, not related to the actual job
        public string CustomName;
        // input variables. Should be set before the job is started
        public int count = 5;
        // output / result variable. This represents the data that this job produces.
        public float Result;
        protected override void DoWork()
        {
            // this is executed on a seperate thread
            float v = 0;
            for (int i = 0; i < count; i++)
            {
                v += Mathf.Pow(0.5f, i);
                // check every 100 iteration if the job should be aborted
                if ((i % 100) == 0 && IsAborted)
                    return;
            }
            Result = v;
        }
        public override void OnFinished()
        {
            // this is executed on the main thread.
            Debug.Log("Job: " + CustomName + " has finished with the result: " + Result);
        }
    }
}


namespace B83.JobQueue
{
    using System;
    using System.Threading;
    using System.Collections.Generic;

    public abstract class JobItem
    {
        private volatile bool m_Abort = false;
        private volatile bool m_Started = false;
        private volatile bool m_DataReady = false;

        /// <summary>
        /// This is the actual job routine. override it in a concrete Job class
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// This is a callback which will be called from the main thread when
        /// the job has finised. Can be overridden.
        /// </summary>
        public virtual void OnFinished() { }

        public bool IsAborted { get { return m_Abort; } }
        public bool IsStarted { get { return m_Started; } }
        public bool IsDataReady { get { return m_DataReady; } }

        public void Execute()
        {
            m_Started = true;
            DoWork();
            m_DataReady = true;
        }

        public void AbortJob()
        {
            m_Abort = true;
        }

        public void ResetJobState()
        {
            m_Started = false;
            m_DataReady = false;
            m_Abort = false;
        }
    }


    public class JobQueue<T> : IDisposable where T : JobItem
    {
        private class ThreadItem
        {
            private Thread m_Thread;
            private AutoResetEvent m_Event;
            private volatile bool m_Abort = false;

            // simple linked list to manage active threaditems
            public ThreadItem NextActive = null;

            // the job item this thread is currently processing
            public T Data;

            public ThreadItem()
            {
                m_Event = new AutoResetEvent(false);
                m_Thread = new Thread(ThreadMainLoop);
                m_Thread.Start();
            }

            private void ThreadMainLoop()
            {
                while (true)
                {
                    if (m_Abort)
                        return;
                    m_Event.WaitOne();
                    if (m_Abort)
                        return;
                    Data.Execute();
                }
            }

            public void StartJob(T aJob)
            {
                aJob.ResetJobState();
                Data = aJob;
                // signal the thread to start working.
                m_Event.Set();
            }

            public void Abort()
            {
                m_Abort = true;
                if (Data != null)
                    Data.AbortJob();
                // signal the thread so it can finish itself.
                m_Event.Set();
            }

            public void Reset()
            {
                Data = null;
            }
        }
        // internal thread pool
        private Stack<ThreadItem> m_Threads = new Stack<ThreadItem>();
        private Queue<T> m_NewJobs = new Queue<T>();
        private volatile bool m_NewJobsAdded = false;
        private Queue<T> m_Jobs = new Queue<T>();
        // start of the linked list of active threads
        private ThreadItem m_Active = null;

        public event Action<T> OnJobFinished;

        public JobQueue(int aThreadCount)
        {
            if (aThreadCount < 1)
                aThreadCount = 1;
            for (int i = 0; i < aThreadCount; i++)
                m_Threads.Push(new ThreadItem());
        }

        public void AddJob(T aJob)
        {
            if (m_Jobs == null)
                throw new System.InvalidOperationException("AddJob not allowed. JobQueue has already been shutdown");
            if (aJob != null)
            {
                m_Jobs.Enqueue(aJob);
                ProcessJobQueue();
            }
        }

        public void AddJobFromOtherThreads(T aJob)
        {
            lock (m_NewJobs)
            {
                if (m_Jobs == null)
                    throw new System.InvalidOperationException("AddJob not allowed. JobQueue has already been shutdown");
                m_NewJobs.Enqueue(aJob);
                m_NewJobsAdded = true;
            }
        }

        public int CountActiveJobs()
        {
            int count = 0;
            for (var thread = m_Active; thread != null; thread = thread.NextActive)
                count++;
            return count;
        }

        private void CheckActiveJobs()
        {
            ThreadItem thread = m_Active;
            ThreadItem last = null;
            while (thread != null)
            {
                ThreadItem next = thread.NextActive;
                T job = thread.Data;
                if (job.IsAborted)
                {
                    if (last == null)
                        m_Active = next;
                    else
                        last.NextActive = next;
                    thread.NextActive = null;

                    thread.Reset();
                    m_Threads.Push(thread);
                }
                else if (thread.Data.IsDataReady)
                {
                    job.OnFinished();
                    if (OnJobFinished != null)
                        OnJobFinished(job);

                    if (last == null)
                        m_Active = next;
                    else
                        last.NextActive = next;
                    thread.NextActive = null;

                    thread.Reset();
                    m_Threads.Push(thread);
                }
                else
                    last = thread;
                thread = next;
            }
        }

        private void ProcessJobQueue()
        {
            if (m_NewJobsAdded)
            {
                lock (m_NewJobs)
                {
                    while (m_NewJobs.Count > 0)
                        AddJob(m_NewJobs.Dequeue());
                    m_NewJobsAdded = false;
                }
            }
            while (m_Jobs.Count > 0 && m_Threads.Count > 0)
            {
                var job = m_Jobs.Dequeue();
                if (!job.IsAborted)
                {
                    var thread = m_Threads.Pop();
                    thread.StartJob(job);
                    // add thread to the linked list of active threads
                    thread.NextActive = m_Active;
                    m_Active = thread;
                }
            }
        }

        public void Update()
        {
            CheckActiveJobs();
            ProcessJobQueue();
        }

        public void ShutdownQueue()
        {
            for (var thread = m_Active; thread != null; thread = thread.NextActive)
                thread.Abort();
            while (m_Threads.Count > 0)
                m_Threads.Pop().Abort();
            while (m_Jobs.Count > 0)
                m_Jobs.Dequeue().AbortJob();
            m_Jobs = null;
            m_Active = null;
            m_Threads = null;
        }

        public void Dispose()
        {
            ShutdownQueue();
        }
    }
}