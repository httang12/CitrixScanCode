using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace CitrixScanCodeKeyReplication
{
    class CitrixScanCode
    {

        // replicate strucs
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }


        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        public struct INPUT
        {
            public int type;
            public ALLINPUTS ki;
        }

        /// <summary>
        /// http://social.msdn.microsoft.com/Forums/en/csharplanguage/thread/f0e82d6e-4999-4d22-b3d3-32b25f61fb2a
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        internal struct ALLINPUTS
        {
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        // CITRIX HACK
        // Function used to get the scan code
        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("User32.dll")]
        private static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] input, int structSize);


        /// <summary>
        /// Calls the Win32 SendInput method ... 0X0D is ENTER KEY
        /// </summary>
        /// <param name="keyCode">The VirtualKeyCode to press</param>

        public void CITRIXEnterKeyPress(ushort keyCode) //prev public static void
        {
            ushort entercode = keyCode;

            INPUT enterInputDown = new INPUT();
            enterInputDown.type = 1;
            enterInputDown.ki.Keyboard.time = 0;
            enterInputDown.ki.Keyboard.wScan = (ushort)MapVirtualKey((UInt16)keyCode, 0);
            enterInputDown.ki.Keyboard.wVk = keyCode;
            enterInputDown.ki.Keyboard.dwFlags = 0;
            enterInputDown.ki.Keyboard.dwExtraInfo = IntPtr.Zero;

            /*
            INPUT enterInputUp = new INPUT();
            enterInputDown.type = 1;
            enterInputDown.ki.Keyboard.time = 0;
            enterInputDown.ki.Keyboard.wScan = (ushort)MapVirtualKey((UInt16)keyCode, 0);
            enterInputDown.ki.Keyboard.wVk = keyCode;
            enterInputDown.ki.Keyboard.dwFlags = 0x0002;
            enterInputDown.ki.Keyboard.dwExtraInfo = IntPtr.Zero;

            */
            INPUT[] inputList = new INPUT[1];
            inputList[0] = enterInputDown;
            //inputList[1] = enterInputUp;


            var numberOfSuccessfulSimulatedInputs = SendInput(1,
                inputList, Marshal.SizeOf(typeof(INPUT)));

            int error = Marshal.GetLastWin32Error();
            System.Diagnostics.Debug.WriteLine(numberOfSuccessfulSimulatedInputs.ToString() + " " + error.ToString());
        }
    }

}
