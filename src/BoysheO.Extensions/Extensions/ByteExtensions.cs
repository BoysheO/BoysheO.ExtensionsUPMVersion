using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BoysheO.Extensions
{
    /// <summary>
    /// 与byte相关的扩展
    /// </summary>
    public static class ByteExtensions
    {
        #region IsGZipHeader

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGZipHeader(this ReadOnlySpan<byte> bytes)
        {
            return bytes.Length >= 2 && bytes[0] == 0x1f && bytes[1] == 0x8b;
        }

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGZipHeader(this Span<byte> bytes)
        {
            return ((ReadOnlySpan<byte>) bytes).IsGZipHeader();
        }

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGZipHeader(this byte[] bytes)
        {
            return ((ReadOnlySpan<byte>) bytes).IsGZipHeader();
        }

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b
        /// </summary>
        public static bool IsGZipHeader(this IEnumerable<byte> bytes)
        {
            using var enumerator = bytes.GetEnumerator();
            if (!enumerator.MoveNext()) return false;
            if (enumerator.Current != 0x1f) return false;
            if (!enumerator.MoveNext()) return false;
            if (enumerator.Current != 0x8b) return false;
            return true;
        }

        #endregion

        #region ToHexText

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”
        /// </summary>
        public static string ToHexText(this IEnumerable<byte> bytes, StringBuilder? usingStringBuilder = null)
        {
            switch (bytes)
            {
                case byte[] byteAry:
                    return byteAry.ToHexText();
                case IList<byte> lst:
                    return lst.ToHexText();
            }

            //4=hex:2 blank:1
            var sb = usingStringBuilder ?? new StringBuilder();
            usingStringBuilder?.Clear();
            foreach (var byte1 in bytes)
            {
                sb.Append(byte1.ToHexText());
                sb.Append(' ');
            }

            if (sb.Length == 0) return "";
            sb.Remove(sb.Length - 1, 1);
            var res = sb.ToString();
            usingStringBuilder?.Clear();
            return res;
        }

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”
        /// </summary>
        public static string ToHexText(this IList<byte> bytes, StringBuilder? usingStringBuilder = null)
        {
            if (bytes.Count == 0) return "";
            //4=hex:2 blank:1
            var sb = usingStringBuilder ?? new StringBuilder(bytes.Count * 3);
            usingStringBuilder?.Clear();
            foreach (var byte1 in bytes)
            {
                sb.Append(byte1.ToHexText());
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
            var res = sb.ToString();
            usingStringBuilder?.Clear();
            return res;
        }

        /// <summary>
        ///     输出byte的hex字符串表达，形如"A1","01"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this byte byte1)
        {
            return byte1.ToString("X2");
        }

        //primitive
        /// <summary>
        ///     根据byte块，输出hex字符串表达，形如“A1 BE C1”
        /// </summary>
        public static string ToHexText(this ReadOnlySpan<byte> bytes, Span<char> charBuffer = default,
            int maxByteCount = 500)
        {
            if (bytes.Length == 0) return "";

            //可设置的最高.net上限应为min(4e9, 2147483648/sizeof(char))，但是要考虑运行环境的内存限制和外部API对超大字串的支持问题，再考虑本API主要是DEBUG使用，很小的限制就够了
            var cut = bytes.Length > maxByteCount; //avoid System.OverflowException OutOfMemoryException
            var charCount = cut ? maxByteCount * 3 : bytes.Length * 3;

            if (charBuffer.IsEmpty)
                charBuffer = new char[charCount];
            else if (charBuffer.Length < charCount)
                throw new OutOfMemoryException($"{nameof(charBuffer)} has not enough space to restore string.");

            unsafe
            {
                fixed (char* c = charBuffer)
                {
                    var curC = c;
                    fixed (byte* b = bytes)
                    {
                        var curB = b;
                        while (curC - c < charCount) //test pass;it was right
                        {
                            *curC = ' ';
                            curC++;
                            var h = *curB >> 4;
                            *curC = (char) (h < 0x0A ? 48 + h : 55 + h);
                            curC++;
                            var l = *curB & 0x0F;
                            *curC = (char) (l < 0x0A ? 48 + l : 55 + l);
                            curC++;
                            curB++;
                        }
                    }

                    if (cut)
                    {
                        c[charCount - 1] = '.';
                        c[charCount - 2] = '.';
                        c[charCount - 3] = '.';
                    }

                    return new string(c, 1, charCount - 1);
                }
            }
        }

        /// <summary>
        /// 使用Hex文本表达字节块内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this byte[] bytes, Span<char> charBuffer = default)
        {
            return new ReadOnlySpan<byte>(bytes).ToHexText(charBuffer);
        }

        /// <summary>
        /// 使用Hex文本表达字节块内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this Span<byte> bytes, Span<char> charBuffer = default)
        {
            return ((ReadOnlySpan<byte>) bytes).ToHexText(charBuffer);
        }

        /// <summary>
        /// 使用Hex文本表达字节块内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this Memory<byte> bytes, Span<char> charBuffer = default)
        {
            return ((ReadOnlySpan<byte>) bytes.Span).ToHexText(charBuffer);
        }

        /// <summary>
        /// 使用Hex文本表达字节块内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this ReadOnlyMemory<byte> bytes, Span<char> charBuffer = default)
        {
            return ToHexText(bytes.Span, charBuffer);
        }

        /// <summary>
        /// 使用Hex文本表达字节块内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this ArraySegment<byte> bytes, Span<char> charBuffer = default)
        {
            return ((ReadOnlySpan<byte>) bytes.AsSpan()).ToHexText(charBuffer);
        }

        #endregion

        #region ToBinText

        //primitives
        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        public static string ToBinText(this ReadOnlySpan<byte> byteSpan, StringBuilder? usingStringBuilder = null)
        {
            if (byteSpan.Length == 0) return "";
            var sb = usingStringBuilder ?? new StringBuilder(byteSpan.Length * 9); //9=8 bit and 1 blank
            usingStringBuilder?.Clear();
            foreach (var byte2 in byteSpan)
            {
                sb.Append(byte2.ToBinText());
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
            usingStringBuilder?.Clear();
            return sb.ToString();
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this Span<byte> span, StringBuilder? usingStringBuilder = null)
        {
            return ((ReadOnlySpan<byte>) span).ToBinText(usingStringBuilder);
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        public static string ToBinText(this byte[] bytes, StringBuilder? usingStringBuilder = null)
        {
            if (bytes.Length == 0) return "";
            var sb = usingStringBuilder ?? new StringBuilder(bytes.Length * 9); //9=8 bit and 1 blank
            usingStringBuilder?.Clear();
            foreach (var byte2 in bytes)
            {
                sb.Append(byte2.ToBinText());
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
            usingStringBuilder?.Clear();
            return sb.ToString();
        }

        //primitives
        /// <summary>
        ///     输出byte的bin字符串表达，形如"00010000","11001000"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this byte byte1)
        {
            return Convert.ToString(byte1, 2).PadLeft(8, '0');
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this ReadOnlyMemory<byte> memory, StringBuilder? usingStringBuilder = null)
        {
            return ToBinText(memory.Span, usingStringBuilder);
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this Memory<byte> memory, StringBuilder? usingStringBuilder = null)
        {
            return ToBinText(memory.Span, usingStringBuilder);
        }

        #endregion

        //primitives
        /// <summary>
        ///     将该值类型转换成内存中byte表示，无复制
        ///     仅限基础类型，否则会异常
        ///     *修改返回的span数组会修改初始值
        /// </summary>
        public static Span<byte> AsMemoryByteSpan<T>(this ref T value) where T : unmanaged
        {
            unsafe
            {
                fixed (T* p = &value)
                {
                    var span = new Span<byte>(p, sizeof(T));
                    // Console.WriteLine(span.ToHexString());
                    return span;
                }
            }
        }


        /// <summary>
        ///     将值视作二进制输出hexString
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToMemoryHexText<T>(this T value, Span<char> charBuffer = default)
            where T : unmanaged
        {
            return ((ReadOnlySpan<byte>) value.AsMemoryByteSpan()).ToHexText(charBuffer);
        }
    }
}