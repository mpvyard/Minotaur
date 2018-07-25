﻿using System;
using System.Runtime.InteropServices;

namespace Minotaur.Native
{
    [StructLayout(LayoutKind.Explicit, Size = SIZE)]
    public struct Int32Entry : IEquatable<Int32Entry>
    {
        public const int SIZE = 12;

        [FieldOffset(0)]
        public long ticks;
        [FieldOffset(8)]
        public int value;

        #region Equality members

        public bool Equals(Int32Entry other)
        {
            return ticks == other.ticks && value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Int32Entry && Equals((Int32Entry)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return (ticks.GetHashCode() * 397) ^ value;
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }

        public static bool operator ==(Int32Entry left, Int32Entry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Int32Entry left, Int32Entry right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override string ToString()
        {
            return $"Index: {new DateTime(ticks)}, Value: {value}";
        }
    }
}