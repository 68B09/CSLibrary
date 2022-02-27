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
using System.Text;

namespace CSLibrary
{
	/// <summary>
	/// バイト配列処理クラス
	/// </summary>
	public class ByteArrayUtility
	{
		/// <summary>
		/// バイト反転用バッファ
		/// </summary>
		private byte[] reverseBuf = new byte[16];

		/// <summary>
		/// 文字列生成用バッファ
		/// </summary>
		private StringBuilder sb = new StringBuilder();

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ByteArrayUtility()
		{
		}
		#endregion

		#region メモリから読み出し
		/// <summary>
		/// メモリからASCII形式の文字列を読み出す
		/// </summary>
		/// <param name="pBuf">byte[]</param>
		/// <param name="pIndex">読み出し開始位置</param>
		/// <param name="pLength">読み出しバイト数</param>
		/// <returns>string</returns>
		/// <remarks>0x00-0x7F以外の文字は存在しないものとして処理します。</remarks>
		public string GetAsciiString(byte[] pBuf, int pIndex, int pLength)
		{
			this.sb.Clear();
			for (int i = 0; i < pLength; i++) {
				this.sb.Append((char)pBuf[pIndex + i]);
			}
			return sb.ToString();
		}

		/// <summary>
		/// メモリからBigEndian形式のUInt64読み出し
		/// </summary>
		/// <param name="pBuf">byte[]</param>
		/// <param name="pIndex">読み出し開始位置</param>
		/// <returns>UInt64</returns>
		static public UInt64 GetUInt64BE(byte[] pBuf, int pIndex)
		{
			int index = pIndex;
			UInt64 num = 0;
			for (int i = 0; i < 8; i++) {
				num = (num << 8) + pBuf[index++];
			}
			return num;
		}


		/// <summary>
		/// メモリからBigEndian形式のUInt32読み出し
		/// </summary>
		/// <param name="pBuf">byte[]</param>
		/// <param name="pIndex">読み出し開始位置</param>
		/// <returns>UInt32</returns>
		static public UInt32 GetUInt32BE(byte[] pBuf, int pIndex)
		{
			int index = pIndex;
			UInt32 num = 0;
			for (int i = 0; i < 8; i++) {
				num = (num << 8) + pBuf[index++];
			}
			return num;
		}

		/// <summary>
		/// メモリからBigEndian形式のUInt16読み出し
		/// </summary>
		/// <param name="pBuf">byte[]</param>
		/// <param name="pIndex">読み出し開始位置</param>
		/// <returns>UInt16</returns>
		static public UInt16 GetUInt16BE(byte[] pBuf, int pIndex)
		{
			int index = pIndex;
			int num = pBuf[index++];
			num = (num << 8) + pBuf[index++];
			return (UInt16)num;
		}
		#endregion
	}
}
