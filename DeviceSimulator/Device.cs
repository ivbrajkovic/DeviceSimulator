using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DeviceSimulator
{
    public class NativeFunctionCallException : Exception
    {
        public NativeFunctionCallException()
        {
        }

        public NativeFunctionCallException(string message)
            : base(message)
        {
        }

        public NativeFunctionCallException(int lastError)
            : base($"Call to native API function failde with error code: {lastError}")
        {
        }

        public NativeFunctionCallException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class Device
    {
        #region Native WinAPI

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInputData mi;

            [FieldOffset(0)]
            public KeyboardInput ki;

            [FieldOffset(0)]
            public HardwareInput hi;
        }

        struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public KeybdEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HardwareInput
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        #endregion

        #region Enumeration

        enum SendInputEventType : int
        {
            InputMouse,
            InputKeyboard,
            InputHardware
        }

        [Flags]
        enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        [Flags]
        enum KeybdEventFlags : uint
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_UNICODE = 0x0004,
            KEYEVENTF_SCANCODE = 0x0008,
        }

        enum SystemMetric
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        #endregion

        /// <summary>
        /// Call to native find window function
        /// </summary>
        /// <param name="className">Class name, null if find by title</param>
        /// <param name="title">Title of window, null if find by class name</param>
        /// <returns>Handle to window</returns>
        public IntPtr FindWindowAPI(string className, string title)
        {
            return FindWindow(className, title);
        }

        /// <summary>
        /// Retreive absolute screen coordinate X
        /// </summary>
        /// <param name="x"></param>
        /// <returns>Converted coordinate x</returns>
        int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        /// <summary>
        /// Retreive absolute screen coordinate Y
        /// </summary>
        /// <param name="y"></param>
        /// <returns>Converted coordinate y</returns>
        int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }

        /// <summary>
        /// Perform mouse left button down
        /// </summary>
        public void MouseLeftDown()
        {
            uint error;
            INPUT mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
            error = SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));
            if (!Convert.ToBoolean(error))
                throw new NativeFunctionCallException(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Perform mouse left button up
        /// </summary>
        public void MouseLeftUp()
        {
            uint error;
            INPUT mouseUpInput = new INPUT();
            mouseUpInput.type = SendInputEventType.InputMouse;
            mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
            error = SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            if (!Convert.ToBoolean(error))
                throw new NativeFunctionCallException(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Perform mouse left button click
        /// </summary>
        public void MouseLeftClick()
        {
            this.MouseLeftDown();
            this.MouseLeftUp();
        }

        /// <summary>
        /// Perform left button double click
        /// </summary>
        public void MouseLeftDoubleClick()
        {
            this.MouseLeftClick();
            this.MouseLeftClick();
        }

        /// <summary>
        /// Perform mouse right button up
        /// </summary>
        public void MouseRightDown()
        {
            uint error;
            INPUT mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
            error = SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));
            if (!Convert.ToBoolean(error))
                throw new NativeFunctionCallException(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Perform mouse right button up
        /// </summary>
        public void MouseRightUp()
        {
            uint error;
            INPUT mouseUpInput = new INPUT();
            mouseUpInput.type = SendInputEventType.InputMouse;
            mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
            error = SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            if (!Convert.ToBoolean(error))
                throw new NativeFunctionCallException(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Perform mouse right button click
        /// </summary>
        public void MouseRightClick()
        {
            this.MouseRightDown();
            this.MouseRightClick();
        }

        /// <summary>
        /// Move cursor to screen absolute coordinate
        /// </summary>
        /// <param name="x">Coordinate X</param>
        /// <param name="y">Voordinate Y</param>
        public void MouseMoveToPosition(int x, int y)
        {
            uint error;
            INPUT mouseMove = new INPUT();
            mouseMove.type = SendInputEventType.InputMouse;
            mouseMove.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
            mouseMove.mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
            mouseMove.mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
            mouseMove.mkhi.mi.mouseData = 0;
            error = SendInput(1, ref mouseMove, Marshal.SizeOf(new INPUT()));
            if (!Convert.ToBoolean(error))
                throw new NativeFunctionCallException(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Send unicode string to focused control
        /// </summary>
        /// <param name="str">String to send to control</param>
        public void KeyboardSendString(string str)
        {
            uint error;

            // Construct list of inputs in order to send them through a single SendInput call at the end.
            List<INPUT> inputs = new List<INPUT>();

            // Loop through each Unicode character in the string.
            foreach (char c in str)
            {
                // First send a key down, then a key up.
                foreach (bool keyUp in new bool[] { false, true })
                {
                    // INPUT is a multi-purpose structure which can be used 
                    // for synthesizing keystrokes, mouse motions, and button clicks.
                    INPUT keybdInput = new INPUT();
                    keybdInput.type = SendInputEventType.InputKeyboard;
                    keybdInput.mkhi.ki.wVk = 0;
                    keybdInput.mkhi.ki.wScan = c;
                    keybdInput.mkhi.ki.dwFlags = KeybdEventFlags.KEYEVENTF_UNICODE | (keyUp ? KeybdEventFlags.KEYEVENTF_KEYUP : 0);
                    keybdInput.mkhi.ki.dwExtraInfo = GetMessageExtraInfo();

                    // Add to the list (to be sent later).
                    inputs.Add(keybdInput);
                }
            }

            // Send all inputs together using a Windows API call.
            error = SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(new INPUT()));
            //SendInput((uint)inputs.Count, ref t, Marshal.SizeOf(new INPUT()));
            if (!Convert.ToBoolean(error))
                throw new NativeFunctionCallException(Marshal.GetLastWin32Error());
        }
    }
}
