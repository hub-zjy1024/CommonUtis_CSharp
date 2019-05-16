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
using System.Threading.Tasks;
using System.Windows.Forms;
using TxtDownLoader.presenter;
using TxtDownLoader.utils;
using System.Web;
namespace TxtDownLoader
{
    public partial class TuziwoForm : Form, TuziwoPresenter.ITuziwoView
    {
        //private string rootUrl = "http://tzwcenter.club";
        private string rootUrl = "http://tzwssr.xyz";
        private TuziwoPresenter mPresenter;
        public TuziwoForm()
        {
            InitializeComponent();
           string surl= GetValue("rootUrl");
            if (!"".Equals(surl)) {
                rootUrl = surl;
            }
            mPresenter = new TuziwoPresenter(rootUrl, this);
            Init();
        }
        public static string GetValue(string key)
        {
          return  PropertiesReader.GetValue(key);
        }
        private void Init () {

            //HexUtil.Test(null);

            //string savedStr = GetValue("savelogin");
            //if ("1".Equals(savedStr)) {
            //    string username = mPresenter.GetRealUname();
            //    string pwd = mPresenter.GetRealPwd();
            //    textBox2.Text = username;
            //    textBox3.Text = pwd;
            //    checkBox1.Checked = true;
            //    //string email = textBox2.Text.ToString();
            //    //string password = textBox3.Text.ToString();
            //}
            if (mPresenter.IsSaved()) {
                string username = mPresenter.GetRealUname();
                string pwd = mPresenter.GetRealPwd();
                textBox2.Text = username;
                textBox3.Text = pwd;
                checkBox1.Checked = true;
            }

        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            string url = rootUrl+ "/user/node";
            string email = textBox2.Text.ToString();
            string password  = textBox3.Text.ToString();
            string uid = textBox4.Text.ToString();
            string key = textBox5.Text.ToString();
            string ip = textBox6.Text.ToString();
            string expirein = textBox7.Text.ToString();
            button2.Enabled = false;
            email = UrlEncode(email);
            mPresenter.StartGetSsrUrls(url, uid, email, key, ip, expirein);

        }
        public void showFinalBox(string msg) {
            textBox1.Text =textBox1.Text+"\n"+"获取完成："+ msg;
            button2.Enabled = true;
        }

        public  string UrlEncode(string str)
        {
            ///工程名上-> 右键,Add Reference-> 选择System.Web->OK
            //HttpUtility.UrlEncode(str, Encoding.UTF8);
            //UrlEncode m=new UrlEncode
            return str.Replace("@", "%40");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBox2.Text.ToString();
            string password = textBox3.Text.ToString();
            email = UrlEncode(email);
            mPresenter.LoginImpl(email, password);
            mPresenter.SavedUserInfo(email, password, checkBox1.Checked);
        }
        public void UpdateLogin(Dictionary<string, string> map) {
            if (map.ContainsKey("error"))
            {
                string error = map["error"];
                textBox1.Text = "登录失败："+error;
            }
            else {
                string uid = map["uid"];
                string key = map["key"];
                string ip = map["ip"];
                string expirein = map["expire_in"];
                textBox4.Text = uid;
                textBox5.Text = key;
                textBox6.Text = ip;
                textBox7.Text = expirein;
                long time = long.Parse(expirein);
                //time = time * 1000;

                //DateTime dm = MillTimeToDt(time);
                DateTime dm = ConvertTimeStampToDateTime(time);
                textBox1.Text = textBox1.Text + "\n超时时间=" + dm.ToLocalTime().ToString();
                //dm.ToLongTimeString();
                string email = textBox2.Text.ToString();
                string url = rootUrl + "/user/node";
                mPresenter.StartGetSsrUrls(url, uid, email, key, ip, expirein);
            }

        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        private DateTime ConvertTimeStampToDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.AddSeconds(timeStamp);
        }
        private DateTime MillTimeToDt(long time) {

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = time;
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            DateTime dm = new DateTime(time);
            return dm;
        }
        public void updateState(string updateStr,string ssr) {
            textBox1.Text = textBox1.Text + "\n" + ssr;
            label8.Text = updateStr;

        }

        public void sendData(Action<string> a, string ret)
        {
            BeginInvoke(a, ret);
        }
        public void updateState2(Delegate method, params object[] args) {
        }

        public void updateState(Action<string, string> a, string ret, string ss)
        {
            BeginInvoke(a, ret,ss);
        }

        public void updateResult(string res, string res2)
        {
            textBox1.Text = textBox1.Text + "\n" + res2;
            label8.Text = res;
        }

        public void LoginResult(Action<Dictionary<string, string>> action, Dictionary<string, string> map)
        {
            BeginInvoke(action, map);
        }
    }
}
