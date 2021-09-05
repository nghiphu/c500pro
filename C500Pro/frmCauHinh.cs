using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C500Pro
{
    public partial class frmCauHinh : Form
    {
        C500 ob;
        public frmCauHinh()
        {
            InitializeComponent();
        }

        private void frmCauHinh_Load(object sender, EventArgs e)
        {
            ob = new C500();
            try
            {
                ob = C500.ReadFromBinaryFile<C500>();
                txtLink.Text = ob.HocTrucTuyen;
                foreach (string t in ob.UngDung)
                {
                    txtListApp.Text += t;
                    txtListApp.Text += "\n";
                }
            }
            catch (Exception)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string []ss = txtListApp.Text.Split('\n');
            if (ob == null)
                ob = new C500();
            foreach (string s in ss)
                if (!string.IsNullOrEmpty(s.Trim()))
                {
                    ob.ThemUngDung(s.Trim());                
                }

            ob.HocTrucTuyen = txtLink.Text;
            C500.WriteToBinaryFile<C500>(ob);
        }
    }
}
