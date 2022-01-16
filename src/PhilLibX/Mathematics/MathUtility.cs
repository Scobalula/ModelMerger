﻿// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: Mathematics/MathUtilities.cs
// Author: Philip/Scobalula
// Description: Mathematic Utilities
using System;

namespace PhilLibX
{
    /// <summary>
    /// Mathematic Utilities
    /// </summary>
    public static class MathUtilities
    {
        /// <summary>
        /// Clamps Value to a range.
        /// </summary>
        /// <param name="value">Value to Clamp</param>
        /// <param name="max">Max value</param>
        /// <param name="min">Min value</param>
        /// <returns>Clamped Value</returns>
        public static T Clamp<T>(T value, T max, T min) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }

        /// <summary>
        /// Converts CM to Inch
        /// </summary>
        /// <param name="value">CM Value</param>
        /// <returns>Value in inches</returns>
        public static double CMToInch(double value)
        {
            return value * 0.3937007874015748031496062992126;
        }
    }
}