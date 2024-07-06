/*
The MIT License (MIT)

Copyright (c) 2024 ZZO.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
namespace CSLibrary.Pairs
{
	/// <summary>
	/// ２値を扱うジェネリッククラス
	/// </summary>
	/// <typeparam name="S">１つ目の型</typeparam>
	/// <typeparam name="T">２つ目の型</typeparam>
	public class Pair<S, T>
	{
		#region フィールド・プロパティー
		/// <summary>
		/// １つ目の値のプロパティー
		/// </summary>
		public S First { get; set; }

		/// <summary>
		/// ２つ目の値のプロパティー
		/// </summary>
		public T Second { get; set; }
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Pair() { }

		/// <summary>
		/// コンストラクタ(初期値指定)
		/// </summary>
		/// <param name="p1st">１つ目の値</param>
		/// <param name="p2nd">２つ目の値</param>
		public Pair(S p1st, T p2nd) : this()
		{
			this.First = p1st;
			this.Second = p2nd;
		}

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="pSrc">コピー元</param>
		public Pair(Pair<S, T> pSrc) : this()
		{
			this.First = pSrc.First;
			this.Second = pSrc.Second;
		}
		#endregion

		/// <summary>
		/// ハッシュ作成
		/// </summary>
		/// <returns>ハッシュ値</returns>
		public override int GetHashCode()
		{
			return this.First.GetHashCode() ^ this.Second.GetHashCode();
		}

		/// <summary>
		/// 同値判定
		/// </summary>
		/// <param name="obj">判定相手</param>
		/// <returns>true=一致</returns>
		public override bool Equals(object obj)
		{
			if ((obj == null) || (this.GetType() != obj.GetType())) {
				return false;
			}

			Pair<S, T> target = (Pair<S, T>)obj;

			if (!this.First.Equals(target.First) || !this.Second.Equals(target.Second)) {
				return false;
			}

			return true;
		}
	}

	/// <summary>
	/// ３値を扱うジェネリッククラス
	/// </summary>
	/// <typeparam name="S">１つ目の型</typeparam>
	/// <typeparam name="T">２つ目の型</typeparam>
	/// <typeparam name="U">３つ目の型</typeparam>
	public class Triple<S, T, U>
	{
		#region フィールド・プロパティー
		/// <summary>
		/// １つ目の値のプロパティー
		/// </summary>
		public S First { get; set; }

		/// <summary>
		/// ２つ目の値のプロパティー
		/// </summary>
		public T Second { get; set; }

		/// <summary>
		/// ３つ目の値のプロパティー
		/// </summary>
		public U Third { get; set; }
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Triple() { }

		/// <summary>
		/// コンストラクタ(初期値指定)
		/// </summary>
		/// <param name="p1st">１つ目の値</param>
		/// <param name="p2nd">２つ目の値</param>
		/// <param name="p3rd">３つ目の値</param>
		public Triple(S p1st, T p2nd, U p3rd) : this()
		{
			this.First = p1st;
			this.Second = p2nd;
			this.Third = p3rd;
		}

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="pSrc">コピー元</param>
		public Triple(Triple<S, T, U> pSrc) : this()
		{
			this.First = pSrc.First;
			this.Second = pSrc.Second;
			this.Third = pSrc.Third;
		}
		#endregion

		/// <summary>
		/// ハッシュ作成
		/// </summary>
		/// <returns>ハッシュ値</returns>
		public override int GetHashCode()
		{
			return this.First.GetHashCode() ^ this.Second.GetHashCode() ^ this.Third.GetHashCode();
		}

		/// <summary>
		/// 同値判定
		/// </summary>
		/// <param name="obj">判定相手</param>
		/// <returns>true=一致</returns>
		public override bool Equals(object obj)
		{
			if ((obj == null) || (this.GetType() != obj.GetType())) {
				return false;
			}

			Triple<S, T, U> target = (Triple<S, T, U>)obj;

			if (!this.First.Equals(target.First) ||
				!this.Second.Equals(target.Second) ||
				!this.Third.Equals(target.Third)) {
				return false;
			}

			return true;
		}
	}
}
