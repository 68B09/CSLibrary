# CSLibrary/C#用クラス群

## 想定言語・バージョン
C#/.net framework 4.5.2  
****
## [●CSLibraryConstants](/doc/CSLibraryConstants.md)
CSLibrary全体で参照する定数などを集めたクラス。  
___
## [●UnicodeUtilities](/doc/UnicodeUtilities.md)
Unicode文字を扱う上での各種便利関数を集めたクラス。  
・半角全角判定  
・半角全角変換  
・外字判定  
・タブ→空白文字変換  
・行分割  
・１文字に分解  
etc..  
___
## [●TextUtility](/doc/TextUtility.md)
UnicodeUtilityに入れるほどでは無い文字処理を収めたクラス。  
・ファイルパスの短縮  
etc..  
___
## [●AimaiKensaku](/doc/AimaiKensaku.md)
あいまい検索を行うクラス。  
___
## [●CSVAssistant](/doc/CSVAssistant.md)
CSVファイルを読み書きするためのクラス。  
フィールド単位で""で囲まれていたか否か判別可能。  
___
## [●WaitCursor](/doc/WaitCursor.md)
時間がかかる処理をusingで囲み、処理が終わるまでマウスカーソルを「ウェイトカーソル」にするクラス。  
```
using(WaitCursor waitCursor = new WaitCursor()){
	処理();
}
```
___
## [●SharedData](/doc/SharedData.md)
共有データ、いわゆるグローバルデータを管理するクラス。  
本クラスを用いなくともstaticメンバを持っているクラスを作れば済むといえばそれまでですが…  
___
## [●RingBuffer](/doc/RingBuffer.md)
リングバッファのコレクションクラス。  
___
## [●ExDrawString](/doc/ExDrawString.md)
縦書き・横書き対応の拡張DrawStringクラス。  
矩形内に収まるようにフォントサイズを縮小したり、均等割り付けで描画出来ます。  
尚、.net(Windows)の仕様によりIVSなどの文字は正しく描画出来ません。  
※IVSが□で描画されてしまう  
___
## [●CSGeometries](/doc/CSGeometries.md)
各種幾何計算メソッドを持つクラス。  
___
## [●ExFileStream](/doc/ExFileStream.md)
FileStreamの拡張クラス。  
___
## [●ByteArrayUtility](/doc/ByteArrayUtility.md)
byte[]を扱う各種処理を持つクラス。  
___
## [●BCDUtility](/doc/BCDUtility.md)
BCD(二進化十進法)を扱うクラス群。  
___
## [●IPv4](/doc/IPv4.md)
IPv4を扱うクラス群。  
___
## [●Pairs](/doc/Pairs.md)
データを単純に扱うクラス群。  
___
## [●DateTimeUtility](/doc/DateTimeUtility.md)
System.DateTimeの拡張メソッドなどを収めたクラスです。  
___
## [●OtherUtility](/doc/OtherUtility.md)
その他に分類されるクラスやメソッド群。  
___
## ライセンス
ライセンスを適用するファイルにはライセンスについての記述があります。  
The MIT License (MIT)  
Copyright (c) 2022 68B09  
___
## 履歴
2024/7/10 68B09  
DateTimeUtility.csを追加。  

2024/7/7 68B09  
OtherUtility.csを追加。  

2024/7/4 68B09  
IPv4.cs、Pairs.csを追加。  

2024/1/8 68B09  
CSLibraryConstants.cs,DPIUnit.csを追加。  

2022/5/28 68B09  
BCDUtility.csを追加。  

2022/4/30 68B09  
CSGeometries.Unitsを追加。  

2021/12/30 68B09  
ExFileStreamおよびByteArrayUtilityを追加。  

2021/8/21 68B09  
CSGeometriesを追加。  

2021/8/2 68B09  
CSVAssistant.cs、WaitCursor.cs、SharedData.cs、RingBuffer.csを追加。  

2021/7/22 68B09  
First release.
