# RingBuffer
**概要**
==========
リングバッファを実現するコレクションクラス。  
**スレッドセーフではありません。**  

動作に関してはソースファイルも参照して下さい。

●**プロパティー**  
-----
**public int Count get**  
格納されているデータ数。  

**public int Capacity get/set**  
バッファ数取得・設定。  
現在保有しているデータ数を下回る値は設定出来ません。  

●**インスタンス作成**  
------
**public RingBuffer(int pCount)**  
バッファの型を指定して作ります。  
```
RingBuffer<int>(10);    // int型の項目を持つリングバッファをバッファ数10で作成
RingBuffer<string>(10); // string型　〃
```

**public RingBuffer(RingBuffer<T> pInitialData)**  
コピーコンストラクタ。  
インスタンスのバッファ数は pInitialData.Count となります。  
また、コピー元からデータを読み出すため pInitialData の Count はゼロになります。  
(コピーではなく「奪う」が正解)

**public RingBuffer(IEnumerable<T> pInitialData)**  
pInitialData に格納されているデータを持つリングバッファを作成します。  
インスタンスのバッファ数は pInitialData に格納されているデータ数 となります。  
pInitialData 内のデータはそのままです。  

●**データ数・バッファ数**  
-----
Capacity プロパティーで変更でき、データは保持されます。  
現在のデータ数以上に増やす場合はメモリの制限内で無制限ですが、減らす場合はCountを下回ることは出来ません。  
また、バッファ数を変更すると時間・メモリ容量のコストがかかります。  
```
// ringbuf.Count == 10,ringbuf.Capacity == 10 だとして
ringbuf.Capacity = 100;    // バッファ数を10から100に増やす OK
ringbuf.Capacity = 9;      // バッファ数を100から9に減らす ArgumentOutOfRangeException発生
```

**public void Clear()**  
データを破棄し Count をゼロにします。  

●**読み書き**
-----
**public T Read()**  
データを一つ取り出し、Countをデクリメントします。  
Countがゼロの時に呼び出すと例外が発生します。  

**public T ReadAt(int pIndex)**  
pIndex番目のデータを取り出しますが、そのデータは保持されたままです。(非破壊読み出し)  
pIndex は ゼロ以上かつCount 未満であること。  

**public T[] ToArray()**  
保持しているデータを配列化して返します。  
データは保持されたままです。(非破壊読み出し)  

**public void Write(T pValue)**  
データをバッファに書き込みます。(追加します)  
Capacity(バッファ数) を超える場合は InvalidOperationException が発生します。  

**public void WriteRange(IEnumerable<T> pSource)**  
データ一括書き込み。  
Capacity(バッファ数) を超える場合は InvalidOperationException が発生します。  

コンストラクタで初期値を設定すると「バッファ数＝データ数」となってしまうため、Capacityでバッファを増やすよりはコンストラクタでバッファ数のみを指定し、インスタンス作成後に本メソッドでデータを追加する方がコストが良いです。  
```
RingBuffer<int> ringbuf = new RingBuffer<int>(src);
ringbuf.Capacity = 100;
```
とするよりは、
```
RingBuffer<int> ringbuf = new RingBuffer<int>(100);
ringbuf.WriteRange(src);
```
とした方が速くメモリも荒らしません。  
 ただし src.Length が 100 を超えていると例外が発生します。  

**IEnumerator IEnumerable.GetEnumerator()**  
IEnumerator を返します。  
破壊読み出しなのでループを回した分だけ Count が減ります。  
```
foreach(int val in ringbuf){
    System.Diagnostics.Debug.WriteLine(val.ToString());
}

// ringbuf.Count is 0
```
