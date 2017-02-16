using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace File_System_Watcher
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        
        public void WriteLog(string log)
        {
            try
            {
                File.AppendAllLines(@"c:\FsLog\"+DateTime.Today.ToString("yyyy_MM_dd")+".log", new string[]{log});
            }
            catch { }
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("service start at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
            foreach (DriveInfo dinfo in DriveInfo.GetDrives())
            {
                if (dinfo.IsReady)
                {
                    FileSystemWatcher fw = new FileSystemWatcher();
                    fw.Path = dinfo.Name;
                    fw.IncludeSubdirectories = true;
                    fw.Filter = "*.*";
                    fw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                        | NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName
                                        | NotifyFilters.Security | NotifyFilters.Size;
                    fw.Changed += fw_Created;
                    fw.Created += fw_Created;
                    fw.Deleted += fw_Created;
                    fw.Renamed += fw_Renamed;
                    fw.EnableRaisingEvents = true;
                }
            }
        }

        void fw_Renamed(object sender, RenamedEventArgs e)
        {
            WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " ChangeType = " + e.ChangeType + " OldPath = " + e.OldFullPath+ " NewPath = "+e.FullPath);
        }

        void fw_Created(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("FsLog"))
            {
                WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt")+" ChangeType = "+e.ChangeType+" FullPath = "+e.FullPath+" ");
            }
        }

        protected override void OnStop()
        {
            WriteLog("service stopped at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
        }
    }
}
