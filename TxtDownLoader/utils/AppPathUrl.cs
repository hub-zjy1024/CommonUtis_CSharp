using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TxtDownLoader.utils
{
    class AppPathUtil
    {


        public void Test() {
            string str1 = Process.GetCurrentProcess().MainModule.FileName;//可获得当前执行的exe的文件名。 
            string str2 = Environment.CurrentDirectory;//获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。(备注:按照定义，如果该进程在本地或网络驱动器的根目录中启动，则此属性的值为驱动器名称后跟一个尾部反斜杠（如“C:\”）。如果该进程在子目录中启动，则此属性的值为不带尾部反斜杠的驱动器和子目录路径[如“C:\mySubDirectory”])。 
            string str3 = Directory.GetCurrentDirectory(); //获取应用程序的当前工作目录。 

            string str5 = Application.StartupPath;//获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。 
            string str6 = Application.ExecutablePath;//获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。

            string str4 = AppDomain.CurrentDomain.BaseDirectory;//获取基目录，它由程序集冲突解决程序用来探测程序集。 
            string str7 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。
        }
    }
}
