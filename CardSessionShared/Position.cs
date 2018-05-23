using System;

namespace CardSessionShared
{
    /// <summary>
    /// Позиция объекта на игровом поле
    /// </summary>
    [Serializable]
    public struct Position : IComparable<Position>
    {
        int x, y;
        /// <summary>
        /// Координата Х объекта
        /// </summary>
        public int X
        {
            get => x;
            set
            { if ((x = value) < 0) throw new ArgumentException("Wrong X coordinate, must be >= zero"); }
        }
        /// <summary>
        /// Координата Y объекта
        /// </summary>
        public int Y
        {
            get => y;
            set
            { if ((y = value) < 0) throw new ArgumentException("Wrong Y coordinate, must be >= zero"); }
        }

        public Position(int x, int y)
        {
            if (x < 0 || y < 0) throw new ArgumentException("Values less than zero");
            this.x = x; this.y = y;
        }
        /// <summary>
        /// Дистанция между двумя позициями
        /// </summary>
        public static int Distance(Position p1, Position p2)
        {
            return (int)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        public int CompareTo(Position other)
        {
            if (other.x > x && other.y > y) return 1;
            if (other.x < x && other.y < y) return -1;
            return 0;
        }
    }
}