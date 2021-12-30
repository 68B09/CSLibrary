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
using System.IO;
using System.Text;

namespace CSLibrary
{
	/// <summary>
	/// 拡張FileStreamクラス
	/// </summary>
	public class ExFileStream : FileStream
	{
		#region フィールド/プロパティー
		/// <summary>
		/// ポジションスタック
		/// </summary>
		private Stack<long> savePositionList = new Stack<long>();

		/// <summary>
		/// 読み込み用ワークバッファ
		/// </summary>
		private byte[] readBuf = new byte[16];

		/// <summary>
		/// 文字列生成用バッファ
		/// </summary>
		private StringBuilder sb = new StringBuilder();

		/// <summary>
		/// バイト配列処理クラス
		/// </summary>
		private ByteArrayUtility byteUtil = new ByteArrayUtility();
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pFilePath">ファイルパス</param>
		/// <param name="pFileMode">ファイルモード</param>
		/// <param name="pFileAccess">アクセスモード</param>
		public ExFileStream(string pFilePath, FileMode pFileMode, FileAccess pFileAccess) : base(pFilePath, pFileMode, pFileAccess)
		{
		}
		#endregion

		#region 位置操作
		/// <summary>
		/// 現在位置保存と位置設定
		/// </summary>
		/// <param name="pSeekPosition">新位置(ファイル先頭からのバイト数)</param>
		/// <returns>現在位置</returns>
		/// <remarks>
		/// 現在位置を記録後、指定された位置へ移動する。
		/// </remarks>
		public long SaveAndSetPosition(long pSeekPosition)
		{
			long currentPos = this.SaveCurrentPosition();
			this.Position = pSeekPosition;
			return currentPos;
		}

		/// <summary>
		/// 現在位置保存
		/// </summary>
		/// <returns>現在位置</returns>
		public long SaveCurrentPosition()
		{
			long currentPos = this.Position;
			this.savePositionList.Push(currentPos);
			return currentPos;
		}

		/// <summary>
		/// 保存位置へ復帰
		/// </summary>
		/// <remarks>
		/// SaveCurrentPosition()などで保存した位置へ移動する。
		/// </remarks>
		public void RestorePosition()
		{
			long newpos = this.savePositionList.Pop();
			this.Position = newpos;
		}
		#endregion

		#region 文字列読み出し
		/// <summary>
		/// ASCII形式の文字列を読み出す
		/// </summary>
		/// <param name="pLength">バイト数</param>
		/// <returns>String</returns>
		/// <remarks>0x00-0x7F以外の文字は存在しないものとして処理します。</remarks>
		public string GetAsciiString(int pLength)
		{
			sb.Clear();
			byte[] buf = new byte[pLength];
			this.Read(buf, 0, buf.Length);
			for (int i = 0; i < buf.Length; i++) {
				sb.Append((char)buf[i]);
			}
			return this.sb.ToString();
		}

		/// <summary>
		/// UTF-16BE形式の文字列を読み出す
		/// </summary>
		/// <param name="pLength">バイト数</param>
		/// <returns>String</returns>
		public string GetUTF16BEString(int pLength)
		{
			byte[] buf = new byte[pLength];
			this.Read(buf, 0, buf.Length);
			return Encoding.BigEndianUnicode.GetString(buf);
		}
		#endregion

		#region 数値読みだし
		/// <summary>
		/// BigEndian形式のUInt32読み出し
		/// </summary>
		/// <returns>UInt32</returns>
		public UInt32 GetUInt32BE()
		{
			this.Read(readBuf, 0, 4);
			return this.byteUtil.GetUInt32BE(this.readBuf, 0);
		}

		/// <summary>
		/// BigEndian形式のUInt16読み出し
		/// </summary>
		/// <returns>UInt16</returns>
		public UInt16 GetUInt16BE()
		{
			this.Read(readBuf, 0, 2);
			return this.byteUtil.GetUInt16BE(this.readBuf, 0);
		}
		#endregion
	}
}
