/*
MIT License

Copyright (c) 2021 68B09(https://twitter.com/MB68C09)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CSLibrary
{
	/// <summary>
	/// UnicodeUtility
	/// </summary>
	/// <remarks>
	/// Windowsおよび.netは文字(Unicode)をUTF-16で表現します。
	/// そのためU+10000以上の文字はサロゲートペアとして表現するため、本クラス内のメソッドは基本的に
	/// int型、char型、string型
	/// の３パターンで呼び出せるよう用意されています。
	/// このうちint型はUTF-32(U+0000～U+10FFFF)を、char型はBMPもしくはUCS-2を、string型はサロゲートペアや
	/// IVS付きの文字を扱う想定です。
	/// </remarks>
	public class UnicodeUtility
	{
		#region 固定値
		public const char JISYENSIGNu5C = '\u005c';         // JIS 半角円記号(バックスラッシュ)
		public const char YENSIGNuA5 = '\u00a5';            // Unicode 半角円記号
		public const char TAB = '\t';                       // Tab
		public const char CR = '\r';                        // Carriage Return
		public const char LF = '\n';                        // Line Feed

		public const int UNICODE_LOW = 0;                   // 最小コードポイント
		public const int UNICODE_HIGH = 0x10FFFF;           // 最大コードポイント

		public const int HISURROGATE_LOW = 0xD800;          // High Surrogate最小
		public const int HISURROGATE_HIGH = 0xDBFF;         // High Surrogate最大
		public const int LOSURROGATE_LOW = 0xDC00;          // Low Surrogate最小
		public const int LOSURROGATE_HIGH = 0xDFFF;         // Low Surrogate最大

		public const int FVS_LOW = 0x180B;                  // モンゴル自由字形選択子最小
		public const int FVS_HIGH = 0x180D;                 // モンゴル自由字形選択子最大

		public const int SVS_LOW = 0xFE00;                  // 標準化された異体字シーケンス最小
		public const int SVS_HIGH = 0xFE0F;                 // 標準化された異体字シーケンス最大

		public const int IVS_LOW = 0xE0100;                 // 漢字異体字シーケンス最小
		public const int IVS_HIGH = 0xE01EF;                // 漢字異体字シーケンス最大

		public const char FW_JOIN_DAKUON = '\u3099';        // 結合用全角濁音
		public const char FW_JOIN_HANDAKUON = '\u309A';     // 結合用全角半濁音

		public const char FW_DAKUON = '\u309B';             // 全角濁音(゛)
		public const char FW_HANDAKUON = '\u309C';          // 全角半濁音(゜)

		public const char HW_DAKUON = '\uFF9E';             // 半角濁音(ﾞ)
		public const char HW_HANDAKUON = '\uFF9F';          // 半角半濁音(ﾟ)

		public const int GAIJI_BMP_LOW = 0xE800;            // BMP内外字最小
		public const int GAIJI_BMP_HIGH = 0xF8FF;           // BMP内外字最大

		public const int GAIJI_F0000_LOW = 0xF0000;         // Fxxxx内外字最小
		public const int GAIJI_F0000_HIGH = 0xFFFFD;        // Fxxxx内外字最大

		public const int GAIJI_100000_LOW = 0x100000;       // 10xxxx内外字最小
		public const int GAIJI_100000_HIGH = 0x10FFFD;      // 10xxxx内外字最大

		public const int HIMOJI_BMP_LOW = 0xFDD0;           // BMP内非文字最小
		public const int HIMOJI_BMP_HIGH = 0xFDEF;          // BMP内非文字最大

		public const int HIMOJI_PLANE_LOW = 0xFFFE;         // 全面内非文字最小
		public const int HIMOJI_PLANE_HIGH = 0xFFFF;        // 全面内非文字最大

		#region 文字変換用データ
		/// <summary>
		/// 文字変換用記号辞書データ
		/// </summary>
		static public readonly IReadOnlyCollection<CharacterReplaceItem> ReplaceMarkItems = new CharacterReplaceItem[] { new CharacterReplaceItem(CharacterReplaceItem.ItemTypes.Mark) };
		/// <summary>
		/// 文字変換用数字辞書データ
		/// </summary>
		static public readonly IReadOnlyCollection<CharacterReplaceItem> ReplaceNumberItems = new CharacterReplaceItem[] { new CharacterReplaceItem(CharacterReplaceItem.ItemTypes.Number) };
		/// <summary>
		/// 文字変換用英字辞書データ
		/// </summary>
		static public readonly IReadOnlyCollection<CharacterReplaceItem> ReplaceAlphabetItems = new CharacterReplaceItem[] { new CharacterReplaceItem(CharacterReplaceItem.ItemTypes.Alphabet) };
		/// <summary>
		/// 文字変換用片仮名辞書データ
		/// </summary>
		static public readonly IReadOnlyCollection<CharacterReplaceItem> ReplaceKatakanaItems = new CharacterReplaceItem[] {
			new CharacterReplaceItem("ｳﾞ", "ヴ"),
			#region
			new CharacterReplaceItem("ｶﾞ", "ガ"),
			new CharacterReplaceItem("ｷﾞ", "ギ"),
			new CharacterReplaceItem("ｸﾞ", "グ"),
			new CharacterReplaceItem("ｹﾞ", "ゲ"),
			new CharacterReplaceItem("ｺﾞ", "ゴ"),
			new CharacterReplaceItem("ｻﾞ", "ザ"),
			new CharacterReplaceItem("ｼﾞ", "ジ"),
			new CharacterReplaceItem("ｽﾞ", "ズ"),
			new CharacterReplaceItem("ｾﾞ", "ゼ"),
			new CharacterReplaceItem("ｿﾞ", "ゾ"),
			new CharacterReplaceItem("ﾀﾞ", "ダ"),
			new CharacterReplaceItem("ﾁﾞ", "ヂ"),
			new CharacterReplaceItem("ﾂﾞ", "ヅ"),
			new CharacterReplaceItem("ﾃﾞ", "デ"),
			new CharacterReplaceItem("ﾄﾞ", "ド"),
			new CharacterReplaceItem("ﾊﾞ", "バ"),
			new CharacterReplaceItem("ﾋﾞ", "ビ"),
			new CharacterReplaceItem("ﾌﾞ", "ブ"),
			new CharacterReplaceItem("ﾍﾞ", "ベ"),
			new CharacterReplaceItem("ﾎﾞ", "ボ"),
			new CharacterReplaceItem("ﾜﾞ", "ヷ"),
			new CharacterReplaceItem("ｦﾞ", "ヺ"),

			new CharacterReplaceItem("ﾊﾟ", "パ"),
			new CharacterReplaceItem("ﾋﾟ", "ピ"),
			new CharacterReplaceItem("ﾌﾟ", "プ"),
			new CharacterReplaceItem("ﾍﾟ", "ペ"),
			new CharacterReplaceItem("ﾎﾟ", "ポ"),

			new CharacterReplaceItem("ｦ", "ヲ"),
			new CharacterReplaceItem("ｧ", "ァ"),
			new CharacterReplaceItem("ｨ", "ィ"),
			new CharacterReplaceItem("ｩ", "ゥ"),
			new CharacterReplaceItem("ｪ", "ェ"),
			new CharacterReplaceItem("ｫ", "ォ"),
			new CharacterReplaceItem("ｬ", "ャ"),
			new CharacterReplaceItem("ｭ", "ュ"),
			new CharacterReplaceItem("ｮ", "ョ"),
			new CharacterReplaceItem("ｯ", "ッ"),

			new CharacterReplaceItem("ｱ", "ア"),
			new CharacterReplaceItem("ｲ", "イ"),
			new CharacterReplaceItem("ｳ", "ウ"),
			new CharacterReplaceItem("ｴ", "エ"),
			new CharacterReplaceItem("ｵ", "オ"),
			new CharacterReplaceItem("ｶ", "カ"),
			new CharacterReplaceItem("ｷ", "キ"),
			new CharacterReplaceItem("ｸ", "ク"),
			new CharacterReplaceItem("ｹ", "ケ"),
			new CharacterReplaceItem("ｺ", "コ"),
			new CharacterReplaceItem("ｻ", "サ"),
			new CharacterReplaceItem("ｼ", "シ"),
			new CharacterReplaceItem("ｽ", "ス"),
			new CharacterReplaceItem("ｾ", "セ"),
			new CharacterReplaceItem("ｿ", "ソ"),
			new CharacterReplaceItem("ﾀ", "タ"),
			new CharacterReplaceItem("ﾁ", "チ"),
			new CharacterReplaceItem("ﾂ", "ツ"),
			new CharacterReplaceItem("ﾃ", "テ"),
			new CharacterReplaceItem("ﾄ", "ト"),
			new CharacterReplaceItem("ﾅ", "ナ"),
			new CharacterReplaceItem("ﾆ", "ニ"),
			new CharacterReplaceItem("ﾇ", "ヌ"),
			new CharacterReplaceItem("ﾈ", "ネ"),
			new CharacterReplaceItem("ﾉ", "ノ"),
			new CharacterReplaceItem("ﾊ", "ハ"),
			new CharacterReplaceItem("ﾋ", "ヒ"),
			new CharacterReplaceItem("ﾌ", "フ"),
			new CharacterReplaceItem("ﾍ", "ヘ"),
			new CharacterReplaceItem("ﾎ", "ホ"),
			new CharacterReplaceItem("ﾏ", "マ"),
			new CharacterReplaceItem("ﾐ", "ミ"),
			new CharacterReplaceItem("ﾑ", "ム"),
			new CharacterReplaceItem("ﾒ", "メ"),
			new CharacterReplaceItem("ﾓ", "モ"),
			new CharacterReplaceItem("ﾔ", "ヤ"),
			new CharacterReplaceItem("ﾕ", "ユ"),
			new CharacterReplaceItem("ﾖ", "ヨ"),
			new CharacterReplaceItem("ﾗ", "ラ"),
			new CharacterReplaceItem("ﾘ", "リ"),
			new CharacterReplaceItem("ﾙ", "ル"),
			new CharacterReplaceItem("ﾚ", "レ"),
			new CharacterReplaceItem("ﾛ", "ロ"),
			new CharacterReplaceItem("ﾜ", "ワ"),
			new CharacterReplaceItem("ﾝ", "ン"),

			new CharacterReplaceItem("ｰ", "ー"),
			new CharacterReplaceItem("ﾞ", "゛"),
			new CharacterReplaceItem("ﾟ", "゜"),
			#endregion
		};
		/// <summary>
		/// 文字変換用片仮名辞書データ(全角→半角方向用)
		/// </summary>
		static public readonly IReadOnlyCollection<CharacterReplaceItem> ReplaceFromFWExKatakanaItems = new CharacterReplaceItem[] {
			new CharacterReplaceItem("ｺﾄ", "ヿ"),
			#region
			new CharacterReplaceItem("ｲﾞ", "ヸ"),
			new CharacterReplaceItem("ｴﾞ", "ヹ"),
			new CharacterReplaceItem("ｲ", "ヰ"),
			new CharacterReplaceItem("ｴ", "ヱ"),
			new CharacterReplaceItem("ｸ", "ㇰ"),
			new CharacterReplaceItem("ｼ", "ㇱ"),
			new CharacterReplaceItem("ｽ", "ㇲ"),
			new CharacterReplaceItem("ﾄ", "ㇳ"),
			new CharacterReplaceItem("ﾇ", "ㇴ"),
			new CharacterReplaceItem("ﾊ", "ㇵ"),
			new CharacterReplaceItem("ﾋ", "ㇶ"),
			new CharacterReplaceItem("ﾌ", "ㇷ"),
			new CharacterReplaceItem("ﾍ", "ㇸ"),
			new CharacterReplaceItem("ﾎ", "ㇹ"),
			new CharacterReplaceItem("ﾑ", "ㇺ"),
			new CharacterReplaceItem("ﾗ", "ㇻ"),
			new CharacterReplaceItem("ﾘ", "ㇼ"),
			new CharacterReplaceItem("ﾙ", "ㇽ"),
			new CharacterReplaceItem("ﾚ", "ㇾ"),
			new CharacterReplaceItem("ﾛ", "ㇿ"),
			new CharacterReplaceItem("ﾜ", "ヮ"),
			new CharacterReplaceItem("ｶ", "ヵ"),
			new CharacterReplaceItem("ｹ", "ヶ"),
			#endregion
		};
		#endregion

		/// <summary>
		/// VS種別
		/// </summary>
		public enum VSTypes
		{
			None = 0,       // VS系では無い
			FVS = 1,        // モンゴル自由字形選択子
			SVS = 2,        // 標準化された異体字シーケンス
			IVS = 3,        // 漢字異体字シーケンス
		}

		/// <summary>
		/// 外字種別
		/// </summary>
		public enum GaijiTypes
		{
			None = 0,       // 外字では無い
			uE800 = 1,      // U+E800～F8FF
			uF0000 = 2,     // U+F0000～FFFFD
			u100000 = 3,    // U+100000～10FFFD
		}

		/// <summary>
		/// 文字置換フラグ
		/// </summary>
		[Flags]
		public enum ReplaceFlags : uint
		{
			Default = 0,
			Tilda_Ignore,           // ItemTypes==Mark時　半角チルダU+7E(~)と全角チルダU+FF5E(～)を変換しない
			Space_Ignore,           // ItemTypes==Mark時　半角空白U+20と全角空白U+3000を変換しない
			YenSign_Ignore,         // ItemTypes==Mark時　円記号は変換しない
			F2HYenSignTo_uA5,       // ItemTypes==Mark時　全角(FullWidth)円記号は半角(HalfWidth)のU+A5へ ※YenSign_Ignoreを優先
			H2FJISYenSign_Ignore,   // ItemTypes==Mark時　JIS半角円記号(U+5C)は全角へ変換しない ※YenSign_Ignoreを優先
		}

		public enum BOMType
		{
			Unknown = 0,
			UTF8 = 1,
			UTF16BE = 2,
			UTF16LE = 3,
			UTF32BE = 4,
			UTF32LE = 5,
		}
		#endregion

		#region フィールド・プロパティー
		/// <summary>
		/// 濁音付き全角平仮名
		/// </summary>
		static public readonly string fullWidthHiraganaWithDakuon = "がぎぐげござじずぜぞだぢづでどばびぶべぼゔ";

		/// <summary>
		/// 半濁音付き全角平仮名
		/// </summary>
		static public readonly string fullWidthHiraganaWithHandakuon = "ぱぴぷぺぽ";

		/// <summary>
		/// 濁音付き全角片仮名
		/// </summary>
		static public readonly string fullWidthKatakanaDakuon = "ガギグゲゴザジズゼゾダヂヅデドバビブベボヴヷヸヹヺ";

		/// <summary>
		/// 半濁音付き全角片仮名
		/// </summary>
		static public readonly string fullWidthKatakanaHandakuon = "パピプペポ";

		/// <summary>
		/// U+A5の全角判定フラグ
		/// </summary>
		/// <remarks>
		/// false(デフォルト)の場合はIsHankaku()などでU+A5を半角と見なす。
		/// </remarks>
		public bool YenSignA5IsFullWidth { get; set; }

		/// <summary>
		/// 文字置換フラグ
		/// </summary>
		private ReplaceFlags replaceFlags = ReplaceFlags.Default;

		/// <summary>
		/// 文字置換フラグ取得･設定
		/// </summary>
		public ReplaceFlags ReplaceFlag
		{
			get { return this.replaceFlags; }
			set { this.replaceFlags = value; }
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UnicodeUtility()
		{
			this.YenSignA5IsFullWidth = false;
		}
		#endregion

		#region 簡易半角全角判定
		/// <summary>
		/// 簡易半角判定(コードポイント)
		/// </summary>
		/// <param name="pUTF32">判定するコードポイント</param>
		/// <returns>true=半角、false=全角</returns>
		/// <remarks>
		/// SJIS視点で半角全角判定を行う。
		/// ■判定ロジック
		/// ・U+00～7F(ASCII範囲)は半角
		/// ・U+80～A0(コントロールコードとNBSP)は半角
		/// ・U+FF61～FF9F(半角カナ類)は半角
		/// ・U+A5を半角と見なすのであれば半角
		/// ・それ以外は全角
		/// ■備考
		/// ・SJIS換算での判定になるため、THIN SPACEなども「全角」と見なされる。
		/// </remarks>
		public bool IsHankaku(int pUTF32)
		{
			if ((pUTF32 < UNICODE_LOW) || (pUTF32 > UNICODE_HIGH)) {
				throw new ArgumentOutOfRangeException(nameof(pUTF32), pUTF32, string.Format("<0x{0:X} or >0x{1:X}", UNICODE_LOW, UNICODE_HIGH));
			}

			// ASCII範囲は半角とみなす
			if (pUTF32 <= 0x7F) {
				return true;    // 半角
			}

			// U+80～A0は半角とみなす
			if ((pUTF32 >= 0x80) && (pUTF32 <= 0xA0)) {
				return true;    // 半角
			}

			// 半角・全角形内の半角カタカナは半角と見なす
			if ((pUTF32 >= 0xFF61) && (pUTF32 <= 0xFF9F)) {
				return true;    // 半角
			}

			// U+A5は全角とみなす？
			if (pUTF32 == YENSIGNuA5) {
				if (this.YenSignA5IsFullWidth == false) {
					return true;    // 半角
				}
			}

			return false;   // 全角
		}

		/// <summary>
		/// 半角判定(char)
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=半角、false=全角</returns>
		/// <remarks>
		/// 1Char単位で判定を行う。
		/// サロゲートペアやIVSなどは考慮しない。
		/// </remarks>
		public bool IsHankaku(char pChar)
		{
			return this.IsHankaku((int)pChar);
		}

		/// <summary>
		/// 半角判定(サロゲートペア考慮)
		/// </summary>
		/// <param name="pString">判定する１文字(サロゲートペアであれば2char)</param>
		/// <returns>true=半角、false=全角</returns>
		/// <remarks>
		/// IsHankakuのサロゲートペア考慮版。
		/// pStringの長さは1以上であること。
		/// サロゲートペアで表される文字は全角と判定される。
		/// VSなどが付加されていても、先頭の1もしくは2charで判定が行われる。
		/// </remarks>
		public bool IsHankaku(string pString)
		{
			// 文字列長が0の場合はエラー
			if (pString.Length == 0) {
				throw new ArgumentException("length is 0", nameof(pString));
			}

			// サロゲートペアでなければ1charで判定
			if (char.IsSurrogatePair(pString, 0) == false) {
				return this.IsHankaku((int)pString[0]);
			}

			// サロゲートペアの文字は全角とみなす
			return false;   // 全角
		}
		#endregion

		#region 外字判定
		/// <summary>
		/// 外字判定(コードポイント)
		/// </summary>
		/// <param name="pUTF32">判定するコードポイント</param>
		/// <returns>GaijiTypes</returns>
		static public GaijiTypes IsGaiji(int pUTF32)
		{
			if ((pUTF32 < UNICODE_LOW) || (pUTF32 > UNICODE_HIGH)) {
				throw new ArgumentOutOfRangeException(nameof(pUTF32), pUTF32, string.Format("<0x{0:X} or >0x{1:X}", UNICODE_LOW, UNICODE_HIGH));
			}

			if ((pUTF32 >= GAIJI_BMP_LOW) && (pUTF32 <= GAIJI_BMP_HIGH)) {
				return GaijiTypes.uE800;
			}

			if ((pUTF32 >= GAIJI_F0000_LOW) && (pUTF32 <= GAIJI_F0000_HIGH)) {
				return GaijiTypes.uF0000;
			}

			if ((pUTF32 >= GAIJI_100000_LOW) && (pUTF32 <= GAIJI_100000_HIGH)) {
				return GaijiTypes.u100000;
			}

			return GaijiTypes.None;
		}

		/// <summary>
		/// 外字判定(char)
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>GaijiTypes</returns>
		static public GaijiTypes IsGaiji(char pChar)
		{
			return IsGaiji((int)pChar);
		}

		/// <summary>
		/// 外字判定(サロゲートペア考慮)
		/// </summary>
		/// <param name="pString">判定する１文字(サロゲートペアであれば2char)</param>
		/// <returns>GaijiTypes</returns>
		static public GaijiTypes IsGaiji(string pString)
		{
			// 文字列長が0の場合はエラー
			if (pString.Length == 0) {
				throw new ArgumentException("length is 0", nameof(pString));
			}

			// charで判定
			if (pString.Length == 1) {
				return IsGaiji((int)pString[0]);
			}

			// サロゲートペア
			if (char.IsSurrogatePair(pString, 0)) {
				return IsGaiji(char.ConvertToUtf32(pString[0], pString[1]));
			}

			return IsGaiji((int)pString[0]);
		}
		#endregion

		#region 非文字判定
		/// <summary>
		/// 非文字チェック(コードポイント)
		/// </summary>
		/// <param name="pUTF32">判定するコードポイント</param>
		/// <returns>true=非文字</returns>
		static public bool IsHimoji(int pUTF32)
		{
			if ((pUTF32 < UNICODE_LOW) || (pUTF32 > UNICODE_HIGH)) {
				throw new ArgumentOutOfRangeException(nameof(pUTF32), pUTF32, string.Format("<0x{0:X} or >0x{1:X}", UNICODE_LOW, UNICODE_HIGH));
			}

			if ((pUTF32 >= HIMOJI_BMP_LOW) && (pUTF32 <= HIMOJI_BMP_HIGH)) {
				return true;
			}

			if (((pUTF32 & 0xffff) >= HIMOJI_PLANE_LOW) && ((pUTF32 & 0xffff) <= HIMOJI_PLANE_HIGH)) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// 非文字判定(char)
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=非文字</returns>
		static public bool IsHimoji(char pChar)
		{
			return IsHimoji((int)pChar);
		}

		/// <summary>
		/// 非文字チェック(サロゲートペア考慮)
		/// </summary>
		/// <param name="pString">判定する１文字(サロゲートペアであれば2char)</param>
		/// <returns>true=非文字</returns>
		static public bool IsHimoji(string pString)
		{
			// 文字列長が0の場合はエラー
			if (pString.Length == 0) {
				throw new ArgumentException("length is 0", nameof(pString));
			}

			if (pString.Length == 1) {
				return IsHimoji(pString[0]);
			}

			if (char.IsSurrogatePair(pString[0], pString[1])) {
				return IsHimoji(char.ConvertToUtf32(pString[0], pString[1]));
			}

			return IsHimoji(pString[0]);
		}
		#endregion

		#region 使用可能文字判定
		/// <summary>
		/// 使用可能文字チェック
		/// </summary>
		/// <param name="pString">判定文字列</param>
		/// <param name="pValidChars">使用可能文字</param>
		/// <returns>-1=使用可能文字のみで構成されている。-1以外=pCharsに存在しない文字の位置(0～)</returns>
		public int CheckValidChar(string pString, params char[] pValidChars)
		{
			for (int i = 0; i < pString.Length; i++) {
				int idx = Array.IndexOf<char>(pValidChars, pString[i]);
				if (idx < 0) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// 使用可能文字チェック(string)
		/// </summary>
		/// <param name="pString">判定文字列</param>
		/// <param name="pValidChars">使用可能文字</param>
		/// <returns>-1=使用可能文字のみで構成されている。-1以外=pCharsに存在しない文字の位置(0～)</returns>
		/// <remarks>
		/// pValidCharsはchar単位で評価されるため、pValidCharsにはサロゲートペアなどを含めてはならない。
		/// (ex)CheckValidChar("ｶ","ｶﾞ")←は'ｶ'と'ﾞ'の2文字が有効と解釈され、本メソッドは-1を返す。
		/// </remarks>
		public int CheckValidChar(string pString, string pValidChars)
		{
			return this.CheckValidChar(pString, pValidChars.ToCharArray());
		}

		/// <summary>
		/// 使用可能文字チェック(コード範囲)
		/// </summary>
		/// <param name="pString">判定文字列</param>
		/// <param name="pStart">有効なコードの範囲の最小値</param>
		/// <param name="pEnd">有効なコードの範囲の最大値</param>
		/// <returns>-1=使用可能文字のみで構成されている。-1以外=pCharsに存在しない文字の位置(0～)</returns>
		public int CheckValidChar(string pString, char pStart, char pEnd)
		{
			if (pStart > pEnd) {
				throw new ArgumentOutOfRangeException(nameof(pStart), pStart, string.Format("{0}>{1}", nameof(pStart), nameof(pEnd)));
			}

			for (int i = 0; i < pString.Length; i++) {
				if (((int)pString[i] < pStart) || ((int)pString[i] > pEnd)) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// 使用可能文字チェック(UTF-32範囲)
		/// </summary>
		/// <param name="pString">判定文字列</param>
		/// <param name="pStartUTF32">有効なコードの最小値</param>
		/// <param name="pEndUTF32">有効なコードの最大値</param>
		/// <returns>-1=使用可能文字のみで構成されている。-1以外=pCharsに存在しない文字の位置(0～)</returns>
		/// <remarks>
		/// サロゲートを考慮して有効な文字のみで構成されているか判定する。
		/// </remarks>
		public int CheckValidCode(string pString, int pStartUTF32, int pEndUTF32)
		{
			if (pStartUTF32 > pEndUTF32) {
				throw new ArgumentOutOfRangeException(nameof(pStartUTF32), pStartUTF32, string.Format("{0}>{1}", nameof(pStartUTF32), nameof(pEndUTF32)));
			}

			int idx = 0;
			foreach (int code in ConvertToUtf32(pString)) {
				if ((code < pStartUTF32) || (code > pEndUTF32)) {
					return idx;
				}
				if (idx < 0x10000) {
					idx++;
				} else {
					idx += -2;
				}
			}

			return -1;
		}
		#endregion

		#region 文字列長取得
		/// <summary>
		/// 半角文字単位での文字列長取得
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <returns>半角文字単位での文字数</returns>
		/// <remarks>
		/// 文字列の長さを半角文字単位で返す。
		/// (ex)"aあ"(U+61,3042)=3、"a𩸽"(U+61,D867,DE3D)=5、"a蝕󠄀"(U+61,8755,DB40,DD00)=7
		/// 半角全角をchar単位で判定するため、サロゲートペアは全角2文字(半角4文字)で計算される。
		/// </remarks>
		public int GetHalfWidthLength(string pString)
		{
			int totalLength = 0;
			foreach (char c in pString) {
				if (this.IsHankaku(c)) {
					totalLength++;
				} else {
					totalLength += 2;
				}
			}
			return totalLength;
		}

		/// <summary>
		/// 半角文字単位での文字列長取得(サロゲートペアを考慮)
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <returns>半角文字単位での文字数</returns>
		/// <remarks>
		/// 文字列の長さを半角文字単位で返す。
		/// (ex)"aあ"(U+61,3042)=3、"a𩸽"(U+61,D867,DE3D)=3、"a蝕󠄀"(U+61,8755,DB40,DD00)=5
		/// サロゲートペアは考慮される(全角1文字)が、VSなどは基本文字とVSが別個(VSは全角1文字)に判定される。
		/// </remarks>
		public int GetHalfWidthLengthBySurrogate(string pString)
		{
			int totalLength = 0;
			foreach (string str in CharacterEnumeratorBySurrogate(pString)) {
				if (this.IsHankaku(str)) {
					totalLength++;
				} else {
					totalLength += 2;
				}
			}
			return totalLength;
		}

		/// <summary>
		/// 半角文字単位での文字列長取得(サロゲートペアやVSを考慮)
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <returns>半角文字単位での文字数</returns>
		/// <remarks>
		/// 文字列の長さを半角文字単位で返す。
		/// (ex)"aあ"(U+61,3042)=3、"a𩸽"(U+61,D867,DE3D)=3、"a蝕󠄀"(U+61,8755,DB40,DD00)=3
		/// サロゲートペアとVSが考慮される。
		/// </remarks>
		public int GetHalfWidthLengthByVS(string pString)
		{
			int totalLength = 0;
			foreach (string str in CharacterEnumeratorByVS(pString)) {
				if (this.IsHankaku(str)) {
					totalLength++;
				} else {
					totalLength += 2;
				}
			}
			return totalLength;
		}
		#endregion

		#region タブ→スペース変換
		/// <summary>
		/// タブ文字を空白文字に変換
		/// </summary>
		/// <param name="pString">変換前文字列</param>
		/// <param name="pTabSize">タブサイズ(1以上)</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// サロゲートペアなどは考慮されず、1Char単位で半角・全角判定が行われ、桁位置が決定する。
		/// よってサロゲートペアは全角２文字と見なされる。
		/// </remarks>
		public string TabToSpace(string pString, int pTabSize = 8)
		{
			if (pTabSize < 1) {
				throw new ArgumentOutOfRangeException(nameof(pTabSize), pTabSize, "<1");
			}

			StringBuilder sb = new StringBuilder(pString.Length);
			int count = 0;

			foreach (char c in pString) {
				if ((c == CR) || (c == LF)) {
					count = 0;
					sb.Append(c);
					continue;
				}

				if (c == TAB) {
					int num = pTabSize - (count % pTabSize);
					sb.Append(' ', num);
					count = 0;
					continue;
				}

				bool isHankaku = this.IsHankaku(c);
				if (isHankaku) {
					count++;
				} else {
					count += 2;
				}
				sb.Append(c);
			}
			return sb.ToString();
		}
		#endregion

		#region １文字単位に分解
		/// <summary>
		/// １文字単位に分解(サロゲートペアを考慮)
		/// </summary>
		/// <param name="pString">解析文字列</param>
		/// <returns>文字(サロゲートペアあり)</returns>
		/// <remarks>
		/// pStringから１文字を切り出して順次返却する。
		/// サロゲートペアの場合は上位・下位サロゲートを組み立てて返却する。
		/// よって、返却される文字のLengthは1もしくは2となる。
		/// IVSなどは考慮しない。
		/// </remarks>
		static public IEnumerable<string> CharacterEnumeratorBySurrogate(string pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if (char.IsSurrogatePair(pString, i)) {
					yield return pString.Substring(i, 2);
					i++;
					continue;
				}

				yield return pString[i].ToString();
			}
		}

		/// <summary>
		/// １文字単位に分解(サロゲートペアやVSを考慮)
		/// </summary>
		/// <param name="pString">解析文字列</param>
		/// <returns>文字(サロゲートペアやVS付加あり)</returns>
		/// <remarks>
		/// pStringから１文字を切り出して順次返却する。
		/// サロゲートペアの場合は上位・下位サロゲートを組み立てて返却する。
		/// よって、返却される文字のLengthは1以上となる。
		/// 基本文字の後にVSや合成文字(例えば濁音)などが付加される。
		/// ■1文字が2char以上になるパターン
		/// ・サロゲートペア
		/// ・VSやIVS (ex) 蝕󠄀((U+8755,DB40,DD00))
		/// ・合成文字 (ex) が(U+304B,3099)
		/// </remarks>
		static public IEnumerable<string> CharacterEnumeratorByVS(string pString)
		{
			TextElementEnumerator charEnum = StringInfo.GetTextElementEnumerator(pString);
			while (charEnum.MoveNext()) {
				yield return charEnum.GetTextElement();
			}
		}

		/// <summary>
		/// コードポイント(UTF-32)へ変換
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>コードポイント(UTF-32)</returns>
		static public IEnumerable<int> ConvertToUtf32(string pString)
		{
			char? before = null;

			foreach (char c in pString) {
				if (before == null) {
					before = c;
					continue;
				}

				if (char.IsSurrogatePair(before.Value, c) == false) {
					yield return before.Value;
					before = c;
					continue;
				}

				// サロゲートペアの場合
				yield return char.ConvertToUtf32(before.Value, c);
				before = null;
			}

			if (before != null) {
				yield return before.Value;
			}
		}
		#endregion

		#region 全角平仮名片仮名判定
		/// <summary>
		/// 全角平仮名判定
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=全角平仮名(濁音・半濁音含む)</returns>
		static public bool IsFullWidthHiragana(char pChar)
		{
			if ((pChar >= 'ぁ') && (pChar <= 'ゖ')) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// 全角平仮名判定(濁音・半濁音付きを除く)
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=全角平仮名(濁音・半濁音付きを除く)</returns>
		static public bool IsFullWidthHiraganaWithoutDakuonHandakuon(char pChar)
		{
			if ((pChar >= 'ぁ') && (pChar <= 'ゖ')) {
				// 濁音付きはNG
				if (fullWidthHiraganaWithDakuon.IndexOf(pChar) != -1) {
					return false;
				}
				// 半濁音付きはNG
				if (fullWidthHiraganaWithHandakuon.IndexOf(pChar) != -1) {
					return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 全角片仮名判定
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=全角片仮名(濁音・半濁音含む)</returns>
		static public bool IsFullWidthKatakana(char pChar)
		{
			if ((pChar >= 'ァ') && (pChar <= 'ヺ')) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// 全角片仮名判定(濁音・半濁音付きを除く)
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=全角片仮名(濁音・半濁音付きを除く)</returns>
		static public bool IsFullWidthKatakanaWithoutDakuonHandakuon(char pChar)
		{
			if ((pChar >= 'ァ') && (pChar <= 'ヶ')) {
				// 濁音付きはNG
				if (fullWidthKatakanaDakuon.IndexOf(pChar) != -1) {
					return false;
				}
				// 半濁音付きはNG
				if (fullWidthKatakanaHandakuon.IndexOf(pChar) != -1) {
					return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 全角拡張(アイヌ語)片仮名判定
		/// </summary>
		/// <param name="pChar">判定する文字</param>
		/// <returns>true=全角拡張(アイヌ語)片仮名</returns>
		static public bool IsFullWidthExKatakana(char pChar)
		{
			if ((pChar >= 'ㇰ') && (pChar <= 'ㇿ')) {
				return true;
			}
			return false;
		}
		#endregion

		#region 異体字セレクタ判定
		/// <summary>
		/// 異体字セレクタ判定
		/// </summary>
		/// <param name="pString">判定する１文字(サロゲートペアの場合は2charそれ以外は1char)</param>
		/// <returns>VSTypes</returns>
		static public VSTypes IsVS(string pString)
		{
			// 文字列長が0の場合はエラー
			if (pString.Length == 0) {
				throw new ArgumentException("length is 0", nameof(pString));
			}

			if (pString.Length == 1) {
				// モンゴル自由字形選択子?
				if ((pString[0] >= FVS_LOW) && (pString[0] <= FVS_HIGH)) {
					return VSTypes.FVS;
				}

				// 標準化された異体字シーケンス?
				if ((pString[0] >= SVS_LOW) && (pString[0] <= SVS_HIGH)) {
					return VSTypes.SVS;
				}
			}

			// サロゲートペア？
			if (char.IsSurrogatePair(pString, 0)) {
				// 漢字異体字シーケンス？
				int codepoint = char.ConvertToUtf32(pString, 0);
				if ((codepoint >= IVS_LOW) && (codepoint <= IVS_HIGH)) {
					return VSTypes.IVS;
				}
			}

			return VSTypes.None;
		}
		#endregion

		#region サロゲートコード判定
		/// <summary>
		/// サロゲートコード判定
		/// </summary>
		/// <param name="pUTF32">判定コード</param>
		/// <returns>true=HiもしくはLoサロゲート</returns>
		static public bool IsSurrogate(int pUTF32)
		{
			if ((pUTF32 >= HISURROGATE_LOW) && (pUTF32 <= LOSURROGATE_HIGH)) {
				return true;
			}
			return false;
		}
		#endregion

		#region 全角平仮名・片仮名用合成濁音・半濁音を結合
		/// <summary>
		/// 文字列中の全ての合成全角平仮名･片仮名文字を１文字に変換
		/// </summary>
		/// <param name="pString">変換文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// (ex)"が"(U+304B,3099) などを "が"(U+304C) に変換する。
		/// </remarks>
		static public string JoinDakuonHandakuon(string pString)
		{
			StringBuilder sb = new StringBuilder(pString.Length);

			foreach (string str in CharacterEnumeratorByVS(pString)) {
				if (str.Length >= 2) {
					// 2文字目が合成用濁音・半濁音か？
					char c2 = str[1];
					if ((c2 == FW_JOIN_DAKUON) || (c2 == FW_JOIN_HANDAKUON)) {
						string normalized = str.Substring(0, 2).Normalize(NormalizationForm.FormC);
						sb.Append(normalized + str.Substring(2));
						continue;
					}
				}

				sb.Append(str);
			}

			return sb.ToString();
		}
		#endregion

		#region 半角を全角に変換
		/// <summary>
		/// 文字列中の全ての半角数字を全角数字に変換(string)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertHalfToFullWidthNumber(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertHalfToFullWidthNumber(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての半角数字を全角数字に変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// U+30～39をU+FF10～に変換する
		/// </remarks>
		public StringBuilder ConvertHalfToFullWidthNumber(StringBuilder pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if ((pString[i] >= '0') && (pString[i] <= '9')) {
					pString[i] = (char)('０' + (pString[i] - '0'));
				}
			}
			return pString;
		}

		/// <summary>
		/// 文字列中の全ての半角英字を全角英字に変換(string)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertHalfToFullWidthAlphabet(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertHalfToFullWidthAlphabet(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての半角英字を全角英字に変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// U+41～5AをU+FF21～へ、U+61～7AをU+FF41～へ変換する
		/// </remarks>
		public StringBuilder ConvertHalfToFullWidthAlphabet(StringBuilder pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if (((pString[i] >= 'A') && (pString[i] <= 'Z')) ||
					((pString[i] >= 'a') && (pString[i] <= 'z'))) {
					pString[i] = (char)('Ａ' + (pString[i] - 'A'));
				}
			}
			return pString;
		}

		/// <summary>
		/// 文字列中の全ての半角記号を全角記号に変換(string)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertHalfToFullWidthMark(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertHalfToFullWidthMark(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての半角記号を全角記号に変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// ・'~'は'～'に変換されるが、ReplaceFlag の Tilda_Ignore が立っている場合は変換されない
		/// ・' '(半角空白)は'　'(全角空白)に変換されるが、ReplaceFlag の Space_Ignore が立っている場合は変換されない
		/// ・'\'(U+5C)も'¥'(U+A5)も'￥'に変換されるが、ReplaceFlag の Space_Ignore が立っている場合はどちらも変換されない
		/// ・ReplaceFlag の H2FJISYenSign_Ignore が立っている場合は'\'(U+5C)は変換されない
		/// </remarks>
		public StringBuilder ConvertHalfToFullWidthMark(StringBuilder pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if (((pString[i] >= '!') && (pString[i] <= '/')) ||
					((pString[i] >= ':') && (pString[i] <= '@')) ||
					(pString[i] == '[') ||
					((pString[i] >= ']') && (pString[i] <= '`')) ||
					((pString[i] >= '{') && (pString[i] <= '}'))
					) {
					pString[i] = (char)('！' + (pString[i] - '!'));
				} else if (pString[i] == '~') {
					if ((this.replaceFlags & ReplaceFlags.Tilda_Ignore) != ReplaceFlags.Tilda_Ignore) {
						pString[i] = '～';
					}
				} else if (pString[i] == ' ') {
					if ((this.replaceFlags & ReplaceFlags.Space_Ignore) == ReplaceFlags.Default) {
						pString[i] = '　';
					}
				} else if (pString[i] == JISYENSIGNu5C) {
					if ((this.replaceFlags & ReplaceFlags.YenSign_Ignore) != ReplaceFlags.YenSign_Ignore) {
						if ((this.replaceFlags & ReplaceFlags.H2FJISYenSign_Ignore) != ReplaceFlags.H2FJISYenSign_Ignore) {
							pString[i] = '￥';
						}
					}
				} else if (pString[i] == YENSIGNuA5) {
					if ((this.replaceFlags & ReplaceFlags.YenSign_Ignore) != ReplaceFlags.YenSign_Ignore) {
						pString[i] = '￥';
					}
				} else if (pString[i] == '･') {
					pString[i] = '・';
				} else if (pString[i] == '｡') {
					pString[i] = '。';
				} else if (pString[i] == '､') {
					pString[i] = '、';
				} else if ((pString[i] >= '｢') && (pString[i] <= '｣')) {
					pString[i] = (char)('「' + (pString[i] - '｢'));
				}
			}
			return pString;
		}

		/// <summary>
		/// 文字列中の全ての半角を全角へ変換
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		public string ConvertHalfToFullWidth(string pString, params IReadOnlyCollection<CharacterReplaceItem>[] pReplaceDatas)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertHalfToFullWidth(sb, pReplaceDatas);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての半角を全角へ変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		public StringBuilder ConvertHalfToFullWidth(StringBuilder pString, params IReadOnlyCollection<CharacterReplaceItem>[] pReplaceDatas)
		{
			foreach (CharacterReplaceItem[] repItems in pReplaceDatas) {
				foreach (CharacterReplaceItem repItem in repItems) {
					if (repItem.ItemType == CharacterReplaceItem.ItemTypes.String) {
						if ((repItem.HalfWidth != null) && (repItem.HalfWidth.Length != 0)) {
							pString = pString.Replace(repItem.HalfWidth, repItem.FullWidth);
						}
					} else if (repItem.ItemType == CharacterReplaceItem.ItemTypes.Number) {
						this.ConvertHalfToFullWidthNumber(pString);
					} else if (repItem.ItemType == CharacterReplaceItem.ItemTypes.Alphabet) {
						this.ConvertHalfToFullWidthAlphabet(pString);
					} else if (repItem.ItemType == CharacterReplaceItem.ItemTypes.Mark) {
						this.ConvertHalfToFullWidthMark(pString);
					} else {
						throw new ArgumentException("unknown parameter value", nameof(repItem.ItemType));
					}
				}
			}

			return pString;
		}

		/// <summary>
		/// 文字列中の全ての半角を全角へ変換(デフォルト動作)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// 標準的だと思われる仕様で変換を行う。
		/// </remarks>
		public string ConvertHalfToFullWidth(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			return this.ConvertHalfToFullWidth(sb).ToString();
		}

		/// <summary>
		/// 文字列中の全ての半角を全角へ変換(デフォルト動作,StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// 標準的だと思われる仕様で変換を行う。
		/// ・数字(0<->０)、英字(A<->Ａ)、片仮名(ｱ<->ア)、記号(!<->！)
		/// ・シフトJISでも表現可能な文字のみを変換する
		/// </remarks>
		public StringBuilder ConvertHalfToFullWidth(StringBuilder pString)
		{
			return this.ConvertHalfToFullWidth(pString,
											   UnicodeUtility.ReplaceNumberItems,
											   UnicodeUtility.ReplaceAlphabetItems,
											   UnicodeUtility.ReplaceKatakanaItems,
											   UnicodeUtility.ReplaceMarkItems
											   );
		}
		#endregion

		#region 全角を半角に変換
		/// <summary>
		/// 文字列中の全ての全角数字を半角数字に変換(string)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertFullToHalfWidthNumber(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertFullToHalfWidthNumber(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての全角数字を半角数字に変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// U+FF10～FF19をU+30～に変換する
		/// </remarks>
		public StringBuilder ConvertFullToHalfWidthNumber(StringBuilder pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if ((pString[i] >= '０') && (pString[i] <= '９')) {
					pString[i] = (char)('0' + (pString[i] - '０'));
				}
			}
			return pString;
		}

		/// <summary>
		/// 文字列中の全ての全角英字を半角英字に変換(string)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertFullToHalfWidthAlphabet(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertFullToHalfWidthAlphabet(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての全角英字を半角英字に変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// U+FF21～FF3AをU+41～へ、U+FF41～FF5AをU+61～へ変換する
		/// </remarks>
		public StringBuilder ConvertFullToHalfWidthAlphabet(StringBuilder pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if (((pString[i] >= 'Ａ') && (pString[i] <= 'Ｚ')) ||
					((pString[i] >= 'ａ') && (pString[i] <= 'ｚ'))) {
					pString[i] = (char)('A' + (pString[i] - 'Ａ'));
				}
			}
			return pString;
		}

		/// <summary>
		/// 文字列中の全ての全角記号を半角記号に変換(string)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertFullToHalfWidthMark(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertFullToHalfWidthMark(sb);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての全角記号を半角記号に変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// ・'～'は'~'に変換されるが、ReplaceFlag の Tilda_Ignore が立っている場合は変換されない
		/// ・'　'(全角空白)は' '(半角空白)に変換されるが、ReplaceFlag の Space_Ignore が立っている場合は変換されない
		/// ・'￥'(全角円記号)は'\'(U+5C)に変換されるが、ReplaceFlag の YenSign_Ignore が立っている場合は変換されない
		/// ・ReplaceFlag の F2HYenSignTo_uA5 が立っている場合は'￥'(全角円記号)は'¥'(U+A5)に変換される
		/// </remarks>
		public StringBuilder ConvertFullToHalfWidthMark(StringBuilder pString)
		{
			for (int i = 0; i < pString.Length; i++) {
				if (((pString[i] >= '！') && (pString[i] <= '／')) ||
					((pString[i] >= '：') && (pString[i] <= '＠')) ||
					(pString[i] == '［') ||
					((pString[i] >= '］') && (pString[i] <= '｀')) ||
					((pString[i] >= '｛') && (pString[i] <= '｝'))
					) {
					pString[i] = (char)('!' + (pString[i] - '！'));
				} else if (pString[i] == '～') {
					if ((this.replaceFlags & ReplaceFlags.Tilda_Ignore) != ReplaceFlags.Tilda_Ignore) {
						pString[i] = '~';
					}
				} else if (pString[i] == '　') {
					if ((this.replaceFlags & ReplaceFlags.Space_Ignore) == ReplaceFlags.Default) {
						pString[i] = ' ';
					}
				} else if (pString[i] == '￥') {
					if ((this.replaceFlags & ReplaceFlags.YenSign_Ignore) != ReplaceFlags.YenSign_Ignore) {
						if ((this.replaceFlags & ReplaceFlags.F2HYenSignTo_uA5) == ReplaceFlags.F2HYenSignTo_uA5) {
							pString[i] = YENSIGNuA5;
						} else {
							pString[i] = JISYENSIGNu5C;
						}
					}
				} else if (pString[i] == '・') {
					pString[i] = '･';
				} else if (pString[i] == '。') {
					pString[i] = '｡';
				} else if (pString[i] == '、') {
					pString[i] = '､';
				} else if ((pString[i] >= '「') && (pString[i] <= '」')) {
					pString[i] = (char)('｢' + (pString[i] - '「'));
				}
			}
			return pString;
		}

		/// <summary>
		/// 文字列中の全ての全角を半角へ変換
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		public string ConvertFullToHalfWidth(string pString, params IReadOnlyCollection<CharacterReplaceItem>[] pReplaceDatas)
		{
			StringBuilder sb = new StringBuilder(pString);
			this.ConvertFullToHalfWidth(sb, pReplaceDatas);
			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての全角を半角へ変換(StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		public StringBuilder ConvertFullToHalfWidth(StringBuilder pString, params IReadOnlyCollection<CharacterReplaceItem>[] pReplaceDatas)
		{
			foreach (CharacterReplaceItem[] repItems in pReplaceDatas) {
				foreach (CharacterReplaceItem repItem in repItems) {
					if (repItem.ItemType == CharacterReplaceItem.ItemTypes.String) {
						if ((repItem.FullWidth != null) && (repItem.FullWidth.Length != 0)) {
							pString = pString.Replace(repItem.FullWidth, repItem.HalfWidth);
						}
					} else if (repItem.ItemType == CharacterReplaceItem.ItemTypes.Number) {
						this.ConvertFullToHalfWidthNumber(pString);
					} else if (repItem.ItemType == CharacterReplaceItem.ItemTypes.Alphabet) {
						this.ConvertFullToHalfWidthAlphabet(pString);
					} else if (repItem.ItemType == CharacterReplaceItem.ItemTypes.Mark) {
						this.ConvertFullToHalfWidthMark(pString);
					} else {
						throw new ArgumentException("unknown parameter value", nameof(repItem.ItemType));
					}
				}
			}

			return pString;
		}

		/// <summary>
		/// 文字列中の全ての全角を半角へ変換(デフォルト動作)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// もっとも標準だと思われる使用で変換を行う。
		/// </remarks>
		public string ConvertFullToHalfWidth(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);
			return this.ConvertFullToHalfWidth(sb).ToString();
		}

		/// <summary>
		/// 文字列中の全ての全角を半角へ変換(デフォルト動作,StringBuilder)
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <param name="pReplaceDatas">変換辞書データ群</param>
		/// <returns>変換後文字列</returns>
		/// <remarks>
		/// 標準的だと思われる仕様で変換を行う。
		/// ・数字(0<->０)、英字(A<->Ａ)、片仮名(ｱ<->ア)、記号(!<->！)
		/// ・シフトJISでも表現可能な文字のみを変換する
		/// </remarks>
		public StringBuilder ConvertFullToHalfWidth(StringBuilder pString)
		{
			return this.ConvertFullToHalfWidth(pString,
											   UnicodeUtility.ReplaceNumberItems,
											   UnicodeUtility.ReplaceAlphabetItems,
											   UnicodeUtility.ReplaceKatakanaItems,
											   UnicodeUtility.ReplaceMarkItems
											   );
		}
		#endregion

		#region 全角平仮名<->片仮名変換
		/// <summary>
		/// 文字列中の全ての全角平仮名を全角片仮名へ変換
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertHiraganaToKatakana(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);

			for (int i = 0; i < sb.Length; i++) {
				if ((sb[i] >= 'ぁ') && (sb[i] <= 'ゖ')) {
					sb[i] = (char)('ァ' + (sb[i] - 'ぁ'));
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// 文字列中の全ての全角片仮名を全角平仮名へ変換
		/// </summary>
		/// <param name="pString">変換元文字列</param>
		/// <returns>変換後文字列</returns>
		public string ConvertKatakanaToHiragana(string pString)
		{
			StringBuilder sb = new StringBuilder(pString);

			for (int i = 0; i < sb.Length; i++) {
				if ((sb[i] >= 'ァ') && (sb[i] <= 'ヶ')) {
					sb[i] = (char)('ぁ' + (sb[i] - 'ァ'));
				}
			}

			return sb.ToString();
		}
		#endregion

		#region 文字列切り詰め
		/// <summary>
		/// 半角文字数以下で文字列をカット
		/// </summary>
		/// <param name="pString">切り出し元文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(2以上)</param>
		/// <returns>切り出した文字列</returns>
		/// <remarks>
		/// pHalfWidthLength以下になるように文字列を切り出す。
		/// サロゲートペアや結合文字は分断されることがある。
		/// </remarks>
		public string Left(string pString, int pHalfWidthLength)
		{
			if (pHalfWidthLength < 2) {
				throw new ArgumentOutOfRangeException(nameof(pHalfWidthLength), pHalfWidthLength, "<2");
			}

			int totalLength = 0;
			StringBuilder sb = new StringBuilder(pString.Length);

			foreach (char c in pString) {
				if (totalLength >= pHalfWidthLength) {
					break;
				}

				if (this.IsHankaku(c)) {
					// HalfWidth
					sb.Append(c);
					totalLength++;
				} else {
					// FullWidth
					if ((totalLength + 2) > pHalfWidthLength) {
						break;
					}
					sb.Append(c);
					totalLength += 2;
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// 半角文字数以下で文字列をカット(サロゲートペアを考慮)
		/// </summary>
		/// <param name="pString">切り出し元文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(2以上)</param>
		/// <returns>切り出した文字列</returns>
		/// <remarks>
		/// pHalfWidthLength以下になるように文字列を切り出す。
		/// サロゲートペアは破壊されないように考慮されるが、結合文字などは分断されることがある。
		/// </remarks>
		public string LeftBySurrogate(string pString, int pHalfWidthLength)
		{
			if (pHalfWidthLength < 2) {
				throw new ArgumentOutOfRangeException(nameof(pHalfWidthLength), pHalfWidthLength, "<2");
			}

			int totalLength = 0;
			StringBuilder sb = new StringBuilder(pString.Length);

			foreach (string oneChar in CharacterEnumeratorBySurrogate(pString)) {
				if (totalLength >= pHalfWidthLength) {
					break;
				}

				if (this.IsHankaku(oneChar)) {
					// HalfWidth
					sb.Append(oneChar);
					totalLength++;
				} else {
					// FullWidth
					if ((totalLength + 2) > pHalfWidthLength) {
						break;
					}
					sb.Append(oneChar);
					totalLength += 2;
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// 半角文字数以下で文字列をカット(サロゲートペアやVSを考慮)
		/// </summary>
		/// <param name="pString">切り出し元文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(2以上)</param>
		/// <returns>切り出した文字列</returns>
		/// <remarks>
		/// pHalfWidthLength以下になるように文字列を切り出す。
		/// サロゲートペアやVS、結合文字などは極力されるため、いちばん文字数と見た目が一致しやすい。
		/// そのかわり実char数は見た目よりもかなり大きくなりやすいため、DBのフィールド長には注意が必要。
		/// </remarks>
		public string LeftByVS(string pString, int pHalfWidthLength)
		{
			if (pHalfWidthLength < 2) {
				throw new ArgumentOutOfRangeException(nameof(pHalfWidthLength), pHalfWidthLength, "<2");
			}

			int totalLength = 0;
			StringBuilder sb = new StringBuilder(pString.Length);

			foreach (string oneChar in CharacterEnumeratorByVS(pString)) {
				if (totalLength >= pHalfWidthLength) {
					break;
				}

				if (this.IsHankaku(oneChar)) {
					// HalfWidth
					sb.Append(oneChar);
					totalLength++;
				} else {
					// FullWidth
					if ((totalLength + 2) > pHalfWidthLength) {
						break;
					}
					sb.Append(oneChar);
					totalLength += 2;
				}
			}

			return sb.ToString();
		}
		#endregion

		#region 行分割
		/// <summary>
		/// 行へ分割
		/// </summary>
		/// <param name="pString">分割元文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(2以上)</param>
		/// <returns>行リスト</returns>
		/// <remarks>
		/// 1行がpHalfWidthLength以下になるように文字列を切り出して行を作る。
		/// サロゲートペアも結合文字も分断されることがある。
		/// </remarks>
		public List<string> ToLine(string pString, int pHalfWidthLength)
		{
			if (pHalfWidthLength < 2) {
				throw new ArgumentOutOfRangeException(nameof(pHalfWidthLength), pHalfWidthLength, "<2");
			}

			List<string> list = new List<string>();
			StringBuilder sb = new StringBuilder(pString.Length);
			int totalLength = 0;

			foreach (char c in pString) {
				bool blOver = false;

				if (this.IsHankaku(c)) {
					// HalfWidth
					sb.Append(c);
					totalLength++;
				} else {
					// FullWidth
					if ((totalLength + 2) > pHalfWidthLength) {
						blOver = true;
					} else {
						sb.Append(c);
						totalLength += 2;
					}
				}

				if (blOver || (totalLength >= pHalfWidthLength)) {
					list.Add(sb.ToString());
					sb.Clear();
					totalLength = 0;

					if (blOver) {
						sb.Append(c);
						totalLength += 2;
					}
				}
			}

			if (sb.Length > 0) {
				list.Add(sb.ToString());
			}

			return list;
		}

		/// <summary>
		/// 行へ分割(サロゲートペアを考慮)
		/// </summary>
		/// <param name="pString">分割元文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(2以上)</param>
		/// <returns>行リスト</returns>
		/// <remarks>
		/// 1行がpHalfWidthLength以下になるように文字列を切り出して行を作る。
		/// サロゲートペアは破壊されないように考慮されるが、結合文字などは分断されることがある。
		/// </remarks>
		public List<string> ToLineBySurrogate(string pString, int pHalfWidthLength)
		{
			if (pHalfWidthLength < 2) {
				throw new ArgumentOutOfRangeException(nameof(pHalfWidthLength), pHalfWidthLength, "<2");
			}

			List<string> list = new List<string>();
			StringBuilder sb = new StringBuilder(pString.Length);
			int totalLength = 0;

			foreach (string oneChar in CharacterEnumeratorBySurrogate(pString)) {
				bool blOver = false;

				if (this.IsHankaku(oneChar)) {
					// HalfWidth
					sb.Append(oneChar);
					totalLength++;
				} else {
					// FullWidth
					if ((totalLength + 2) > pHalfWidthLength) {
						blOver = true;
					} else {
						sb.Append(oneChar);
						totalLength += 2;
					}
				}

				if (blOver || (totalLength >= pHalfWidthLength)) {
					list.Add(sb.ToString());
					sb.Clear();
					totalLength = 0;

					if (blOver) {
						sb.Append(oneChar);
						totalLength += 2;
					}
				}
			}

			if (sb.Length > 0) {
				list.Add(sb.ToString());
			}

			return list;
		}

		/// <summary>
		/// 行へ分割(サロゲートペアやVSを考慮)
		/// </summary>
		/// <param name="pString">分割元文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(2以上)</param>
		/// <returns>行リスト</returns>
		/// <remarks>
		/// 1行がpHalfWidthLength以下になるように文字列を切り出して行を作る。
		/// サロゲートペアやVS、結合文字などは極力されるため、いちばん文字数と見た目が一致しやすい。
		/// そのかわり実char数は見た目よりもかなり大きくなりやすいため、DBのフィールド長には注意が必要。
		/// </remarks>
		public List<string> ToLineByVS(string pString, int pHalfWidthLength)
		{
			if (pHalfWidthLength < 2) {
				throw new ArgumentOutOfRangeException(nameof(pHalfWidthLength), pHalfWidthLength, "<2");
			}

			List<string> list = new List<string>();
			StringBuilder sb = new StringBuilder(pString.Length);
			int totalLength = 0;

			foreach (string oneChar in CharacterEnumeratorByVS(pString)) {
				bool blOver = false;

				if (this.IsHankaku(oneChar)) {
					// HalfWidth
					sb.Append(oneChar);
					totalLength++;
				} else {
					// FullWidth
					if ((totalLength + 2) > pHalfWidthLength) {
						blOver = true;
					} else {
						sb.Append(oneChar);
						totalLength += 2;
					}
				}

				if (blOver || (totalLength >= pHalfWidthLength)) {
					list.Add(sb.ToString());
					sb.Clear();
					totalLength = 0;

					if (blOver) {
						sb.Append(oneChar);
						totalLength += 2;
					}
				}
			}

			if (sb.Length > 0) {
				list.Add(sb.ToString());
			}

			return list;
		}
		#endregion

		#region 文字列長調整(文字付加)
		/// <summary>
		/// 先頭へ文字を付加
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(1以上)</param>
		/// <param name="pPaddingChar">付加文字(半角文字を指定すること)</param>
		/// <returns>付加後文字列</returns>
		/// <remarks>
		/// pHalfWidthLengthで指定された長さになるように文字列の先頭に文字を追加する。
		/// </remarks>
		public string PadLeft(string pString, int pHalfWidthLength, char pPaddingChar = ' ')
		{
			int pad = pHalfWidthLength - this.GetHalfWidthLength(pString);
			if (pad <= 0) {
				return pString;
			}
			return new string(pPaddingChar, pad) + pString;
		}

		/// <summary>
		/// 末尾へ文字を付加
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(1以上)</param>
		/// <param name="pPaddingChar">付加文字(半角文字を指定すること)</param>
		/// <returns>付加後文字列</returns>
		/// <remarks>
		/// pHalfWidthLengthで指定された長さになるように文字列の末尾に文字を追加する。
		/// </remarks>
		public string PadRight(string pString, int pHalfWidthLength, char pPaddingChar = ' ')
		{
			int pad = pHalfWidthLength - this.GetHalfWidthLength(pString);
			if (pad <= 0) {
				return pString;
			}
			return pString + new string(pPaddingChar, pad);
		}

		/// <summary>
		/// 先頭へ文字を付加(サロゲートペアを考慮)
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(1以上)</param>
		/// <param name="pPaddingChar">付加文字(半角文字を指定すること)</param>
		/// <returns>付加後文字列</returns>
		/// <remarks>
		/// pHalfWidthLengthで指定された長さになるように文字列の先頭に文字を追加する。
		/// 文字長の判定はサロゲートペアを考慮する。
		/// </remarks>
		public string PadLeftBySurrogate(string pString, int pHalfWidthLength, char pPaddingChar = ' ')
		{
			int pad = pHalfWidthLength - this.GetHalfWidthLengthBySurrogate(pString);
			if (pad <= 0) {
				return pString;
			}
			return new string(pPaddingChar, pad) + pString;
		}

		/// <summary>
		/// 末尾へ文字を付加(サロゲートペアを考慮)
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(1以上)</param>
		/// <param name="pPaddingChar">付加文字(半角文字を指定すること)</param>
		/// <returns>付加後文字列</returns>
		/// <remarks>
		/// pHalfWidthLengthで指定された長さになるように文字列の末尾に文字を追加する。
		/// 文字長の判定はサロゲートペアを考慮する。
		/// </remarks>
		public string PadRightBySurrogate(string pString, int pHalfWidthLength, char pPaddingChar = ' ')
		{
			int pad = pHalfWidthLength - this.GetHalfWidthLengthBySurrogate(pString);
			if (pad <= 0) {
				return pString;
			}
			return pString + new string(pPaddingChar, pad);
		}

		/// <summary>
		/// 先頭へ文字を付加(サロゲートペアやVSを考慮)
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(1以上)</param>
		/// <param name="pPaddingChar">付加文字(半角文字を指定すること)</param>
		/// <returns>付加後文字列</returns>
		/// <remarks>
		/// pHalfWidthLengthで指定された長さになるように文字列の先頭に文字を追加する。
		/// 文字長の判定はサロゲートペアとVSを考慮する。
		/// </remarks>
		public string PadLeftByVS(string pString, int pHalfWidthLength, char pPaddingChar = ' ')
		{
			int pad = pHalfWidthLength - this.GetHalfWidthLengthByVS(pString);
			if (pad <= 0) {
				return pString;
			}
			return new string(pPaddingChar, pad) + pString;
		}

		/// <summary>
		/// 末尾へ文字を付加(サロゲートペアやVSを考慮)
		/// </summary>
		/// <param name="pString">文字列</param>
		/// <param name="pHalfWidthLength">最大半角文字数(1以上)</param>
		/// <param name="pPaddingChar">付加文字(半角文字を指定すること)</param>
		/// <returns>付加後文字列</returns>
		/// <remarks>
		/// pHalfWidthLengthで指定された長さになるように文字列の末尾に文字を追加する。
		/// 文字長の判定はサロゲートペアとVSを考慮する。
		/// </remarks>
		public string PadRightByVS(string pString, int pHalfWidthLength, char pPaddingChar = ' ')
		{
			int pad = pHalfWidthLength - this.GetHalfWidthLengthByVS(pString);
			if (pad <= 0) {
				return pString;
			}
			return pString + new string(pPaddingChar, pad);
		}
		#endregion

		#region BOM判別
		/// <summary>
		/// BOM判別
		/// </summary>
		/// <param name="pDatas">Byteデータ列</param>
		/// <returns>BOMType</returns>
		/// <remarks>
		/// UTF-32LEとUTF-16LEが区別できるように、pDatasには4Byte以上のデータが入っていることが望ましい。
		/// ■BOM
		/// UTF-16BE FE,FF
		/// UTF-32LE FF,FE,00,00
		/// UTF-16LE FF,FE
		/// UTF-8    EF,BB,BF
		/// UTF-32BE 00,00,FE,FF
		/// </remarks>
		static public BOMType GetBOMType(byte[] pDatas)
		{
			// 2Byte未満は判別できない
			if (pDatas.Length < 2) {
				return BOMType.Unknown;
			}

			if ((pDatas[0] == 0xFE) && (pDatas[1] == 0xFF)) {
				return BOMType.UTF16BE;
			}

			if ((pDatas[0] == 0xFF) && (pDatas[1] == 0xFE)) {
				if (pDatas.Length >= 4) {
					if ((pDatas[2] == 0x00) && (pDatas[3] == 0x00)) {
						return BOMType.UTF32LE;
					}
				}
				return BOMType.UTF16LE;
			}

			if (pDatas.Length >= 3) {
				if ((pDatas[0] == 0xEF) && (pDatas[1] == 0xBB) && (pDatas[2] == 0xBF)) {
					return BOMType.UTF8;
				}

				if (pDatas.Length >= 4) {
					if ((pDatas[0] == 0x00) && (pDatas[1] == 0x00) &&
						(pDatas[2] == 0xFE) && (pDatas[3] == 0xFF)) {
						return BOMType.UTF32BE;
					}
				}
			}

			return BOMType.Unknown;
		}
		#endregion

#if DEBUG
		#region コードポイント作成
		/// <summary>
		/// 全コードポイント作成
		/// </summary>
		/// <param name="pStart">開始コード</param>
		/// <param name="pEnd">終了コード</param>
		/// <param name="pUseSurrogate">true=サロゲートペアで使用するコードも含める</param>
		/// <returns>指定範囲のコードポイント(UTF-32)</returns>
		/// <remarks>
		/// 指定範囲のコードポイントを生成して順次返却する。
		/// useSurrogateがtrueの場合はサロゲートペアで使用するコードも出力する。(本来は不正なコード)
		/// 非文字は出力されない。
		/// サロゲートペア用コードを含まなければ、合計111万個強を返す。
		/// </remarks>
		static public IEnumerable<int> GetUnicodePoints(int pStart = UNICODE_LOW, int pEnd = UNICODE_HIGH, bool pUseSurrogate = false)
		{
			if ((pStart < UNICODE_LOW) ||
				(pStart > UNICODE_HIGH)) {
				throw new ArgumentOutOfRangeException(nameof(pStart), pStart, string.Format("<0x{0:X} or >0x{1:X}", UNICODE_LOW, UNICODE_HIGH));
			}
			if ((pEnd < UNICODE_LOW) ||
				(pEnd > UNICODE_HIGH)) {
				throw new ArgumentOutOfRangeException(nameof(pEnd), pEnd, string.Format("<0x{0:X} or >0x{1:X}", UNICODE_LOW, UNICODE_HIGH));
			}
			if (pStart > pEnd) {
				throw new ArgumentOutOfRangeException(nameof(pStart), pStart, string.Format("{0}>{1}", nameof(pStart), nameof(pEnd)));
			}

			for (int c = pStart; c <= pEnd; c++) {
				if (pUseSurrogate == false) {
					if (UnicodeUtility.IsSurrogate(c)) {
						continue;
					}
				}

				if (IsHimoji(c) == false) {
					yield return c;
				}
			}
		}

		/// <summary>
		/// 全文字作成
		/// </summary>
		/// <returns>全文字</returns>
		/// <remarks>
		/// 全コードポイントに対応する(111万個強の)文字を持ったStringBuilderを返す。
		/// </remarks>
		static public StringBuilder CreateAllCharactors()
		{
			StringBuilder sb = new StringBuilder((65536 * 17) - (1024 * 2));
			foreach (int c in GetUnicodePoints()) {
				string strwk = char.ConvertFromUtf32(c);
				sb.Append(strwk);
			}
			return sb;
		}
		#endregion
#endif
	}

	#region CharacterReplaceItem
	/// <summary>
	/// 半角全角変換用辞書データクラス
	/// </summary>
	public class CharacterReplaceItem
	{
		/// <summary>
		/// データタイプ
		/// </summary>
		public enum ItemTypes
		{
			String = 0,
			Number,                 // '0-9'
			Alphabet,               // 'a-z','A-Z'
			Mark,                   // '!',etc..
		}

		/// <summary>
		/// データタイプ取得
		/// </summary>
		public ItemTypes ItemType { get; protected set; }

		/// <summary>
		/// 半角文字取得
		/// </summary>
		public string HalfWidth { get; protected set; }

		/// <summary>
		/// 全角文字取得
		/// </summary>
		public string FullWidth { get; protected set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private CharacterReplaceItem()
		{
			this.ItemType = ItemTypes.String;
		}

		/// <summary>
		/// コンストラクタ(データタイプのみ指定)
		/// </summary>
		/// <param name="pType">データタイプ</param>
		public CharacterReplaceItem(ItemTypes pType) : this()
		{
			this.ItemType = pType;
		}

		/// <summary>
		/// コンストラクタ(変換用文字列指定)
		/// </summary>
		/// <param name="pHalfWidth">半角文字</param>
		/// <param name="pFullWidth">全角文字</param>
		public CharacterReplaceItem(string pHalfWidth, string pFullWidth) : this()
		{
			this.HalfWidth = pHalfWidth;
			this.FullWidth = pFullWidth;
		}
	}
	#endregion
}