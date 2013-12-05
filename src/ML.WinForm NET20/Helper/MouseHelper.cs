using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ML.WinForm
{
    public class MouseHelper
    {
        #region dll类库调用

        [DllImport("User32.dll")]
        private static extern void mouse_event(MouseEvent me, int dx, int dy, uint data, UIntPtr extraInfo);

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("User32.dll")]
        private static extern bool GetCursorPos(out Point pt);

        #endregion

        public enum ButtonType
        {
            Left, Middle, Right
        }

        public enum Direction
        {
            Left, Right, Up, Down
        }

        private enum MouseEvent : uint
        {
            Move = 0x0001,

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
            Absolute = 0x8000
        }

        const uint WHEEL_DELTA = 120;

        #region 方法

        /// <summary>
        /// 移动鼠标指针到当前位置
        /// </summary>
        /// <param name="pt"></param>
        public void MoveTo(Point pt)
        {
            MoveTo(pt.X, pt.Y);
        }
        public void MoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void Move(Direction dirc)
        {
            Point curPt;
            if (GetCursorPos(out curPt))
            {
                switch (dirc)
                {
                    case Direction.Left:
                        SetCursorPos(curPt.X - 1, curPt.Y);
                        break;

                    case Direction.Right:
                        SetCursorPos(curPt.X + 1, curPt.Y);
                        break;

                    case Direction.Down:
                        SetCursorPos(curPt.X, curPt.Y + 1);
                        break;

                    case Direction.Up:
                        SetCursorPos(curPt.X, curPt.Y - 1);
                        break;
                }
            }
        }

        /// <summary>
        /// 在当前位置单击，默认鼠标左键
        /// </summary>
        public void Click()
        {
            Click(ButtonType.Left);
        }

        public void Click(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Left:
                    mouse_event(MouseEvent.LeftDown, 0, 0, 0, UIntPtr.Zero);
                    mouse_event(MouseEvent.LeftUp, 0, 0, 0, UIntPtr.Zero);
                    break;

                case ButtonType.Middle:
                    mouse_event(MouseEvent.MiddleDown, 0, 0, 0, UIntPtr.Zero);
                    mouse_event(MouseEvent.MiddleUp, 0, 0, 0, UIntPtr.Zero);
                    break;

                case ButtonType.Right:
                    mouse_event(MouseEvent.RightDown, 0, 0, 0, UIntPtr.Zero);
                    mouse_event(MouseEvent.RightUp, 0, 0, 0, UIntPtr.Zero);
                    break;
            }
        }


        /// <summary>
        /// 在当前位置双击，默认鼠标左键
        /// </summary>
        public void DoubleClick()
        {
            DoubleClick(ButtonType.Left);
        }

        public void DoubleClick(ButtonType type)
        {
            Click(type);
            Click(type);
        }

        /// <summary>
        /// wheel the mouse wheel
        /// </summary>
        /// <param name="num"> 
        /// A positive value indicates that the wheel was rotated forward, away from the user; 
        /// a negative value indicates that the wheel was rotated backward, toward the user
        /// </param>
        public void Wheel(uint num)
        {
            mouse_event(MouseEvent.Wheel, 0, 0, num * WHEEL_DELTA, UIntPtr.Zero);
        }

        /// <summary>
        /// 鼠标按下，默认鼠标左键
        /// </summary>
        public void MouseDown()
        {
            MouseDown(ButtonType.Left);
        }

        public void MouseDown(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Left:
                    mouse_event(MouseEvent.LeftDown, 0, 0, 0, UIntPtr.Zero);
                    break;

                case ButtonType.Middle:
                    mouse_event(MouseEvent.MiddleDown, 0, 0, 0, UIntPtr.Zero);
                    break;

                case ButtonType.Right:
                    mouse_event(MouseEvent.RightDown, 0, 0, 0, UIntPtr.Zero);
                    break;
            }
        }

        /// <summary>
        /// 鼠标升起，默认鼠标左键
        /// </summary>
        public void MouseUp()
        {
            MouseUp(ButtonType.Left);
        }

        public void MouseUp(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Left:
                    mouse_event(MouseEvent.LeftUp, 0, 0, 0, UIntPtr.Zero);
                    break;

                case ButtonType.Middle:
                    mouse_event(MouseEvent.MiddleUp, 0, 0, 0, UIntPtr.Zero);
                    break;

                case ButtonType.Right:
                    mouse_event(MouseEvent.RightUp, 0, 0, 0, UIntPtr.Zero);
                    break;
            }
        }
        #endregion



        /*
        #region 鼠标API
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="X">目标x坐标</param>
        /// <param name="Y">目标y坐标</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        /// <summary>
        /// 鼠标事件
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="cButtons"></param>
        /// <param name="dwExtraInfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        const int MMove = 0x0001;      //移动鼠标 
        const int LeftDown = 0x0002; //模拟鼠标左键按下 
        const int LeftUp = 0x0004; //模拟鼠标左键抬起 
        const int RightDown = 0x0008;// 模拟鼠标右键按下 
        const int RightUp = 0x0010;// 模拟鼠标右键抬起 
        const int MiddleDown = 0x0020;// 模拟鼠标中键按下 
        const int MiddleUp = 0x0040;// 模拟鼠标中键抬起 
        const int XDown = 0x0080;
        const int XUp = 0x0100;
        const int Wheel = 0x0800;
        const int VirtualDesk = 0x4000;
        const int Absolute = 0x8000;// 标示是否采用绝对坐标 
        #endregion


        /// <summary>
        /// 向下或向上滑动间距(下为负)
        /// </summary>
        /// <param name="x"></param>
        private void BDMouseWheel(int x)
        {
            int TempMY = 0;
            int TempMY2 = 0;
            while (true)
            {
                if (MainForm.StopAll)
                    return;
                OutNum = 60000;
                System.Threading.Thread.Sleep(100);
                if (TempMY == x)
                {
                    return;
                }
                if (x < 0)
                    TempMY2 = new Random().Next(x - TempMY, 0);
                else
                    TempMY2 = new Random().Next(1, x - TempMY);
                mouse_event(Wheel, 0, 0, TempMY2, 0);
                TempMY += TempMY2;
            }
        }


        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="EndP">目标坐标</param>
        /// <returns>false即没有到达目的</returns>
        private void MouseMove(Point EndP)
        {
            if (EndP.X > Screen.PrimaryScreen.WorkingArea.Size.Width - 50)
            {
                EndP.X = Screen.PrimaryScreen.WorkingArea.Size.Width - 50;
            }
            if (EndP.Y > Screen.PrimaryScreen.WorkingArea.Size.Height - 50)
            {
                EndP.Y = Screen.PrimaryScreen.WorkingArea.Size.Height - 50;
            }
            if (EndP.X <= 0)
            {
                EndP.X = 50;
            }
            if (EndP.Y <= 0)
            {
                EndP.Y = 50;
            }
            //SetCursorPos
            Point NowMouseP = new Point(Control.MousePosition.X, Control.MousePosition.Y);
            Random Rd = new Random(GetRandomSeed());//0,100
            int spl = 50;
            int spIx = 0;
            int spIy = 0;
            bool Xb = false;
            bool Yb = false;
            int Spx = 0;
            int Spy = 0;
            while (NowMouseP != EndP)
            {
                if (MainForm.StopAll)
                    return;
                OutNum = 60000;
                System.Threading.Thread.Sleep(Rd.Next(20, 50));
                NowMouseP = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                Spx = NowMouseP.X - EndP.X;
                Spy = NowMouseP.Y - EndP.Y;
                if (Spx > 0)
                    Xb = true;
                else
                    Xb = false;
                if (Spy > 0)
                    Yb = true;
                else
                    Yb = false;
                Spx = System.Math.Abs(Spx);
                Spy = System.Math.Abs(Spy);
                if (Spx > Spy && Spy > 0 && Spx > 0)
                    spIx = (Spx * spl) / Spy;
                else if (Spx <= Spy && Spy > 0 && Spx > 0)
                    spIy = (Spy * spl) / Spx;
                else
                {

                }
                SetCursorPos(NowMouseP.X == EndP.X ? NowMouseP.X : (NowMouseP.X + (Xb ? -(Rd.Next(Spx > spIx ? spIx : Spx) + 1) : (Rd.Next(Spx > spIx ? spIx : Spx) + 1))) , NowMouseP.Y == EndP.Y ? NowMouseP.Y : (NowMouseP.Y + (Yb ? -(Rd.Next(Spy > spIy ? spIy : Spy) + 1) : (Rd.Next(Spy > spIy ? spIy : Spy) + 1))));
            }
        }
        
        /// <summary>
        /// 随机轴
        /// </summary>
        /// <returns></returns>
        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        private void ClickMouse()
        {
            mouse_event(LeftDown, 0, 0, 0, 0);
            System.Threading.Thread.Sleep(50);
            mouse_event(LeftUp, 0, 0, 0, 0);
        }
        */
    }
}