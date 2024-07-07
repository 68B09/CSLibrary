# OtherUtility
**概要**
==========
その他に分類されるクラスやメソッド群です。  

●**個数分解化**
------
**static public int[] DivideNumbers(int pNumbers, int pDivide, int pMinimumLength = 1)**  
**static public long[] DivideNumbers(long pNumbers, long pDivide, long pMinimumLength = 1)**  

pMinimumLength以上になるように個数(pNumbers)を分割し、分割個数テーブルを返します。
DivideNumbers(10,3,1)は10を3分割。一つの要素は1個以上になるように。 -> [4,3,3]。  
DivideNumbers(10,3,4)は10を3分割。一つの要素は4個以上になるように。 -> [5,5] ※最低個数を保つために2分割になる。  
DivideNumbers(5,6,1)は5を6分割。一つの要素は1個以上になるように。 -> [1,1,1,1,1] ※最低個数を保つために5分割になる。  

●**各個数をインデックスに変換**
------
**static public int[] NumbersToRange(int[] pNumbersTbl, int pStart = 0)**  
**static public long[] NumbersToRange(long[] pNumbersTbl, long pStart = 0)**  

各個数を各要素の開始インデックスに変換します。  
[5,6,7] -> ans[0,5,11,18]  

    (例)N個のデータに施す処理を5スレッドに分割する。
    一つのスレッドには最低100個の処理をさせる。
    int N = dataTable.Length;
    int[] nums = OtherUtility.DivideNumbers(N, 5, 100);
    int[] indices = OtherUtility.NumbersToRange(nums);
    Parallel.For(0, indices.Length, threadno =>
    {
        for (int i = indices[threadno]; i < indices[threadno + 1]; i++) {
            Proc(dataTable[i]);
        }
    });

●**要素をシャッフル**
------
**static public void Shuffle&lt;T&gt;(IList&lt;T&gt; pList)**  

Fisher–Yatesアルゴリズムを使用してpListの要素の並びをランダムにシャッフルします。  

    (例)
    string[] names = new string[] { "A", "B", "C", "D" };
    OtherUtility.Shuffle(names);
