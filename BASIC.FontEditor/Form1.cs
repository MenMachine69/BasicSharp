using GraphicsConsole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BASIC.FontEditor
{
    public partial class Form1 : Form
    {
        CharTable charTable = null;
        CharDef currChar = null;
        int currIdx = 0;
        string currFile = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lblStatus.Text = "Loading...";
            Application.DoEvents();

            for (int row = 0; row <= 15; ++row)
            {
                for (int col = 0; col <= 7; ++col)
                {
                    dynamic tagObject = new ExpandoObject();
                    tagObject.Row = row;
                    tagObject.Col = col;
                    tagObject.IsSet = false;
                    Label lbl = new Label { AutoSize = false, Dock = DockStyle.Fill, Text = "", Tag = tagObject, BackColor = Color.White, Margin = new Padding(1) };
                    tableLayoutPanel1.Controls.Add(lbl, col, row);
                    lbl.Click += toggleByte;
                }
            }

            charTable = new CharTable();

            _loadList();

            lblStatus.Text = "Ready...";
        }

        void toggleByte(object sender, EventArgs e)
        {
            ((dynamic)((Label)sender).Tag).IsSet = !((dynamic)((Label)sender).Tag).IsSet;
            if (((dynamic)((Label)sender).Tag).IsSet)
                ((Label)sender).BackColor = Color.Black;
            else
                ((Label)sender).BackColor = Color.White;
        }

        void _loadList()
        {
            listView1.Items.Clear();
            imageList1.Images.Clear();

            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                foreach (var chr in charTable.Characters)
                {
                    Bitmap charImage = new Bitmap(32, 32);
                    using (var canvas = Graphics.FromImage(charImage))
                    {
                        chr.Draw(brush, canvas, new Point(4, 0), 2, 2);
                    }
                    imageList1.Images.Add(charImage);

                    listView1.Items.Add(new ListViewItem(new string[] { (31 + imageList1.Images.Count).ToString() + " ('" + chr.Character.ToString() + "')", chr.SymbolCode }, imageList1.Images.Count - 1));
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                currChar = null;
                tableLayoutPanel1.Enabled = false;
                sleChar.Enabled = false;
                sleSymName.Enabled = false;
                pshSaveChar.Enabled = false;
                pshReset.Enabled = false;

                _loadChar();
            }
            else
            {
                tableLayoutPanel1.Enabled = true;
                sleChar.Enabled = true;
                sleSymName.Enabled = true;
                pshSaveChar.Enabled = true;
                pshReset.Enabled = true;

                currIdx = listView1.SelectedIndices[0];
                currChar = charTable.Characters[listView1.SelectedIndices[0]];
                _loadChar();

            }
        }

        private void _loadChar()
        {
            sleChar.Text = (currChar != null ? currChar.Character.ToString() : "");
            sleSymName.Text = (currChar != null ? currChar.SymbolCode : "");

            // mark Bits as selected...
            for (int row = 1; row <= 16; ++row)
            {
                for (int col = 1; col <= 8; ++col)
                {
                    Label lbl = (Label)tableLayoutPanel1.GetControlFromPosition(col - 1, row - 1);
                    ((dynamic)(lbl).Tag).IsSet = (currChar != null ? currChar.GetBit(row, col) : false);
                    if (((dynamic)(lbl).Tag).IsSet)
                        lbl.BackColor = Color.Black;
                    else
                        lbl.BackColor = Color.White;
                }
            }
        }

        private void pshSaveChar_Click(object sender, EventArgs e)
        {
            if (currChar != null)
            {
                currChar.SymbolCode = sleSymName.Text.Trim().ToUpper();

                if (sleChar.Text.Trim().Length > 0)
                    currChar.Character = sleChar.Text.Trim()[0];
                else
                    currChar.Character = ' ';

                for (int row = 1; row <= 16; ++row)
                {
                    for (int col = 1; col <= 8; ++col)
                    {
                        Label lbl = (Label)tableLayoutPanel1.GetControlFromPosition(col - 1, row - 1);
                        currChar.SetBit(row, col, ((dynamic)(lbl).Tag).IsSet);
                    }
                }

                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    Bitmap charImage = new Bitmap(32, 32);
                    using (var canvas = Graphics.FromImage(charImage))
                    {
                        currChar.Draw(brush, canvas, new Point(4, 0), 2, 2);
                    }
                    imageList1.Images[currIdx] = charImage;
                }

                listView1.Refresh();
            }
        }

        private void pshReset_Click(object sender, EventArgs e)
        {
            _loadChar();
        }

        private void pshCopyFrom_Click(object sender, EventArgs e)
        {
            sleCopyFrom.Visible = true;
            lblCopyFrom.Visible = true;
            pshCopyCancel.Visible = true;
            pshCopy.Visible = true;

            tableLayoutPanel1.Enabled = false;
            sleChar.Enabled = false;
            sleSymName.Enabled = false;
            pshSaveChar.Enabled = false;
            pshReset.Enabled = false;
            pshCopyFrom.Enabled = false;

            listView1.Enabled = false;

        }

        void _hideCopyFrom()
        {
            sleCopyFrom.Visible = false;
            lblCopyFrom.Visible = false;
            pshCopy.Visible = false;
            pshCopyCancel.Visible = false;

            tableLayoutPanel1.Enabled = true;
            sleChar.Enabled = true;
            sleSymName.Enabled = true;
            pshSaveChar.Enabled = true;
            pshReset.Enabled = true;
            pshCopyFrom.Enabled = true;

            listView1.Enabled = true;
        }

        private void pshCopyCancel_Click(object sender, EventArgs e)
        {
            _hideCopyFrom();
        }

        private void pshCopy_Click(object sender, EventArgs e)
        {
            int charPos = 0;

            if (int.TryParse(sleCopyFrom.Text.Trim(), out charPos) == false || charPos < 32 || charPos > 256)
                MessageBox.Show("You must input a value between 32 and 256.");
            else
            {
                CharDef from = charTable.Characters[charPos - 32];
                // mark Bits as selected...
                for (int row = 1; row <= 16; ++row)
                {
                    for (int col = 1; col <= 8; ++col)
                    {
                        Label lbl = (Label)tableLayoutPanel1.GetControlFromPosition(col - 1, row - 1);
                        ((dynamic)(lbl).Tag).IsSet = from.GetBit(row, col);
                        if (((dynamic)(lbl).Tag).IsSet)
                            lbl.BackColor = Color.Black;
                        else
                            lbl.BackColor = Color.White;
                    }
                }
                _hideCopyFrom();
            }
        }

        private void menFileName_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Do you want to create a new font? All unsaved changes on current font will be lost.", "QUESTION", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                lblStatus.Text = "Loading...";
                Application.DoEvents();

                charTable = new CharTable();

                _loadList();

                currChar = null;
                currIdx = 0;

                _loadChar();


                currFile = string.Empty;
                lblFont.Text = "<new font>";

                lblStatus.Text = "Ready...";
            };
        }

        private void menFileSaveAs_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog(this) == DialogResult.OK && string.IsNullOrEmpty(sfd.FileName) == false)
            {
                lblStatus.Text = "Saving...";
                Application.DoEvents();

                string msg = "";
                if (CharTable.SaveToFile(charTable, sfd.FileName, true, out msg) == false)
                    MessageBox.Show(this, "File could't not be saved. " + msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    currFile = sfd.FileName;
                    lblFont.Text = currFile;
                }

                lblStatus.Text = "Ready...";
            }
        }

        private void menFileSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currFile))
                menFileSaveAs_Click(null, EventArgs.Empty);

            string msg = "";

            lblStatus.Text = "Saving...";
            Application.DoEvents();

            if (CharTable.SaveToFile(charTable, currFile, true, out msg) == false)
                MessageBox.Show(this, "File could't not be saved. " + msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            lblStatus.Text = "Ready...";
        }

        private void menFileOpen_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog(this) == DialogResult.OK && string.IsNullOrEmpty(ofd.FileName) == false && System.IO.File.Exists(ofd.FileName))
            {
                lblStatus.Text = "Loading...";
                Application.DoEvents();

                string msg = "";
                CharTable fnt = CharTable.LoadFromFile(ofd.FileName, out msg);

                if (fnt == null)
                    MessageBox.Show(this, "File could't not be loaded. " + msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    currFile = ofd.FileName;
                    lblFont.Text = currFile;
                    charTable = fnt;

                    _loadList();

                    currChar = null;
                    currIdx = 0;

                    _loadChar();
                }

                lblStatus.Text = "Ready...";
            }

        }

        private void menFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Do you want to close this application? All unsaved changes on current font will be lost.", "QUESTION", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        private void pshClear_Click(object sender, EventArgs e)
        {
            if (currChar != null)
            {
                for (int row = 1; row <= 16; ++row)
                {
                    for (int col = 1; col <= 8; ++col)
                    {
                        Label lbl = (Label)tableLayoutPanel1.GetControlFromPosition(col - 1, row - 1);
                        ((dynamic)(lbl).Tag).IsSet = false;
                        lbl.BackColor = Color.White;
                    }
                }
            }
        }

        private void pshFill_Click(object sender, EventArgs e)
        {
            if (currChar != null)
            {
                for (int row = 1; row <= 16; ++row)
                {
                    for (int col = 1; col <= 8; ++col)
                    {
                        Label lbl = (Label)tableLayoutPanel1.GetControlFromPosition(col - 1, row - 1);
                        ((dynamic)(lbl).Tag).IsSet = true;
                        lbl.BackColor = Color.Black;
                    }
                }
            }
        }

        private void pshInvert_Click(object sender, EventArgs e)
        {
            if (currChar != null)
            {
                for (int row = 1; row <= 16; ++row)
                {
                    for (int col = 1; col <= 8; ++col)
                    {
                        Label lbl = (Label)tableLayoutPanel1.GetControlFromPosition(col - 1, row - 1);
                        ((dynamic)(lbl).Tag).IsSet = !((dynamic)(lbl).Tag).IsSet;
                        if (((dynamic)(lbl).Tag).IsSet)
                            lbl.BackColor = Color.Black;
                        else
                            lbl.BackColor = Color.White;
                    }
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string line = "Characters[#IDX#] = new CharDef { Character = '#CHAR#', SymbolCode = \"#CODE#\", CharBytes = new byte[] { #BYTES# } };";
            StringBuilder sb = new StringBuilder();
            int idx = 0;

            foreach (var chr in charTable.Characters)
            {
                StringBuilder strBytes = new StringBuilder();
                
                for (int i = 0; i <= 15; ++i)
                    strBytes.Append("0x" + Convert.ToString(chr.CharBytes[i], 16) + (i < 15 ? ", " : ""));

                sb.AppendLine(line.Replace("#IDX#", idx.ToString()).Replace("#CHAR#", chr.Character.ToString()).Replace("#CODE#", (idx == 0 || chr.SymbolCode != "SPACE" ? chr.SymbolCode : "")).Replace("#BYTES#", strBytes.ToString()));

                ++idx;
            }

            Clipboard.Clear();
            Clipboard.SetText(sb.ToString());

        }
    }
}
