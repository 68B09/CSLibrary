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
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using static CSLibrary.Network.IPv4Range;

namespace CSLibrary.Network
{
	/// <summary>
	/// IPv4アドレスを対象とする基本クラス
	/// </summary>
	public class IPv4Base
	{
		/// <summary>
		/// 文字列型のIPv4アドレスをバイナリへ変換。
		/// </summary>
		/// <param name="pIPv4">IPv4アドレス文字列(x.x.x.x)</param>
		/// <returns>バイナリ化したIPv4アドレス</returns>
		/// <remarks>
		/// 文字列をバイナリに変換します。
		/// (ex)"198.51.100.1" -> 3325256705
		/// </remarks>
		static public uint StringToUInt(string pIPv4)
		{
			IPAddress ipaddress = IPAddress.Parse(pIPv4);
			byte[] bytes = ipaddress.GetAddressBytes();
			return (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
		}

		/// <summary>
		/// バイナリ型のIPv4アドレスを文字列へ変換。
		/// </summary>
		/// <param name="pIPv4">IPv4アドレス</param>
		/// <returns>文字列化したIPv4アドレス</returns>
		/// <remarks>
		/// バイナリを文字列へ変換します。
		/// </remarks>
		static public string UIntToString(uint pIPv4)
		{
			byte[] bytes = BitConverter.GetBytes(pIPv4);
			if (BitConverter.IsLittleEndian) {
				return string.Format("{0}.{1}.{2}.{3}"
					, bytes[3], bytes[2], bytes[1], bytes[0]);
			} else {
				return string.Format("{0}.{1}.{2}.{3}"
					, bytes[0], bytes[1], bytes[2], bytes[3]);
			}
		}

		/// <summary>
		/// 各種アドレス表記文字列からオブジェクトを生成
		/// </summary>
		/// <param name="pAddress">IPv4アドレス</param>
		/// <returns>null=エラー</returns>
		/// <remarks>
		/// 認識するアドレス文字列と返却されるオブジェクト。
		/// "198.51.100.0"					IPAddress
		/// "198.51.100.0/24"				IPv4Range
		/// "198.51.100.0 198.51.100.1"		IPv4Range
		/// "198.51.100.0-198.51.100.1"		IPv4Range
		/// "198.51.100.0/255.255.255.0"	IPv4Range
		/// </remarks>
		static public object Parse(string pAddress)
		{
			// "198.51.100.0" IPAddress
			Regex regex = new Regex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");
			Match match = regex.Match(pAddress);
			if (match.Success) {
				return IPAddress.Parse(pAddress);
			}

			// "198.51.100.0/24" IPv4Range
			regex = new Regex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+/[0-9]+$");
			match = regex.Match(pAddress);
			if (match.Success) {
				return new IPv4Range(pAddress);
			}

			// "198.51.100.0 198.51.100.1" IPv4Range
			// "198.51.100.0-198.51.100.1" IPv4Range
			regex = new Regex(@"^(?<ADDRFROM>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)[ -](?<ADDRTO>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)$");
			match = regex.Match(pAddress);
			if (match.Success) {
				return new IPv4Range(match.Groups["ADDRFROM"].Value, match.Groups["ADDRTO"].Value);
			}

			// "198.51.100.0/255.255.255.0"	IPv4Range
			regex = new Regex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+/[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");
			match = regex.Match(pAddress);
			if (match.Success) {
				return new IPv4Range(pAddress);
			}

			return null;
		}

		/// <summary>
		/// ホスト部のマスクビットを作成(ネットワーク部ビット数指定)
		/// </summary>
		/// <param name="pBits">マスクビット数</param>
		/// <returns>ビットマスク</returns>
		/// <remarks>
		/// MakeHostMask(24) -> 255
		/// </remarks>
		static public uint MakeHostMask(int pBits)
		{
			if (pBits == 32) {
				return 0;
			}
			return uint.MaxValue >> pBits;
		}

		/// <summary>
		/// ネットワーク部のマスクビットを作成(ネットワーク部4バイト値指定)
		/// </summary>
		/// <param name="pMask">マスク(x.x.x.x)</param>
		/// <returns>ビットマスク</returns>
		/// <remarks>
		/// MakeNetworkMask("255.255.255.0") -> 0xffffff00
		/// </remarks>
		static public uint MakeNetworkMask(string pMask)
		{
			string[] fields = pMask.Split('.');
			if (fields.Length != 4) {
				throw new ArgumentException();
			}

			Int64 mask = 0;
			for (int i = 0; i < 4; i++) {
				mask <<= 8;
				switch (fields[i]) {
					case "128":
					case "192":
					case "224":
					case "240":
					case "248":
					case "252":
					case "254":
					case "255":
						if (i != 0) {
							if (fields[i - 1] != "255") {
								throw new ArgumentException();
							}
						}

						mask |= Int64.Parse(fields[i]);
						break;

					case "0":
						break;

					default:
						throw new ArgumentException();
				}
			}

			return (uint)mask;
		}
	}

	/// <summary>
	/// IPv4アドレスの範囲を扱うクラス
	/// </summary>
	public class IPv4Range
	{
		#region フィールド・プロパティー
		/// <summary>
		/// 正規表現－CIDR表記
		/// </summary>
		static public readonly Regex regexCIDR = new Regex(@"^(?<ADDR>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)/(?<BITS>[0-9]+)$");

		/// <summary>
		/// 正規表現－マスク表現
		/// </summary>
		static public readonly Regex regexMask = new Regex(@"^(?<ADDR>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)/(?<MASK>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)$");

		/// <summary>
		/// アドレス最小値
		/// </summary>
		private uint min;
		/// <summary>
		/// アドレス最小値取得
		/// </summary>
		public uint Min
		{
			get
			{
				return this.min;
			}
		}

		/// <summary>
		/// アドレス最大値
		/// </summary>
		private uint max;
		/// <summary>
		/// アドレス最大値取得
		/// </summary>
		public uint Max
		{
			get
			{
				return this.max;
			}
		}

		/// <summary>
		/// アドレス個数取得
		/// </summary>
		public long Count
		{
			get
			{
				return (long)this.max - (long)this.min + 1;
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IPv4Range()
		{
			this.min = 0;
			this.max = 0;
		}

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="pSrc">コピー元</param>
		public IPv4Range(IPv4Range pSrc)
		{
			this.min = pSrc.min;
			this.max = pSrc.max;
		}

		/// <summary>
		/// コンストラクタ(バイナリ指定)
		/// </summary>
		/// <param name="pMin">アドレス最小値</param>
		/// <param name="pMax">アドレス最大値</param>
		/// <remarks>
		/// new IPv4Range((uint)3325256705, (uint)3325256706)
		/// </remarks>
		public IPv4Range(uint pMin, uint pMax) : this()
		{
			this.min = pMin;
			this.max = pMax;
		}

		/// <summary>
		/// コンストラクタ(CIDR表記)
		/// </summary>
		/// <param name="pCIDR">CIDR表記のアドレス文字列</param>
		/// <remarks>
		/// new IPv4Range("198.51.100.0/24")
		/// </remarks>
		public IPv4Range(string pCIDR) : this()
		{
			Match match = regexCIDR.Match(pCIDR);
			if (match.Success) {
				this.min = IPv4Base.StringToUInt(match.Groups["ADDR"].Value);

				int bits = int.Parse(match.Groups["BITS"].Value);
				if ((bits < 0) || (bits > 32)) {
					throw new ArgumentOutOfRangeException();
				}
				this.max = this.min | IPv4Base.MakeHostMask(bits);
				return;
			}

			match = regexMask.Match(pCIDR);
			if (match.Success) {
				this.min = IPv4Base.StringToUInt(match.Groups["ADDR"].Value);
				Int64 mask = IPv4Base.MakeNetworkMask(match.Groups["MASK"].Value);
				this.max = this.min | ~((uint)mask);
				return;
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// コンストラクタ(.区切りアドレス文字列)
		/// </summary>
		/// <param name="pIPv4From">最小アドレス</param>
		/// <param name="pIPv4To">最大アドレス</param>
		/// <remarks>
		/// new IPv4Range("198.51.100.0", "198.51.100.255")
		/// </remarks>
		public IPv4Range(string pIPv4From, string pIPv4To) : this()
		{
			this.min = IPv4Base.StringToUInt(pIPv4From);
			this.max = IPv4Base.StringToUInt(pIPv4To);
			if (this.min > this.max) {
				throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// コンストラクタ(最小アドレス文字列と個数)
		/// </summary>
		/// <param name="pIPv4">最小アドレス</param>
		/// <param name="pCount">個数(1以上)</param>
		/// <remarks>
		/// new IPv4Range("198.51.100.0", 256)
		/// </remarks>
		public IPv4Range(string pIPv4, int pCount) : this()
		{
			this.min = IPv4Base.StringToUInt(pIPv4);

			if (pCount < 0) {
				throw new ArgumentOutOfRangeException();
			}

			Int64 max = (Int64)this.min;
			if (pCount >= 1) {
				max += (pCount - 1);
			}

			if (max > uint.MaxValue) {
				throw new ArgumentOutOfRangeException();
			}
			this.max = (uint)max;
		}
		#endregion

		/// <summary>
		/// 最小・最大値設定
		/// </summary>
		/// <param name="pMin">最小値</param>
		/// <param name="pMax">最大値</param>
		/// <param name="blPower">true=値のチェックを行わない。</param>
		public void SetMinMax(uint pMin, uint pMax, bool blPower = false)
		{
			if (!blPower) {
				if (pMin > pMax) {
					throw new ArgumentOutOfRangeException();
				}
			}
			this.min = pMin;
			this.max = pMax;
		}

		/// <summary>
		/// 現在の範囲を表すCIDR表記文字列を生成
		/// </summary>
		/// <param name="pList">CIDRを追加する配列</param>
		/// <returns>CIDR表記文字列リスト</returns>
		/// <remarks>
		/// 複数の文字列が返されることがあります。
		/// pListにnullを指定した場合は内部で生成したリストオブジェクトを返します。
		/// </remarks>
		public IList<string> ToCIDR(IList<string> pList = null)
		{
			IList<string> result;
			if (pList == null) {
				result = new List<string>();
			} else {
				result = pList;
				result.Clear();
			}

			long start = this.min;
			long end = this.max;
			double log2 = Math.Log(2);

			while (end >= start) {
				int maxSize = 32;
				while (maxSize > 0) {
					long mask = ~IPv4Base.MakeHostMask(maxSize - 1);
					long maskedBase = start & mask;

					if (maskedBase != start) {
						break;
					}

					maxSize--;
				}

				int maxDiff = (int)(Math.Log(end - start + 1) / log2);
				int maxDiffSize = (32 - maxDiff);
				if (maxSize < maxDiffSize) {
					maxSize = maxDiffSize;
				}

				result.Add($"{IPv4Base.UIntToString((uint)start)}/{maxSize}");
				start += (long)Math.Pow(2, (32 - maxSize));
			}

			return result;
		}

		/// <summary>
		/// アドレス内包チェック(バイナリ)
		/// </summary>
		/// <param name="pIPv4">チェックするアドレス</param>
		/// <returns>true=内包する</returns>
		public bool InRange(uint pIPv4)
		{
			return (pIPv4 >= this.min) && (pIPv4 <= this.max);
		}

		/// <summary>
		/// 内包チェック(IPV4Range)
		/// </summary>
		/// <param name="pRange">チェックするアドレス</param>
		/// <returns>true=内包する</returns>
		/// <remarks>
		/// pRangeが示す範囲のアドレスが本範囲に完全に含まれているかをチェックします。
		/// </remarks>
		public bool InRange(IPv4Range pRange)
		{
			return (pRange.min >= this.min) && (pRange.max <= this.max);
		}

		/// <summary>
		/// 重なりフラグ
		/// </summary>
		public enum OverlapFlags : int
		{
			Min = -2,   // 重ならず相手が小さい
			Max = -1,   // 重ならず相手が大きい
			Equal = 0,  // 完全一致(どちらかを削除できる)
			Inside,     // 内側に入る(相手を削除できる)
			Surrounded, // 自分を囲う(自身を削除できる)
			CrossMin,   // 小さい方に重なる(小さい方へ拡張できる)
			CrossMax,   // 大きい方に重なる(大きい方へ拡張できる)
			ContactMin, // 小さい方に接触している(小さい方へ拡張できる)
			ContactMax, // 大きい方に接触している(大きい方へ拡張できる)
		}

		/// <summary>
		/// 重なりチェック
		/// </summary>
		/// <param name="pTarget">チェック相手</param>
		/// <returns>OverlapFlags</returns>
		/// <remarks>
		/// 自身と相手のアドレス範囲の重なり具合を返します。
		/// </remarks>
		public OverlapFlags CheckOverlap(IPv4Range pTarget)
		{
			// 小さい方に重なる？
			if (((long)pTarget.max + 1) == (long)this.min) {
				return OverlapFlags.ContactMin;
			}

			// 大きい方に重なる？
			if (((long)this.max + 1) == (long)pTarget.min) {
				return OverlapFlags.ContactMax;
			}

			// 重ならず相手が小さい？
			if (pTarget.max < this.min) {
				return OverlapFlags.Min;
			}

			// 重ならず相手が大きい？
			if (this.max < pTarget.min) {
				return OverlapFlags.Max;
			}

			// 完全一致？
			if ((this.min == pTarget.min) && (this.max == pTarget.max)) {
				return OverlapFlags.Equal;
			}

			// 内側に入る？
			if (this.InRange(pTarget)) {
				return OverlapFlags.Inside;
			}

			// 自分を囲う？
			if ((this.min >= pTarget.min) && (this.max <= pTarget.max)) {
				return OverlapFlags.Surrounded;
			}

			// 小さい方に重なる(小さい方を拡張できる)？
			if (this.InRange(pTarget.min)) {
				return OverlapFlags.CrossMin;
			}

			// 大きい方に重なる(大きい方を拡張できる)？
			if (this.InRange(pTarget.max)) {
				return OverlapFlags.CrossMax;
			}

			throw new InvalidProgramException();
		}

		/// <summary>
		/// ハッシュ作成
		/// </summary>
		/// <returns>ハッシュ値</returns>
		public override int GetHashCode()
		{
			return this.min.GetHashCode() ^ this.max.GetHashCode();
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

			IPv4Range target = (IPv4Range)obj;

			if ((this.min != target.min) || (this.max != target.max)) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// 文字列化
		/// </summary>
		/// <returns>文字列</returns>
		public override string ToString()
		{
			return IPv4Base.UIntToString(this.min) + "-" + IPv4Base.UIntToString(this.max);
		}
	}

	/// <summary>
	/// IPv4Rangeのコレクションクラス
	/// </summary>
	public class IPv4RangeCollection : List<IPv4Range>
	{
		/// <summary>
		/// ソート
		/// </summary>
		/// <remarks>
		/// 最小アドレス(Min)をキーに昇順ソートを行います。
		/// </remarks>
		public void SortByMinimumAddress()
		{
			this.Sort((x, y) => x.Min.CompareTo(y.Min));
		}

		/// <summary>
		/// アドレスマージ
		/// </summary>
		/// <returns>マージ後データリスト</returns>
		/// <remarks>
		/// 結合可能なアドレス同士を結合し、マージ後のデータリストを返します。
		/// 自身のオブジェクトの内容は変更されません。
		/// マージ後のデータリストは必要であればソートを行ってください。
		/// </remarks>
		public IPv4RangeCollection Merge()
		{
			return Merge(this);
		}

		/// <summary>
		/// アドレスマージ
		/// </summary>
		/// <param name="pRangeDatas">マージデータリスト</param>
		/// <returns>マージ後データリスト</returns>
		/// <seealso cref="Merge"/>
		/// <remarks>
		/// 結合可能なアドレス同士を結合し、マージ後のデータリストを返します。
		/// pRangeDatas内のオブジェクトの内容は変更されません。
		/// </remarks>
		static public IPv4RangeCollection Merge(IEnumerable<IPv4Range> pRangeDatas)
		{
			// コピーを作る。
			IPv4RangeCollection result = new IPv4RangeCollection();
			foreach (IPv4Range srcRange in pRangeDatas) {
				result.Add(new IPv4Range(srcRange));
			}
			result.SortByMinimumAddress();

			// 結合相手が無くなるまで相手を探して結合する
			while (result.Count >= 2) {
				bool blProcessed = false;
				for (int i = 0; i < result.Count - 1; i++) {
					for (int j = i + 1; j < result.Count; j++) {
						OverlapFlags flag = result[i].CheckOverlap(result[j]);
						if (flag < 0) {
							continue;
						} else if ((flag == OverlapFlags.Equal) || (flag == OverlapFlags.Inside)) {
							result.RemoveAt(j);
							blProcessed = true;
							j--;
						} else if (flag == OverlapFlags.Surrounded) {
							result.RemoveAt(i);
							blProcessed = true;
						} else if ((flag == OverlapFlags.CrossMin) || (flag == OverlapFlags.ContactMin)) {
							result[i].SetMinMax(result[j].Min, result[i].Max);
							result.RemoveAt(j);
							blProcessed = true;
							j--;
						} else if ((flag == OverlapFlags.CrossMax) || (flag == OverlapFlags.ContactMax)) {
							result[i].SetMinMax(result[i].Min, result[j].Max);
							result.RemoveAt(j);
							blProcessed = true;
							j--;
						}
					}
				}

				if (!blProcessed) {
					break;
				}
			}

			return result;
		}
	}
}
