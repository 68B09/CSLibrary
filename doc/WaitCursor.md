# WaitCursor
**概要**
==========
時間がかかる処理の実行中にマウスカーソルを砂時計(WaitCursor)にするためのクラスです。  
処理の前でカーソルを変更し、処理終了後に元のカーソルに戻します。  

動作に関してはソースファイルも参照して下さい。

●**使い方**
------
```
using(WaitCursor = new WaitCursor()){
	時間がかかる処理();
}
```
usingを使用して、時間がかかる処理の終了後にWaitCursorのインスタンスが破棄されるようにする方法が一番簡単確実です。