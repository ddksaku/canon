namespace CanonImportService
{
    partial class CanonImportService
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
            this.MainTimer = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.MainTimer)).BeginInit();
            // 
            // MainTimer
            // 
            this.MainTimer.Enabled = true;
            this.MainTimer.Interval = 30000;
            this.MainTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.MainTimer_Elapsed);
            // 
            // CanonImportService
            // 
            this.ServiceName = "Service1";
            ((System.ComponentModel.ISupportInitialize)(this.MainTimer)).EndInit();

        }

        #endregion

        private System.Timers.Timer MainTimer;
    }
}
