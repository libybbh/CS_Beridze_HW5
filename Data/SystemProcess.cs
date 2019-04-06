using System.ComponentModel;
using System.Diagnostics;

namespace Beridze_5.Data
{
    internal class SystemProcess
    {
        #region Properties

        internal PerformanceCounter RamCounter { get; }

        internal PerformanceCounter CpuCounter { get; }

        public string Name
        {
            get;
        }

        public int Id
        {
            get;
        }

        public bool IsActive
        {
            get;
        }

        public int CpuTaken
        {
            get;
            set;
        }

        public int RamTaken
        {
            get;
            set;
        }

        public int ThreadsNumber
        {
            get;
            set;
        }

        public string Username
        {
            get;
        }

        public string FilePath
        {
            get;
        }

        public string RunOn
        {
            get;
        }
        #endregion

        internal SystemProcess(System.Diagnostics.Process systemProcess)
        {
            RamCounter = new PerformanceCounter("Process", "Working Set", systemProcess.ProcessName);
            CpuCounter = new PerformanceCounter("Process", "% Processor Time", systemProcess.ProcessName);
            Name = systemProcess.ProcessName;
            Id = systemProcess.Id;
            IsActive = systemProcess.Responding;
            CpuTaken = (int)CpuCounter.NextValue();
            RamTaken = (int)(RamCounter.NextValue() / 1024 / 1024);
            ThreadsNumber = systemProcess.Threads.Count;
            try
            {
                FilePath = systemProcess.MainModule.FileName;
            }
            catch (Win32Exception e)
            {
                FilePath = e.Message;
            }
            try
            {
                RunOn = systemProcess.StartTime.ToString();
            }
            catch (Win32Exception e)
            {
                RunOn = e.Message;
            }
        }
        
    }
}

