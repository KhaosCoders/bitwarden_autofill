using System;

namespace Bitwarden_Autofill.Hotkey;

/// <summary>
/// Provides data for the message event.
/// </summary>
/// <param name="hWnd">A handle to the window whose window procedure received the message.</param>
/// <param name="uMsg">Specifies the message.</param>
/// <param name="wParam">Additional message information. The content of this parameter depends on the value of the uMsg parameter.</param>
/// <param name="lParam">Additional message information. The content of this parameter depends on the value of the uMsg parameter.</param>
internal class MessageEventArgs(nint hWnd, uint uMsg, nint wParam, nint lParam) : EventArgs
{
    /// <summary>
    /// Gets the handle to the window whose window procedure received the message.
    /// </summary>
    public nint HWnd { get; } = hWnd;

    /// <summary>
    /// Gets the message.
    /// </summary>
    public uint Message { get; } = uMsg;

    /// <summary>
    /// Gets additional message information. The content of this property depends on the value of the Message property.
    /// </summary>
    public nint WParam { get; } = wParam;

    /// <summary>
    /// Gets additional message information. The content of this property depends on the value of the Message property.
    /// </summary>
    public nint LParam { get; } = lParam;

    /// <summary>
    /// Gets or sets the result of the message processing.
    /// </summary>
    public virtual nint? Result { get; set; }
}
