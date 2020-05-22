using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCApi
{
    public class PLCVarThread 
    {
        Task m_task;
        AutoResetEvent m_event = new AutoResetEvent(false);
        int m_time;
        PLCAny m_plcVar;
        bool m_running = false;
        public delegate void NotifyVar(object o, string varName);
        NotifyVar pNotify;
        string m_varName;

        public PLCVarThread(PLCAny var, string VarName, int time, NotifyVar p)
        {
            m_time = time;
            m_plcVar = var;
            pNotify = p;
            m_varName = VarName;
        }
        public void Start()
        { 
            if (m_running == true)
                return;
            if (m_task == null)
            {
                m_task = new Task(Process);
                m_task.Start();
            }
        }
        void Process()
        {
            bool b;
            m_running = true;
            while (m_running)
            {
                m_event.WaitOne(m_time);
                if (m_running == false)
                    return;
                m_plcVar.Read(out b);
                pNotify(b, m_varName);
            }
        }
        public void Stop()
        {
            m_running = false;
            m_event.Set();
            if (m_task != null)
                m_task.Wait();

        }
        public void Pause()
        {

        }
    }
}
