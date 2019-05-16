using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TxtDownLoader.utils.encrypt;
using TxtDownLoader.utils.hooks;

namespace TxtDownLoader
{
    public partial class Form1 : BaseIconForm,IMessageFilter
    {
        public Form1()
        {
            InitializeComponent();
            //try {
            //    string id = ConfigurationManager.AppSettings["saveDir"];
            //    label1.Text = id;
            //    Console.WriteLine("readSetting:" + id);
            //} catch {

            //}
            tb_downloadDlay.KeyPress += new KeyPressEventHandler(this.tb_downloadDlay_KeyPress);
            tb_downloadDlay.KeyDown += new KeyEventHandler(this.tb_downloadDlay_KeyDown);
            tb_downloadDlay.MouseClick += new MouseEventHandler(this.tb_downloadDlay_MouseClick);
            //KeyAndMouseHook mouseKeyHook1 = new KeyAndMouseHook(true, true);//鼠标，键盘
            //mouseKeyHook1.KeyDown += new KeyEventHandler(Form1_KeyDown);
            //mouseKeyHook1.KeyUp += new KeyEventHandler(mouseKeyHook1_KeyUp);


            //mouseKeyHook1.KeyPress += new KeyPressEventHandler(mouseKeyHook1_KeyPress);
            //mouseKeyHook1.OnMouseActivity += new MouseEventHandler(Form1_MouseDown);
            //new EventHandler(tb_downloadDlay_KeyPress);
            //    this.button1.Click += new System.EventHandler(this.button1_Click);
            //Application.AddMessageFilter(this);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                return;
            }
        }
        public bool PreFilterMessage(ref System.Windows.Forms.Message myMessage)
        {
            Console.WriteLine("message==" + myMessage.Msg);
            //屏蔽鼠标双击
            if (myMessage.Msg == 515)
            {
                return true;
            }

            //屏蔽鼠标右键
            else if (myMessage.Msg >= 516 && myMessage.Msg <= 517)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

     
        //public partial class Form1 : Form
        //{
        
        //    public Form1()
        //    {
        //        InitializeComponent();

        //        mouseKeyHook1.KeyDown += new KeyEventHandler(Form1_KeyDown);
        //        //mouseKeyHook1.KeyPress += new KeyPressEventHandler(mouseKeyHook1_KeyPress);
        //        //mouseKeyHook1.KeyUp += new KeyEventHandler(mouseKeyHook1_KeyUp);
        //        mouseKeyHook1.OnMouseActivity += new MouseEventHandler(Form1_MouseDown);
        //    }
        //}

        //public event MouseEventHandlerImpl ;
        public delegate void MouseEventHandlerImpl(object sender, CustEventHandler e);

        public class CustEventHandler : EventArgs
        {

            public CustEventHandler()
            {
            }
        }
        private NovelDownLoader downLoader;
        bool start = false;
        private void tb_downloadDlay_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //e.KeyCode
            if (e.Button == MouseButtons.Right) {
                return;
            }
            
        }

        
        private void tb_downloadDlay_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //e.KeyCode
        }
        private void tb_downloadDlay_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Console.WriteLine("press="+e.KeyChar);
            //阻止从键盘输入键
            e.Handled = true;
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= 'a' && e.KeyChar <= 'z')//拦截a_z
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '\b')//这是允许输入退格键
            {
                e.Handled = false;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string targetUrl = textBox1.Text;

            //南山书院https://www.szyangxiao.com/
            //testUrl:https://www.szyangxiao.com/txt203988.shtml
            var Url = targetUrl;
            //BeginInvoke()
            string pages=textBox3.Text;
            //downLoader = new NovelDownLoader(Url, this); string pages;
            string strDelay = tb_downloadDlay.Text;
            downLoader = new NovelDownLoader(Url,pages, this,int.Parse(strDelay) );
          
            Func<string> export = () => downLoader.DownloadTxt();
            export.BeginInvoke((de) =>
            {
                string ret = export.EndInvoke(de);
                BeginInvoke(new Action<string>(showFinalBox), ret);
            }, null);
        }


        public void showFinalBox(string msg)
        {
            label1.Text = "下载到第:"+msg;

        }

        public void updateState(string msg)
        {
            textBox2.Text = msg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            start = false;
            if (downLoader != null) {
                downLoader.Stop();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DyttForm newForm = new DyttForm();
            newForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TuziwoForm tzwForm = new TuziwoForm();
            tzwForm.Show();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
