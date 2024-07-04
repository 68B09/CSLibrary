# IPv4Range
**概要**
==========
IPv4アドレスを範囲で扱う基本クラスです。  

●**コンストラクタ**
------
**public IPv4Range()**  
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

●**最小・最大値設定**
------
**public void SetMinMax(uint pMin, uint pMax)**  

アドレスの最小・最大値を設定するメソッドです。  
pMin<=pMaxが守られていない場合は例外がスローされます。  

●**現在の範囲を表すCIDR表記文字列を生成**
------
**public IList&lt;string&gt; ToCIDR()**  

Min,Maxの範囲に対応するCIDR表記の文字列を生成して返します。  
範囲によっては2つ以上返す事があります。  

●**アドレス内包チェック**
------
**public bool InRange(uint pIPv4)**  
**public bool InRange(IPv4Range pRange)**  

指定されたアドレスが自身の範囲内であるかをチェックしその判定結果を返します。  

●**ホスト部のマスクビットを作成**
------
**static public uint MakeHostMask(int pBits)**  

pBitsで示されるビット長に対応するホスト部のマスク値を返します。  
pBitsに24を指定すると255(0xFF)が返ります。  
