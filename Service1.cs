using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Sales
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Saless service = new Saless();
                service.Run();
            }
            catch (Exception ex)
            {

                EventLog.WriteEntry("Ошибка при запуске службы: " + ex.ToString(), EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
        }
    }

}

