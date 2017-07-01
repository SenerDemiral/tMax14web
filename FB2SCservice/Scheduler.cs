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

        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            timer1.Interval = 30000;    // 30sec
            timer1.Elapsed += Timer1_Elapsed;
            timer1.Enabled = true;
            Library.WriteErrorLog("FB2SC Service started");
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            Library.WriteErrorLog("Timer ticked");
            Library.FrtCron("F");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("FB2SC Service stopped");
        }
    }
}
