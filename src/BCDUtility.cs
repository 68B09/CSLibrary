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

namespace CSLibrary.BCDUtility
{
	/// <summary>
	/// BCD Base
	/// </summary>
	public abstract class BCDBase
	{
		/// <summary>
		/// ゾーン値
		/// </summary>
		protected byte zone = 0x03;

		/// <summary>
		/// ゾーン値取得/設定
		/// </summary>
		public byte Zone
		{
			get
			{
				return this.zone;
			}
			set
			{
				if ((value <= 9) || (value > 0x0f)) {
					throw new ArgumentOutOfRangeException(nameof(Zone));
				}
				if (this.zone != value) {
					this.zone = value;
				}
			}
		}

		/// <summary>
		/// 正の符号値
		/// </summary>
		protected byte plusSign = 0x0c;

		/// <summary>
		/// 正の符号値取得/設定
		/// </summary>
		public byte PlusSign
		{
			get
			{
				return this.plusSign;
			}
			set
			{
				if ((value <= 9) || (value > 0x0f)) {
					throw new ArgumentOutOfRangeException(nameof(PlusSign));
				}
				if (this.plusSign != value) {
					this.plusSign = value;
				}
			}
		}

		/// <summary>
		/// 負の符号値
		/// </summary>
		protected byte minusSign = 0x0d;

		/// <summary>
		/// 負の符号値取得/設定
		/// </summary>
		public byte MinusSign
		{
			get
			{
				return this.minusSign;
			}
			set
			{
				if ((value <= 9) || (value > 0x0f)) {
					throw new ArgumentOutOfRangeException(nameof(MinusSign));
				}
				if (this.minusSign != value) {
					this.minusSign = value;
				}
			}
		}

		/// <summary>
		/// 符号解釈列挙
		/// </summary>
		public enum IncludeSigns
		{
			NotInclude = 0,     // 含まない
			Include = 1,        // 含む
			Ambiguous = 2,      // 曖昧(有っても無くてもよい)
		}

		/// <summary>
		/// 符号解釈
		/// </summary>
		protected IncludeSigns includeSign = IncludeSigns.Include;

		/// <summary>
		/// 符号解釈取得/設定
		/// </summary>
		public IncludeSigns IncludeSign
		{
			get
			{
				return this.includeSign;
			}
			set
			{
				if (this.includeSign != value) {
					this.includeSign = value;
				}
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BCDBase()
		{
		}

		/// <summary>
		/// 値種別列挙
		/// </summary>
		public enum ValueType
		{
			Unknown = 0,
			Value = 1,
			PlusSign = 2,
			MinusSign = 3,
		}

		/// <summary>
		/// 値種別取得
		/// </summary>
		/// <param name="pValue">値</param>
		/// <returns>ValueType</returns>
		public ValueType GetType(int pValue)
		{
			if ((pValue >= 0) && (pValue <= 9)) {
				return ValueType.Value;
			}

			if (pValue == this.plusSign) {
				return ValueType.PlusSign;
			}

			if (pValue == this.minusSign) {
				return ValueType.MinusSign;
			}

			return ValueType.Unknown;
		}

		/// <summary>
		/// BCDをlongへ変換
		/// </summary>
		/// <param name="pBCDs">BCDデータ</param>
		/// <param name="pOffset">pBCDs内BCD解析開始オフセット</param>
		/// <param name="pBytes">BCD解析対象バイト数</param>
		/// <returns>変換結果</returns>
		abstract public long ToLong(byte[] pBCDs, int pOffset, int pBytes);

		/// <summary>
		/// longをBCDへ変換
		/// </summary>
		/// <param name="pValue">変換元</param>
		/// <param name="pBCDs">格納先</param>
		/// <param name="pOffset">格納先開始オフセット</param>
		/// <param name="pBytes">格納先バイト数。pBCDのLengthではなく、pOffset位置から書き込み可能なバイト数。</param>
		abstract public void ToBCD(long pValue, byte[] pBCDs, int pOffset, int pBytes);
	}

	/// <summary>
	/// Packed BCD
	/// </summary>
	public class PackedBCD : BCDBase
	{
		/// <summary>
		/// BCDをlongへ変換
		/// </summary>
		/// <param name="pBCDs">BCDデータ</param>
		/// <param name="pOffset">pBCDs内BCD解析開始オフセット</param>
		/// <param name="pBytes">BCD解析対象バイト数</param>
		/// <returns>変換結果</returns>
		/// <remarks>
		/// BCDをバイナリ(long)へ変換する。
		/// </remarks>
		/// <exception cref="System.Exception">不正な値を検出した。</exception>
		public override long ToLong(byte[] pBCDs, int pOffset, int pBytes)
		{
			long val = 0;

			int idx = pOffset;
			for (int i = 0; i < pBytes; i++, idx++) {
				bool isLast = (i + 1) >= pBytes;

				int hi = pBCDs[idx] >> 4;
				if (hi > 9) {
					throw new Exception("不正な値が見つかった。");
				}

				int lo = pBCDs[idx] & 0x0f;
				if (isLast == false) {
					if (lo > 9) {
						throw new Exception("不正な値が見つかった。");
					}

					val = (val * 100) + (hi * 10) + lo;
					continue;
				}

				// last byte

				val = (val * 10) + hi;
				ValueType valueType = this.GetType(lo);

				switch (valueType) {
					case ValueType.PlusSign:
						if (this.includeSign == IncludeSigns.NotInclude) {
							throw new Exception("符号無し指定時に符号が見つかった。");
						}
						break;

					case ValueType.MinusSign:
						if (this.includeSign == IncludeSigns.NotInclude) {
							throw new Exception("符号無し指定時に符号が見つかった。");
						}

						val = 0 - val;
						break;

					case ValueType.Value:
						if (this.includeSign == IncludeSigns.Include) {
							throw new Exception("符号が含まれていない。");
						}

						val = (val * 10) + lo;
						break;

					default:
						throw new Exception("不正な値が見つかった。");
				}
			}

			return val;
		}

		/// <summary>
		/// longをBCDへ変換
		/// </summary>
		/// <param name="pValue">変換元</param>
		/// <param name="pBCDs">格納先</param>
		/// <param name="pOffset">格納先開始オフセット</param>
		/// <param name="pBytes">格納先バイト数。pBCDのLengthではなく、pOffset位置から書き込み可能なバイト数。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">バッファサイズが足りない。符号無しを指定されているときにpValueが負数だった。</exception>
		/// <remarks>
		/// pValueをBCD化して格納先にセットする。
		/// 格納先には右詰で書き込まれる。
		/// </remarks>
		public override void ToBCD(long pValue, byte[] pBCDs, int pOffset, int pBytes)
		{
			if (pBytes <= 0) {
				throw new ArgumentOutOfRangeException(nameof(pBytes));
			}

			if ((pValue < 0) && (this.includeSign == IncludeSigns.NotInclude)) {
				throw new ArgumentOutOfRangeException(
					nameof(pValue),
					string.Format("{0}が{1}、かつ、{2}が負数。",
								  nameof(IncludeSign), nameof(IncludeSigns.NotInclude), nameof(pValue)));
			}

			bool blMinus = pValue < 0;
			long unsignedValue = Math.Abs(pValue);

			// 必須桁数チェック
			int clmNum = (unsignedValue == 0) ? 1 : (int)Math.Log10(unsignedValue) + 1;
			if ((this.includeSign == IncludeSigns.Include) || blMinus) {
				clmNum++;
			}

			if ((pBytes * 2) < clmNum) {
				throw new ArgumentOutOfRangeException(nameof(pBytes), "格納先のサイズが足りない。");
			}

			// buffer zero clear
			for (int i = 0; i < pBytes; i++) {
				pBCDs[pOffset + i] = 0x00;
			}

			int idx = pOffset + pBytes - 1;     // write index

			// 符号付の場合は1桁目の特殊処理を行う
			if ((this.includeSign == IncludeSigns.Include) || blMinus) {
				pBCDs[idx] = (byte)(((unsignedValue % 10) << 4) + (blMinus ? this.minusSign : this.plusSign));
				idx--;
				unsignedValue /= 10;
			}

			while (unsignedValue > 0) {
				long twoDigit = unsignedValue % 100;
				unsignedValue /= 100;

				pBCDs[idx] = (byte)(((twoDigit / 10) << 4) + (twoDigit % 10));
				idx--;
			}
		}
	}

	/// <summary>
	/// Unpacked BCD
	/// </summary>
	public class UnpackedBCD : BCDBase
	{
		/// <summary>
		/// ゾーン非解析フラグ
		/// </summary>
		private bool ignoreZone = false;

		/// <summary>
		/// ゾーン非解析フラグ取得/設定
		/// </summary>
		/// <remarks>
		/// ToLong()が解析する際のゾーン値判定を無効にする。
		/// </remarks>
		private bool IgnoreZone
		{
			get
			{
				return this.ignoreZone;
			}
			set
			{
				if (this.ignoreZone != value) {
					this.ignoreZone = value;
				}
			}
		}

		/// <summary>
		/// BCDをlongへ変換
		/// </summary>
		/// <param name="pBCDs">BCDデータ</param>
		/// <param name="pOffset">pBCDs内BCD解析開始オフセット</param>
		/// <param name="pBytes">BCD解析対象バイト数</param>
		/// <returns>変換結果</returns>
		/// <remarks>
		/// BCDをバイナリ(long)へ変換する。
		/// </remarks>
		/// <exception cref="System.Exception">不正な値を検出した。</exception>
		public override long ToLong(byte[] pBCDs, int pOffset, int pBytes)
		{
			long val = 0;

			int idx = pOffset;
			for (int i = 0; i < pBytes; i++, idx++) {
				bool isLast = (i + 1) >= pBytes;

				int lo = pBCDs[idx] & 0x0f;
				if (lo > 9) {
					throw new Exception("不正な値が見つかった。");
				}
				val = (val * 10) + lo;

				int zone = pBCDs[idx] >> 4;
				ValueType zoneType = this.GetType(zone);
				if (isLast) {
					switch (zoneType) {
						case ValueType.PlusSign:
							break;

						case ValueType.MinusSign:
							val = 0 - val;
							break;

						default:
							throw new Exception("ゾーンが不正。");
					}
				} else {
					if (this.ignoreZone == false) {
						if (zone != this.zone) {
							throw new Exception("ゾーンが不正。");
						}
					}
				}
			}

			return val;
		}

		/// <summary>
		/// longをBCDへ変換
		/// </summary>
		/// <param name="pValue">変換元</param>
		/// <param name="pBCDs">格納先</param>
		/// <param name="pOffset">格納先開始オフセット</param>
		/// <param name="pBytes">格納先バイト数。pBCDのLengthではなく、pOffset位置から書き込み可能なバイト数。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">バッファサイズが足りない。符号無しを指定されているときにpValueが負数だった。</exception>
		/// <remarks>
		/// pValueをBCD化して格納先にセットする。
		/// 格納先には右詰で書き込まれる。
		/// </remarks>
		public override void ToBCD(long pValue, byte[] pBCDs, int pOffset, int pBytes)
		{
			if (pBytes <= 0) {
				throw new ArgumentOutOfRangeException(nameof(pBytes));
			}

			if ((pValue < 0) && (this.includeSign == IncludeSigns.NotInclude)) {
				throw new ArgumentOutOfRangeException(
					nameof(pValue),
					string.Format("{0}が{1}、かつ、{2}が負数。",
								  nameof(IncludeSign), nameof(IncludeSigns.NotInclude), nameof(pValue)));
			}

			bool blMinus = pValue < 0;
			long unsignedValue = Math.Abs(pValue);

			// 必須桁数チェック
			int clmNum = (unsignedValue == 0) ? 1 : (int)Math.Log10(unsignedValue) + 1;

			if (pBytes < clmNum) {
				throw new ArgumentOutOfRangeException(nameof(pBytes), "格納先のサイズが足りない。");
			}

			// buffer zero clear
			for (int i = 0; i < pBytes; i++) {
				pBCDs[pOffset + i] = 0x00;
			}

			int idx = pOffset + pBytes - 1;     // write index

			// 1桁目の処理を行う
			pBCDs[idx] = (byte)(((blMinus ? this.minusSign : this.plusSign) << 4) + (unsignedValue % 10));
			idx--;
			unsignedValue /= 10;

			while (idx >= pOffset) {
				pBCDs[idx] = (byte)((this.zone << 4) + (unsignedValue % 10));
				idx--;
				unsignedValue /= 10;
			}
		}
	}
}
