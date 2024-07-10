# DateTimeUtility
**概要**
==========
System.DateTimeの拡張メソッドなどを収めたクラスです。  
動作に関してはソースファイルも参照して下さい。

●**結合フラグ**  
-----
**static public long CombinedBinary(this DateTime pSrc, CombineFlags pFlag = CombineFlags.YYYYMMDDHHMMSS)**  
CombinedBinary()で使用する結合フラグです。  
```
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
```

●**連結した値を取得**  
-----
**static public long CombinedBinary(this DateTime pSrc, CombineFlags pFlag = CombineFlags.YYYYMMDDHHMMSS)**  
pFlagで指定された項目を並べた値を返します。  
項目の並び順はCombineFlagsの定義順(Yearが先頭)です。  
WeekOfYearとDayOfYearは現在のカルチャに依存します。  
桁あふれを起こさないようpFlagに指定する項目数には注意してください。  
```
DateTime dt = new DateTime(2024,1,2,12,34,56,789);
long ll = dt.CombinedBinary();
System.Diagnostics.Debug.WriteLine(ll); // 20240102123456

ll = dt.CombinedBinary(DateTimeUtility.CombineFlags.Year | DateTimeUtility.CombineFlags.Month);
System.Diagnostics.Debug.WriteLine(ll); // 202401

ll = dt.CombinedBinary(DateTimeUtility.CombineFlags.Month | DateTimeUtility.CombineFlags.Day);
System.Diagnostics.Debug.WriteLine(ll); // 102

ll = dt.CombinedBinary(DateTimeUtility.CombineFlags.Day | DateTimeUtility.CombineFlags.Second);
System.Diagnostics.Debug.WriteLine(ll); // 256
```

●**パース**  
-----
**static public DateTime Parse(long pValue, CombineFlags pFlag = CombineFlags.YYYYMMDDHHMMSS)**  
CombinedBinary()の値からDateTimeを作成します。  
pFlagに指定できるフラグはYear,Month,Day,Hour,Minute,Second,Milliの組み合わせのみです。  
省略された日付には1、時刻には0があてられます。  
例えばDayとHourのみを指定した場合、"0001/01/dd hh:00:00.000"のDateTimeが返されます。  
```
DateTime dt = DateTimeUtility.Parse(20241231123456L);
System.Diagnostics.Debug.WriteLine(dt.ToString("yyyy/MM/dd HH:mm:ss.fff"));
// 2024/12/31 12:34:56.000
```