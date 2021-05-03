
namespace GraphicsConsole.Test
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.pshStart = new System.Windows.Forms.ToolStripButton();
            this.pshStop = new System.Windows.Forms.ToolStripButton();
            this.lblRunning = new System.Windows.Forms.ToolStripLabel();
            this.graphicsConsole1 = new GraphicsConsole.GraphicsConsoleControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mleCode = new System.Windows.Forms.TextBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pshStart,
            this.pshStop,
            this.lblRunning});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1426, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // pshStart
            // 
            this.pshStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pshStart.Image = ((System.Drawing.Image)(resources.GetObject("pshStart.Image")));
            this.pshStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pshStart.Name = "pshStart";
            this.pshStart.Size = new System.Drawing.Size(23, 22);
            this.pshStart.Text = "toolStripButton1";
            this.pshStart.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // pshStop
            // 
            this.pshStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pshStop.Enabled = false;
            this.pshStop.Image = ((System.Drawing.Image)(resources.GetObject("pshStop.Image")));
            this.pshStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pshStop.Name = "pshStop";
            this.pshStop.Size = new System.Drawing.Size(23, 22);
            this.pshStop.Text = "toolStripButton2";
            this.pshStop.Click += new System.EventHandler(this.pshStop_Click);
            // 
            // lblRunning
            // 
            this.lblRunning.Name = "lblRunning";
            this.lblRunning.Size = new System.Drawing.Size(60, 22);
            this.lblRunning.Text = "RUNNING";
            this.lblRunning.Visible = false;
            // 
            // graphicsConsole1
            // 
            this.graphicsConsole1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphicsConsole1.Location = new System.Drawing.Point(0, 0);
            this.graphicsConsole1.Name = "graphicsConsole1";
            this.graphicsConsole1.Size = new System.Drawing.Size(947, 836);
            this.graphicsConsole1.TabIndex = 0;
            this.graphicsConsole1.Text = "graphicsConsole1";
            this.graphicsConsole1.Started += new System.EventHandler(this.graphicsConsole1_Started);
            this.graphicsConsole1.Stopped += new System.EventHandler(this.graphicsConsole1_Stopped);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mleCode);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.graphicsConsole1);
            this.splitContainer1.Size = new System.Drawing.Size(1426, 836);
            this.splitContainer1.SplitterDistance = 475;
            this.splitContainer1.TabIndex = 2;
            // 
            // mleCode
            // 
            this.mleCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mleCode.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mleCode.Location = new System.Drawing.Point(0, 0);
            this.mleCode.Multiline = true;
            this.mleCode.Name = "mleCode";
            this.mleCode.Size = new System.Drawing.Size(475, 836);
            this.mleCode.TabIndex = 0;
            this.mleCode.Text = "CLS\r\nPRINTL \"Hello world!\"\r\nEND";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1426, 861);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GraphicsConsole.GraphicsConsoleControl graphicsConsole1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton pshStart;
        private System.Windows.Forms.ToolStripButton pshStop;
        private System.Windows.Forms.ToolStripLabel lblRunning;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox mleCode;
    }
}

