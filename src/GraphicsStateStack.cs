/*
MIT License

Copyright (c) 2026 68B09(https://twitter.com/MB68C09)

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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CSLibrary
{
	/// <summary>
	/// GraphicsState管理クラス
	/// </summary>
	public sealed class GraphicsStateStack : IDisposable
	{
		#region フィールド/プロパティー
		/// <summary>
		/// 管理対象Graphics
		/// </summary>
		private readonly Graphics targetGraphics;

		/// <summary>
		/// Graphics取得
		/// </summary>
		public Graphics Graphics { get => this.targetGraphics; }

		/// <summary>
		/// GraphicsStateスタック
		/// </summary>
		private readonly Stack<GraphicsState> stack = new Stack<GraphicsState>();

		/// <summary>
		/// disposed
		/// </summary>
		private bool disposed;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pGraphics">管理対象Graphics</param>
		public GraphicsStateStack(Graphics pGraphics)
		{
			this.targetGraphics = pGraphics ?? throw new ArgumentNullException(nameof(pGraphics));
		}
		#endregion

		/// <summary>
		/// Saveしてusingスコープを返します。
		/// 戻り値は必ず using で受け取ってください。
		/// </summary>
		/// <remarks>
		///		using (CSLibrary.GraphicsStateStack g = new CSLibrary.GraphicsStateStack(e.Graphics)) {
		///			using (g.Save()) {	// スコープから出たときに自動的にRestoreが実行される。
		///				・・・
		///			}
		///		}
		/// </remarks>
		public IDisposable Save()
		{
			if (this.disposed) {
				throw new ObjectDisposedException(nameof(GraphicsStateStack));
			}

			GraphicsState state = this.targetGraphics.Save();
			this.stack.Push(state);
			return new PopScope(this);
		}

		/// <summary>
		/// Restore実行
		/// </summary>
		private void Restore()
		{
			if (this.stack.Count == 0) {
				throw new InvalidOperationException("GraphicsState stack is empty.");
			}
			this.targetGraphics.Restore(this.stack.Pop());
		}

		/// <summary>
		/// 未 Restore の State をすべて戻す
		/// </summary>
		public void Dispose()
		{
			if (this.disposed) return;

			while (stack.Count > 0) {
				this.targetGraphics.Restore(this.stack.Pop());
			}

			this.disposed = true;
		}

		/// <summary>
		/// using(自動Pop)用クラス
		/// </summary>
		private sealed class PopScope : IDisposable
		{
			#region フィールド/プロパティー
			/// <summary>
			/// GraphicsState管理クラス
			/// </summary>
			private GraphicsStateStack owner;
			#endregion

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="pOwner">GraphicsState管理クラス</param>
			public PopScope(GraphicsStateStack pOwner)
			{
				this.owner = pOwner;
			}
			#endregion

			/// <summary>
			/// Save した GraphicsState を Pop
			/// </summary>
			public void Dispose()
			{
				if (this.owner != null) {
					this.owner.Restore();
					this.owner = null;
				}
			}
		}
	}
}
