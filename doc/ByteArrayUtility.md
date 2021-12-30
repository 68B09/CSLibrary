# ByteArrayUtility
**概要**
==========
byte[]を対象とした各種データ処理クラスです。  
動作に関してはソースファイルも参照して下さい。

●**文字列読み出し**
------
**string GetAsciiString(byte[] pBuf, int pIndex, int pLength)**  

指定位置から文字列を読み込みます。  
pLengthはバイト数です。  

●**数値取得(BigEndian)**
------
**UInt32 GetUInt32BE(byte[] pBuf, int pIndex)**  
**UInt16 GetUInt16BE(byte[] pBuf, int pIndex)**  

指定位置からBigEndian形式で数値を取り出します。  
