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
	}

	/// <summary>
	/// IPv4アドレスの範囲を扱うクラス
	/// </summary>
	public class IPv4Range
	{
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
		public IPv4Range(string pCIDR)
		{
			Regex regex = new Regex(@"^(?<ADDR>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)/(?<BITS>[0-9]+)$");
			Match match = regex.Match(pCIDR);
			if (match.Success) {
				this.min = IPv4Base.StringToUInt(match.Groups["ADDR"].Value);

				int bits = int.Parse(match.Groups["BITS"].Value);
				if ((bits < 0) || (bits > 32)) {
					throw new ArgumentOutOfRangeException();
				}
				this.max = this.min | MakeHostMask(bits);
				return;
			}

			regex = new Regex(@"^(?<ADDR>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)/(?<MASK>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)$");
			match = regex.Match(pCIDR);
			if (match.Success) {
				this.min = IPv4Base.StringToUInt(match.Groups["ADDR"].Value);

				string[] fields = match.Groups["MASK"].Value.Split('.');
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
									throw new ArgumentOutOfRangeException();
								}
							}

							mask |= Int64.Parse(fields[i]);
							break;

						case "0":
							break;

						default:
							throw new ArgumentOutOfRangeException();
					}
				}
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
		public IPv4Range(string pIPv4From, string pIPv4To)
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
		public IPv4Range(string pIPv4, int pCount)
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
		public void SetMinMax(uint pMin, uint pMax)
		{
			if (pMin > pMax) {
				throw new ArgumentOutOfRangeException();
			}
			this.min = pMin;
			this.max = pMax;
		}

		/// <summary>
		/// 現在の範囲を表すCIDR表記文字列を生成
		/// </summary>
		/// <returns>CIDR表記文字列リスト</returns>
		/// <remarks>
		/// 複数の文字列が返されることがあります。
		/// </remarks>
		public IList<string> ToCIDR()
		{
			List<string> result = new List<string>();
			uint start = this.min;
			uint end = this.max;

			while (end >= start) {
				byte maxSize = 32;
				while (maxSize > 0) {
					uint mask = ~MakeHostMask(maxSize - 1);
					uint maskedBase = start & mask;

					if (maskedBase != start) {
						break;
					}

					maxSize--;
				}

				double maxDiff = Math.Log(end - start + 1) / Math.Log(2);
				byte maxDiffSize = (byte)(32 - Math.Floor(maxDiff));
				if (maxSize < maxDiffSize) {
					maxSize = maxDiffSize;
				}

				result.Add($"{IPv4Base.UIntToString(start)}/{maxSize}");
				start += (uint)Math.Pow(2, (32 - maxSize));
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
		/// ホスト部のマスクビットを作成
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
}
