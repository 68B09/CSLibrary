# CSLibrary/C#用クラス群

## 想定言語・バージョン
C#/.net framework 4.5.2  
****
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
## ライセンス
ライセンスを適用するファイルにはライセンスについての記述があります。   
The MIT License (MIT)  
Copyright (c) 2021 68B09  
___
## 履歴
2021/8/21 68B09  
CSGeometriesを追加。  

2021/8/2 68B09  
CSVAssistant.cs、WaitCursor.cs、SharedData.cs、RingBuffer.csを追加。  

2021/7/22 68B09  
First release.

