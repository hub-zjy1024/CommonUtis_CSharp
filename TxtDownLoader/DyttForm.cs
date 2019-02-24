using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TxtDownLoader
{
    public partial class DyttForm : Form
    {
        public DyttForm()
        {
            InitializeComponent();
            textBox2.Text = "1";
        }
        DyttExporter exporter;
        private void button1_Click(object sender, EventArgs e)
        {
            string regx = "<a href.{1,100}ulink.{1,100}</a>";
            string pages = this.textBox2.Text;
            string urlPath = textBox1.Text.Trim();
            exporter = new DyttExporter(regx, urlPath, pages,this);
            button1.Enabled = false;
            this.button2.Enabled = true;
            //Func<string> export = () => exporter.export();
            Func<string> export = new Func<string>(exporter.export);
            //Action<string> testa = new Action<string>(UpdateState);
            //testa.BeginInvoke("", (ad)=> {
               
            //}, this);
            export.BeginInvoke((de) =>
            {
                string ret = export.EndInvoke(de);
                string total = textBox3.Text +"\n"+ ret;
                BeginInvoke(new Action<string>(UpdateState), ret);
            }, null);
        }

        public void UpdateState(string obj)
        {
            label4.Text= obj;
            this.button2.Enabled = false;
            button1.Enabled = true;
        }
        public void printErrors(string msg) {
            textBox3.Text = msg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (exporter != null) {
                exporter.stop();
                this.button2.Enabled = false;
            }
        }
    }
}
