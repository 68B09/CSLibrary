# ExDrawString
**概要**
==========
指定領域に文字列を横書き、または縦書きで描画します。  
指定領域の文字が収まらない場合はフォントを小さくしたり端で折り返す指定も可能です。  

動作に関してはソースファイルも参照して下さい。

●**プロパティー**
-----
**Font Font get/set**  
フォントを設定します。  
縦書き時にフォント名の先頭に'@'を付加する必要はありません。  
*****
**Brush Brush get;set;**  
文字を描画する際に使用するブラシを設定します。  
*****
**FormatFlags FormatFlag get/set**  
各種フラグを設定します。  

|FormatFlags||
----|----
|Default|下記フラグ類を設定しない場合のデフォルト値|
|NoWrap|改行文字で改行する(自動改行しない)|
|IgnoreFontResize|フォントサイズを調整しない|
|Vertical|縦書き|
|HalfToFullWidth|半角文字を全角文字に変換|
|HCenter|水平方向中央寄せ|
|HRight|水平方向右寄せ。縦書きの時は左寄せ。|
|HKintou|水平方向均等割り付け|
|VCenter|垂直方向中央寄せ|
|VBottom|垂直方向下寄せ|
|VKintou|垂直方向均等割り付け|

●**使い方**
------
```
ExDrawString exDraw = new ExDrawString();
exDraw.Font = font;
exDraw.Brush = Brushes.Black;
exDraw.FormatFlag = ExDrawString.FormatFlags.Default;
exDraw.Draw(text, graphics, x, y, width, height);
```
●**備考**
------
- 文字は `Graphics.DrawString` で描画されます。  
そのため印刷用のGraphicsにも使用出来ます。  

- サロゲートペアやIVSを認識します。  
一文字は `StringInfo` で認識される単位です。  
`Graphics.DrawString` はIVS付きの文字を正しく描画出来ないため、異体字などは不正確になります。  
また、国旗(🇯🇵)などは `StringInfo` が認識しないため不正確になります。  

- フォントサイズの縮小  
`FormatFlags.IgnoreFontResize` が指定されていない場合はフォントサイズを縮小して文字列全体を描画しようとしますが、フォントサイズが**0.1pt(約0.0352mm)以下**になった場合は描画を行わずリターンします。  

- 合成文字  
ガ(U+30AB,3099) などの合成用濁点付きの文字は完成形の ガ(U+30AC)に変換されて描画されます。  
