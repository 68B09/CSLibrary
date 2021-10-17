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
using System.Text;

namespace CSLibrary
{
	/// <summary>
	/// TextUtility
	/// </summary>
	/// <remarks>
	/// UnicodeUtilityに入れるほどでは無い文字処理を収めたクラスです。
	/// </remarks>
	public class TextUtility
	{
		#region フィールド・プロパティー
		/// <summary>
		/// ファイルパス省略タイプ
		/// </summary>
		public enum FilePathCompactTypes
		{
			File = 0,               // ファイル名を可能な限り残しつつ省略
			Top = 1,                // 先頭から省略
			Last = 2,               // 末尾から省略
		}

		/// <summary>
		/// ファイルパス省略記号取得/設定
		/// </summary>
		/// <remarks>
		/// 半角文字(UnicodeUtility.IsHankakuがtrueを返す文字)でなければなりません。
		/// </remarks>
		public char PathCompactOmitMark { get; set; }

		/// <summary>
		/// UnicodeUtility取得/設定
		/// </summary>
		public UnicodeUtility UnicodeUtility { get; set; }
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TextUtility()
		{
			this.PathCompactOmitMark = '.';
			this.UnicodeUtility = new UnicodeUtility();
		}
		#endregion

		/// <summary>
		/// ファイルパス短縮
		/// </summary>
		/// <param name="pFilePath">ファイルパス</param>
		/// <param name="pMaxHalfLength">パス長(半角文字単位。0以上)</param>
		/// <param name="pFilePathCompactType">短縮タイプ</param>
		/// <returns>省略後文字列</returns>
		/// <remarks>
		/// pMaxHalfLength以下になるようにpFilePathを短縮する。
		/// サロゲートぺアは考慮するが結合文字などは考慮されない。
		/// 短縮結果はpMaxHalfLengthより短くなることがある。
		/// (ex)"C:\...\foo.txt" ※FilePathCompactTypes.Fileの場合
		/// </remarks>
		public string MakeCompactPath(string pFilePath, int pMaxHalfLength, FilePathCompactTypes pFilePathCompactType)
		{
			// 指定文字数が0以下なら空文字を返して終わる
			if (pMaxHalfLength <= 0) {
				return "";
			}

			// 指定文字数に収まっているならパスを加工せずにそのまま返して終わる
			int halfLen = this.UnicodeUtility.GetHalfWidthLength(pFilePath);
			if (halfLen <= pMaxHalfLength) {
				return pFilePath;
			}

			// 以降は指定文字数オーバー時の処理

			// 最大長が3以下の場合は制限文字数分の省略記号を返して終わる
			if (pMaxHalfLength <= 3) {
				return new string(PathCompactOmitMark, pMaxHalfLength);
			}

			// Fileタイプの処理を実行
			if (pFilePathCompactType == FilePathCompactTypes.File) {
				return this.MakeCompactPathFileType(pFilePath, pMaxHalfLength);
			}

			// 省略用記号を除いた必要文字数を算出
			halfLen = pMaxHalfLength - 3;

			if (pFilePathCompactType == FilePathCompactTypes.Last) {
				// 末尾から削除
				return this.UnicodeUtility.Left(pFilePath, halfLen) + (new string(PathCompactOmitMark, 3));
			}

			// 先頭から削除
			return (new string(PathCompactOmitMark, 3)) +
				   this.UnicodeUtility.Right(pFilePath, halfLen);
		}

		/// <summary>
		/// ファイルパス短縮(ファイルタイプ)
		/// </summary>
		/// <param name="pFilePath">ファイルパス</param>
		/// <param name="pMaxHalfLength">パス長(半角文字単位。0以上)</param>
		/// <returns>省略後文字列</returns>
		/// <remarks>
		/// pMaxHalfLength以下になるようにpFilePathを短縮する。
		/// 半角・全角の判定はChar単位で行うためサロゲートぺアなどは考慮されない。
		/// pMaxHalfLengthより短くなることがある。
		/// (ex)"C:\...\foo.txt"
		/// </remarks>
		protected string MakeCompactPathFileType(string pFilePath, int pMaxHalfLength)
		{
			// 以降は指定文字数オーバー時の処理

			// ディレクトリとファイル名に分離。
			// ディレクトリパス長もファイル名長もゼロがあり得る。
			// ただし両方がゼロということは無い。
			// ファイル名の直前に'\'がある場合は、その'\'はファイル名に含める。※\foo.txt
			CharacterInfoCollection dirPathCharTbl = new CharacterInfoCollection();
			dirPathCharTbl.UnicodeUtility = this.UnicodeUtility;
			CharacterInfoCollection fileNameCharTbl = new CharacterInfoCollection();
			fileNameCharTbl.UnicodeUtility = this.UnicodeUtility;

			int separatorPos = pFilePath.LastIndexOf('\\');
			for (int i = 0; i < pFilePath.Length; i++) {
				if (i < separatorPos) {
					dirPathCharTbl.Add(pFilePath[i].ToString());
				} else {
					fileNameCharTbl.Add(pFilePath[i].ToString());
				}
			}

			// 以降は最大長(p_MaxHalfLength)が4以上
			do {
				// ディレクトリパスが無い場合はファイル名のみを処理する
				if (dirPathCharTbl.Length == 0) {
					this.PathCompact(fileNameCharTbl, pMaxHalfLength, false);
					break;
				}

				// 以降ディレクトリ付き

				// ファイル名が収まらない場合
				if ((fileNameCharTbl.Length + 3) > pMaxHalfLength) {
					// ディレクトリパスを"..."に
					dirPathCharTbl.Clear();
					dirPathCharTbl.Add(PathCompactOmitMark.ToString(), 3);
					// ディレクトリパスはそのまま生かしてファイル名を短縮
					this.PathCompact(fileNameCharTbl, pMaxHalfLength - dirPathCharTbl.Length, false);
					break;
				}

				// ファイル名が収まる場合はディレクトリを短縮
				int remain = pMaxHalfLength - fileNameCharTbl.Length;
				this.PathCompact(dirPathCharTbl, remain, true);
			} while (false);

			StringBuilder sb = new StringBuilder();
			dirPathCharTbl.AppendTo(sb);
			fileNameCharTbl.AppendTo(sb);
			return sb.ToString();
		}

		/// <summary>
		/// ファイルパス短縮処理
		/// </summary>
		/// <param name="pCharInfoCollection">CharacterInfoCollection</param>
		/// <param name="pMaxLength">省略後文字数(半角文字単位)</param>
		/// <param name="pCompactTop">true=最初の1文字目も省略対象にする</param>
		public void PathCompact(CharacterInfoCollection pCharInfoCollection, int pMaxLength, bool pCompactTop)
		{
			// pCompactTop = false
			// \foo.txt 6 \f.txt   remain addCount
			// \foo.txt 5 \f...    5-3=2  +2=3
			// \foo.txt 4 \...     4-3=1  +2=3
			// \foo.txt 3 \..      3-3=0  +2=2
			// \foo.txt 2 \.       2-3=-1 +2=1
			// \foo.txt 1 \        1-3=-2 +2=0

			// pCompactTop = true
			// C:\temp  5 C:\wk    remain addCount
			// C:\temp  4 C...     4-3=1  +3=3
			// C:\temp  3 ...      3-3=0  +3=3
			// C:\temp  2 ..       2-3=-1 +3=2
			// C:\temp  1 .        1-3=-2 +3=1

			// 最大長に収まるなら終わり
			if (pCharInfoCollection.Length <= pMaxLength) {
				return;
			}

			// 最大長がゼロなら全文字削除
			if (pMaxLength <= 0) {
				pCharInfoCollection.Clear();
				return;
			}

			// 省略記号3文字("...")を除いた残りの文字数を算出
			int remain = pMaxLength - 3;

			// 末尾から文字を削る
			int targetLength = Math.Max(remain, pCompactTop ? 0 : 1);
			while (pCharInfoCollection.Length > targetLength) {
				pCharInfoCollection.RemoveLast(1);
			}

			// '.'を付加
			int markAddOffset = pCompactTop ? 3 : 2;
			int addCount = Math.Min(remain + markAddOffset, 3);
			pCharInfoCollection.Add(PathCompactOmitMark.ToString(), addCount);
		}
	}
}
