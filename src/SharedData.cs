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

namespace CSLibrary
{
	/// <summary>
	/// 共有データ管理
	/// </summary>
	static public class SharedData
	{
		#region フィールド・プロパティー
		/// <summary>
		/// 共有データ
		/// </summary>
		static private Dictionary<object, object> typeObjects = new Dictionary<object, object>();
		#endregion

		#region Clear,Remove
		/// <summary>
		/// 共有データ全削除
		/// </summary>
		static public void Clear()
		{
			typeObjects.Clear();
		}

		/// <summary>
		/// 共有データ削除
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <remarks>
		/// 存在しない場合はエラーにならず何も処理されない。
		/// </remarks>
		static public void Remove<T>()
		{
			Type keyType = typeof(T);
			if (typeObjects.ContainsKey(keyType)) {
				typeObjects.Remove(keyType);
			}
		}

		/// <summary>
		/// 共有データ削除(キー名)
		/// </summary>
		/// <param name="pKey">オブジェクトの名前</param>
		/// <remarks>
		/// 存在しない場合はエラーにならず何も処理されない。
		/// </remarks>
		static public void Remove(string pKey)
		{
			if (typeObjects.ContainsKey(pKey)) {
				typeObjects.Remove(pKey);
			}
		}
		#endregion

		#region Contains
		/// <summary>
		/// オブジェクト保持チェック
		/// </summary>
		/// <typeparam name="T">存在を確認したいオブジェクトの型</typeparam>
		/// <returns>true=存在する</returns>
		static public bool Contains<T>()
		{
			return typeObjects.ContainsKey(typeof(T));
		}

		/// <summary>
		/// オブジェクト保持チェック(キー名)
		/// </summary>
		/// <typeparam name="T">存在を確認したいオブジェクトの名前</typeparam>
		/// <param name="pKey">オブジェクトの名前</param>
		/// <returns>true=存在する</returns>
		static public bool Contains<T>(string pKey)
		{
			return typeObjects.ContainsKey(pKey);
		}
		#endregion

		#region GetObject
		/// <summary>
		/// オブジェクト取得・作成
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <returns>オブジェクトのインスタンス</returns>
		/// <remarks>
		/// 型Tのオブジェクトを作成して管理下に置く。
		/// すでに作成されている場合は作成済みのオブジェクトを返す。
		/// オブジェクトを作成する場合は引数無しのコンストラクタが呼ばれる。
		/// </remarks>
		static public T GetObject<T>()
		{
			return GetObject<T>(typeof(T), (object[])null);
		}

		/// <summary>
		/// オブジェクト取得・作成(引数有り)
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="pArg">コンストラクタに渡す引数</param>
		/// <returns>オブジェクトのインスタンス</returns>
		static public T GetObject<T>(object[] pArg)
		{
			return GetObject<T>(typeof(T), pArg);
		}

		/// <summary>
		/// オブジェクト取得・作成(名前指定)
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="pKey">オブジェクトの名前</param>
		/// <returns>オブジェクトのインスタンス</returns>
		static public T GetObject<T>(string pKey)
		{
			return GetObject<T>(pKey, (object[])null);
		}

		/// <summary>
		/// オブジェクト取得・作成(名前指定、引数有り)
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="pKey">オブジェクトの名前</param>
		/// <param name="pArg">コンストラクタに渡す引数</param>
		/// <returns>オブジェクトのインスタンス</returns>
		static public T GetObject<T>(string pKey, object[] pArg)
		{
			return GetObject<T>((object)pKey, pArg);
		}

		/// <summary>
		/// オブジェクト取得・作成(キー、引数有り)
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="pKey">オブジェクトの名前</param>
		/// <param name="pArg">コンストラクタに渡す引数</param>
		/// <returns>オブジェクトのインスタンス</returns>
		static public T GetObject<T>(object pKey, object[] pArg)
		{
			// オブジェクトが存在するならそれを返す
			if (typeObjects.ContainsKey(pKey)) {
				return (T)typeObjects[pKey];
			}

			// オブジェクト作成
			T newObject;
			if (pKey is Type) {
				if (pArg == null) {
					newObject = Activator.CreateInstance<T>();
				} else {
					newObject = (T)Activator.CreateInstance((Type)pKey, pArg);
				}
			} else {
				if (pArg == null) {
					newObject = Activator.CreateInstance<T>();
				} else {
					newObject = (T)Activator.CreateInstance(typeof(T), pArg);
				}
			}

			typeObjects.Add(pKey, newObject);

			return newObject;
		}
		#endregion
	}
}
