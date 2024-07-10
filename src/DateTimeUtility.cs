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
using System.Globalization;

namespace CSLibrary
{
	/// <summary>
	/// DateTimeのユーティリティークラスです。
	/// </summary>
	static public class DateTimeUtility
	{
		/// <summary>
		/// 結合フラグ
		/// </summary>
		[Flags]
		public enum CombineFlags : int
		{
			None = 0,
			Year = 1,                       // 西暦4桁(1～)
			Month = Year * 2,               // 月2桁(1～12)
			Day = Month * 2,                // 日2桁(1～31)
			WeekOfYear = Day * 2,           // 週2桁(1～53)
			DayOfYear = WeekOfYear * 2,     // 日数3桁(1～366)

			Hour = DayOfYear * 2,           // 24時2桁(0～23)
			Minute = Hour * 2,              // 分2桁(0～59)
			Second = Minute * 2,            // 秒2桁(0～59)
			Milli = Second * 2,             // ミリ秒3桁(0～999)
			SecondOfDay = Milli * 2,        // 経過秒数5桁(0～86399)
			MilliOfDay = SecondOfDay * 2,   // 経過ミリ秒数8桁(0～86399999)

			All = -1,
			YYYYMMDD = Year | Month | Day,
			HHMMSS = Hour | Minute | Second,
			HHMMSSFFF = Hour | Minute | Second | Milli,
			YYYYMMDDHHMMSS = YYYYMMDD | HHMMSS,
		}

		/// <summary>
		/// 年月日などを連結した値を取得
		/// </summary>
		/// <param name="pSrc">DateTime</param>
		/// <param name="pFlag">結合フラグ</param>
		/// <remarks>
		/// pFlagで指定された項目を並べた値を返します。
		/// 項目の並び順はCombineFlagsの定義順(Yearが先頭)です。
		/// WeekOfYearとDayOfYearは現在のカルチャに依存します。
		/// 桁あふれを起こさないようpFlagに指定する項目数には注意してください。
		/// 
		/// (例)pSrc=2024年1月2日 12時34分56.789秒
		/// pFlag = Year|Month -> 202401
		/// Month|Day -> 102
		/// Day|Second -> 256
		/// </remarks>
		static public long CombinedBinary(this DateTime pSrc, CombineFlags pFlag = CombineFlags.YYYYMMDDHHMMSS)
		{
			long result = 0;

			if ((pFlag & CombineFlags.Year) != CombineFlags.None) {
				result += pSrc.Year;
			}
			if ((pFlag & CombineFlags.Month) != CombineFlags.None) {
				result = (result * 100) + pSrc.Month;
			}
			if ((pFlag & CombineFlags.Day) != CombineFlags.None) {
				result = (result * 100) + pSrc.Day;
			}
			if ((pFlag & CombineFlags.WeekOfYear) != CombineFlags.None) {
				Calendar calen = CultureInfo.CurrentCulture.Calendar;
				DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
				result = (result * 100) + calen.GetWeekOfYear(pSrc, dtfi.CalendarWeekRule, dtfi.FirstDayOfWeek);
			}
			if ((pFlag & CombineFlags.DayOfYear) != CombineFlags.None) {
				Calendar calen = CultureInfo.CurrentCulture.Calendar;
				result = (result * 1000) + calen.GetDayOfYear(pSrc);
			}

			if ((pFlag & CombineFlags.Hour) != CombineFlags.None) {
				result = (result * 100) + pSrc.Hour;
			}
			if ((pFlag & CombineFlags.Minute) != CombineFlags.None) {
				result = (result * 100) + pSrc.Minute;
			}
			if ((pFlag & CombineFlags.Second) != CombineFlags.None) {
				result = (result * 100) + pSrc.Second;
			}
			if ((pFlag & CombineFlags.Milli) != CombineFlags.None) {
				result = (result * 1000) + pSrc.Millisecond;
			}
			if ((pFlag & CombineFlags.SecondOfDay) != CombineFlags.None) {
				result = (result * 100000) + (pSrc.Hour * 3600) + (pSrc.Minute * 60) + pSrc.Second;
			}
			if ((pFlag & CombineFlags.MilliOfDay) != CombineFlags.None) {
				result = (result * 100000000) + (pSrc.Hour * 3600000) + (pSrc.Minute * 60000) + (pSrc.Second * 1000) + pSrc.Millisecond;
			}

			return result;
		}

		/// <summary>
		/// パース
		/// </summary>
		/// <param name="pValue">連結値</param>
		/// <param name="pFlag">結合フラグ</param>
		/// <returns>DateTime</returns>
		/// <remarks>
		/// CombinedBinary()の値からDateTimeを作成します。
		/// pFlagに指定できるフラグはYear,Month,Day,Hour,Minute,Second,Milliの組み合わせのみです。
		/// 省略された日付には1、時刻には0があてられます。
		/// 例えばDayとHourのみを指定した場合、"0001/01/dd hh:00:00.000"のDateTimeが返されます。
		/// </remarks>
		static public DateTime Parse(long pValue, CombineFlags pFlag = CombineFlags.YYYYMMDDHHMMSS)
		{
			if ((pFlag & ~(CombineFlags.YYYYMMDDHHMMSS | CombineFlags.Milli)) != CombineFlags.None) {
				throw new ArgumentOutOfRangeException();
			}

			int year = 1;
			int month = 1;
			int day = 1;
			int hour = 0;
			int minute = 0;
			int second = 0;
			int milli = 0;

			if ((pFlag & CombineFlags.Milli) != CombineFlags.None) {
				milli = (int)(pValue % 1000);
				pValue /= 1000;
			}
			if ((pFlag & CombineFlags.Second) != CombineFlags.None) {
				second = (int)(pValue % 100);
				pValue /= 100;
			}
			if ((pFlag & CombineFlags.Minute) != CombineFlags.None) {
				minute = (int)(pValue % 100);
				pValue /= 100;
			}
			if ((pFlag & CombineFlags.Hour) != CombineFlags.None) {
				hour = (int)(pValue % 100);
				pValue /= 100;
			}

			if ((pFlag & CombineFlags.Day) != CombineFlags.None) {
				day = (int)(pValue % 100);
				pValue /= 100;
			}
			if ((pFlag & CombineFlags.Month) != CombineFlags.None) {
				month = (int)(pValue % 100);
				pValue /= 100;
			}
			if ((pFlag & CombineFlags.Year) != CombineFlags.None) {
				year = (int)(pValue % 10000);
				pValue /= 10000;
			}

			return new DateTime(year, month, day, hour, minute, second, milli);
		}
	}
}
