using System;

namespace UnityAlgorithms.Core
{
    [Serializable]
    public struct Coord : IComparable<Coord>, IEquatable<Coord>
    {
        public int Y { get; set; }
        public int X { get; set; }

        public Coord(int y = 0, int x = 0)
        {
            Y = y;
            X = x;
        }

        public int CompareTo(Coord other)
        {
            int yComparison = Y.CompareTo(other.Y);
            return yComparison != 0 ? yComparison : X.CompareTo(other.X);
        }

        public bool Equals(Coord other)
        {
            return Y == other.Y && X == other.X;
        }

        public override bool Equals(object? obj)
        {
            return obj is Coord coord && Equals(coord);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Y, X);
        }

        public static bool operator ==(Coord left, Coord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coord left, Coord right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({Y}, {X})";
        }
    }
}