# IPv4Base
**概要**
==========
IPv4アドレスを扱う基本クラスです。  

●**アドレス値(文字列)のバイナリ(整数)化**
------
**static public uint StringToUInt(string pIPv4)**  

アドレス文字列("x.x.x.x")をUInt32型に変換します。  

●**アドレス値(バイナリ)の文字列化**
------
**static public string UIntToString(uint pIPv4)**  

アドレス値(UInt32)を文字列("x.x.x.x")に変換します。  

●**各種アドレス表記文字列からオブジェクトを生成**
------
**static public object Parse(string pAddress)**  

各種アドレス表記の文字列から対応するオブジェクトを生成します。  
認識可能な表記と生成されるオブジェクト型は以下の通りです。  
認識出来ない場合はnullが返ります。  
"198.51.100.0" IPAddress  
"198.51.100.0/24" IPv4Range  
"198.51.100.0 198.51.100.1" IPv4Range  
"198.51.100.0-198.51.100.1" IPv4Range  
"198.51.100.0/255.255.255.0" IPv4Range  

