using OpenHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Winform02
{
    public partial class Form1 : Form
    {
        System.Diagnostics.ProcessStartInfo proInfo = new System.Diagnostics.ProcessStartInfo();
        System.Diagnostics.Process pro = new System.Diagnostics.Process();
        static Computer _thisComputer;
        static FileStream _fs;
        static string res = "";
        /* Designer.cs에서 만들어놓은 버튼이나 텍스트 박스 등을 실제로 
            다루는 C# 소스 코드를 작성하는 역할입니다. */
        int amount = 5;
        long number = 0;
        string result;
        int sum = 0;
        bool flag = true;
        //private BackgroundWorker worker;

        public Form1()
        {
            //Execute();
            InitializeComponent();
        }
        private void getTemperature()
        {
            // OpenHardware 라이브러리 사용 참고 시작
            // https://www.sysnet.pe.kr/Default.aspx?mode=2&sub=0&detail=1&pageno=0&wid=11904&rssMode=1&wtype=0

            _thisComputer = new Computer() { CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, HDDEnabled = true };

            _thisComputer.Open();
            // 로그 저장
            // DateTime now = DateTime.Now;
            // string fileStart = now.ToString("yyyyMMdd_HHmmss") + ".log";
            // _fs = new FileStream(fileStart, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder log = new StringBuilder();

            foreach (var hardwareItem in _thisComputer.Hardware)
            {
                switch (hardwareItem.HardwareType)
                {
                    case HardwareType.CPU:
                    case HardwareType.GpuNvidia:
                    case HardwareType.HDD:
                    case HardwareType.Mainboard:
                    case HardwareType.RAM:
                        AddCpuInfo(sb, sb2, log, hardwareItem);
                        break;
                }
            }

            //log.AppendLine();
            res = sb.ToString();
            Console.WriteLine(res);
            /*
            byte[] buf = Encoding.ASCII.GetBytes(log.ToString());
            _fs.Write(buf, 0, buf.Length);
            _fs.Flush();*/

            // OpenHardware 라이브러리 사용 참고 끝
            string[] arr = res.Split('\n');

            amount = arr.Length - 2;
            for (int i = 0; i < arr.Length - 2; i++)
            {
                sum += Convert.ToInt32(arr[i]);
            }
            result = Convert.ToString(Convert.ToInt32(sum / amount));
            textBox2.Text = sb2.ToString();//"온도평균: " + result;
            sum = 0;

            Image test_image = DrawText(result, new Font("Verdana", 32), Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255, 255));
            pictureBox1.Image = test_image;

            Bitmap test_bitmap = new Bitmap(test_image);
            IntPtr Hicon = test_bitmap.GetHicon();
            Icon test_icon = Icon.FromHandle(Hicon);
            notifyIcon1.Icon = test_icon;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            getTemperature();
        }

        private static void AddCpuInfo(StringBuilder sb, StringBuilder sb2, StringBuilder log, IHardware hardwareItem)
        {
            hardwareItem.Update();
            foreach (IHardware subHardware in hardwareItem.SubHardware)
                subHardware.Update();

            string text;
            string text2;

            foreach (var sensor in hardwareItem.Sensors)
            {
                string name = sensor.Name;
                string value = sensor.Value.HasValue ? sensor.Value.Value.ToString() : "-1";

                switch (sensor.SensorType)
                {
                    case SensorType.Temperature:
                        text = $"{value}";
                        text2 = $"{name} Temperature = {value}";
                        sb.AppendLine(text);
                        sb2.AppendLine(text2);
                        break;
                    case SensorType.Voltage:
                        text2 = $"{name} Voltage = {value}";
                        sb2.AppendLine(text2);
                        break;

                    case SensorType.Fan:
                        text2 = $"{name} RPM = {value}";
                        sb2.AppendLine(text2);
                        break;

                    case SensorType.Load:
                        text2 = $"{name} % = {value}";
                        sb2.AppendLine(text2);
                        break;
                }
            }
        }

        private Image DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            flag = false;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Opacity = 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Opacity = 0;

            while(true)
            {
                if(!flag)
                {
                    break;
                }
                getTemperature();
                Thread.Sleep(1000);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 멈추고 끝내기
            //flag = false;
            // net stop WinRing0_1_2_0 커맨드 실행해야 함
            textBox3.Text = "종료되는 중...";
            textBox3.Update();

            Thread.Sleep(500);
            proInfo.FileName = @"cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            // cmd 데이터 받기
            proInfo.RedirectStandardOutput = true;
            // cmd 데이터 보내기
            proInfo.RedirectStandardInput = true;
            // cmd 오류내용 받기
            proInfo.RedirectStandardError = true;

            pro.StartInfo = proInfo;
            pro.Start();

            pro.StandardInput.Write(@"net stop WinRing0_1_2_0" + Environment.NewLine);
            pro.StandardInput.Close();
            pro.StandardOutput.ReadToEnd(); // return 값 string 있음 (결과 값)
            pro.WaitForExit();
            pro.Close();

            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            getTemperature();
        }
    }
}
