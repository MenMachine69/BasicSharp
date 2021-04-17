
namespace BASIC.FontEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.menFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFont = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.listView1 = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.colPosition = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChar = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSymbolCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.sleChar = new System.Windows.Forms.TextBox();
            this.sleSymName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDesc = new System.Windows.Forms.Label();
            this.pshSaveChar = new System.Windows.Forms.Button();
            this.pshReset = new System.Windows.Forms.Button();
            this.pshCopyFrom = new System.Windows.Forms.Button();
            this.lblCopyFrom = new System.Windows.Forms.Label();
            this.sleCopyFrom = new System.Windows.Forms.TextBox();
            this.pshCopy = new System.Windows.Forms.Button();
            this.pshCopyCancel = new System.Windows.Forms.Button();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pshClear = new System.Windows.Forms.Button();
            this.pshFill = new System.Windows.Forms.Button();
            this.pshInvert = new System.Windows.Forms.Button();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1157, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menFileName,
            this.menFileOpen,
            this.toolStripMenuItem1,
            this.menFileSave,
            this.menFileSaveAs,
            this.toolStripMenuItem2,
            this.menFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // menFileName
            // 
            this.menFileName.Name = "menFileName";
            this.menFileName.Size = new System.Drawing.Size(180, 22);
            this.menFileName.Text = "&New font...";
            this.menFileName.Click += new System.EventHandler(this.menFileName_Click);
            // 
            // menFileOpen
            // 
            this.menFileOpen.Image = global::BASIC.FontEditor.Properties.Resources.folder_open;
            this.menFileOpen.Name = "menFileOpen";
            this.menFileOpen.Size = new System.Drawing.Size(180, 22);
            this.menFileOpen.Text = "&Open font...";
            this.menFileOpen.Click += new System.EventHandler(this.menFileOpen_Click);
            // 
            // menFileSave
            // 
            this.menFileSave.Image = global::BASIC.FontEditor.Properties.Resources.floppy_disk;
            this.menFileSave.Name = "menFileSave";
            this.menFileSave.Size = new System.Drawing.Size(180, 22);
            this.menFileSave.Text = "&Save font";
            this.menFileSave.Click += new System.EventHandler(this.menFileSave_Click);
            // 
            // menFileSaveAs
            // 
            this.menFileSaveAs.Image = global::BASIC.FontEditor.Properties.Resources.save_as;
            this.menFileSaveAs.Name = "menFileSaveAs";
            this.menFileSaveAs.Size = new System.Drawing.Size(180, 22);
            this.menFileSaveAs.Text = "Save font &as...";
            this.menFileSaveAs.Click += new System.EventHandler(this.menFileSaveAs_Click);
            // 
            // menFileExit
            // 
            this.menFileExit.Image = global::BASIC.FontEditor.Properties.Resources.door_exit;
            this.menFileExit.Name = "menFileExit";
            this.menFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.menFileExit.Size = new System.Drawing.Size(180, 22);
            this.menFileExit.Text = "E&xit editor...";
            this.menFileExit.Click += new System.EventHandler(this.menFileExit_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4});
            this.toolStrip2.Location = new System.Drawing.Point(0, 24);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1157, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblFont});
            this.statusStrip1.Location = new System.Drawing.Point(0, 617);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1157, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1072, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Ready...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFont
            // 
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(70, 17);
            this.lblFont.Text = "<new font>";
            this.lblFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(30, 30);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ofd
            // 
            this.ofd.DefaultExt = "fnt";
            this.ofd.Filter = "Font-Files|*.fnt|All files|*.*";
            // 
            // sfd
            // 
            this.sfd.DefaultExt = "fnt";
            this.sfd.Filter = "Font-Files|*.fnt|All files|*.*";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPosition,
            this.colChar,
            this.colSymbolCode});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(675, 568);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 49);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pshInvert);
            this.splitContainer1.Panel2.Controls.Add(this.pshFill);
            this.splitContainer1.Panel2.Controls.Add(this.pshClear);
            this.splitContainer1.Panel2.Controls.Add(this.pshCopyCancel);
            this.splitContainer1.Panel2.Controls.Add(this.pshCopy);
            this.splitContainer1.Panel2.Controls.Add(this.sleCopyFrom);
            this.splitContainer1.Panel2.Controls.Add(this.lblCopyFrom);
            this.splitContainer1.Panel2.Controls.Add(this.pshCopyFrom);
            this.splitContainer1.Panel2.Controls.Add(this.pshReset);
            this.splitContainer1.Panel2.Controls.Add(this.pshSaveChar);
            this.splitContainer1.Panel2.Controls.Add(this.lblDesc);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.sleSymName);
            this.splitContainer1.Panel2.Controls.Add(this.sleChar);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1157, 568);
            this.splitContainer1.SplitterDistance = 675;
            this.splitContainer1.TabIndex = 5;
            // 
            // colPosition
            // 
            this.colPosition.Text = "Char index";
            // 
            // colChar
            // 
            this.colChar.Text = "Character";
            // 
            // colSymbolCode
            // 
            this.colSymbolCode.Text = "Symbol code";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Location = new System.Drawing.Point(22, 38);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 17;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(172, 339);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(217, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Use for character";
            // 
            // sleChar
            // 
            this.sleChar.Location = new System.Drawing.Point(312, 35);
            this.sleChar.MaxLength = 10;
            this.sleChar.Name = "sleChar";
            this.sleChar.Size = new System.Drawing.Size(34, 20);
            this.sleChar.TabIndex = 2;
            // 
            // sleSymName
            // 
            this.sleSymName.Location = new System.Drawing.Point(312, 61);
            this.sleSymName.Name = "sleSymName";
            this.sleSymName.Size = new System.Drawing.Size(100, 20);
            this.sleSymName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Symbolic name";
            // 
            // lblDesc
            // 
            this.lblDesc.Location = new System.Drawing.Point(217, 102);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(247, 263);
            this.lblDesc.TabIndex = 5;
            // 
            // pshSaveChar
            // 
            this.pshSaveChar.Location = new System.Drawing.Point(22, 383);
            this.pshSaveChar.Name = "pshSaveChar";
            this.pshSaveChar.Size = new System.Drawing.Size(107, 23);
            this.pshSaveChar.TabIndex = 6;
            this.pshSaveChar.Text = "SAVE";
            this.pshSaveChar.UseVisualStyleBackColor = true;
            this.pshSaveChar.Click += new System.EventHandler(this.pshSaveChar_Click);
            // 
            // pshReset
            // 
            this.pshReset.Location = new System.Drawing.Point(135, 383);
            this.pshReset.Name = "pshReset";
            this.pshReset.Size = new System.Drawing.Size(107, 23);
            this.pshReset.TabIndex = 7;
            this.pshReset.Text = "RESET";
            this.pshReset.UseVisualStyleBackColor = true;
            this.pshReset.Click += new System.EventHandler(this.pshReset_Click);
            // 
            // pshCopyFrom
            // 
            this.pshCopyFrom.Location = new System.Drawing.Point(248, 383);
            this.pshCopyFrom.Name = "pshCopyFrom";
            this.pshCopyFrom.Size = new System.Drawing.Size(107, 23);
            this.pshCopyFrom.TabIndex = 8;
            this.pshCopyFrom.Text = "COPY FROM";
            this.pshCopyFrom.UseVisualStyleBackColor = true;
            this.pshCopyFrom.Click += new System.EventHandler(this.pshCopyFrom_Click);
            // 
            // lblCopyFrom
            // 
            this.lblCopyFrom.AutoSize = true;
            this.lblCopyFrom.Location = new System.Drawing.Point(19, 432);
            this.lblCopyFrom.Name = "lblCopyFrom";
            this.lblCopyFrom.Size = new System.Drawing.Size(57, 13);
            this.lblCopyFrom.TabIndex = 9;
            this.lblCopyFrom.Text = "Copy from ";
            this.lblCopyFrom.Visible = false;
            // 
            // sleCopyFrom
            // 
            this.sleCopyFrom.Location = new System.Drawing.Point(82, 429);
            this.sleCopyFrom.MaxLength = 3;
            this.sleCopyFrom.Name = "sleCopyFrom";
            this.sleCopyFrom.Size = new System.Drawing.Size(47, 20);
            this.sleCopyFrom.TabIndex = 10;
            this.sleCopyFrom.Text = "32";
            this.sleCopyFrom.Visible = false;
            // 
            // pshCopy
            // 
            this.pshCopy.Location = new System.Drawing.Point(135, 427);
            this.pshCopy.Name = "pshCopy";
            this.pshCopy.Size = new System.Drawing.Size(107, 23);
            this.pshCopy.TabIndex = 11;
            this.pshCopy.Text = "COPY";
            this.pshCopy.UseVisualStyleBackColor = true;
            this.pshCopy.Visible = false;
            this.pshCopy.Click += new System.EventHandler(this.pshCopy_Click);
            // 
            // pshCopyCancel
            // 
            this.pshCopyCancel.Location = new System.Drawing.Point(248, 427);
            this.pshCopyCancel.Name = "pshCopyCancel";
            this.pshCopyCancel.Size = new System.Drawing.Size(107, 23);
            this.pshCopyCancel.TabIndex = 12;
            this.pshCopyCancel.Text = "CANCEL";
            this.pshCopyCancel.UseVisualStyleBackColor = true;
            this.pshCopyCancel.Visible = false;
            this.pshCopyCancel.Click += new System.EventHandler(this.pshCopyCancel_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::BASIC.FontEditor.Properties.Resources.folder_open;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Open font...";
            this.toolStripButton1.Click += new System.EventHandler(this.menFileOpen_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::BASIC.FontEditor.Properties.Resources.floppy_disk;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "Save font...";
            this.toolStripButton2.Click += new System.EventHandler(this.menFileSave_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::BASIC.FontEditor.Properties.Resources.save_as;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            this.toolStripButton3.ToolTipText = "Save font as...";
            this.toolStripButton3.Click += new System.EventHandler(this.menFileSaveAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // pshClear
            // 
            this.pshClear.Location = new System.Drawing.Point(22, 9);
            this.pshClear.Name = "pshClear";
            this.pshClear.Size = new System.Drawing.Size(54, 23);
            this.pshClear.TabIndex = 13;
            this.pshClear.Text = "CLEAR";
            this.pshClear.UseVisualStyleBackColor = true;
            this.pshClear.Click += new System.EventHandler(this.pshClear_Click);
            // 
            // pshFill
            // 
            this.pshFill.Location = new System.Drawing.Point(82, 9);
            this.pshFill.Name = "pshFill";
            this.pshFill.Size = new System.Drawing.Size(54, 23);
            this.pshFill.TabIndex = 14;
            this.pshFill.Text = "FILL";
            this.pshFill.UseVisualStyleBackColor = true;
            this.pshFill.Click += new System.EventHandler(this.pshFill_Click);
            // 
            // pshInvert
            // 
            this.pshInvert.Location = new System.Drawing.Point(142, 9);
            this.pshInvert.Name = "pshInvert";
            this.pshInvert.Size = new System.Drawing.Size(64, 23);
            this.pshInvert.TabIndex = 15;
            this.pshInvert.Text = "INVERT";
            this.pshInvert.UseVisualStyleBackColor = true;
            this.pshInvert.Click += new System.EventHandler(this.pshInvert_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 639);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "BASICSharp Fontedit";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menFileName;
        private System.Windows.Forms.ToolStripMenuItem menFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menFileSave;
        private System.Windows.Forms.ToolStripMenuItem menFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menFileExit;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblFont;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colPosition;
        private System.Windows.Forms.ColumnHeader colChar;
        private System.Windows.Forms.ColumnHeader colSymbolCode;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sleSymName;
        private System.Windows.Forms.TextBox sleChar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pshSaveChar;
        private System.Windows.Forms.Button pshReset;
        private System.Windows.Forms.Button pshCopyFrom;
        private System.Windows.Forms.Button pshCopyCancel;
        private System.Windows.Forms.Button pshCopy;
        private System.Windows.Forms.TextBox sleCopyFrom;
        private System.Windows.Forms.Label lblCopyFrom;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.Button pshClear;
        private System.Windows.Forms.Button pshInvert;
        private System.Windows.Forms.Button pshFill;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
    }
}

