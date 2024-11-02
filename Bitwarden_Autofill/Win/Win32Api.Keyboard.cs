using System.Runtime.InteropServices;
using System;

namespace Bitwarden_Autofill.Win;

internal partial class Win32Api
{
    /// <summary>
    /// Virtual-Key code for the SHIFT key.
    /// </summary>
    public const ushort VK_SHIFT = 0x10;

    /// <summary>
    /// Virtual-Key code for the CONTROL key.
    /// </summary>
    public const ushort VK_CONTROL = 0x11;

    /// <summary>
    /// Virtual-Key code for the MENU key (ALT key).
    /// </summary>
    public const ushort VK_MENU = 0x12;

    /// <summary>
    /// Specifies the type of the input event. This value is used for keyboard input.
    /// </summary>
    public const uint INPUT_KEYBOARD = 1;

    /// <summary>
    /// Flag to indicate a key release event.
    /// </summary>
    public const uint KEYEVENTF_KEYUP = 0x0002;

    /// <summary>
    /// Translates a character to the corresponding virtual-key code and shift state.
    /// </summary>
    /// <param name="ch">The character to be translated.</param>
    /// <returns>
    /// The return value specifies the virtual-key code and shift state as a SHORT value.
    /// The low byte contains the virtual-key code and the high byte contains the shift state,
    /// which can be a combination of the following flag bits:
    /// 1 - Either SHIFT key is pressed.
    /// 2 - Either CTRL key is pressed.
    /// 4 - Either ALT key is pressed.
    /// 8 - The Hankaku key is pressed.
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern short VkKeyScan(char ch);

    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks.
    /// </summary>
    /// <param name="nInputs">The number of structures in the pInputs array.</param>
    /// <param name="pInputs">An array of INPUT structures. Each structure represents an event to be inserted into the input stream.</param>
    /// <param name="cbSize">The size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
    /// <returns>
    /// The function returns the number of events that it successfully inserted into the input stream.
    /// If the function returns zero, the input was already blocked by another thread.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

    /// <summary>
    /// Represents an input event in the input stream.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        /// <summary>
        /// The type of the input event. This can be a keyboard, mouse, or hardware event.
        /// </summary>
        public uint type;

        /// <summary>
        /// The input data for the event. This can be a keyboard input, mouse input, or hardware input.
        /// </summary>
        public InputUnion u;
    }

    /// <summary>
    /// Represents a union of different input types (mouse, keyboard, hardware).
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        /// <summary>
        /// The mouse input data.
        /// </summary>
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        /// <summary>
        /// The keyboard input data.
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT ki;

        /// <summary>
        /// The hardware input data.
        /// </summary>
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    /// <summary>
    /// Represents the mouse input data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        /// <summary>
        /// The x-coordinate of the mouse event.
        /// </summary>
        public int dx;

        /// <summary>
        /// The y-coordinate of the mouse event.
        /// </summary>
        public int dy;

        /// <summary>
        /// Additional data associated with the mouse event.
        /// </summary>
        public uint mouseData;

        /// <summary>
        /// Flags that specify various aspects of mouse motion and button clicks.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// The time stamp for the event, in milliseconds.
        /// </summary>
        public uint time;

        /// <summary>
        /// Additional information associated with the mouse event.
        /// </summary>
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// Represents the keyboard input data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        /// <summary>
        /// The virtual-key code. This value must be a valid virtual-key code or 0 if no key is pressed.
        /// </summary>
        public ushort wVk;

        /// <summary>
        /// The hardware scan code of the key. This value is device-dependent.
        /// </summary>
        public ushort wScan;

        /// <summary>
        /// Specifies various aspects of a keystroke, such as key-up or key-down events.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.
        /// </summary>
        public uint time;

        /// <summary>
        /// Additional information associated with the keystroke. This value can be used by the application.
        /// </summary>
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// Represents the hardware input data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        /// <summary>
        /// The message generated by the hardware.
        /// </summary>
        public uint uMsg;

        /// <summary>
        /// The low-order word of the message parameter.
        /// </summary>
        public ushort wParamL;

        /// <summary>
        /// The high-order word of the message parameter.
        /// </summary>
        public ushort wParamH;
    }
}
