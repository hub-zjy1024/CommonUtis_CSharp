using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TxtDownLoader
{
    class NovelDownLoader
    {
        private string regxStr;
        private string urlPath;
        private string pages;
        private bool start = true;
        private Form1 createForm;
        private int downloadDelay= 400;

        public NovelDownLoader(string urlPath, Form1 createForm)
        {
            this.urlPath = urlPath;
            this.createForm = createForm;
        }

        public NovelDownLoader(string urlPath, string pages, Form1 createForm)
        {
            this.urlPath = urlPath;
            this.pages = pages;
            this.createForm = createForm;
        }
        public NovelDownLoader(string urlPath, string pages, Form1 createForm, int downloadDelay) : this(urlPath, pages, createForm)
        {
            this.downloadDelay = downloadDelay;
        }

        //public int DownloadDelay { get => downloadDelay; set => downloadDelay = value; }

        public string DownloadTxt()
        {
            int page1 = 1;
            int page2 = 1;
            int indexD = pages.IndexOf("-");
            if (indexD == -1)
            {
                page2 = int.Parse(pages);
            }
            else
            {
                page1 = int.Parse(pages.Substring(0, indexD));
                page2 = int.Parse(pages.Substring(indexD + 1));
            }
            start = true;
            WebRequest wReq = WebRequest.Create(urlPath);
            try{
                WebResponse wResp = wReq.GetResponse();
                Stream respStream = wResp.GetResponseStream();
                StringBuilder errorBuilder = new StringBuilder();
                int totalCount = 0;
                using (StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding("GBK")))
                {
                    var result = reader.ReadToEnd(); //result就是返回值
                                                   
                    int indexF = urlPath.LastIndexOf("/");
                    int indexE = urlPath.LastIndexOf(".");
                    string tradeTime = DateTime.Now.ToString("MMddHHmmss");
                    string name = urlPath.Substring(indexF + 1, indexE - indexF - 1) + "_" + tradeTime + ".txt";
                    string dir = "D:/Downloads";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    FileStream fs = new FileStream(dir + "/" + name, FileMode.OpenOrCreate);
                    int index1 = 0;

                    string partern = @"www.{0,40}packdown.{0,100}</a>";
                    StringBuilder builder2 = new StringBuilder();
                    MatchCollection items = Regex.Matches(result, partern);
                    int m = 0;
                    totalCount = items.Count;
                    DateTime time1 = DateTime.Now;
                    for (int i = page1; i < page2+1; i++) {
                        string matchStr=items[i].Value;
                             int indexA1 = matchStr.IndexOf("www", index1);
                        int indexHref = matchStr.IndexOf("</a>", index1);
                        string s = matchStr.Substring(indexA1, indexHref - indexA1);
                        string partern2 = @"《.+》";
                        Match name1 = Regex.Match(s, partern2);
                        string title = name1.Value;
                        string nUrl = "https://" + s.Substring(0, s.IndexOf("\""));
                        if (!start)
                        {
                            fs.Close();
                            return "终止下载";
                        }
                        try
                        {

                            WebResponse wResp2 = WebRequest.Create(nUrl).GetResponse();
                            Stream respStream2 = wResp2.GetResponseStream();
                            using (StreamReader reader2 = new StreamReader(respStream2, Encoding.GetEncoding("GBK")))
                            {
                                string tempStr = reader2.ReadToEnd(); //result就是返回值
                                tempStr = title + "\n" + tempStr;
                                builder2.AppendLine(tempStr);
                                byte[] b = System.Text.Encoding.UTF8.GetBytes(tempStr + "\n");
                                fs.Write(b, 0, b.Length);
                            }
                            Thread.Sleep(this.downloadDelay);
                            createForm.BeginInvoke(new Action<string>(createForm.showFinalBox),"下载到"+ i + "/" + totalCount);
                        }
                        catch (Exception ex)
                        {
                            DateTime time2 = DateTime.Now;
                            TimeSpan time = time2 - time1;
                            time1 = time2;
                            errorBuilder.AppendLine(string.Format("{0}下载失败!!距离上次失败时间为{2} 地址为:{1}，errmsg={3}", title, nUrl, time.TotalMilliseconds / (float)1000,ex.Message));
                            createForm.BeginInvoke(new Action<string>(createForm.updateState), errorBuilder.ToString());
                        }
                        m++;
                    }
                    fs.Close();
              
                }
                return "下载完成总的章节数为：" + totalCount + "\n" + errorBuilder.ToString(); ;
            } catch (Exception e){
                if (e is WebException)
                {
                    return "访问错误：" + e.Message + "\n" + e.StackTrace;
                }
                else {
                    return "其他错误：" + e.Message + "\n" + e.StackTrace;
                }
            }
        }
        public void Stop() {
            start = false;
        }
    }
}
