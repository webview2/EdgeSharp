// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;

namespace EdgeSharp.Core.Infrastructure
{
    /// <summary>
    /// Application settings - for saving and reading application properties.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        /// <remarks>
        /// Note: this is used in naming the configuration file when app settings are saved.
        /// </remarks>
        string AppName { get; set; }
        /// <summary>
        /// Gets the application settings full file path.
        /// </summary>
        string SettingsFilePath { get; }
        /// <summary>
        /// Gets the application settings.
        /// </summary>
        dynamic Settings { get; }

        /// <summary>
        /// Reads application settings data.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> instance.</param>
        void Read(IConfiguration config);
        /// <summary>
        /// Save application settings data.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/> instance.</param>
        void Save(IConfiguration config);
    }
}