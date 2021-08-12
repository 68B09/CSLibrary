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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;

namespace CSLibrary
{
	#region CSVAssistant
	/// <summary>
	/// CSVファイルアシスト
	/// </summary>
	public class CSVAssistant
	{
		#region フィールド・プロパティー
		/// <summary>
		/// 項目出力カウンター
		/// </summary>
		private int writeFieldCount = 0;

		/// <summary>
		/// 項目区切り文字
		/// </summary>
		private char fieldSeparator = ',';

		/// <summary>
		/// 項目区切り文字取得/設定
		/// </summary>
		public char FieldSeparator
		{
			get
			{
				return this.fieldSeparator;
			}
			set
			{
				this.fieldSeparator = value;
			}
		}

		/// <summary>
		/// 文字列囲み文字
		/// </summary>
		private char quote = '"';

		/// <summary>
		/// 文字列囲み文字取得/設定
		/// </summary>
		public char Quote
		{
			get
			{
				return this.quote;
			}
			set
			{
				this.quote = value;
			}
		}

		/// <summary>
		/// 空行スキップフラグ
		/// </summary>
		private bool skipNullLine = true;

		/// <summary>
		/// 空行スキップフラグ取得/設定
		/// </summary>
		public bool SkipNullLine
		{
			get
			{
				return this.skipNullLine;
			}
			set
			{
				this.skipNullLine = value;
			}
		}

		/// <summary>
		/// 常時テキスト囲いフラグ
		/// </summary>
		private bool anytimeQuote = true;

		/// <summary>
		/// 常時テキスト囲いフラグ取得/設定
		/// </summary>
		/// <remarks>
		/// trueの場合は文字列は常に囲み文字で囲む。
		/// falseの場合は文字列中に「囲み文字」もしくは「項目区切り文字」が存在する場合のみ囲み文字で囲む。
		/// falseの場合は文字列をチェックするため処理速度が低下する。
		/// </remarks>
		public bool AnytimeQuote
		{
			get
			{
				return this.anytimeQuote;
			}
			set
			{
				this.anytimeQuote = value;
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CSVAssistant()
		{
		}
		#endregion

		#region Read
		/// <summary>
		/// 全レコード読込
		/// </summary>
		/// <param name="pReader">StreamReader</param>
		/// <returns>CSVRecordCollection</returns>
		/// <remarks>
		/// 全レコードを読み込んでコレクションを返す。
		/// </remarks>
		public CSVRecordCollection ReadRecords(StreamReader pReader)
		{
			CSVRecordCollection list = new CSVRecordCollection();

			while (true) {
				CSVRecord rec = this.ReadRecord(pReader);
				if (rec == null) {
					break;
				}
				list.Add(rec);
			}

			return list;
		}

		/// <summary>
		/// １レコード読込
		/// </summary>
		/// <param name="pReader">StreamReader</param>
		/// <returns>CSVRecord。null=EOF</returns>
		/// <remarks>
		/// １レコードを読み込んで返す。
		/// SkipNullLine==trueの場合は空行を返さない。
		/// </remarks>

		public CSVRecord ReadRecord(StreamReader pReader)
		{
			CSVRecord rec = null;
			while (true) {
				rec = this.ReadRecordMain(pReader);
				if (rec == null) {
					break;
				}
				if (rec.Count > 0) {
					break;
				}
				if (this.skipNullLine) {
					continue;
				}
				break;
			}
			return rec;
		}

		/// <summary>
		/// １レコード読込(主処理)
		/// </summary>
		/// <param name="pReader">StreamReader</param>
		/// <returns>CSVRecord。null=EOF</returns>
		/// <remarks>
		/// １レコードを読み込んで返す。
		/// </remarks>
		private CSVRecord ReadRecordMain(StreamReader pReader)
		{
			CSVRecord rec = new CSVRecord();
			bool validSB = false;
			StringBuilder sb = new StringBuilder();
			bool inSide = false;
			int readCharCount = 0;
			bool eof = true;
			bool quoted = false;

			while (true) {
				int c = pReader.Read();
				if (c == -1) {
					if (readCharCount > 0) {
						if (validSB) {
							CSVField field = new CSVField();
							field.Text = sb.ToString();
							field.Quoted = quoted;
							rec.Add(field);
							sb.Clear();
							validSB = false;
							quoted = false;
						}
					}
					break;
				}

				if (c == '\r') {
					continue;
				}
				readCharCount++;
				eof = false;

				// "～"の中？
				if (inSide) {
					// "以外は溜める
					if (c != this.quote) {
						sb.Append((char)c);
						continue;
					}

					// ""の場合は"を一つ格納後、"分を読み飛ばす
					int cnext = pReader.Peek();
					if (cnext == this.quote) {
						sb.Append(this.quote);
						pReader.Read();
						continue;
					}

					// 閉じ"
					inSide = false;
					continue;
				}

				// "～"の外

				// 1行の終わり？
				if (c == '\n') {
					if (readCharCount > 1) {
						CSVField field = new CSVField();
						field.Text = sb.ToString();
						field.Quoted = quoted;
						rec.Add(field);
						sb.Clear();
						validSB = false;
						quoted = false;
					}
					break;
				}

				// フィールドセパレータ？
				if (c == this.fieldSeparator) {
					CSVField field = new CSVField();
					field.Text = sb.ToString();
					field.Quoted = quoted;
					rec.Add(field);
					sb.Clear();
					validSB = false;
					quoted = false;
					continue;
				}

				if (c == this.quote) {
					// フィールド最初の"？
					if (validSB == false) {
						inSide = true;
						validSB = true;
						quoted = true;
						continue;
					}
				}

				sb.Append((char)c);
				validSB = true;
			}

			if (eof) {
				return null;
			}

			return rec;
		}
		#endregion

		#region Write
		/// <summary>
		/// 出力リセット
		/// </summary>
		/// <remarks>
		/// 初回のWrite()を呼ぶ前に本メソッドを一度呼び出しておくこと。
		/// </remarks>
		public void WriteReset()
		{
			this.writeFieldCount = 0;
			if (this.quote == this.fieldSeparator) {
				throw new InvalidDataException("FieldSeparator==Quote. not allowed.");
			}
		}

		/// <summary>
		/// 項目区切り文字出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		private void WriteFieldSeparator(StreamWriter pStream)
		{
			if (this.writeFieldCount > 0) {
				pStream.Write(this.fieldSeparator);
			}
			this.writeFieldCount++;
		}

		/// <summary>
		/// 改行出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		public void WriteLine(StreamWriter pStream)
		{
			pStream.WriteLine();
			this.writeFieldCount = 0;
		}

		/// <summary>
		/// 書式出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pFormat">書式文字列</param>
		/// <param name="pArgs">引数</param>
		public void Write(StreamWriter pStream, string pFormat, params object[] pArgs)
		{
			this.Write(pStream, string.Format(pFormat, pArgs));
		}

		/// <summary>
		/// 文字列型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pText">string</param>
		public void Write(StreamWriter pStream, string pText)
		{
			this.WriteFieldSeparator(pStream);

			bool hasQuote = pText.IndexOf(this.quote) != -1;
			bool hasCRLF = (pText.IndexOf('\r') != -1) || (pText.IndexOf('\n') != -1);

			if (this.anytimeQuote == false) {
				if ((hasQuote == false) &&
					(hasCRLF == false) &&
					(pText.IndexOf(this.fieldSeparator) == -1)) {
					pStream.Write(pText);
					return;
				}
			}

			pStream.Write(this.quote);

			if (hasQuote) {
				pText = pText.Replace(this.quote.ToString(), string.Format("{0}{0}", this.quote));
			}
			pStream.Write(pText);

			pStream.Write(this.quote);
			return;
		}

		/// <summary>
		/// char
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">char</param>
		public void Write(StreamWriter pStream, char pValue)
		{
			this.Write(pStream, pValue.ToString());
		}

		/// <summary>
		/// short型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">short</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, short pValue)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString("G", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// ushort型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">ushort</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, ushort pValue)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString("G", CultureInfo.InvariantCulture));
		}
		/// <summary>
		/// int型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">int</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, int pValue)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString("G", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// uint型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">uint</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, uint pValue)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString("G", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// long型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">long</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, long pValue)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString("G", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// ulong型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">ulong</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, ulong pValue)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString("G", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// float型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">float</param>
		/// <param name="pFormat">書式文字列</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, float pValue, string pFormat)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString(pFormat, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// double型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">double</param>
		/// <param name="pFormat">書式文字列</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, double pValue, string pFormat)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString(pFormat, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// decimal型出力
		/// </summary>
		/// <param name="pStream">StreamWriter</param>
		/// <param name="pValue">decimal</param>
		/// <param name="pFormat">書式文字列</param>
		/// <remarks>
		/// 書式変換後に区切り文字や囲み文字を含む場合は書式出力か文字列型出力で出力してください。
		/// </remarks>
		public void Write(StreamWriter pStream, decimal pValue, string pFormat)
		{
			this.WriteFieldSeparator(pStream);
			pStream.Write(pValue.ToString(pFormat, CultureInfo.InvariantCulture));
		}
		#endregion
	}
	#endregion

	#region CSVField
	/// <summary>
	/// CSV項目クラス
	/// </summary>
	public class CSVField
	{
		#region フィールド・プロパティー
		/// <summary>
		/// 文字列
		/// </summary>
		private string text = null;

		/// <summary>
		/// 文字列取得/設定
		/// </summary>
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		/// <summary>
		/// 囲みフラグ
		/// </summary>
		private bool quoted = false;

		/// <summary>
		/// 囲みフラグ取得/設定
		/// </summary>
		/// <remarks>読み込み時、囲み文字で囲まれていた場合はtrue。</remarks>
		public bool Quoted
		{
			get
			{
				return this.quoted;
			}
			set
			{
				this.quoted = value;
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CSVField()
		{
		}
		#endregion
	}
	#endregion

	#region CSVRecord
	/// <summary>
	/// CSVレコードクラス
	/// </summary>
	public class CSVRecord : List<CSVField>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CSVRecord()
		{
		}
		#endregion

		/// <summary>
		/// テキスト取得(Safe)
		/// </summary>
		/// <param name="pIndex">項目インデックス(0～)</param>
		/// <returns>string</returns>
		/// <remarks>
		/// pIndexに該当するCSV項目のテキストを返す。
		/// pIndexが範囲外の場合は空文字を返す。
		/// </remarks>
		public string SafeGetText(int pIndex)
		{
			if (pIndex < this.Count) {
				return this[pIndex].Text;
			}
			return "";
		}
	}
	#endregion

	#region CSVRecordCollection
	/// <summary>
	/// CSVレコードコレクションクラス
	/// </summary>
	public class CSVRecordCollection : List<CSVRecord>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CSVRecordCollection()
		{
		}
		#endregion

		/// <summary>
		/// 最大項目数取得
		/// </summary>
		/// <returns>最大項目数</returns>
		public int GetMaxField()
		{
			if (this.Count > 0) {
				return this.Max((x) => (x.Count));
			}
			return 0;
		}
	}
	#endregion
}
