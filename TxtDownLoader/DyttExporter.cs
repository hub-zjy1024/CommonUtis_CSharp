using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TxtDownLoader
{
    class DyttExporter
    {
        //<a.{1,100}ulink.{1,100}</a>
        private string regxStr;
        private string urlPath;
        private string pages;
        private bool start = true;
        private DyttForm createForm;
        public DyttExporter(string regxStr, string urlPath, string pages)
        {
            this.regxStr = regxStr;
            this.urlPath = urlPath;
            this.pages = pages;
        }

        public DyttExporter(string regxStr, string urlPath, string pages, DyttForm createForm) : this(regxStr, urlPath, pages)
        {
            this.createForm = createForm;
        }

        public string export() {
            string url = urlPath;

            StringBuilder errorBuilder = new StringBuilder();
            int totalCount = 0;
            int indexD = pages.IndexOf("-");
            int page1 = 1;
            int page2 = 1;
            if (indexD == -1)
            {
                page2 = int.Parse(pages);
            }
            else {
                page1 = int.Parse(pages.Substring(0, indexD));
                page2 = int.Parse(pages.Substring(indexD + 1));
            }
            string addTag = url.Substring(url.LastIndexOf("/") + 1);
            string name = string.Format("dytt_{0}_{1}_{2}.txt", addTag, page1, page2);
            string dir = "D:/Downloads";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileStream fs = new FileStream(dir + "/" + name, FileMode.OpenOrCreate);
            try
            {

                for (int i = page1; i < page2 + 1; i++) {
                    if (!start) {
                        break;
                    }
                    string tempUrl = url + "_" + i + ".html";
                    WebRequest wReq = WebRequest.Create(tempUrl);
                    wReq.Timeout = 30 * 1000;
                    AddReqCommonHeaders(wReq);
                    WebResponse wResp = wReq.GetResponse();
                    Stream respStream = wResp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding("GBK")))
                    {
                        var result = reader.ReadToEnd(); //result就是返回值
                        string partern = regxStr;
                        StringBuilder builder2 = new StringBuilder();
                        int m = 0;
                        MatchCollection items = Regex.Matches(result, partern);
                        totalCount = items.Count;
                        if (totalCount == 0) {
                            Match titleMatch = Regex.Match(result, "<title>.{1,100}</title>");
                            throw new Exception("" + titleMatch.Value);
                        }
                        DateTime time1 = DateTime.Now;
                        foreach (Match mt in items) {
                            if (!start)
                            {
                                break;
                            }
                            string matchStr = mt.Value;
                            int yh1 = matchStr.IndexOf("\"");
                            int yh2 = matchStr.IndexOf("\"", yh1 + 1);
                            string childAdd = matchStr.Substring(yh1 + 1, yh2 - yh1 - 1);
                            int h1 = tempUrl.IndexOf("w");
                            int h2 = tempUrl.IndexOf("/", h1);
                            string head = tempUrl.Substring(0, h2);
                            string deatilUrl = head + childAdd;
                            try
                            {
                                time1 = DateTime.Now;
                                WebRequest req2 = WebRequest.Create(deatilUrl);
                                AddReqCommonHeaders(req2);
                                WebResponse wResp2 = req2.GetResponse();
                                Stream respStream2 = wResp2.GetResponseStream();
                                using (StreamReader reader2 = new StreamReader(respStream2, Encoding.GetEncoding("GBK")))
                                {
                                    string tempStr = reader2.ReadToEnd(); //result就是返回值
                                    string specialTag = "<div id=\"Zoom\">";
                                    int content1 = tempStr.IndexOf(specialTag);
                                    if (content1 == -1) {
                                        throw new Exception("No tag:" + specialTag);
                                    }
                                    int content2 = tempStr.IndexOf("</div>", content1);
                                    string uesfulContent = tempStr.Substring(content1, content2 - content1);
                                    string tagYM = "◎译　　名";
                                    string tagRealName = "◎片　　名";
                                    uesfulContent = uesfulContent.Replace("&middot;", "");
                                    string tagType = "◎类　　别";
                                    string tagDate = "◎年　　代";
                                    string tagCountry = "◎产　　地";
                                    string tagManager = "◎导　　演";
                                    string tagIntroduce = "◎简　　介";
                                    string tagIMDb = "◎IMDb评分";
                                    string tagDouban = "◎豆瓣评分";
                                    builder2.AppendLine(tagYM + getValueByTag(uesfulContent, tagYM));
                                    builder2.AppendLine(tagRealName + getValueByTag(uesfulContent, tagRealName));
                                    builder2.AppendLine(tagType + getValueByTag(uesfulContent, tagType));
                                    builder2.AppendLine(tagDate + getValueByTag(uesfulContent, tagDate));
                                    builder2.AppendLine(tagCountry + getValueByTag(uesfulContent, tagCountry));
                                    builder2.AppendLine(tagManager + getValueByTag(uesfulContent, tagManager));
                                    builder2.AppendLine(tagIMDb + getValueByTag(uesfulContent, tagIMDb));
                                    builder2.AppendLine(tagDouban + getValueByTag(uesfulContent, tagDouban));
                                    int k = uesfulContent.IndexOf(tagIntroduce);
                                    int introEnd = uesfulContent.IndexOf("<br", k + 20);
                                    string introStr = uesfulContent.Substring(k, introEnd - k).Replace("<br />", "");
                                    builder2.AppendLine(introStr);
                                    builder2.AppendLine("下载链接：" + getATagAddress(uesfulContent));
                                    builder2.AppendLine("链接：" + deatilUrl);
                                    builder2.AppendLine("");
                                }
                                Thread.Sleep(200);
                                createForm.BeginInvoke(new Action<string>(createForm.UpdateState), string.Format("状态(rows:{0}/{1})(pages:{2}/{3})", m, totalCount, i, page2));
                            }
                            catch (Exception ex)
                            {
                                DateTime time2 = DateTime.Now;
                                TimeSpan time = time2 - time1;
                                time1 = time2;
                                string msg = ex.Message;
                                if (ex is WebException) {
                                    msg = string.Format("{0}下载失败!!请求间隔{2} 地址为:{1}", i, deatilUrl, time.TotalMilliseconds / (float)1000);
                                }
                                errorBuilder.AppendLine(msg);
                                errorBuilder.AppendLine("detail:" + ex.StackTrace);
                                createForm.BeginInvoke(new Action<string>(createForm.printErrors), errorBuilder.ToString());
                            }
                            m++;
                        }
                        byte[] b = System.Text.Encoding.UTF8.GetBytes(builder2.ToString() + "\n---------PAGE:" + i);
                        fs.Write(b, 0, b.Length);
                    }
                }
                fs.Close();
            }
            catch (Exception totalE)
            {
                return "error:" + totalE.Message + "\ndetail:" + totalE.StackTrace;
            };
            return "完成所有任务！！" + url + "页数" + pages;
        }
        public void stop()
        {

            start = false;
        }
        public string getValueByTag(string content, string tag) {
            int index1 = content.IndexOf(tag);
            if (index1 == -1) {
                return "未知";
            }
            int endIndex = content.IndexOf("<br", index1);
            return content.Substring(index1 + tag.Length, endIndex - (index1 + tag.Length));
        }
        private string getATagAddress(string content) {
            //< a href =
            string tag = "<a href=";
            int index1 = content.IndexOf(tag);
            int d1 = content.IndexOf("\"", index1);
            int index2 = content.IndexOf("\"", d1 + 1);
            return content.Substring(d1 + 1, index2 - d1 - 1);
        }
        private void AddReqCommonHeaders(WebRequest req)
        {
            //HttpRequestHeader head1 = new HttpRequestHeader();
            req.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko)" +
                " Chrome/63.0.3239.26 Safari/537.36 Core/1.63.5478.400 QQBrowser/10.1.1550.400");
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.9");
            req.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");

            }
        private void AddReferer(WebRequest req,string refererUrl)
        {
            req.Headers.Add(HttpRequestHeader.Referer, refererUrl);
        }
    }
    
}
