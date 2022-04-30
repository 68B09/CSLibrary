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
using System.Collections.ObjectModel;

namespace CSLibrary.CSGeometries.Units
{
	/// <summary>
	/// ２進(1024)単位クラス
	/// </summary>
	public class BinUnit
	{
		#region 固定値
		#region 2進(Binary)/1024単位定義 https://ja.wikipedia.org/wiki/2%E9%80%B2%E6%8E%A5%E9%A0%AD%E8%BE%9E
		public const int Tei = 2;
		public const string UnitNameYobi = "Yi";
		public const string UnitNameZebi = "Zi";
		public const string UnitNameExbi = "Ei";
		public const string UnitNamePebi = "Pi";
		public const string UnitNameTebi = "Ti";
		public const string UnitNameGibi = "Gi";
		public const string UnitNameMebi = "Mi";
		public const string UnitNameKibi = "Ki";
		#endregion
		#endregion

		#region フィールド・プロパティー
		static readonly public ReadOnlyCollection<string> UnitBigNames = new ReadOnlyCollection<string>(new string[] {
			"", UnitNameKibi, UnitNameMebi, UnitNameGibi, UnitNameTebi, UnitNamePebi,
			UnitNameExbi, UnitNameZebi, UnitNameYobi });
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BinUnit()
		{
		}
		#endregion

		#region Normalize
		/// <summary>
		/// 2進(1024)単位へ丸め(正規化)
		/// </summary>
		/// <param name="pValue">値</param>
		/// <param name="pUnitName">単位名</param>
		/// <returns>丸めたあとの値</returns>
		/// <remarks>
		/// ・本メソッドはpValueを2進単位へ丸める。
		/// ・doubleの精度や計算誤差により必ずしも正確な値へ変換されるとは限らない。
		/// ・戻り値の小数点以下を切り上げた場合は整数部が単位(1024)と等しくなる可能性があることに注意。
		/// 例えば1023.9Kiの小数点を切り上げると1024Kiとなる。
		/// ・上限を超える場合は上限の単位で表現される。
		/// 
		/// (例)
		/// pValue:1263.616 → return:1.234 pUnitName:"Ki"
		/// pValue:39614081257132169000000000000 → return:32768 pUnitName:"Yi"
		/// </remarks>
		public double Normalize(double pValue, out string pUnitName)
		{
			const int unitValue = 1024;
			int unitIndex = 0;

			double ans = Math.Abs(pValue);

			while (true) {
				if ((ans < unitValue) || (unitIndex >= (UnitBigNames.Count - 1))) {
					break;
				}
				ans /= unitValue;
				unitIndex++;
			}

			if (pValue < 0) {
				ans *= -1;
			}

			pUnitName = UnitBigNames[unitIndex];
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
		/// 単位名("Ki"や"Mi"など)に対応する冪指数(2^nのn)を返す。
		/// "@"など認識出来ない単位名が渡された場合はint.MinValueを返す。
		/// 空文字""には0を返す。
		/// 単位名の大文字小文字は厳密に扱われる。
		/// </remarks>
		public int GetBekisisu(string pUnitName)
		{
			// Ki,Mi,Gi...
			for (int i = 0; i < UnitBigNames.Count; i++) {
				if (pUnitName == UnitBigNames[i]) {
					return i * 10;
				}
			}

			return int.MinValue;
		}
		#endregion
	}
}