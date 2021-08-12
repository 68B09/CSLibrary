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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSLibrary
{
	#region RingBuffer
	/// <summary>
	/// リングバッファ
	/// </summary>
	public class RingBuffer<T> : IEnumerable
	{
		#region フィールド・プロパティー
		/// <summary>
		/// 読み取り位置
		/// </summary>
		private int readIndex;

		/// <summary>
		/// 書き込み位置
		/// </summary>
		private int writeIndex;

		/// <summary>
		/// データ数
		/// </summary>
		private int dataCount;

		/// <summary>
		/// データ数取得
		/// </summary>
		public int Count
		{
			get
			{
				return this.dataCount;
			}
		}

		/// <summary>
		/// バッファ
		/// </summary>
		private T[] buffer = null;

		/// <summary>
		/// バッファ数取得/設定
		/// </summary>
		/// <remarks>
		/// 保有データ数より少ない値には設定出来ない。
		/// </remarks>
		public int Capacity
		{
			get
			{
				return this.buffer.Length;
			}
			set
			{
				// 0以下？
				if (value <= 0) {
					throw new ArgumentOutOfRangeException(nameof(Capacity), value, "0>=");
				}

				// 保有データ数より少ない？
				if (value < this.dataCount) {
					throw new ArgumentOutOfRangeException(nameof(Capacity), value, "0>=");
				}

				this.CreateBuffer(value, true);
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ(バッファ数指定)
		/// </summary>
		/// <param name="pCount">バッファ数</param>
		public RingBuffer(int pCount)
		{
			if (pCount < 0) {
				throw new ArgumentOutOfRangeException(nameof(Capacity), pCount, "0>");
			}
			this.CreateBuffer(pCount, false);
		}

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="pInitialData">コピー元</param>
		public RingBuffer(RingBuffer<T> pInitialData)
		{
			this.CreateBuffer(pInitialData.Count, false);

			// コピー
			for (int i = 0; i < pInitialData.Count; i++) {
				this.Write(pInitialData.ReadAt(i));
			}
		}

		/// <summary>
		/// コンストラクタ(初期値指定)
		/// </summary>
		/// <param name="pInitialData">初期値</param>
		/// <remarks>
		/// 初期値の個数が判っている場合は、new RingBuffer(n)でインスタンスを作った後にWriteRange()でコピーを行った方が高速です。
		/// IEnumerableは個数を保持していないため、全要素を読み出さなければ個数が得られないことに注意してください。
		/// </remarks>
		public RingBuffer(IEnumerable<T> pInitialData)
		{
			this.CreateBuffer(pInitialData.Count(), false);

			// コピー
			this.WriteRange(pInitialData);
		}
		#endregion

		/// <summary>
		/// バッファ作成
		/// </summary>
		/// <param name="pCount">バッファ数</param>
		/// <param name="pInherit">true=現在のデータを継承</param>
		private void CreateBuffer(int pCount, bool pInherit)
		{
			T[] newBuffer = new T[pCount];

			if (pInherit) {
				// データ継承

				// 既存データをコピー
				int i = 0;
				while (this.Count > 0) {
					newBuffer[i++] = this.Read();
				}

				this.dataCount = i;
				this.readIndex = 0;
				this.writeIndex = i % newBuffer.Length;
			} else {
				// 継承しない

				this.dataCount = 0;
				this.readIndex = 0;
				this.writeIndex = 0;
			}

			this.buffer = newBuffer;
		}

		/// <summary>
		/// データクリア
		/// </summary>
		public void Clear()
		{
			this.dataCount = 0;
			this.readIndex = 0;
			this.writeIndex = 0;
		}

		/// <summary>
		/// データ読み出し
		/// </summary>
		/// <returns>データ</returns>
		public T Read()
		{
			if (this.dataCount <= 0) {
				throw new InvalidOperationException("buffer empty");
			}

			T result = this.buffer[this.readIndex];
			this.readIndex = (this.readIndex + 1) % this.buffer.Length;
			this.dataCount--;
			return result;
		}

		/// <summary>
		/// データ読み出し(位置指定)
		/// </summary>
		/// <param name="pIndex">要素番号(0～)</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// pIndexで指定されたデータを返す。
		/// 次回のRead()で読み出されるデータが要素番号0に該当する。
		/// 読み出し位置やデータ数などは変化しない。
		/// </remarks>
		public T ReadAt(int pIndex)
		{
			if (pIndex >= this.Count) {
				throw new ArgumentOutOfRangeException(nameof(pIndex), pIndex, ">=Count");
			}

			int wkIndex = (this.readIndex + pIndex) % this.buffer.Length;
			return this.buffer[wkIndex];
		}

		/// <summary>
		/// データ書き込み
		/// </summary>
		/// <param name="pValue">T</param>
		public void Write(T pValue)
		{
			if (this.dataCount >= this.buffer.Length) {
				throw new InvalidOperationException("buffer full");
			}
			this.buffer[this.writeIndex] = pValue;
			this.writeIndex = (this.writeIndex + 1) % this.buffer.Length;
			this.dataCount++;
		}

		/// <summary>
		/// データ一括書き込み
		/// </summary>
		/// <param name="pSource">コピー元</param>
		public void WriteRange(IEnumerable<T> pSource)
		{
			foreach (T item in pSource) {
				this.Write(item);
			}
		}

		/// <summary>
		/// 配列取得
		/// </summary>
		/// <returns>保持しているデータが格納されている配列</returns>
		public T[] ToArray()
		{
			T[] result = new T[this.Count];

			int wkIndex = this.readIndex;
			for (int i = 0; i < this.Count; i++) {
				result[i] = this.buffer[wkIndex];
				wkIndex = (wkIndex + 1) % this.buffer.Length;
			}

			return result;
		}

		#region IEnumerable
		/// <summary>
		/// GetEnumerator
		/// </summary>
		/// <returns>RinBufferEnumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new RinBufferEnumerator(this);
		}

		/// <summary>
		/// RinBufferEnumerator
		/// </summary>
		public class RinBufferEnumerator : IEnumerator
		{
			/// <summary>
			/// データソース
			/// </summary>
			RingBuffer<T> source = null;

			/// <summary>
			/// カレントオブジェクト取得
			/// </summary>
			public object Current
			{
				get
				{
					return this.source.Read();
				}
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="pSource"データ>ソース</param>
			public RinBufferEnumerator(RingBuffer<T> pSource)
			{
				this.source = pSource;
			}

			/// <summary>
			/// 次データ有無確認
			/// </summary>
			/// <returns>true=次データあり</returns>
			public bool MoveNext()
			{
				return this.source.Count > 0;
			}

			/// <summary>
			/// リセット
			/// </summary>
			public void Reset()
			{
			}
		}
		#endregion
	}
	#endregion
}
