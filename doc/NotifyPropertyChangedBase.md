# NotifyPropertyChangedBase
**概要**
==========
PropertyChangedイベントを発生させるためのユーティリティークラスです。  
PropertyChangedEventHandlerの定義やPropertyChangedを発生させるsetterメソッドが用意されており、実装が手早く行えるようになっています。  
動作に関してはソースファイルも参照して下さい。

●**使い方**
------
```
class HogeHoge : NotifyPropertyChangedBase … ➀
{
	private int counter = 0;
	public int Counter{
		get => this.counter;
		set => this.SetProperty(ref this.counter, value); … ②
	}

	public int SaturationCounter{
		get => this.counter;
		set { … ③
			int newValue = CSGeometries.Geometry.Saturation(value, 1, 100);
			this.SetProperty(ref counter, newValue);
		}
	}
}
```
➀`NotifyPropertyChangedBase`を継承します。
  
②単純なset(ter)であれば`SetProperty()`にフィールドと新しい値を渡すだけです。  
  
③入力チェックが必要なら`SetProperty()`を呼び出す前に行います。

`SetProperty()`は呼び出し元のメソッド名をプロパティー名として使用します。  
通常はプロパティー名は省略できますが、別の名前を使いたい場合はその名を第三引数として`SetProperty()`に渡してください。  
