using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ScreenShotTest
{
    public partial class Form1 : Form
    {
        ClientAreaScreenshot clientAreaScreenshot = new ClientAreaScreenshot();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ウィンドウハンドル
            IntPtr handle = (IntPtr)0x0100D92;

            clientAreaScreenshot.ScreenShot(handle);
        }
    }
}
