# TextUtility
**概要**
==========
UnicodeUtilityに入れるほどでは無い、もしくはUNICODEに依存しないような文字処理を収めたクラスです。  

動作に関してはソースファイルも参照して下さい。

●**ファイルパス短縮**
------
**string MakeCompactPath(string pFilePath, int pMaxHalfLength, FilePathCompactTypes pFilePathCompactType)**  

半角・全角の判定基準はUnicodeUtilityに従います。  
本メソッドはWindowsのファイルパス短縮表示に似た文字列を生成します。  
(例)"C:\temp\foo.txt" → "C:\\...\foo.txt"  

pMaxHalfLengthには半角1文字を1とした文字数を指定します。  

pFilePathCompactTypeには省略する位置を指定します。
|FilePathCompactTypes||
----|----
|File|Windowsの短縮表示ライクな文字列を生成します。"C:\\...\foo.txt"|
|Top|パスの先頭から省略します。"...temp\foo.txt"|
|Last|パスの末尾から省略します。"C:\temp\foo..."|

短縮後の文字列はpMaxHalfLengthよりも短くなることがあります。  
また、サロゲートぺアは考慮しますが結合文字などは考慮しません。  