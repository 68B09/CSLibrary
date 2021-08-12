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
using System.Drawing;
using System.Drawing.Drawing2D;
using CSLibrary;

namespace CSLibrary
{
	/// <summary>
	/// 拡張DrawString
	/// </summary>
	public class ExDrawString
	{
		#region デバッグ用
#if DEBUG
		private Pen penWhite = new Pen(Color.White);
		private Pen penRed = new Pen(Color.Red);
		private Pen penGreen = new Pen(Color.Green);
		private Brush brushBlack = new SolidBrush(Color.Black);
#endif
		#endregion

		#region 固定値
		[Flags]
		public enum FormatFlags : uint
		{
			Default = 0,
			NoWrap = 1,                 // 改行文字で改行する
			IgnoreFontResize = 2,       // フォントサイズを調整しない
			Vertical = 4,               // 縦書き
			HalfToFullWidth = 8,        // 半角文字を全角文字に変換
			HCenter = 16,               // 水平方向中央寄せ
			HRight = 32,                // 水平方向右寄せ。縦書きの時は左寄せ。
			HKintou = 64,               // 水平方向均等割り付け
			VCenter = 128,              // 垂直方向中央寄せ
			VBottom = 256,              // 垂直方向下寄せ
			VKintou = 512,              // 垂直方向均等割り付け
		}

		/// <summary>
		/// フォントサイズを小さくしていくときの縮小値
		/// </summary>
		private const float fontSizeDecrementStepPt = (float)((72 / 25.4) * 0.5);   // 0.5mm≒1.41732pt
		#endregion

		#region フィールド・プロパティー
		/// <summary>
		/// フォント設定・取得
		/// </summary>
		public Font Font { get; set; }

		/// <summary>
		/// 文字描画用ブラシ取得・設定
		/// </summary>
		public Brush Brush { get; set; }

		/// <summary>
		/// 書式フラグ取得・設定
		/// </summary>
		public FormatFlags FormatFlag { get; set; }
		#endregion

		#region コンストラクタ
		public ExDrawString()
		{
		}
		#endregion

		/// <summary>
		/// 文字列描画(座標個別指定)
		/// </summary>
		/// <param name="pText">文字列</param>
		/// <param name="pGraphics">Graphics</param>
		/// <param name="pX">X座標(横書き時=左上、縦書き時=右上)</param>
		/// <param name="pY">Y座標(横書き時=左上、縦書き時=右上)</param>
		/// <param name="pWidth">幅</param>
		/// <param name="pHeight">高さ</param>
		public void Draw(string pText, Graphics pGraphics, int pX, int pY, int pWidth, int pHeight)
		{
			Rectangle rect = new Rectangle(pX, pY, pWidth, pHeight);
			this.Draw(pText, pGraphics, rect);
		}

		/// <summary>
		/// 文字列描画(座標Rectangle指定)
		/// </summary>
		/// <param name="pText">文字列</param>
		/// <param name="pGraphics">Graphics</param>
		/// <param name="pRect">領域</param>
		/// <remarks>
		/// フォント、ブラシ、フラグは本メソッドを呼出す前に設定すること。
		/// FormatFlags.HRightは、FormatFlags.Verticalが指定されているときは"Left"の意味になる。
		/// (注意)合成用濁音・半濁音付き全角平仮名および片仮名は合成されて描画される。
		/// (注意)IVSはサポートされず、化けて表示される。
		/// </remarks>
		public void Draw(string pText, Graphics pGraphics, Rectangle pRect)
		{
			float x = pRect.Left;
			float y = pRect.Top;

			GraphicsState saveGraphics = pGraphics.Save();
			try {
				LineDataCollection list = this.CreateDrawData(pText, pGraphics, this.Font, pRect.Width, pRect.Height, this.FormatFlag);
				using (list.Font) {
					foreach (LineData line in list) {
						foreach (CharData chars in line.CharDataCollection) {
							float xx = x + chars.X;
							float yy = y + chars.Y;
							pGraphics.DrawString(chars.Text, list.Font, this.Brush, xx, yy, list.StringFormat);
						}
					}
				}
			} finally {
				pGraphics.Restore(saveGraphics);
			}
		}

		/// <summary>
		/// 描画データ作成
		/// </summary>
		/// <param name="pText">文字列</param>
		/// <param name="pGraphics">Graphics</param>
		/// <param name="pFont">フォント</param>
		/// <param name="pWidth">出力先幅</param>
		/// <param name="pHeight">出力先高さ</param>
		/// <param name="pFlags">書式フラグ</param>
		/// <param name="pMinimumFontPointSize">フォントサイズ最小値(≧0.5)</param>
		/// <returns>描画データ</returns>
		private LineDataCollection CreateDrawData(string pText, Graphics pGraphics, Font pFont, int pWidth, int pHeight, FormatFlags pFlags, float pMinimumFontPointSize = 0.5f)
		{
			if (pMinimumFontPointSize < 0.5f) {
				throw new ArgumentOutOfRangeException(nameof(pMinimumFontPointSize), pMinimumFontPointSize, "<0.5f");
			}

			LineDataCollection list;
			Font font = new Font(pFont, pFont.Style);

			// 段階的にフォントをサイズを小さくし、指定領域に収まるようになるまで繰り返す。
			while (true) {
				list = CreateCharData(pText, pGraphics, font, pWidth, pHeight, pFlags);

				float nextFontSize = font.SizeInPoints - fontSizeDecrementStepPt;
				bool blDecide = false;
				do {
					if ((pFlags & FormatFlags.IgnoreFontResize) != 0) {
						blDecide = true;
						break;
					}
					if ((list.Height <= pHeight) && (list.Width <= pWidth)) {
						blDecide = true;
						break;
					}
					if (font.SizeInPoints <= pMinimumFontPointSize) {
						blDecide = true;
						break;
					}
					if (nextFontSize <= 0.5f) {
						blDecide = true;
						break;
					}
				} while (false);

				if (blDecide) {
					list.Font = font;
					break;
				}

				Font newFont = new Font(font.FontFamily, nextFontSize, font.Style, GraphicsUnit.Point);
				font.Dispose();
				font = newFont;
			}

			list.Font = font;

			// 書式フラグに従い文字位置を調整
			if (list.Vertical == false) {
				// 横書き

				// 水平方向中央寄せ
				bool blHCenter = (pFlags & FormatFlags.HCenter) != 0;
				bool blHRight = (pFlags & FormatFlags.HRight) != 0;
				bool blHKintou = (pFlags & FormatFlags.HKintou) != 0;
				if (blHCenter) {
					foreach (LineData line in list) {
						float diff = (pWidth - line.Width) / 2;
						line.AddOffset(diff, 0);
					}
				} else if (blHRight) {
					foreach (LineData line in list) {
						float diff = pWidth - line.Width;
						line.AddOffset(diff, 0);
					}
				} else if (blHKintou) {
					foreach (LineData line in list) {
						float diff = pWidth - line.Width;
						if (line.Count >= 2) {
							// 2文字以上であれば両端合わせ
							diff /= (line.Count - 1);
							for (int i = 1; i < line.Count; i++) {
								line[i].X += diff * i;
							}
							line.AdjustMinimum(0, float.MinValue);      // 左端からはみ出さないように調整
						} else if (line.Count == 1) {
							// 1文字なら中央寄せ
							diff /= 2;
							line.AddOffset(diff, 0);
						}
					}
				}

				bool blVCenter = (pFlags & FormatFlags.VCenter) != 0;
				bool blVBottom = (pFlags & FormatFlags.VBottom) != 0;
				bool blVKintou = (pFlags & FormatFlags.VKintou) != 0;
				if (blVCenter) {
					float diff = (pHeight - list.Height) / 2;
					list.AddOffset(0, diff);
				} else if (blVBottom) {
					float diff = pHeight - list.Height;
					list.AddOffset(0, diff);
				} else if (blVKintou) {
					float diff = pHeight - list.Height;
					if (list.Count >= 2) {
						// 2行以上であれば上下端合わせ
						diff /= (list.Count - 1);
						for (int i = 1; i < list.Count; i++) {
							list[i].AddOffset(0, diff * i);
						}
						list.AdjustMinimum(float.MinValue, 0);          // 上端からはみ出さないように調整
					} else if (list.Count == 1) {
						// 1行なら中央寄せ
						list.AddOffset(0, diff / 2);
					}
				}
			} else {
				// 縦書き

				// 水平方向中央寄せ
				bool blHCenter = (pFlags & FormatFlags.HCenter) != 0;
				bool blHRight = (pFlags & FormatFlags.HRight) != 0;
				bool blHKintou = (pFlags & FormatFlags.HKintou) != 0;
				if (blHCenter) {
					float diff = (pWidth - list.Width) / 2;
					list.AddOffset(-diff, 0);
				} else if (blHRight) {
					float diff = pWidth - list.Width;
					list.AddOffset(-diff, 0);
				} else if (blHKintou) {
					float diff = pWidth - list.Width;
					if (list.Count >= 2) {
						// 2行以上であれば左右端合わせ
						diff /= (list.Count - 1);
						for (int i = 1; i < list.Count; i++) {
							list[i].AddOffset(-diff * i, 0);
						}
						list.AdjustMaximum(list[0].BaseX, float.MaxValue);
					} else if (list.Count == 1) {
						// 1行であれば中央寄せ
						list.AddOffset(-(diff / 2), 0);
					}
				}

				bool blVCenter = (pFlags & FormatFlags.VCenter) != 0;
				bool blVBottom = (pFlags & FormatFlags.VBottom) != 0;
				bool blVKintou = (pFlags & FormatFlags.VKintou) != 0;
				if (blVCenter) {
					foreach (LineData line in list) {
						float diff = (pHeight - line.Height) / 2;
						line.AddOffset(0, diff);
					}
				} else if (blVBottom) {
					foreach (LineData line in list) {
						float diff = pHeight - line.Height;
						line.AddOffset(0, diff);
					}
				} else if (blVKintou) {
					foreach (LineData line in list) {
						float diff = pHeight - line.Height;
						if (line.Count >= 2) {
							// 2文字以上であれば上下端合わせ
							diff /= (line.Count - 1);
							for (int i = 1; i < line.Count; i++) {
								line[i].AddOffset(0, diff * i);
							}
							//line.AdjustMinimum(float.MinValue, 0);
						} else if (line.Count == 1) {
							// 1文字なら中央寄せ
							line.AddOffset(0, diff / 2);
						}
					}
				}
			}

			return list;
		}

		/// <summary>
		/// 文字データ作成
		/// </summary>
		/// <param name="pText">文字列</param>
		/// <param name="pGraphics">Graphics</param>
		/// <param name="pFont">フォント</param>
		/// <param name="pWidth">出力先幅</param>
		/// <param name="pHeight">出力先高さ</param>
		/// <param name="pFlags">書式フラグ</param>
		/// <returns>描画データ</returns>
		private LineDataCollection CreateCharData(string pText, Graphics pGraphics, Font pFont, int pWidth, int pHeight, FormatFlags pFlags)
		{
			bool blNoWrap = (pFlags & FormatFlags.NoWrap) != 0;
			bool blHalfToFull = (pFlags & FormatFlags.HalfToFullWidth) != 0;
			bool blVertical = (pFlags & FormatFlags.Vertical) != 0;

			// 文字の調整
			string strwk = pText.Replace("\r\n", "\n").Replace('\r', '\n');     // 改行文字統一
			if (blHalfToFull) {                                                 // 半角→全角
				UnicodeUtility uu = new UnicodeUtility();
				strwk = uu.ConvertHalfToFullWidth(strwk);
			}
			strwk = UnicodeUtility.JoinDakuonHandakuon(strwk);                  // 全角合成濁音・半濁音結合

			// 文字単位に分解
			List<String> charList;
			charList = new List<string>(UnicodeUtility.CharacterEnumeratorByVS(strwk));

			// 文字単位にサイズなどを取得してリスト化
			LineDataCollection lineList = new LineDataCollection();
			lineList.Vertical = blVertical;
			// 文字が無ければ終わり
			if (charList.Count <= 0) {
				return lineList;
			}

			// MeasureString用フラグ作成
			StringFormat strFmt = new StringFormat(StringFormat.GenericTypographic);
			strFmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			if (blVertical) {
				strFmt.FormatFlags |= StringFormatFlags.DirectionVertical;
			}
			strFmt.FormatFlags |= StringFormatFlags.NoWrap;
			lineList.StringFormat = new StringFormat(strFmt);

			// 基本文字サイズ取得
			SizeF strBaseSize = pGraphics.MeasureString("Ｗ", pFont, Point.Empty, strFmt);

			// 文字解析
			float x = 0;
			float y = 0;
			if (blVertical) {
				x = -strBaseSize.Width;
			}
			LineData curLineData = new LineData(x, y);
			curLineData.Vertical = blVertical;
			lineList.Add(curLineData);
			for (int i = 0; i < charList.Count; i++) {
				string c = charList[i];                                                 // １文字取得
				SizeF strSize = pGraphics.MeasureString(c, pFont, Point.Empty, strFmt); // 文字サイズ取得

				// 改行時処理
				bool blCRLF = string.CompareOrdinal(c, "\n") == 0;
				if (lineList.Vertical == false) {
					// 横書き

					if (blCRLF) {
						// 新しい行のオブジェクトを作成
						x = 0;
						y += strSize.Height;
						curLineData = new LineData(x, y);
						curLineData.Vertical = blVertical;
						lineList.Add(curLineData);
						continue;
					}

					// 文字情報を入れるオブジェクトを作成
					CharData charData = new CharData();
					charData.Text = c;                      // 文字
					charData.Size = strSize;                // 文字のサイズを設定

					// 行最初の文字 or NoWrap or 幅に収まる？
					if ((curLineData.Count == 0) || blNoWrap || ((x + strSize.Width) <= pWidth)) {
						charData.X = x;
						charData.Y = y;
						curLineData.Add(charData);

						x += strSize.Width;
						continue;
					}

					// 次の行の先頭文字

					// 新しい行のオブジェクトを作成
					x = 0;
					y += strSize.Height;

					curLineData = new LineData(x, y);
					curLineData.Vertical = blVertical;
					lineList.Add(curLineData);

					charData.X = x;
					charData.Y = y;
					curLineData.Add(charData);

					x += strSize.Width;
				} else {
					// 縦書き

					if (blCRLF) {
						// 新しい行のオブジェクトを作成
						y = 0;
						x -= strSize.Width;
						curLineData = new LineData(x, y);
						curLineData.Vertical = blVertical;
						lineList.Add(curLineData);
						continue;
					}

					// 文字情報を入れるオブジェクトを作成
					CharData charData = new CharData();
					charData.Text = c;                      // 文字
					charData.Size = strSize;                // 文字のサイズを設定

					// 行最初の文字 or NoWrap or 幅に収まる？
					if ((curLineData.Count == 0) || blNoWrap || ((y + strSize.Height) <= pHeight)) {
						charData.X = x;
						charData.Y = y;
						curLineData.Add(charData);

						y += strSize.Height;
						continue;
					}

					// 次の行の先頭文字

					// 新しい行のオブジェクトを作成
					x -= strSize.Width;
					y = 0;

					curLineData = new LineData(x, y);
					curLineData.Vertical = blVertical;
					lineList.Add(curLineData);

					charData.X = x;
					charData.Y = y;
					curLineData.Add(charData);

					y += strSize.Height;
				}
			}

			// 全体の幅･高さを算出
			lineList.SetWH(strBaseSize);

			return lineList;
		}

		/// <summary>
		/// 行データコレクション
		/// </summary>
		internal class LineDataCollection : List<LineData>
		{
			public bool Vertical { get; set; }
			public float Width { get; set; }
			public float Height { get; set; }
			public Font Font { get; set; }
			public StringFormat StringFormat { get; set; }

			public void SetWH(SizeF pBaseSize)
			{
				this.Width = 0;
				this.Height = 0;

				if (this.Vertical == false) {
					// 横書き

					foreach (LineData line in this) {
						line.SetWH(pBaseSize);
						float w = line.Width;
						if (this.Width < w) {
							this.Width = w;
						}
						this.Height += line.Height;
					}
				} else {
					// 縦書き

					foreach (LineData line in this) {
						line.SetWH(pBaseSize);
						float h = line.Height;
						if (this.Height < h) {
							this.Height = h;
						}
						this.Width += line.Width;
					}
				}
			}

			public void AddOffset(float pOffsetX, float pOffsetY)
			{
				foreach (LineData line in this) {
					line.AddOffset(pOffsetX, pOffsetY);
				}
			}

			public void AdjustMaximum(float pMaxX, float pMaxY)
			{
				foreach (LineData line in this) {
					line.AdjustMaximum(pMaxX, pMaxY);
				}
			}

			public void AdjustMinimum(float pMinX, float pMinY)
			{
				foreach (LineData line in this) {
					line.AdjustMinimum(pMinX, pMinY);
				}
			}
		}

		internal class LineData
		{
			public CharDataCollection CharDataCollection { get; private set; }
			public int Count
			{
				get
				{
					return this.CharDataCollection.Count;
				}
			}
			public CharData this[int pIndex]
			{
				get
				{
					return this.CharDataCollection[pIndex];
				}
			}

			public bool Vertical { get; set; }
			public float Width { get; set; }
			public float Height { get; set; }
			public float BaseX { get; set; }
			public float BaseY { get; set; }

			public LineData(float pBaseX, float pBaseY)
			{
				this.CharDataCollection = new CharDataCollection();
				this.BaseX = pBaseX;
				this.BaseY = pBaseY;
			}

			public void Add(CharData pData)
			{
				this.CharDataCollection.Add(pData);
			}

			public void SetWH(SizeF pBaseSize)
			{
				if (this.Vertical == false) {
					this.Width = 0;
					this.Height = pBaseSize.Height;
					foreach (CharData c in CharDataCollection) {
						this.Width += c.Width;
						if (this.Height < c.Height) {
							this.Height = c.Height;
						}
					}
				} else {
					this.Width = pBaseSize.Width;
					this.Height = 0;
					foreach (CharData c in CharDataCollection) {
						this.Height += c.Height;
						if (this.Width < c.Width) {
							this.Width = c.Width;
						}
					}
				}
			}

			public void AddOffset(float pOffsetX, float pOffsetY)
			{
				foreach (CharData c in this.CharDataCollection) {
					c.AddOffset(pOffsetX, pOffsetY);
				}
			}

			public void AdjustMaximum(float pMaxX, float pMaxY)
			{
				foreach (CharData c in this.CharDataCollection) {
					c.AdjustMaximum(pMaxX, pMaxY);
				}
			}

			public void AdjustMinimum(float pMinX, float pMinY)
			{
				foreach (CharData c in this.CharDataCollection) {
					c.AdjustMinimum(pMinX, pMinY);
				}
			}
		}

		internal class CharDataCollection : List<CharData>
		{
		}

		internal class CharData
		{
			public string Text { get; set; }

			private float width = 0;
			public float Width
			{
				get
				{
					return this.width;
				}
				set
				{
					this.width = value;
				}
			}
			private float height = 0;
			public float Height
			{
				get
				{
					return this.height;
				}
				set
				{
					this.height = value;
				}
			}
			public SizeF Size
			{
				get
				{
					return new SizeF(this.width, this.height);
				}
				set
				{
					this.width = value.Width;
					this.height = value.Height;
				}
			}

			private float x = 0;
			public float X
			{
				get
				{
					return x;
				}
				set
				{
					this.x = value;
				}
			}

			private float y = 0;
			public float Y
			{
				get
				{
					return y;
				}
				set
				{
					this.y = value;
				}
			}

			public CharData()
			{
				this.Text = "";
			}

			public void AddOffset(float pOffsetX, float pOffsetY)
			{
				this.x += pOffsetX;
				this.y += pOffsetY;
			}

			public void AdjustMaximum(float pMaxX, float pMaxY)
			{
				if (this.x > pMaxX) {
					this.x = pMaxX;
				}
				if (this.y > pMaxY) {
					this.y = pMaxY;
				}
			}
			public void AdjustMinimum(float pMinX, float pMinY)
			{
				if (this.x < pMinX) {
					this.x = pMinX;
				}
				if (this.y < pMinY) {
					this.y = pMinY;
				}
			}
		}
	}
}
