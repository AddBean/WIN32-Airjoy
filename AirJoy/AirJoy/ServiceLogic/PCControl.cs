using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AirJoy
{
    class PCControl
    {
        #region 键盘事件
        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="bVk">按键实体</param>
        /// <param name="bScan">硬件扫描指令</param>
        /// <param name="dwFlags"> 标志指定各种功能选项</param>
        /// <param name="dwExtraInfo"> 按键的附加数据</param>
        /// bvk为键值，例如回车13，bScan设置为0，dwFlags设置0表示按下，2表示抬起；dwExtraInfo也设置为0即可。
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        #endregion
       
        #region 鼠标操作
        /// <summary>
        /// 鼠标事件
        /// </summary>
        /// <param name="flags">事件类型</param>
        /// <param name="dx">x坐标值(0~65535)</param>
        /// <param name="dy">y坐标值(0~65535)</param>
        /// <param name="data">滚动值(120一个单位)</param>
        /// <param name="extraInfo">不支持</param>
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, int data, UIntPtr extraInfo);
        /// <summary>
        /// 鼠标操作标志位集合
        /// </summary>
        [Flags]
        enum MouseEventFlag : uint
        {
            /// <summary>
            /// 鼠标移动事件
            /// </summary>
            Move = 0x0001,

            /// <summary>
            /// 鼠标左键按下事件
            /// </summary>
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            /// <summary>
            /// 设置鼠标坐标为绝对位置（dx,dy）,否则为距离最后一次事件触发的相对位置
            /// </summary>
            Absolute = 0x8000
        }
        /// <summary>
        /// 触发鼠标事件
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DoMouseMove(int dx, int dy)
        {

            int x = Control.MousePosition.X + (dx+20) / 15;
            int y = Control.MousePosition.Y + (dy+20) / 15;
            int x1 = (int)((double)x / Screen.PrimaryScreen.Bounds.Width * 65535); //屏幕分辨率映射到0~65535(0xffff,即16位)之间
            int y1 = (int)((double)y / Screen.PrimaryScreen.Bounds.Height * 0xffff); //转换为double类型运算，否则值为0、1
            mouse_event(MouseEventFlag.Move | MouseEventFlag.Absolute, x1, y1, 0, new UIntPtr(0)); //点击
        }
        /// <summary>
        /// 移动绝对距离
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public static void DoMouseSet(int x, int y)
        {
            int x1 = (int)((double)x / Screen.PrimaryScreen.Bounds.Width * 65535); //屏幕分辨率映射到0~65535(0xffff,即16位)之间
            int y1 = (int)((double)y / Screen.PrimaryScreen.Bounds.Height * 0xffff); //转换为double类型运算，否则值为0、1
            mouse_event(MouseEventFlag.Move | MouseEventFlag.Absolute, x1, y1, 0, new UIntPtr(0)); //点击
        }
        /// <summary>
        /// 触发鼠标事件
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DoMouseClick(string Event)
        {
            switch (Event)
            {
                case "LDOWN":
                     mouse_event(MouseEventFlag.LeftDown , 0, 0, 0, new UIntPtr(0)); //点击
                    break;
                case "LUP":
                     mouse_event(MouseEventFlag.LeftUp , 0, 0, 0, new UIntPtr(0)); //点击
                    break;
                case "RDOWN":
                     mouse_event( MouseEventFlag.RightDown, 0, 0, 0, new UIntPtr(0)); //点击
                    break;
                case "RUP":
                     mouse_event(MouseEventFlag.RightUp , 0, 0, 0, new UIntPtr(0)); //点击
                    break;
                case "WDOWN":
                    mouse_event(MouseEventFlag.Wheel, 0, 0, -120,new UIntPtr(0)); //点击
                    break;
                case "WUP":
                    mouse_event(MouseEventFlag.Wheel, 0, 0, 120, new UIntPtr(0)); //点击
                    break;
            } 
        }
        /// <summary>
        /// 触发鼠标事件
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DoMouseWheel(int dy)
        {
            mouse_event(MouseEventFlag.Wheel, 0, 0, dy, new UIntPtr(0)); //点击
        }
        #endregion
        
    }
}
