# SharedData
**概要**
==========
プロセス内で使用する共有オブジェクトを保持するstaticクラスです。  
本クラスを使用せずともstaticメンバを持つstaticクラスを作れば済むことですが、メンバを増やす度にstaticクラスを書き換える必要がない点が利点です。  

動作に関してはソースファイルも参照して下さい。

●**使い方 名前無し版**
------
```
InitialData iniData = SharedData.GetObject<InitialData>();
```
名前無しオブジェクトの取得・登録。  
InitialData クラスの作成済みオブジェクトを取得します。  
登録されていなければ `new InitialData()` でオブジェクトが作成・登録されます。  
名前無しでは型(T)がキーになるため InitialData を複数登録することは出来ません。  
*****
```
InitialData iniData = SharedData.GetObject<InitialData>(new Object[]{10,"ABC"});
```
前述の例の初期化時引数付きです。  
new実行時、InitialDataのコンストラクタに {10,"ABC"}  が渡されます。  
*****
```
main()
{
    InitialData iniData = new InitialData();
    iniData.No = 10;
    iniData.Text = "ABC";
    SharedData.SetObject(iniData);
	sub();
}

sub()
{
	// mainで登録したオブジェクトを取得
    InitialData iniData = SharedData.GetObject<InitialData>();
}
```
自動作成できない、もしくはしたくない場合は SetObject() で登録します。  

●**使い方 名前付き版**
------
名前無しは型(T)でオブジェクトを管理するため、同一クラスを複数登録することが出来ません。  
```
PointData p1 = SharedData.GetObject<PointData>();
PointData p2 = SharedData.GetObject<PointData>();    // p2==p1
```
名前付き版はオブジェクトを名前で管理するため、同一クラスを複数登録することが出来ます。  
```
PointData p1 = SharedData.GetObject<PointData>("P1");    // "P1"という名前のオブジェクト
PointData p2 = SharedData.GetObject<PointData>("P2");    // "P2"という名前のオブジェクト
// p1とp2にはそれぞれ異なるオブジェクトが返ってくる。
```

SetObject()もあります。  
```
SharedData.SetObject("MIN_WIDTH", 1);
SharedData.SetObject("MAX_WIDTH", 10);

int min = Shared.GetObject<int>("MIN_WIDTH");	// 1
int max = Shared.GetObject<int>("MAX_WIDTH");	// 10
```
