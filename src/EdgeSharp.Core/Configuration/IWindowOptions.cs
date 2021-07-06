// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Drawing;

namespace EdgeSharp.Core.Configuration
{
    /// <summary>
    /// Represents a hosting window options.
    /// </summary>
    public interface IWindowOptions
    {
        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Gets or sets the window icon relative window icon.
        /// </summary>
        string RelativePathToIconFile { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the window should be created in kiosk mode.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        bool KioskMode { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the window should be created in fullscreen mode.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        bool Fullscreen { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the window should be centered when created.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        bool StartCentered { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the window should be created borderless.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        bool Borderless { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the window should be created with no resize option.
        /// </summary>
        bool NoResize { get; set; }
        /// <summary>
        /// Gets or sets a value setting the window minimum size.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        Size MinimumSize { get; set; }
        /// <summary>
        /// Gets or sets a value setting the window maximun size.
        /// </summary>
        /// <remarks>
        ///  Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        Size MaximumSize { get; set; }
        /// <summary>
        /// Gets or sets a high dpi mode.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        HighDpiMode HighDpiMode { get; set; }
        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
        /// <remarks>
        /// Applies on to Win32 EdgeSarp but can be used for WinForms, Wpf too.
        /// </remarks>
        WindowState WindowState { get; set; }
        /// <summary>
        /// Gets or sets window <see cref="BorderlessOption"/>.
        /// </summary>
        BorderlessOption BorderlessOption { get; set; }
    }
}