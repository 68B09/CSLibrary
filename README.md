# CSLibrary
C#用クラス群
======================
CSLibrary 
ExGraphics\ExDrawString.cs 
ExUnicode\UnicodeUtilities.cs 
 
想定言語・バージョン
------
C#/.NET Framework 4.5.2 

ExDrawString.cs
------
(注意)作成中 
縦書き・横書き対応の拡張DrawStringクラス。 
矩形内に収まるようにフォントサイズを縮小したり、均等割り付けが可能になる予定。 
尚、.net(Windows)の仕様によりIVS付きの文字は正しく描画出来ない。
※IVSが□で描画されてしまう 
 
UnicodeUtilities.cs
------
Unicode文字を扱う上での各種便利関数を集めたクラス。 
・半角全角判定 
・半角全角変換 
・外字判定 
・タブ→空白文字変換 
・行分割 
・１文字に分解 
etc..

ライセンス
------
ライセンスを適用するファイルにはライセンスについての記述があります。  
The MIT License (MIT)  
Copyright (c) 2021 68B09 
see also 'LICENSE' file 
 
履歴
-----
2021/7/22 68B09 
First release. 
