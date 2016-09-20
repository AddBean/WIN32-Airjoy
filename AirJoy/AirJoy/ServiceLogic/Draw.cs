using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;

namespace AirJoy.ServiceLogic
{
    class Draw
    {
        /// <summary>
        /// 通用画图方法；
        /// </summary>
        /// <param name="Form">当前所在窗体，一般用this</param>
        /// <param name="g">创建Graphics实体</param>
        public static void Draw2D(Form Form, Graphics g)
        {
            DateTime t1 = DateTime.Now;
            Bitmap bmp = new Bitmap(600, 600);
            System.Drawing.Drawing2D.LinearGradientBrush brush = null;
            brush = new LinearGradientBrush(new PointF(0.0f, 0.0f), new PointF(700.0f, 300.0f), Color.Red, Color.Blue);
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    g.FillEllipse(brush, i * 10, j * 10, 10, 10);
                }
            }
            Form.CreateGraphics().DrawImage(bmp, 0, 0);
        }
        /// <summary>
        /// 通用画图方法；
        /// </summary>
        /// <param name="Form">当前所在窗体，一般用this</param>
        /// <param name="g">创建Graphics实体</param>
        public static void DrawRound(Form Form, Graphics g,int x,int y,int w,int h)
        {
            DateTime t1 = DateTime.Now;
            Bitmap bmp = new Bitmap(100,100);
            System.Drawing.Drawing2D.LinearGradientBrush brush = null;
            g.SmoothingMode = SmoothingMode.HighQuality; //高质量
            g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量
            brush = new LinearGradientBrush(new PointF(0.0f, 0.0f), new PointF(700.0f, 300.0f), Color.Red, Color.Blue);
            //g.Clear(Form.BackColor);
            Form.Refresh();
            g.FillEllipse(brush, x,y, w, h);
            Form.CreateGraphics().DrawImage(bmp, 0, 0);
        }
        public static void ssss(Form Form, Graphics g)
        {
            int x, y, w, h, r, i;
            Bitmap bt = new Bitmap(400, 400);
            System.Random rnd = new Random();
            for (i = 0; i < 10000; i++)
            {
                x = rnd.Next(400);
                y = rnd.Next(400);
                r = rnd.Next(20);
                w = rnd.Next(10);
                h = rnd.Next(10);
                Form.Refresh();
                g.DrawEllipse(Pens.Blue, x, y, w, h);

            }
            Form.CreateGraphics().DrawImage(bt, new Point(0, 0)); 
        }
            
    }
}
