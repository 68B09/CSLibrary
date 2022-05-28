# BCDUtility
**概要**
==========
BCD(二進化十進法)のデコード・エンコードをサポートします。  
PackedBCDはパック形式、UnpackedBCDはアンパック形式をサポートします。  

●**整数型へ変換**
------
**public long ToLong(byte[] pBCDs, int pOffset, int pBytes)**  
BCDをlong型に変換します。  
pOffsetには解析開始位置を、pBytesにはpOffsetからの解析バイト数を指定します。  

●**BCDへ変換**
------
**public void ToBCD(long pValue, byte[] pBCDs, int pOffset, int pBytes)**  
long型をBCDに変換します。  
pOffsetには格納開始位置を、pBytesにはpOffsetからの格納領域バイト数を指定します。  

●**byte Zone**
------
UnpackedBCDで使用するゾーン値を指定します。  
既定値は3です。  

●**byte PlusSign**
------
正の符号に使用する値を指定します。  
既定値は0x0Cです。  

●**byte MinusSign**
------
負の符号に使用する値を指定します。  
既定値は0x0Dです。  

●**IncludeSigns IncludeSign**
------
符号値を格納する・しないを指定します。  
既定値はIncludeです。  

IncludeSigns.NotInclude…含まない  
IncludeSigns.Include…含む  
IncludeSigns.Ambiguous…曖昧(有っても無くてもよい)  
