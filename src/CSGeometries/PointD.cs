/*
The MIT License (MIT)

Copyright (c) 2016-2021 ZZO.

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

using System;
using System.Drawing;

namespace CSLibrary.CSGeometries
{
	/// <summary>
	/// double型Point構造体
	/// </summary>
	public struct PointD : IEquatable<PointD>
	{
		/// <summary>
		/// (0,0)をもつPointDを返します
		/// </summary>
		public static readonly PointD Empty = new PointD(0, 0);

		/// <summary>
		/// X
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Y
		/// </summary>
		public double Y { get; set; }

		/// <summary>
		/// この PointD が空かどうかを示す値を取得します。
		/// </summary>
		/// <remarks>true=(0,0)である</remarks>
		public bool IsEmpty
		{
			get
			{
				return (this.X == 0) && (this.Y == 0);
			}
		}

		/// <summary>
		/// コンストラクタ(XY座標指定)
		/// </summary>
		/// <param name="pX">X</param>
		/// <param name="pY">Y</param>
		public PointD(double pX, double pY)
		{
			this.X = pX;
			this.Y = pY;
		}

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="pPoint">コピー元</param>
		public PointD(PointD pPoint)
		{
			this.X = pPoint.X;
			this.Y = pPoint.Y;
		}

		/// <summary>
		/// コンストラクタ(Point構造体から生成)
		/// </summary>
		/// <param name="pPoint">コピー元</param>
		public PointD(System.Drawing.Point pPoint)
		{
			this.X = pPoint.X;
			this.Y = pPoint.Y;
		}

		/// <summary>
		/// コンストラクタ(PointF構造体から生成)
		/// </summary>
		/// <param name="pPoint">コピー元</param>
		public PointD(System.Drawing.PointF pPoint)
		{
			this.X = pPoint.X;
			this.Y = pPoint.Y;
		}

		/// <summary>
		/// 最小の整数値取得
		/// </summary>
		/// <returns>PointD</returns>
		/// <remarks>
		/// 各値を最小の整数値に変換したオブジェクトを返します。
		/// </remarks>
		public PointD GetCeiling()
		{
			double x = Math.Ceiling(this.X);
			double y = Math.Ceiling(this.Y);
			return new PointD(x, y);
		}

		/// <summary>
		/// 最大の整数値取得
		/// </summary>
		/// <returns>PointD</returns>
		/// <remarks>
		/// 各値を最大の整数値に変換したオブジェクトを返します。
		/// </remarks>
		public PointD GetFloor()
		{
			double x = Math.Floor(this.X);
			double y = Math.Floor(this.Y);
			return new PointD(x, y);
		}

		/// <summary>
		/// 四捨五入値取得
		/// </summary>
		/// <param name="pFlag">MidpointRounding(既定値:ToEven)</param>
		/// <returns>PointD</returns>
		public PointD GetRound(MidpointRounding pFlag = MidpointRounding.ToEven)
		{
			double x = Math.Round(this.X, pFlag);
			double y = Math.Round(this.Y, pFlag);
			return new PointD(x, y);
		}

		/// <summary>
		/// 整数部取得
		/// </summary>
		/// <returns>PointD</returns>
		/// <remarks>
		/// 各値を整数値に変換したオブジェクトを返します。
		/// </remarks>
		public PointD GetTruncate()
		{
			double x = Math.Truncate(this.X);
			double y = Math.Truncate(this.Y);
			return new PointD(x, y);
		}

		/// <summary>
		/// 加算(PointD)
		/// </summary>
		/// <param name="pOffset">加算値</param>
		public void Add(PointD pOffset)
		{
			this.X += pOffset.X;
			this.Y += pOffset.Y;
		}

		/// <summary>
		/// 加算
		/// </summary>
		/// <param name="pX">X加算値</param>
		/// <param name="pY">Y加算値</param>
		public void Add(double pX, double pY)
		{
			this.X += pX;
			this.Y += pY;
		}

		/// <summary>
		/// 減算(PointD)
		/// </summary>
		/// <param name="pOffset">減算値</param>
		public void Subtract(PointD pOffset)
		{
			this.X -= pOffset.X;
			this.Y -= pOffset.Y;
		}

		/// <summary>
		/// 減算
		/// </summary>
		/// <param name="pX">X減算値</param>
		/// <param name="pY">Y減算値</param>
		public void Subtract(double pX, double pY)
		{
			this.X -= pX;
			this.Y -= pY;
		}

		/// <summary>
		/// 加算演算
		/// </summary>
		/// <param name="p1">値1</param>
		/// <param name="p2">値2</param>
		/// <returns>PointD</returns>
		public static PointD operator +(PointD p1, PointD p2)
		{
			return new PointD(p1.X + p2.X, p1.Y + p2.Y);
		}

		/// <summary>
		/// 減算演算
		/// </summary>
		/// <param name="p1">値1</param>
		/// <param name="p2">値2</param>
		/// <returns>PointD</returns>
		public static PointD operator -(PointD p1, PointD p2)
		{
			return new PointD(p1.X - p2.X, p1.Y - p2.Y);
		}

		/// <summary>
		/// 一致検査
		/// </summary>
		/// <param name="p1">比較元1</param>
		/// <param name="p2">比較元2</param>
		/// <returns>true=同一</returns>
		public static bool operator ==(PointD p1, PointD p2)
		{
			return (p1.X == p2.X) && (p1.Y == p2.Y);
		}

		/// <summary>
		/// 不一致検査
		/// </summary>
		/// <param name="p1">比較元1</param>
		/// <param name="p2">比較元2</param>
		/// <returns>true=不一致</returns>
		public static bool operator !=(PointD p1, PointD p2)
		{
			return !(p1 == p2);
		}

		/// <summary>
		/// 同一チェック
		/// </summary>
		/// <param name="pObject">比較先</param>
		/// <returns>true=同一</returns>
		public override bool Equals(object pObject)
		{
			if ((pObject == null) || !this.GetType().Equals(pObject.GetType())) {
				return false;
			}

			PointD p = (PointD)pObject;
			return this == p;
		}

		/// <summary>
		/// 同一チェック(PointD)
		/// </summary>
		/// <param name="pPoint"></param>
		/// <returns>true=同一</returns>
		public bool Equals(PointD pPoint)
		{
			return this.Equals(pPoint);
		}

		/// <summary>
		/// 明示的型変換(Pointへ)
		/// </summary>
		/// <param name="pPointD">変換元</param>
		public static explicit operator Point(PointD pPointD)
		{
			return new Point((int)pPointD.X, (int)pPointD.Y);
		}

		/// <summary>
		/// 明示的型変換(PointFへ)
		/// </summary>
		/// <param name="pPointD">変換元</param>
		public static explicit operator PointF(PointD pPointD)
		{
			return new PointF((float)pPointD.X, (float)pPointD.Y);
		}

		/// <summary>
		/// 暗黙的型変換(Point->PointD)
		/// </summary>
		/// <param name="pPoint">変換元</param>
		public static implicit operator PointD(Point pPoint)
		{
			return new PointD(pPoint);
		}

		/// <summary>
		/// 暗黙的型変換(PointF->PointD)
		/// </summary>
		/// <param name="pPoint">変換元</param>
		public static implicit operator PointD(PointF pPoint)
		{
			return new PointD(pPoint);
		}

		/// <summary>
		/// ハッシュコード取得
		/// </summary>
		/// <returns>ハッシュコード</returns>
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}
	}
}
