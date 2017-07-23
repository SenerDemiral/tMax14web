using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FB2SCservice
{
    public partial class Scheduler : ServiceBase
    {
        private Timer timer1 = null;
        private bool sending = false;

        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            //timer1.Interval = 30000;    // 30sec
            timer1.Interval = 15 * 60 * 1000;    // 15dak
            timer1.Elapsed += Timer1_Elapsed;
            timer1.Enabled = true;
            FbLibrary.Logs.WriteErrorLog("FB2SC Service started");
        
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            FbLibrary.Logs.WriteErrorLog("Timer ticked");

            if (!sending)
            {
                sending = true;
                FbLibrary.SendWithWebSocket.FrtSend("M");
                FbLibrary.SendWithWebSocket.FrcSend("M");
                FbLibrary.SendWithWebSocket.OpmSend("M");
                FbLibrary.SendWithWebSocket.OphSend("M");
                FbLibrary.SendWithWebSocket.AfbSend("M");
                sending = false;
            }
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            FbLibrary.Logs.WriteErrorLog("FB2SC Service stopped");
        }
    }
}
