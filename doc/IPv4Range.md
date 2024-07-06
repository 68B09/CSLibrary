# IPv4Range
**概要**
==========
IPv4アドレスを範囲で扱う基本クラスです。  

●**コンストラクタ**
------
**public IPv4Range()**  
**public IPv4Range(IPv4Range pSrc)**  
**public IPv4Range(uint pMin, uint pMax)**  
**public IPv4Range(string pCIDR)**  
**public IPv4Range(string pIPv4From, string pIPv4To)**  
**public IPv4Range(string pIPv4, int pCount)**  

各種アドレス表記に対応したコンストラクタです。  

●**最小・最大値取得**
------
**public uint Min**  
**public uint Max**  

アドレスの最小・最大値を取得するプロパティーです。  
値の設定にはSetMinMax()を使用してください。  

●**アドレス数取得**
------
**public long Count**  

MinからMaxまでの個数を返します。  
Minが0、Maxが1であれば2を返します。  

●**最小・最大値設定**
------
**public void SetMinMax(uint pMin, uint pMax, bool blPower = false)**  

アドレスの最小・最大値を設定するメソッドです。  
blPowerがfalseの場合、pMin<=pMaxが守られていないと例外がスローされます。  

●**現在の範囲を表すCIDR表記文字列を生成**
------
**public IList&lt;string&gt; ToCIDR(IList&lt;string&gt; pList = null)**  

Min,Maxの範囲に対応するCIDR表記の文字列を格納したリストオブジェクトを返します。  
範囲によっては2つ以上返す事があります。  
pListがnullの場合は内部で生成したリストオブジェクトを返します。  
null以外の場合はpListに格納され、戻り値にはpListが返されます。  

●**アドレス内包チェック**
------
**public bool InRange(uint pIPv4)**  
**public bool InRange(IPv4Range pRange)**  

指定されたアドレスが自身の範囲内であるかをチェックしその判定結果を返します。  

●**重なりフラグ**
------
**public enum OverlapFlags : int**  

CheckOverlapメソッドが返すフラグです。  

●**重なりチェック**
------
**public OverlapFlags CheckOverlap(IPv4Range pTarget)**  

自身とpTargetが持つアドレスの重なり具合を返します。  

●**マージ**
------
**static public List&lt;IPv4Range&gt;> Merge(IEnumerable&lt;IPv4Range&gt; pRangeDatas)**  

結合可能なアドレス同士を結合し、マージ後のデータリストを返します。  
pRangeDatas内のオブジェクトの内容は変更されません。  
総当たりで処理を行うためデータ数によっては時間がかかります。  

 ※[IPv4Range.Min,IPv4Range.Max]※  
pRangeDatas[0,2],[8,9],[5,6],[3,4],[10,20]  
return [8,9],[0,6],[10,20] ※ソートされません※  
