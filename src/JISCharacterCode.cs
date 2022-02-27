/*
MIT License

Copyright (c) 2021 68B09(https://twitter.com/MB68C09)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CSLibrary.JISCharacterCode
{
	/// <summary>
	/// ShiftJIS
	/// </summary>
	/// <remarks>
	/// Shift_JISに関するクラス
	/// </remarks>
	public class ShiftJIS
	{
		/// <summary>
		/// JIS漢字コードをShift_JISに変換
		/// </summary>
		/// <param name="pJISCode">JIS漢字コード(0x2121～0x7E7E)</param>
		/// <returns>pJISCodeが不正な場合は0、それ以外はShift_JIS</returns>
		static public int JIStoSJIS(ushort pJISCode)
		{
			int hi = pJISCode >> 8;
			if ((hi < 0x21) || (hi > 0x7e)) {
				return 0;
			}

			int lo = pJISCode & 0xff;
			if ((lo < 0x21) || (lo > 0x7e)) {
				return 0;
			}

			if ((hi & 1) != 0) {
				if (lo < 0x60) {
					lo += 0x1f;
				} else {
					lo += 0x20;
				}
			} else {
				lo += 0x7e;
			}

			if (hi < 0x5f) {
				hi = (hi + 0xe1) >> 1;
			} else {
				hi = (hi + 0x161) >> 1;
			}

			return (hi << 8) | lo;
		}

		/// <summary>
		/// Shift_JISをJIS漢字コードに変換
		/// </summary>
		/// <param name="pSJISCode">Shift_JIS(0x8140～0xEFFC)</param>
		/// <returns>pSJISCodeが不正な場合は0、JIS漢字コード(0x2121～0x7E7E)</returns>
		static public int SJIStoJIS(ushort pSJISCode)
		{
			int hi = pSJISCode >> 8;
			if (((hi >= 0x81) && (hi <= 0x9f)) || ((hi >= 0xe0) && (hi <= 0xfc))) {
			} else {
				return 0;
			}

			int lo = pSJISCode & 0xff;
			if (((lo >= 0x40) && (lo <= 0xfc)) && (lo != 0x7e)) {
			} else {
				return 0;
			}

			if (hi <= 0x9f) {
				if (lo < 0x9f) {
					hi = (hi << 1) - 0xe1;
				} else {
					hi = (hi << 1) - 0xe0;
				}
			} else {
				if (lo < 0x9f) {
					hi = (hi << 1) - 0x161;
				} else {
					hi = (hi << 1) - 0x160;
				}
			}

			if (lo < 0x7f) {
				lo -= 0x1f;
			} else if (lo < 0x9f) {
				lo -= 0x20;
			} else {
				lo -= 0x7e;
			}

			return (hi << 8) | lo;
		}
	}

	/// <summary>
	/// CP932
	/// </summary>
	/// <remarks>
	/// CP932に関するクラス
	/// </remarks>
	public class CP932
	{
		/// <summary>
		/// JIS漢字コードをCP932に変換
		/// </summary>
		/// <param name="pJISCode">JIS漢字コード(0x2121～0x987E)</param>
		/// <returns>pJISCodeが不正な場合は0、それ以外はCP932</returns>
		static public int JIStoCP932(ushort pJISCode)
		{
			int hi = pJISCode >> 8;
			if ((hi < 0x21) || (hi > 0x98)) {
				return 0;
			}

			int lo = pJISCode & 0xff;
			if ((lo < 0x21) || (lo > 0x7e)) {
				return 0;
			}

			if ((hi & 1) != 0) {
				if (lo < 0x60) {
					lo += 0x1f;
				} else {
					lo += 0x20;
				}
			} else {
				lo += 0x7e;
			}

			if (hi < 0x5f) {
				hi = (hi + 0xe1) >> 1;
			} else {
				hi = (hi + 0x161) >> 1;
			}

			return (hi << 8) | lo;
		}

		/// <summary>
		/// CP932をJIS漢字コードに変換
		/// </summary>
		/// <param name="pCP932Code">CP932(0x8140～0xFCFC)</param>
		/// <returns>pSJISCodeが不正な場合は0、JIS漢字コード(0x2121～0x987E)</returns>
		static public int CP932toJIS(ushort pCP932Code)
		{
			int hi = pCP932Code >> 8;
			if (((hi >= 0x81) && (hi <= 0x9f)) || ((hi >= 0xe0) && (hi <= 0xfc))) {
			} else {
				return 0;
			}

			int lo = pCP932Code & 0xff;
			if (((lo >= 0x40) && (lo <= 0xfc)) && (lo != 0x7e)) {
			} else {
				return 0;
			}

			if (hi <= 0x9f) {
				if (lo < 0x9f) {
					hi = (hi << 1) - 0xe1;
				} else {
					hi = (hi << 1) - 0xe0;
				}
			} else {
				if (lo < 0x9f) {
					hi = (hi << 1) - 0x161;
				} else {
					hi = (hi << 1) - 0x160;
				}
			}

			if (lo < 0x7f) {
				lo -= 0x1f;
			} else if (lo < 0x9f) {
				lo -= 0x20;
			} else {
				lo -= 0x7e;
			}

			return (hi << 8) | lo;
		}
	}
}