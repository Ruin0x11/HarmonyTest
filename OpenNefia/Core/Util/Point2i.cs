// From https://github.com/Roderik11/Squid/blob/master/Structs/Point.cs

#region License
// The MIT License (MIT)
// 
// Copyright (c) 2016
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace OpenNefia.Core.Util
{
    /// <summary>
    /// Struct Point
    /// </summary>
    [TypeConverter(typeof(PointTypeConverter))]
    public struct Point2i
    {
        public static readonly Point2i Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2i"/> struct.
        /// </summary>
        /// <param name="pt">The pt.</param>
        public Point2i(Point2i pt)
        {
            this.X = pt.X;
            this.Y = pt.Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2i"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point2i(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2i operator +(Point2i p1, Point2i p2)
        {
            return new Point2i(p1.X + p2.X, p1.Y + p2.Y);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2i operator -(Point2i p1, Point2i p2)
        {
            return new Point2i(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point2i p1, Point2i p2)
        {
            return ((p1.X == p2.X) && (p1.Y == p2.Y));
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point2i p1, Point2i p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2i operator *(Point2i a, int b)
        {
            return new Point2i(a.X * b, a.Y * b);
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2i operator /(Point2i a, int b)
        {
            return new Point2i(a.X / b, a.Y / b);
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2i operator *(Point2i a, float b)
        {
            return new Point2i((int)((float)a.X * b), (int)((float)a.Y * b));
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2i operator /(Point2i a, float b)
        {
            return new Point2i((int)((float)a.X / b), (int)((float)a.Y / b));
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool IsEmpty
        {
            get { return ((X == 0) && (Y == 0)); }
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public int X;

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public int Y;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (!(obj is Point2i))
                return false;

            Point2i size = (Point2i)obj;
            return ((size.X == this.X) && (size.Y == this.Y));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return (this.X ^ this.Y);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}; {1}", X, Y);
        }

        /// <summary>
        /// Eases to.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>Point.</returns>
        public static Point2i EaseTo(Point2i start, Point2i end, float divisor)
        {
            float x = ((float)end.X - (float)start.X) / divisor;
            float y = ((float)end.Y - (float)start.Y) / divisor;

            return start + new Point2i((int)x, (int)y);
        }

        public void Deconstruct(out int x, out int y)
        {
            x = this.X;
            y = this.Y;
        }

        static Point2i()
        {
            Zero = new Point2i();
        }
    }

    public class PointTypeConverter : TypeConverter
    {
        // Methods
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (!(value is string))
                return base.ConvertFrom(context, culture, value);

            string str = ((string)value).Trim();

            if (str.Length == 0)
                return null;

            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            string[] arr = str.Split(new char[2] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int[] numArray = new int[arr.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));

            for (int i = 0; i < numArray.Length; i++)
                numArray[i] = (int)converter.ConvertFromString(context, culture, arr[i])!;

            if (numArray.Length != 2)
                throw new ArgumentException();

            return new Point2i(numArray[0], numArray[1]);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentNullException">destinationType</exception>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type? destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if ((destinationType == typeof(string)) && (value is Point2i))
            {
                Point2i size = (Point2i)value;
                if (culture == null)
                    culture = CultureInfo.CurrentCulture;

                string separator = "; ";
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                string?[] strArray = new string[2];
                int num = 0;
                strArray[num++] = converter.ConvertToString(context, culture, size.X);
                strArray[num++] = converter.ConvertToString(context, culture, size.Y);
                return string.Join(separator, strArray);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
