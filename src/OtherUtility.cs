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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary
{
	public class OtherUtility
	{
		/// <summary>
		/// 個数分解(int型)
		/// </summary>
		/// <param name="pNumbers">個数(1以上)</param>
		/// <param name="pDivide">分割数(1以上)</param>
		/// <param name="pMinimumLength">最低個数(1以上)</param>
		/// <returns>個数テーブル</returns>
		/// <exception cref="ArgumentOutOfRangeException">引数範囲外</exception>
		/// <seealso cref="NumbersToRange"/>
		/// <remarks>
		/// pMinimumLength以上になるように個数(pNumbers)を分割し、分割個数テーブルを返します。
		/// (10,3,1)10を3分割で最低1 -> [4,3,3]。
		/// (10,3,4)10を3分割で最低4 -> [5,5] ※最低個数を保つために2分割になる。
		/// (5,6,1)5を6分割で最低1 -> [1,1,1,1,1] ※最低個数を保つために5分割になる。
		/// 
		/// 用途についてはNumbersToRange()も参照してください。
		/// </remarks>
		static public int[] DivideNumbers(int pNumbers, int pDivide, int pMinimumLength = 1)
		{
			if (pNumbers <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			if (pDivide <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			if (pMinimumLength <= 0) {
				throw new ArgumentOutOfRangeException();
			}

			int step = pNumbers / pDivide;
			int tblnum = pDivide;
			if (step < pMinimumLength) {
				int newDivide = pNumbers / pMinimumLength;
				if (newDivide <= 0) {
					newDivide = 1;
				}
				step = pNumbers / newDivide;
				tblnum = newDivide;
			}

			int[] result = new int[tblnum];
			int remain = pNumbers - (step * result.Length);
			for (int i = 0; i < result.Length; i++) {
				result[i] = step;
				if (remain > 0) {
					result[i]++;
					remain--;
				}
			}

			return result;
		}

		/// <summary>
		/// 個数分解(long型)
		/// </summary>
		/// <param name="pNumbers">個数(1以上)</param>
		/// <param name="pDivide">分割数(1以上)</param>
		/// <param name="pMinimumLength">最低個数(1以上)</param>
		/// <returns>個数テーブル</returns>
		/// <exception cref="ArgumentOutOfRangeException">引数範囲外</exception>
		/// <remarks>
		/// int型DivideNumbers()のlong型版です。
		/// </remarks>
		static public long[] DivideNumbers(long pNumbers, long pDivide, long pMinimumLength = 1)
		{
			if (pNumbers <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			if (pDivide <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			if (pMinimumLength <= 0) {
				throw new ArgumentOutOfRangeException();
			}

			long step = pNumbers / pDivide;
			long tblnum = pDivide;
			if (step < pMinimumLength) {
				long newDivide = pNumbers / pMinimumLength;
				if (newDivide <= 0) {
					newDivide = 1;
				}
				step = pNumbers / newDivide;
				tblnum = newDivide;
			}

			long[] result = new long[tblnum];
			long remain = pNumbers - (step * result.LongLength);
			for (long i = 0; i < result.LongLength; i++) {
				result[i] = step;
				if (remain > 0) {
					result[i]++;
					remain--;
				}
			}

			return result;
		}

		/// <summary>
		/// 各個数をインデックスに変換(int型)
		/// </summary>
		/// <param name="pNumbersTbl">個数テーブル</param>
		/// <param name="pStart">開始インデックス</param>
		/// <returns>インデックステーブル</returns>
		/// <seealso cref="DivideNumbers"/>
		/// <remarks>
		/// 各個数を各要素の開始インデックスに変換します。
		/// [5,6,7] -> ans[0,5,11,18]
		/// 
		/// (例)N個のデータに施す処理を5スレッドに分割する。
		/// 一つのスレッドには最低100個の処理をさせる。
		/// int N = dataTable.Length;
		/// int[] nums = OtherUtility.DivideNumbers(N, 5, 100);
		/// int[] indices = OtherUtility.NumbersToRange(nums);
		/// Parallel.For(0, indices.Length, threadno =>
		/// {
		/// 	for (int i = indices[threadno]; i < indices[threadno + 1]; i++) {
		/// 		Proc(dataTable[i]);
		/// 	}
		/// });
		/// </remarks>
		static public int[] NumbersToRange(int[] pNumbersTbl, int pStart = 0)
		{
			int[] result = new int[pNumbersTbl.Length + 1];
			int val = pStart;
			int idx;
			for (idx = 0; idx < pNumbersTbl.Length; idx++) {
				result[idx] = val;
				val += pNumbersTbl[idx];
			}
			result[idx] = val;

			return result;
		}

		/// <summary>
		/// 各個数をインデックスに変換(long型)
		/// </summary>
		/// <param name="pNumbersTbl">個数テーブル</param>
		/// <param name="pStart">開始インデックス</param>
		/// <returns>インデックステーブル</returns>
		/// <seealso cref="NumbersToRange"/>
		/// <remarks>
		/// int型NumbersToRange()のlong型版です。
		/// </remarks>
		static public long[] NumbersToRange(long[] pNumbersTbl, long pStart = 0)
		{
			long[] result = new long[pNumbersTbl.Length + 1];
			long val = pStart;
			int idx;
			for (idx = 0; idx < pNumbersTbl.Length; idx++) {
				result[idx] = val;
				val += pNumbersTbl[idx];
			}
			result[idx] = val;

			return result;
		}

		/// <summary>
		/// 要素をシャッフル
		/// </summary>
		/// <typeparam name="T">要素の型</typeparam>
		/// <param name="pList">リスト</param>
		/// <remarks>
		/// Fisher–Yatesアルゴリズムを使用してpListの要素の並びをランダムにシャッフルします。
		/// (例)
		/// string[] names = new string[] { "A", "B", "C", "D" };
		/// OtherUtility.Shuffle(names);
		/// </remarks>
		static public void Shuffle<T>(IList<T> pList)
		{
			Random rnd = new Random((int)DateTime.Now.Ticks);
			int currentIndex = pList.Count - 1;
			while (currentIndex >= 0) {
				int rndIndex = rnd.Next(currentIndex + 1);
				if (rndIndex != currentIndex) {
					T wk = pList[rndIndex];
					pList[rndIndex] = pList[currentIndex];
					pList[currentIndex] = wk;
				}
				currentIndex--;
			}
		}
	}
}
