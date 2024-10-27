# AimaiKensaku
**概要**
==========
半角全角の英字を同一視したりと、いわゆる「あいまい検索」を行うクラスです。  
正規化の FormKD を使用しています。  

●**使用方法**
------
```
AimaiKensaku aimai = new AimaiKensaku();
aimai.SearchKey = "ﾊﾞAbC";
bool hit;
hit = aimai.IsHit("バABc");			// true
hit = aimai.IsHit("ハ゛ａＢＣ");	// true
```
最初にキー文字列をSearchKeyに設定します。  
次にIsHit()に検索対象文字列を渡して一致するかを判定します。  
同一視される文字については[`同一視される文字`](#同一視される文字)を参照してください。  

●**プロパティー**
------
<a name="#IgnoreCase"></a>**public bool IgnoreCase get/set**  
falseを設定すると大文字小文字を区別します。  
デフォルトは true 。  

<a name="#IgnoreBackSlashIsYenSign"></a>**public bool IgnoreBackSlashIsYenSign get/set**  
falseを設定するとバックスラッシュと円記号(¥)を区別します。  
デフォルトは true 。  
バックスラッシュ:\(U+5C)、＼(U+FF3C)  
円記号:¥(U+A5)、￥(U+FFE5)  

<a name="#SearchKey"></a>**public string SearchKey get/set**  
検索のキーとなる文字列を設定・取得します。  
[`IsHit()`](#IsHit)を呼ぶ前に必ず設定してください。  

<a name="IsHit"></a>●**public bool IsHit(string pTarget)**  
-----
[`SearchKey`](#SearchKey) に設定した文字列と pTarget で指定した文字列が一致するかを判定し、一致する場合はtrueを返します。  

<a name="#同一視される文字"></a>●**同一視される文字(一例)**  
----
**英大文字と小文字**  
'A'、'a'、'Ａ'、'ａ'  
[`IgnoreCase`](#IgnoreCase)で区別させることが出来ます。  

**バックスラッシュと円記号**  
'\'(U+5C)、'＼'(U+FF3C)、'¥'(U+A5)、'￥'(U+FFE5)  
[`IgnoreBackSlashIsYenSign`](#IgnoreBackSlashIsYenSign)で区別させることが出来ます。  

**ダブルクォート**  
'"'(U+22)、'“'(U+201C)、'”'(U+201D)、'„'(U+201E)、'‟'(U+201F)、'＂'(U+FF02)  

**波ダッシュとチルダ**  
'~'(U+7E)、'〜'(U+301C)、'～'(U+FF5E)  

**(株)括弧株**  
'(株)'、'（株）'、'㈱'  

**元号**  
'明治'、'㍾'  
'大正'、'㍽'  
'昭和'、'㍼'  
'平成'、'㍻'  
'令和'、'㋿'  
