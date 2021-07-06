// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Drawing;

namespace EdgeSharp.Core.Borderless
{
    public class MouseMoveEventArgs : EventArgs
    {
        public MouseMoveEventArgs(int xDelta, int yDelta)
        {
            DeltaChangeSize = new Size(xDelta, yDelta);
        }

        public MouseMoveEventArgs(Size deltaSize)
        {
            DeltaChangeSize = deltaSize;
        }

        public Size DeltaChangeSize { get; set; }
    }
}