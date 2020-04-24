namespace YDMCBackend
{
    partial class ExcelConverterSvc
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.messageQueue1 = new System.Messaging.MessageQueue();
            // 
            // messageQueue1
            // 
            this.messageQueue1.MessageReadPropertyFilter.LookupId = true;
            this.messageQueue1.Path = "arbauws006\\sar_gis_pp";
            // 
            // ExcelConverterSvc
            // 
            this.ServiceName = "ExcelConverterSvc";

        }

        #endregion

        private System.Messaging.MessageQueue messageQueue1;
    }
}
