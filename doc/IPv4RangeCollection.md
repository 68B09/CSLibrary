# IPv4RangeCollection
**概要**
==========
List&lt;IPv4Range&gt;を継承したクラスです。  

●**ソート**
------
**public void SortByMinimumAddress()**  

最小アドレスをキーに自身の要素を昇順ソートします。

●**マージ**
------
**public IPv4RangeCollection Merge()**  
**static public List&lt;IPv4Range&gt; Merge(IEnumerable&lt;IPv4Range&gt; pRangeDatas)**  

結合可能なアドレス同士を結合し、マージ後のデータリストを返します。  
元のオブジェクトの内容は変更されません。  
総当たりで処理を行うためデータ数によっては時間がかかります。  
マージ後のデータリストは必要であればソートを行ってください。  

 ※[IPv4Range.Min,IPv4Range.Max]※  
pRangeDatas[0,2],[8,9],[5,6],[3,4],[10,20] ※順不同※  
return [8,9],[0,6],[10,20] ※順不同※  
