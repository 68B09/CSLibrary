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
	public class AimaiKensaku
	{
		/*
		 *・半角と全角は同一視。
		 *・ただし「カナ」と「かな」は区別。
		 *
		 *・英字の大文字小文字は同一視される。
		 *　IgnoreCase = false とすれば区別される。
		 *
		 *・\(U+5C)や＼(U+FF3C)のバックスラッシュは円記号「￥(U+FFE5)¥(U+A5)」と見なされるが、
		 *　IgnoreBackSlashIsYenSign = false とすれば区別される。
		 *　
		 *・ダブルクオート系は全て同一視。　
		 *　"(U+0022) “(U+201C) ”(U+201D) „(U+201E) ‟(U+201F) ＂(U+FF02)
		 *　
		 *・シングルクオート系は全て同一視。　
		 *　'(U+0027) ‘(U+2018) ’(U+2019) ‚(U+201A) ‛(U+201B) ＇(U+FF07)
		 *　
		 *・全半角チルド、波ダッシュは同一視。
		 *　~(U+007E) ～(U+FF5E) 〜(U+301C)
		 */

		#region フィールド・プロパティー
		/// <summary>
		/// 大文字小文字を同一視する
		/// </summary>
		public bool IgnoreCase { get; set; }

		/// <summary>
		/// バックスラッシュと円記号を同一視する
		/// </summary>
		public bool IgnoreBackSlashIsYenSign { get; set; }

		/// <summary>
		/// 検索文字列取得･設定
		/// </summary>
		private string searchKey = "";
		public string SearchKey
		{
			get
			{
				return this.searchKey;
			}

			set
			{
				if (this.searchKey != value) {
					this.searchKey = value;
					this.searchKeyProcessed = this.PrepareForProcessing(this.searchKey);
				}
			}
		}

		/// <summary>
		/// 検索処理用に加工した検索文字列
		/// </summary>
		private string searchKeyProcessed;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AimaiKensaku()
		{
			this.IgnoreCase = true;
			this.IgnoreBackSlashIsYenSign = true;
			this.SearchKey = "";
			this.searchKeyProcessed = "";
		}
		#endregion

		/// <summary>
		/// 一致判定
		/// </summary>
		/// <param name="pTarget">検索対象文字列</param>
		/// <returns>true=一致、false=不一致</returns>
		public bool IsHit(string pTarget)
		{
			string target = this.PrepareForProcessing(pTarget);

			int hitpos;
			if (this.IgnoreCase) {
				// 大文字小文字区別を同一視
				hitpos = target.IndexOf(this.searchKeyProcessed, StringComparison.OrdinalIgnoreCase);
			} else {
				// 大文字小文字を区別
				hitpos = target.IndexOf(this.searchKeyProcessed, StringComparison.Ordinal);
			}

			return hitpos >= 0;
		}

		/// <summary>
		/// 処理用に文字を整える
		/// </summary>
		/// <param name="pText">文字列</param>
		/// <returns>整えた後の文字列</returns>
		private string PrepareForProcessing(string pText)
		{
			StringBuilder sb = new StringBuilder(pText);

			// U+309B(゛)やU+309C(゜)は正規化するとU+20が前に付加されるため正規化前に置き換える。
			sb.Replace('\x309B', '\x3099');
			sb.Replace('\x309C', '\x309A');

			// ㋿は令和に変換されないので正規化前に置き換える。
			sb.Replace("㋿", "令和");

			// バックスラッシュを円記号と見なすために置き換える。
			if (this.IgnoreBackSlashIsYenSign) {
				sb.Replace('\x005C', '\x00A5');
				sb.Replace('＼', '\x00A5');
			}

			// ダブルクォートを同一視させるため置き換える。
			sb.Replace('“', '"');
			sb.Replace('”', '"');
			sb.Replace('„', '"');
			sb.Replace('‟', '"');
			sb.Replace('＂', '"');

			// シングルクォートを同一視させるため置き換える。
			sb.Replace('‘', '\'');
			sb.Replace('’', '\'');
			sb.Replace('‚', '\'');
			sb.Replace('‛', '\'');

			// 波ダッシュはチルダと同一視させるため置き換える。
			sb.Replace('〜', '~');

			string strNormalized = sb.ToString().Normalize(NormalizationForm.FormKD);
			return strNormalized;
		}
	}
}
