# CSVAssistant
**概要**
==========
CSVファイルを読み書きするためのクラスです。  
・RFC 4180に準拠しているファイルが対象です  
・改行のみの行の取得・スキップを指定可能
・項目が""で囲まれていたか否かの判定が可能  
・ファイルを一気に読み込まず、１行単位で取得可能

動作に関してはソースファイルも参照して下さい。

●**プロパティー**
------
**public char FieldSeparator get/set**  
各項目を区切る文字。  
デフォルトはカンマ(,)。  
タブ文字で区切られているときは'\t'を指定して下さい。  

**public char Quote**  
区切り文字を文字の一部として認識させるときなどに使用する囲み文字。  
デフォルトはダブルクォーテーション(")。  
読み込み時には囲み文字は取り除かれた状態で提供されます。  

**public bool SkipNullLine**  
空行を、true の場合は読み込まずに捨てます。  
デフォルトは true 。  

**public bool AnytimeQuote**  
文字列を書き込む際に囲み文字を付加するか否かを決めるフラグ。  
true の場合は囲み文字を付加します。  
デフォルトは true 。  

●**読み込み**  
-----
**public CSVRecordCollection ReadRecords(StreamReader pReader)**  
全レコードを格納した CSVRecordCollection を返します。  
ファイルサイズに比例したメモリ量を消費するため、サイズが大きなファイルの読み込みには不向きです。  
```
CSVAssistant csv = new CSVAssistant();  
CSVRecordCollection list = csv.ReadRecords(sr);
for(int recidx = 0; recidx < list.Count; recidx++){
    CSVRecord rec = list[recidx];
    if(rec.Count >= 1){             // 項目が1個以上存在するか？
        string item1 = rec[0].Text; // 項目を取得。囲み文字は含まれない。
        if(rec[0].Quoted){          // 項目は囲まれていた？
            囲まれていた場合の処理();
        }
    }
}
```
**public CSVRecord ReadRecord(StreamReader pReader)**  
１レコードを読み込んで CSVRecord を返します。  
ファイルの終端に達した場合は null を返します。  
ビッグサイズのファイルから読み込む場合に適しています。  
全てのレコードを取得し終えるまでは pReader を操作しないで下さい。  
```
CSVAssistant csv = new CSVAssistant();  
int recidx = 0;
while(true){
	CSVRecord rec = csv.ReadRecord(sr);
	if(rec == null){
		break;
	}
	string item1 = rec[0].Text;
	recidx++;
}
```
●**書き込み**  
-----
**public void WriteReset()**  
書き込み前リセット。  
書き込みを開始する前に１回呼び出して下さい。  

<a name="writestring"></a>**public void Write(StreamWriter pStream, string pText)**  
**public void Write(StreamWriter pStream, string pFormat, params object[] pArgs)**  
文字列出力。  
文字列 pText に  
+ 区切り文字  
+ 囲み文字  
+ 改行文字  

のいずれかが含まれる場合は文字列の両端に囲み文字が付加されます。  
また、AnytimeQuote プロパティーが true の場合も囲み文字が付加されます。  

**public void Write(StreamWriter pStream, char pValue)**  
**public void Write(StreamWriter pStream, short pValue)**  
**public void Write(StreamWriter pStream, ushort pValue)**  
**public void Write(StreamWriter pStream, int pValue)**  
**public void Write(StreamWriter pStream, uint pValue)**  
**public void Write(StreamWriter pStream, long pValue)**  
**public void Write(StreamWriter pStream, ulong pValue)**  
**public void Write(StreamWriter pStream, float pValue, string pFormat)**  
**public void Write(StreamWriter pStream, double pValue, string pFormat)**  
**public void Write(StreamWriter pStream, decimal pValue, string pFormat)**  
それぞれの型に合わせた書き込みメソッド。  
文字列化した後に[`Write(StreamWriter pStream, string pText)`](#writestring)を使用して出力するため、区切り文字や囲み文字が含まれている場合は囲み文字で囲まれて出力されます。  

**public void WriteLine(StreamWriter pStream)**  
改行を書き出します。  
レコード終了を表す改行は必ず本メソッドを呼び出して書き込んで下さい。  
```
CSVAssistant csv = new CSVAssistant();  
csv.WriteReset();
for(int idx = 123; … ){
	csv.Write(sw, idx);
	csv.Write(sw, "ABC\r\nDEF");
	csv.Write(sw, Math.PI, "0.00");
	csv.WriteLine(sw);  // 行末の改行
}

(writefile)
123,"ABC\r\n
DEF",3.14\r\n
```
