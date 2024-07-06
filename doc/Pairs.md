# Pairs
**概要**
==========
指定型の複数データを保持するだけのジェネリッククラス群です。  
動作に関してはソースファイルも参照して下さい。

●**Pair&lt;S,T&gt;**
------
２値を扱うジェネリックラスです。  
FirstおよびSecondプロパティーで値の取得･設定が行えます。  
KeyValuePair&lt;&gt;と違い、プロパティーを介して値の変更が行えます。  
オブジェクト同士の内容の一致を調べたいときはEquals()を使用してください。  

    Pair<int,string> pair = new Pair<int,string>(1, "ABC");
    pair.First = 2;
    pair.Second = "DEF";

●**Triple&lt;S,T,U&gt;**
------
３値を扱うジェネリックラスです。  
３つ目の値はThirdプロパティーでアクセス出来ます。  
３つの値が扱える以外はPair&lt;&gt;と同じです。  
