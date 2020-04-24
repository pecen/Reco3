using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YDMCBackend
{
    public partial class ExcelConverterSvc : ServiceBase
    {
        protected ExcelConverterSvcLib m_ConverterLib = null;

        public ExcelConverterSvc()
        {
            InitializeComponent();
            m_ConverterLib = new ExcelConverterSvcLib();
            
        }

        protected override void OnStart(string[] args)
        {
            m_ConverterLib.OnStart(args);
        }
        protected override void OnStop()
        {
            m_ConverterLib.OnStop();
        }


    }
}
