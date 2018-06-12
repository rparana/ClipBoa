using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipBoa
{
    public partial class FrmMain : Form
    {
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private string TextoAtual { get; set; }
        private string TxtFile { get; set; }
        private bool IsSelected { get; set; }

        //private Clipboard;

        IntPtr nextClipboardViewer;


        public FrmMain()
        {
            string[] args = Environment.GetCommandLineArgs();

            int arg = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (!int.TryParse(args[i], out arg))
                {
                    arg = 0;
                };
            }
            
            if (arg == 1)
                this.WindowState = FormWindowState.Minimized;

            InitializeComponent();
            TxtFile = @".\Clipboard.txt";
            TextoAtual = "";
            IsSelected = false;
            StartText();
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(1);
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        // Use this event handler for the FormClosing event.
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }

        private void StartText()
        {
            bool firstLine = true;
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                if (!File.Exists(TxtFile))
                {
                    File.Create(TxtFile);
                }

                //Continue to read until you reach end of file
                using (var reader = new StreamReader(TxtFile))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        lstCliBoard.Items.Insert(lstCliBoard.Items.Count, new ListViewItem(line));
                        if (firstLine)
                        {
                            TextoAtual = line;
                            firstLine = false;
                        }
                    }

                }

                //close the file
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.Message);
            }

        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void exibirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            exibirToolStripMenuItem_Click(Sender, e);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {

            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (!IsSelected)
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        void DisplayClipboardData()
        {

            string texto ="";
            try
            {
                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Rtf))
                    texto=(string)iData.GetData(DataFormats.Rtf);
                else if (iData.GetDataPresent(DataFormats.Text))
                    texto=(string)iData.GetData(DataFormats.Text);

                if (!String.IsNullOrEmpty(texto))
                {
                    if (texto != TextoAtual)
                    {
                        TextoAtual = texto;
                        lstCliBoard.Items.Insert(0, new ListViewItem(texto));
                        WriteNewLine(texto);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void WriteNewLine(string txt)
        {
            string tempfile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempfile))
            using (var reader = new StreamReader(TxtFile))
            {
                writer.WriteLine(txt);
                while (!reader.EndOfStream)
                    writer.WriteLine(reader.ReadLine());
            }
            File.Copy(tempfile, TxtFile, true);

        }

        private void LstClipBoard_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(ListViewItem lvi in lstCliBoard.SelectedItems)
            {
                if(TextoAtual != lvi.Text)
                {
                    TextoAtual = lvi.Text;
                    IsSelected = true;
                    Clipboard.SetText(lvi.Text);
                    IsSelected = false;
                }
            }
        }
    }
}
