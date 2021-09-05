using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C500Pro
{
    public partial class frMain : Form
    {
        public frMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            C500 ob = C500.ReadFromBinaryFile<C500>();
            Process.Start("chrome.exe", ob.HocTrucTuyen);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown", "/s /t 0");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            frmCauHinh frm = new frmCauHinh();
            frm.ShowDialog();
        }
    }
}
