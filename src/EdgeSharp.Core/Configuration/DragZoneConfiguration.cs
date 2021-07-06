using System.Drawing;

namespace EdgeSharp.Core.Configuration
{
    /// <summary> 
    /// Represents a drag zone on the main borderles window. 
    /// </summary>
    public class DragZoneConfiguration
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DragZoneConfiguration"/>.
        /// </summary>
        public DragZoneConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DragZoneConfiguration"/>
        /// </summary>
        /// <param name="height">The height of the drag zone.</param>
        /// <param name="topoffset">The offset from the top of the frame.</param>
        /// <param name="leftoffset">The offset from the left of the frame.</param>
        /// <param name="rightoffset">The offset from the right of the frame.</param>
        public DragZoneConfiguration(int height, int topoffset, int leftoffset, int rightoffset)
        {
            EntireWindow = false;
            Height = height;
            TopOffset = topoffset;
            LeftOffset = leftoffset;
            RightOffset = rightoffset;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DragZoneConfiguration"/>
        /// </summary>
        /// <param name="entireWindow">A boolean value to indicate if entire window is to be the drag zone.</param>
        public DragZoneConfiguration(bool entireWindow)
        {
            EntireWindow = entireWindow;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entire window should be dragged/moved by clicking and dragging anywhere on the window.
        /// </summary>
        public bool EntireWindow { get; }

        /// <summary>
        /// Gets or sets the height of the drag zone, typically at the top of the window.
        /// This is the height of the drag zone.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the offset from the top of the frame to start the drag area.
        /// This is the offset from the top of the frame.
        /// </summary>
        public int TopOffset { get; set; }

        /// <summary>
        /// Gets or sets the offset from the left of the frame to start the drag area.
        /// This is the offset from the left of the frame.
        /// </summary>
        public int LeftOffset { get; set; }

        /// <summary>
        /// Gets or sets the offset from the right of the frame to start the drag area.
        /// This is the offset from the right of the frame.
        /// </summary>
        public int RightOffset { get; set; }

        /// <summary> 
        /// Determines if the point is in the zone. 
        /// </summary>
        /// <param name="size">The size of the area to calculate the offsets. </param>
        /// <param name="point">The point.</param>
        /// <param name="scale"> The scale to use for dpi / desktop scale compensation.</param>
        /// <returns>True if in the zone.</returns>
        public bool InZone(Size size, Point point, float scale)
        {
            if (this.EntireWindow)
            {
                return true;
            }

            var HeightScaled = Height * scale;
            var TopOffsetScaled = TopOffset * scale;
            var LeftOffsetScaled = LeftOffset * scale;
            var RightOffsetScaled = RightOffset * scale;

            var point_rightoffset = size.Width - point.X;
            // Define a bounding box for the drag area
            return point.Y <= (HeightScaled + TopOffsetScaled) &&
                   point.Y >= TopOffsetScaled &&
                   point.X >= LeftOffsetScaled &&
                   point_rightoffset > RightOffsetScaled;
        }
    }
}
