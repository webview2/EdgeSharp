// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;

namespace EdgeSharp.Core.Configuration
{
    /// <summary>
    /// Configuration options associated with the borderless window.
    /// </summary>
    public class BorderlessOption
    {
        private static int RESIZER_GRIP_SIZE = 1;
        private static int DRAGZONE_HEIGHT = 32;
        private static int DRAGZONE_TOP_OFFSET = 0;
        private static int DRAGZONE_LEFT_OFFSET = 0;
        private static int DRAGZONE_RIGHT_OFFSET = 140;

        /// <summary>
        /// Initializes a new instance of <see cref="BorderlessOption"/>.
        /// </summary>
        public BorderlessOption()
        {
            Resizer = RESIZER_GRIP_SIZE;
            Draggable = true;
            UseDefaultDragHandler = true;
            DragZones = new List<DragZoneConfiguration>();
            DragZones.Add(new DragZoneConfiguration(DRAGZONE_HEIGHT, DRAGZONE_TOP_OFFSET, DRAGZONE_LEFT_OFFSET, DRAGZONE_RIGHT_OFFSET));
        }

        /// <summary>
        /// Gets or sets the window resizer grip for borderless windows.
        /// </summary>
        public int Resizer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the borderless window is draggable. 
        /// </summary>
        public bool Draggable { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the borderless window uses the default drag handler. 
        /// </summary>
        public bool UseDefaultDragHandler { get; set; }

        /// <summary>
        /// Gets or sets list of draggable areas.
        /// </summary>
        public List<DragZoneConfiguration> DragZones { get; set; }
    }
}