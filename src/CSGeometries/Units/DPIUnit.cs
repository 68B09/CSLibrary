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

using System;

namespace CSLibrary.CSGeometries.Units
{
	#region フィールド・プロパティー
	/// <summary>
	/// DPI(Dot Per Inch)基準変換クラス
	/// </summary>
	public class DPIUnit
	{
		/// <summary>
		/// DPI
		/// </summary>
		private float dpi = 0;

		/// <summary>
		/// DPI取得・設定
		/// </summary>
		public float DPI
		{
			get
			{
				return dpi;
			}
			set
			{
				dpi = value;
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DPIUnit() { }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pDPI">DPI</param>
		public DPIUnit(float pDPI) : this()
		{
			this.dpi = pDPI;
		}
		#endregion

		#region ミリメートル
		/// <summary>
		/// ミリメートルをドットへ変換
		/// </summary>
		/// <typeparam name="T">返却値型</typeparam>
		/// <param name="pMM">ミリメートル</param>
		/// <returns>ドット数</returns>
		public T MMToDot<T>(double pMM)
		{
			double ans = (pMM * this.dpi) / CSLibraryConstants.MilliMeterPerInch;
			return (T)DoubleTo(typeof(T), ans);
		}

		/// <summary>
		/// ドットをミリメートルへ変換
		/// </summary>
		/// <typeparam name="T">返却値型</typeparam>
		/// <param name="pDot">ドット数</param>
		/// <returns>ミリメートル</returns>
		public T DotToMM<T>(double pDot)
		{
			double ans = (pDot * CSLibraryConstants.MilliMeterPerInch) / this.dpi;
			return (T)DoubleTo(typeof(T), ans);
		}
		#endregion

		#region ポイント
		/// <summary>
		/// ポイント(DTPポイント,1pt=1/72inch)をドットへ変換
		/// </summary>
		/// <typeparam name="T">返却値型</typeparam>
		/// <param name="pPT">ポイント</param>
		/// <returns>ドット数</returns>
		public T PTToDot<T>(double pPT)
		{
			double ans = (pPT * this.dpi) / CSLibraryConstants.PointPerInch;
			return (T)DoubleTo(typeof(T), ans);
		}

		/// <summary>
		/// ドットをポイントへ変換
		/// </summary>
		/// <typeparam name="T">返却値型</typeparam>
		/// <param name="pDot">ドット数</param>
		/// <returns>ポイント</returns>
		public T DotToPT<T>(double pDot)
		{
			double ans = (pDot * CSLibraryConstants.PointPerInch) / this.dpi;
			return (T)DoubleTo(typeof(T), ans);
		}
		#endregion

		#region 型変換
		/// <summary>
		/// double型を指定型へ変換
		/// </summary>
		/// <param name="pType">変換先の型</param>
		/// <param name="pValue">変換値</param>
		/// <returns>型変換後の値(object型)</returns>
		/// <exception cref="InvalidOperationException">変換出来ない型を指定された</exception>
		static public object DoubleTo(Type pType, double pValue)
		{
			if (pType == typeof(Double)) {
				return pValue;						// Double
			} else if (pType == typeof(Single)) {
				return (Single)pValue;				// Single(float)
			} else if (pType == typeof(Decimal)) {
				return (Decimal)pValue;				// Decimal
			} else if (pType == typeof(SByte)) {
				return (SByte)pValue;				// SByte
			} else if (pType == typeof(Byte)) {
				return (Byte)pValue;				// Byte
			} else if (pType == typeof(Int16)) {
				return (Int16)pValue;				// Int16(short)
			} else if (pType == typeof(UInt16)) {
				return (UInt16)pValue;				// UInt16(ushort)
			} else if (pType == typeof(Int32)) {
				return (Int32)pValue;				// Int32(int)
			} else if (pType == typeof(UInt32)) {
				return (UInt32)pValue;				// UInt32(uint)
			} else if (pType == typeof(Int64)) {
				return (Int64)pValue;				// Int64(long)
			} else if (pType == typeof(UInt64)) {
				return (UInt64)pValue;				// UInt64(ulong)
			}
			throw new InvalidOperationException();
		}
		#endregion
	}
}
