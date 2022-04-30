/*
The MIT License (MIT)

Copyright (c) 2022 ZZO.

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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CSLibrary.CSGeometries.Units
{
	/// <summary>
	/// SI(1000)単位クラス
	/// </summary>
	public class SIUnit
	{
		#region 固定値
		#region SI/1000単位定義 https://ja.wikipedia.org/wiki/SI%E6%8E%A5%E9%A0%AD%E8%BE%9E#%E4%B8%80%E8%A6%A7
		public const int Tei = 10;
		public const string UnitNameQuetta = "Q";
		public const string UnitNameRonna = "R";
		public const string UnitNameYotta = "Y";
		public const string UnitNameZetta = "Z";
		public const string UnitNameExa = "E";
		public const string UnitNamePeta = "P";
		public const string UnitNameTera = "T";
		public const string UnitNameGiga = "G";
		public const string UnitNameMega = "M";
		public const string UnitNameKilo = "k";
		public const string UnitNameMilli = "m";
		public const string UnitNameMicro = "u";
		public const string UnitNameNano = "n";
		public const string UnitNamePico = "p";
		public const string UnitNameFemto = "f";
		public const string UnitNameAtto = "a";
		public const string UnitNameZepto = "z";
		public const string UnitNameYocto = "y";
		public const string UnitNameRonto = "r";
		public const string UnitNameQuecto = "q";
		#endregion
		#endregion

		#region フィールド・プロパティー
		/// <summary>
		/// 正の冪指数単位名配列
		/// </summary>
		static readonly public ReadOnlyCollection<string> UnitBigNames = new ReadOnlyCollection<string>(new string[] {
			"", UnitNameKilo, UnitNameMega, UnitNameGiga, UnitNameTera, UnitNamePeta,
			UnitNameExa, UnitNameZetta, UnitNameYotta, UnitNameRonna, UnitNameQuetta });

		/// <summary>
		/// 負の冪指数単位名配列
		/// </summary>
		static readonly public ReadOnlyCollection<string> UnitLittleNames = new ReadOnlyCollection<string>(new string[] {
			"", UnitNameMilli, UnitNameMicro, UnitNameNano, UnitNamePico, UnitNameFemto,
			UnitNameAtto, UnitNameZepto, UnitNameYocto, UnitNameRonto, UnitNameQuecto });
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SIUnit()
		{
		}
		#endregion

		#region Normalize
		/// <summary>
		/// SI(1000)単位へ丸め(正規化)
		/// </summary>
		/// <param name="pValue">値</param>
		/// <param name="pUnitName">単位名</param>
		/// <returns>丸めたあとの値</returns>
		/// <remarks>
		/// ・本メソッドはpValueをSI単位へ丸める。
		/// ・doubleの精度や計算誤差により必ずしも正確な値へ変換されるとは限らない。
		/// ・戻り値の小数点以下を切り上げた場合は整数部が単位(1000)と等しくなる可能性があることに注意。
		/// 例えば999.9kの小数点を切り上げると1000kとなる。
		/// ・上限や下限を超える場合は上限もしくは下限の単位で表現される。
		/// 
		/// (例)
		/// pValue:1000 → return:1 pUnitName:"k"
		/// pValue:10000000000000000000000000000000000 → return:10000 pUnitName:"Q"
		/// </remarks>
		public double Normalize(double pValue, out string pUnitName)
		{
			const int unitValue = 1000;
			IList<string> unitNames;
			int unitIndex = 0;

			double ans = Math.Abs(pValue);

			if (ans >= 1) {
				// 1以上の単位
				unitNames = UnitBigNames;
				while (true) {
					if ((ans < unitValue) || (unitIndex >= (unitNames.Count - 1))) {
						break;
					}
					ans /= unitValue;
					unitIndex++;
				}
			} else {
				// 1未満の単位
				unitNames = UnitLittleNames;
				while (true) {
					if ((ans > 1.0) || (unitIndex >= (unitNames.Count - 1))) {
						break;
					}
					ans *= unitValue;
					unitIndex++;
				}
			}

			if (pValue < 0) {
				ans *= -1;
			}

			pUnitName = unitNames[unitIndex];
			return ans;
		}
		#endregion

		#region GetBekisisu
		/// <summary>
		/// 単位に対応する冪指数を返す
		/// </summary>
		/// <param name="pUnitName">単位名</param>
		/// <returns>冪指数。エラー時はint.MinValue</returns>
		/// <remarks>
		/// 単位名("k"や"M"など)に対応する冪指数(10^nのn)を返す。
		/// "@"など認識出来ない単位名が渡された場合はint.MinValueを返す。
		/// 空文字""には0を返す。
		/// 単位名の大文字小文字は厳密に扱われる。
		/// </remarks>
		public int GetBekisisu(string pUnitName)
		{
			// k,M,G...
			for (int i = 0; i < UnitBigNames.Count; i++) {
				if (pUnitName == UnitBigNames[i]) {
					return i * 3;
				}
			}

			// m,u,n...
			for (int i = 0; i < UnitLittleNames.Count; i++) {
				if (pUnitName == UnitLittleNames[i]) {
					return i * -3;
				}
			}

			return int.MinValue;
		}
		#endregion
	}
}