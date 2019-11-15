using OpenHardwareMonitor.Hardware;
using System;
using System.IO;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace Winform02
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Form이라는 Window 창 객체를 화면에 보여주고,
            // 메시지를 만들어서 마우스 키보드 등의 입력 수단을 통해
            // STAThread의 스레드에 전달하는 기능
            Application.Run(new Form1());
        }
        
    }

}
