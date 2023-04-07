using System;
using System.Collections.Generic;
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
    internal class ClientAreaScreenshot
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hwnd, out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private extern static bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);


        /// <summary>
        /// ウィンドウのクライアント領域をスクリーンショットする
        /// </summary>
        /// <param name="handle">スクショするウィンドウのハンドル</param>
        /// <returns></returns>
        public Bitmap ScreenShot(IntPtr handle)
        {
            //ストップウォッチインスタンスの生成
            //var sw = new Stopwatch();

            //計測の開始
            //sw.Start();

            //クライアント座標（top,leftが0のやつ）
            RECT clientRect;
            GetClientRect(handle, out clientRect);

            //スクリーン座標（画面の左上が原点のやつ）
            POINT screenPoint;
            ClientToScreen(handle, out screenPoint);

            //ウィンドウサイズ取得
            RECT rect;
            GetWindowRect(handle, out rect);

            //枠込みのサイズ
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            //枠なしのサイズ
            int clientWidth = clientRect.right;
            int clientHeight = clientRect.bottom;

            //枠のleftとtopの座標
            int frameX = screenPoint.X - rect.left;
            int frameY = screenPoint.Y - rect.top;

            //切り抜くサイズ（クライアント領域）
            Rectangle rectangle = new Rectangle(frameX, frameY, clientWidth, clientHeight);

            using (Bitmap img = new Bitmap(width, height))
            {
                //ウィンドウをキャプチャする
                Graphics memg = Graphics.FromImage(img);
                IntPtr dc = memg.GetHdc();

                //https://stackoverflow.com/questions/30965343/printwindow-could-not-print-google-chrome-window-chrome-widgetwin-1
                PrintWindow(handle, dc, 0x00000002);

                //お片付け
                memg.ReleaseHdc(dc);
                memg.Dispose();

                // 画像を切り抜く
                using (Bitmap clippedImg = img.Clone(rectangle, img.PixelFormat))
                {
                    //計測の停止
                    //sw.Stop();

                    //MessageBox.Show(sw.ElapsedMilliseconds + "ms");

                    //デバッグ用
                    clippedImg.Save("img.bmp");
                    System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + @"\img.bmp");

                    return clippedImg;
                }
            }
        }
    }
}
