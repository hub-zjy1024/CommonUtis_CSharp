using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TxtDownLoader.configs;
using TxtDownLoader.utils;
using TxtDownLoader.utils.encrypt;

namespace TxtDownLoader.presenter
{
    public class TuziwoPresenter
    {
        private string rootUrl = "";
        BaseConfig confUser = new BaseConfig(BaseConfig.TAG_userINFO);
        public interface ITuziwoView {
            void showFinalBox(string msg);
            void sendData(Action<string> a, string ret);
            void updateState(Action<string, string> a, string ret,string ss);
            void updateResult(string res,string res2);
            void UpdateLogin(Dictionary<string, string> map);
            void LoginResult(Action<Dictionary<string, string>> action,Dictionary<string, string> map);
        }

        public TuziwoPresenter(string rootUrl, ITuziwoView mView)
        {
            this.rootUrl = rootUrl;
            this.mView = mView;
        }

        private ITuziwoView mView;

        public void StartGetSsrUrls(string url,string uid,string email,string key, string ip,string expirein) {

            Func<string> export = () =>GetNodesImpl(url, uid, email, key, ip, expirein);
            export.BeginInvoke((de) =>
            {
                string ret = export.EndInvoke(de);
                mView.sendData(new Action<string>(mView.showFinalBox), ret);
            }, null);
        }

       /// <summary>
       /// 登录
       /// </summary>
       /// <param name="email"></param>
       /// <param name="password"></param>
        public void LoginImpl (string email, string password)
        {
            Func<Dictionary<string, string>> export = () => Login(email, password);
            export.BeginInvoke((de) =>
                {
                    Dictionary<string, string> map = export.EndInvoke(de);
                    mView.LoginResult(new Action<Dictionary<string, string>>(mView.UpdateLogin), map);
                }, null);
        }

        /// <summary>
        /// 是否保存用户密码
        /// </summary>
        /// <returns></returns>
        public bool IsSaved() {
            string uanme = confUser.getKeyValue("savelogin");
            if ("1".Equals(uanme)) {
                return true;
            }
            return false;

        }
        public void SavedUserInfo(string username, string pwd,bool saved) 
        {
            string encrypteName = HexUtil.StrToHex(username);
            string encryptePwd = HexUtil.StrToHex(pwd);
            if (saved)
            {
                //PropertiesReader.saveData("uanme", encrypteName);
                //PropertiesReader.saveData("pwd", encryptePwd);
                //PropertiesReader.saveData("savelogin", "1");
                confUser.SavedData("uanme", encrypteName);
                confUser.SavedData("pwd", encryptePwd);
                confUser.SavedData("savelogin", "1");
              
            }
            else {
                //PropertiesReader.saveData("savelogin", "0");
                //PropertiesReader.saveData("uanme", "");
                //PropertiesReader.saveData("pwd", "");

                confUser.SavedData("uanme", "");
                confUser.SavedData("pwd", "");
                confUser.SavedData("savelogin", "0");
            }
         
        }
        /// <summary>
        /// 获取登录email
        /// </summary>
        /// <returns></returns>
        public string GetRealUname() {
       
            //string uanme=PropertiesReader.GetValue("uanme");
            string uanme= confUser.getKeyValue("uanme");
            string reName = HexUtil.HexToStr(uanme);
            return reName;
        }
        /// <summary>
        /// 获取保存的登录pwd
        /// </summary>
        /// <returns></returns>
        public string GetRealPwd()
        {
            //string upwd = PropertiesReader.GetValue("pwd"); 
            string upwd = confUser.getKeyValue("pwd");
            string rePwd = HexUtil.HexToStr(upwd);
            return rePwd;
        }
        private Dictionary<string, string> Login(string email, string password)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            try
            {
                string loginUrl = rootUrl + "/auth/login";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                httpWebRequest.Method = "POST";
                string reqBody = "email=" + email;
                //无法发送具有此谓词类型的内容正文
                reqBody += "&passwd=" + password;
                WebHeaderCollection headers = new WebHeaderCollection();
                headers.Add("Accept-Encoding", "gzip, deflate");
                headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                httpWebRequest.Headers = headers;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                httpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                httpWebRequest.UserAgent = "Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.26 Safari / 537.36 Core / 1.63.5478.400 QQBrowser / 10.1.1550.400";
                Stream reqStream = httpWebRequest.GetRequestStream();
                byte[] b = Encoding.UTF8.GetBytes(reqBody);
                reqStream.Write(b, 0, b.Length);
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                WebHeaderCollection resHeaders = response.Headers;
                response.Close();
                string[] keys = resHeaders.AllKeys;

                string uid = "";
                string key = "";
                string ip = "";
                string expirein = "";
                foreach (string k in keys)
                {
                    string value = resHeaders[k];
                    if ("Set-Cookie".Equals(k))
                    {
                        string keyEmail = "email";
                        string keyIp = "ip";
                        string keyKey = "key";
                        string keyExpire = "expire_in";
                        string keyUid = "uid";
                        string[] array = new string[] { "email", "ip", "key", "expire_in", "uid" };
                        int indexTag = -1;
                        foreach (string ts in array)
                        {
                            indexTag = value.IndexOf(ts);
                            if (indexTag != -1)
                            {
                                int fsIndex = value.IndexOf(";", indexTag);
                                string v1 = value.Substring(indexTag, fsIndex - indexTag);
                                string[] str = v1.Split(new char[] { '=' });
                                if (str.Length == 2)
                                {
                                    if ("uid".Equals(str[0]))
                                    {
                                        uid = str[1];
                                    }
                                    map.Add(str[0], str[1]);
                                }
                            }
                        }
                    }
                }
                if ("".Equals(uid))
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string rsStr = reader.ReadToEnd();
                    reader.Close();
                    map.Add("error", "请求结果有误：" + rsStr);
                }
                return map;
            }
            catch (Exception e)
            {
                string msg = "错误：" + e.Message + "\n" + e.StackTrace;
                map.Add("error", msg);
                return map;
            }

        }
        private string GetResponseBody(HttpWebResponse res)
        {
            Stream deStream = res.GetResponseStream();
            StreamReader dtReader = new StreamReader(deStream, Encoding.GetEncoding("UTF-8"));
            String dtResult = dtReader.ReadToEnd();
            dtReader.Close();
            return dtResult;
        }
        /// <summary>
        /// 根据登录信息获取ssr链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uid"></param>
        /// <param name="email"></param>
        /// <param name="key"></param>
        /// <param name="ip"></param>
        /// <param name="expirein"></param>
        /// <returns></returns>
        private string GetNodesImpl(string url, string uid, string email, string key, string ip, string expirein)
        {
            StringBuilder ssrBuilder = new StringBuilder("结果：\n");
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                CookieContainer cookies = new CookieContainer();
                string domin = httpWebRequest.Host;
                cookies.Add(new Cookie("uid", uid, "/", domin));
                cookies.Add(new Cookie("email", email, "/", domin));
                cookies.Add(new Cookie("key", key, "/", domin));
                cookies.Add(new Cookie("ip", ip, "/", domin));
                cookies.Add(new Cookie("expire_in", expirein, "/", domin));
                httpWebRequest.Method = "GET";
                httpWebRequest.CookieContainer = cookies;
                WebHeaderCollection headers = new WebHeaderCollection();
                //headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.9");
                httpWebRequest.Headers = headers;
                httpWebRequest.Referer = url;
                httpWebRequest.Accept = "text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8";
                httpWebRequest.UserAgent = "Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.26 Safari / 537.36 Core / 1.63.5478.400 QQBrowser / 10.1.1550.400";
                HttpWebResponse respone = (HttpWebResponse)httpWebRequest.GetResponse();
                String result = GetResponseBody(respone);
                respone.Close();
                string partern = "urlChange\\('\\d+',0,0\\)";
                MatchCollection items = Regex.Matches(result, partern);
                //http://tzwcenter.club/user/node/{ID}?ismu=0&relay_rule=0;
                string nodeDetailUrl = rootUrl + "/user/node/{0}?ismu=0&relay_rule=0";
                int counts = items.Count;
                for (int i = 0; i < counts; i++)
                {

                    Match m = items[i];
                    string matchStr = m.Value;
                    int index1 = matchStr.IndexOf("'");
                    int index2 = matchStr.IndexOf("'", index1 + 1);
                    string id = matchStr.Substring(index1 + 1, index2 - index1 - 1);
                    string nowUrl = string.Format(nodeDetailUrl, id);
                    HttpWebRequest nowReq = (HttpWebRequest)WebRequest.Create(nowUrl);
                    nowReq.Method = "GET";
                    nowReq.CookieContainer = cookies;
                    nowReq.Referer = nowUrl;
                    nowReq.Accept = "text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8";
                    nowReq.UserAgent = "Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.26 Safari / 537.36 Core / 1.63.5478.400 QQBrowser / 10.1.1550.400";
                    nowReq.Headers = headers;
                    HttpWebResponse deRes = (HttpWebResponse)nowReq.GetResponse();
                    string dtResult = GetResponseBody(deRes);
                    deRes.Close();
                    //string tag = "服务器地址";
                    //int indexServer = dtResult.IndexOf(tag);
                    //string tagEnd = "<br>";
                    //int endIndex = dtResult.IndexOf(tagEnd,indexServer );
                    //dtResult.Substring(indexServer+tag.Length, endIndex);
                    string dePartern = "ssr://\\w{1,200}";
                    MatchCollection dtItems = Regex.Matches(dtResult, dePartern);
                    if (dtItems.Count > 0)
                    {
                        string ssr = dtItems[0].Value;
                        //ssrBuilder.AppendLine(ssr);
                        string stat = "获取链接中：" + (i + 1) + "/" + counts;
                        mView.updateState(new Action<string, string>(mView.updateResult), stat, ssr);
                    }
                }
            }
            catch (Exception e)
            {
                string msg = "错误：" + e.Message + "\n" + e.StackTrace;
                return msg;
            }
            return ssrBuilder.ToString();
        }
    }
}
