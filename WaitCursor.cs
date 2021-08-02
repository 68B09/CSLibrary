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
using System.Windows.Forms;

namespace CSLibrary
{
	/// <summary>
	/// ウェイトカーソル
	/// </summary>
	/// <remarks>
	/// using(WaitCursor = new WaitCursor()){
	///     時間がかかる処理();
	/// }
	/// </remarks>
	public class WaitCursor : IDisposable
	{
		#region フィールド・プロパティー
		/// <summary>
		/// 保存したカーソルオブジェクト
		/// </summary>
		private Cursor saveCursor = null;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks>
		/// カレントカーソルオブジェクトを保存し、カレントカーソルをウェイトカーソルに変更する。
		/// </remarks>
		public WaitCursor()
		{
			this.saveCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			Restore();
		}

		/// <summary>
		/// 保存していたカーソルオブジェクトをカレントカーソルに設定
		/// </summary>
		public void Restore()
		{
			if (this.saveCursor != null) {
				Cursor.Current = this.saveCursor;
				this.saveCursor = null;
			}
		}
	}
}
