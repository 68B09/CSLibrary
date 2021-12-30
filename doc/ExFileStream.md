# ExFileStream
**概要**
==========
FileStreamに現在位置保存・復帰メソッドなどを追加した拡張クラスです。  
動作に関してはソースファイルも参照して下さい。

●**現在位置保存・復帰**
------
**long SaveCurrentPosition()**  
**void RestorePosition()**  
**long SaveAndSetPosition(long pSeekPosition)**  

位置情報はスタックに積まれるためSaveCurrentPosition()とRestorePosition()はペアで使用します。  
SaveAndSetPosition()は現在位置を保存した後、指定位置にシークします。  
「位置」とはファイル先頭からのバイト位置です。  

●**文字列読み出し**
------
**string GetAsciiString(int pLength)**  
**string GetUTF16BEString(int pLength)**  

現在位置から文字列を読み込みます。  
pLengthはバイト数です。  
文字はGetAsciiString()はASCIIで、GetUTF16BEString()はUTF-16BEで認識されます。  
例えば、ASCII範囲外の文字をGetAsciiString()で読み込んだ場合の返却文字列の文字は不定です。  

●**数値読みだし**
------
**UInt32 GetUInt32BE()**  
**UInt16 GetUInt16BE()**  

現在位置からBigEndian形式で数値を取り出します。  
