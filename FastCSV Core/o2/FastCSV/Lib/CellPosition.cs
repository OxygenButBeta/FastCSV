
namespace o2.FastCSV
{
    /// <summary>
    /// This struct represents a 2D vector with integer components.
    /// To be used as a position in a 2D grid.
    /// </summary>
    public struct CellPosition
    {
        /// <summary>
        /// The Index on the X axis.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The Index on the Y axis.
        /// </summary>
        public int Y { get; set; }
        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString() => $"Row: '{X}', Column: '{Y}'";

    }
}

