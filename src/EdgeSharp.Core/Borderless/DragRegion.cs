// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Drawing;

namespace EdgeSharp.Core.Borderless
{
    public static class DragRegion
    {
        private const int RESIZE_TOLERANCE = 10;
        public static int Width { get; set; }
        public static int Height { get; set; }

        public static Rectangle TopGrip => new(0, 0, Width, RESIZE_TOLERANCE);
        public static Rectangle LeftGrip => new(0, 0, RESIZE_TOLERANCE, Height);
        public static Rectangle BottomGrip => new(0, Height - RESIZE_TOLERANCE, Width, RESIZE_TOLERANCE);
        public static Rectangle RightGrip => new(Width - RESIZE_TOLERANCE, 0, RESIZE_TOLERANCE, Height);
        public static Rectangle TopLeftGrip => new(0, 0, RESIZE_TOLERANCE, RESIZE_TOLERANCE);
        public static Rectangle TopRightGrip => new(Width - RESIZE_TOLERANCE, 0, RESIZE_TOLERANCE, RESIZE_TOLERANCE);
        public static Rectangle BottomLeftGrip => new(0, Height - RESIZE_TOLERANCE, RESIZE_TOLERANCE, RESIZE_TOLERANCE);
        public static Rectangle BottomRightGrip => new(Width - RESIZE_TOLERANCE, Height - RESIZE_TOLERANCE, RESIZE_TOLERANCE, RESIZE_TOLERANCE);
    }
}