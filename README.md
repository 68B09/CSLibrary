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
リングバッファ  
```
RingBuffer<int> buffer = new RingBuffer<int>(5); // buffer size = 5
// datacount=0,remain=5
buffer.Write(10);		// datacount=1,remain=4
buffer.Write(20);		// datacount=2,remain=3
buffer.Write(30);		// datacount=3,remain=2
count = buffer.Count;	// count=3
count = buffer.Capacity;// count=5(buffer size)
data = buffer.Read();	// data=10,count=2,remain=3
data = buffer.Read();	// data=20,count=1,remain=4
buffer.Clear();			// count=0,remain=5
buffer.Capacity=10;		// バッファ数を10に拡張。Count以上であれば縮小も可能。基本的にCapacityへの代入はおすすめしない。最初から最適なバッファ数を指定するべき。
buffer.WriteRange(new int[5] { 0, 1, 2, 3, 4 });// datacount=5,remain=5

```
***
## [●ExDrawString](/doc/ExDrawString.md)
(注意)作成中  
縦書き・横書き対応の拡張DrawStringクラス。  
矩形内に収まるようにフォントサイズを縮小したり、均等割り付けが可能になる予定。  
尚、.net(Windows)の仕様によりIVS付きの文字は正しく描画出来ない。  
※IVSが□で描画されてしまう  
___
## ライセンス
ライセンスを適用するファイルにはライセンスについての記述があります。   
The MIT License (MIT)  
Copyright (c) 2021 68B09  
___
## 履歴
2021/8/2 68B09  
CSVAssistant.cs、WaitCursor.cs、SharedData.cs、RingBuffer.csを追加。  

2021/7/22 68B09  
First release.
