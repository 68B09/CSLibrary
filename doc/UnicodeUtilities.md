# UnicodeUtility
**概要**
==========
Unicodeを扱いやすくするためのクラスです。  
.net(Windows)は文字コードとしてUnicode、そのエンコードにUTF-16を使用しており、1文字はChar型(2Byte)で表します。  
UTF-16を扱う上でサロゲートペアは避けて通れないため、.netではこれをString型もしくはChar[2]、またはInt型(32bit、UTF-32)で扱う必要があります。  
本クラスではそれらを考慮して3つの型を引数に取るメソッドが用意されています、  
(例外あり)
1. UCS-2を想定した(Char)型  
1. サロゲートペアやIVS付きの文字を扱う(String)型  
1. UTF-32で文字を扱う(Int)型  

動作に関してはソースファイルも参照して下さい。

●**簡易半角全角判定**
------
**bool IsHankaku(int pUTF32)**  
**bool IsHankaku(char pChar)**  
**bool IsHankaku(string pString)**  

大前提として、**文字の幅はフォントによって異なるため、文字コードから文字の幅を知ることは出来ない**ことを強調しておきます。  
また、**文字の幅は半角と全角だけではなく、1/3幅なども存在する**ことも強調しておきます。  

本判定の仕様は下記の通りです。  
1. 半角であれば true を、全角であれば false を返す  
1. シフトJISで半角とされる文字は半角、それ以外の文字は全角  
1. 円記号(U+A5)は半角・全角の判定を切り替え可能

円記号(U+A5)を全角と見なしたい場合は、YenSignA5IsFullWidth に true を設定して下さい。

どのメソッドにも1文字を渡して下さい。  
(string)型に渡す文字列は「基本文字＋IVS」でもOKですが、このとき半角全角判定されるのは基本文字となります。  
また、基本文字がサロゲートペアであってもOKです。  

どのメソッドも壊れたサロゲートを渡しても構いませんが、そのときは全角(false)を返します。  
このときサロゲートペア1文字が全角2文字となることに注意して下さい。

    UnicodeUtility uu = new UnicodeUtility();  
    uu.IsHankaku(0x41);    // true
    uu.IsHankaku(0x29E3D); // true  U+29E3D=𩸽
    uu.IsHankaku('A');     // true
    uu.IsHankaku('あ');    // false
    uu.IsHankaku("A");     // true
    uu.IsHankaku("𩸽");    // false
    uu.IsHankaku("蝕󠄀");    // false 蝕=U+8755,E0100(0xDB40,0xDD00)

    uu.YenSignA5IsFullWidth = false;  // default
    uu.IsHankaku('\');    // true U+5C
    uu.IsHankaku('¥');    // true U+A5
    uu.IsHankaku('￥');   // false U+FFE5

    uu.YenSignA5IsFullWidth = true;
    uu.IsHankaku('\');    // true U+5C
    uu.IsHankaku('¥');    // false U+A5
    uu.IsHankaku('￥');   // false U+FFE5

●**外字判定**
------
**GaijiTypes IsGaiji(int pUTF32)**  
**GaijiTypes IsGaiji(char pChar)**  
**GaijiTypes IsGaiji(string pString)**  

どのメソッドにも1文字を渡して下さい。  
渡された文字が外字(私用文字)であるかを判定し GaijiTypes を返します。  
|GaijiTypes||
----|----
|None|外字では無い|
|uE800|U+E800系の外字|
|uF0000|U+F0000系の外字|
|u100000|U+10000系の外字|

●**非文字判定**
------
**bool IsHimoji(int pUTF32)**  
**bool IsHimoji(char pChar)**  
**bool IsHimoji(string pString)**  

どのメソッドにも1文字を渡して下さい。  
渡された文字が非文字であるかを判定し、非文字であれば true を返します。  
非文字とは下記の文字を指します。  
+ U+FDD0～FDEF
+ U+xxFFFE～xxFFFF (各面の下位16bit)

●**使用可能文字判定**
------
**int CheckValidChar(string pString, params char[] pValidChars)**  
**int CheckValidChar(string pString, string pValidChars)**  
**int CheckValidChar(string pString, char pStart, char pEnd)**  
**int CheckValidCode(string pString, int pStartUTF32, int pEndUTF32)**  

指定された文字(char単位)で構成されているかをチェックします。  
指定外の文字が使用されている場合は0以上の値(char配列内インデックス)を返します。  
指定された文字のみで構成されている場合は -1 を返します。  
`("ABCD", 'A','B','C')` と `("ABCD", "ABC")` と `("ABCD", 'A','C')`  と `("ABCD", 0x41,0x43)` は同じ意味です。  

    // 半角数字のみで構成されているかチェック
    uu.CheckValidChar(inputText, "0123456789");

    // ASCII文字のみで構成されているかチェック
    uu.CheckValidChar(inputText, 0x00,0x7F);

●**文字列長取得**
------
**int GetHalfWidthLength(string pString)**  
**int GetHalfWidthLengthBySurrogate(string pString)**  
**int GetHalfWidthLengthByVS(string pString)**  

半角文字を1とした文字列長を返します。  
GetHalfWidthLength() はchar単位で長さを得るため、サロゲートぺア1文字は半角4文字(全角2文字)で計算されます。  
～BySurrogate() はサロゲートペアを考慮しますがIVSなどは無視するため、基本文字とIVSが個別に計算されます。  
～ByVS() はサロゲートぺアもIVSも合成文字も考慮して計算します。  
尚、合成文字の組み合わせは.netの StringInfo クラスの仕様に準じます。

    GetHalfWidthLength("Aあ");    // 3

    GetHalfWidthLength("蝕󠄀");     // 6
    GetHalfWidthLengthBySurrogate("蝕󠄀"); // 4
    GetHalfWidthLengthByVS("蝕󠄀"); // 2

●**タブ→スペース変換**
------
**string TabToSpace(string pString, int pTabSize = 8)**

タブ文字を空白文字に展開した結果を返します。  
タブサイズは pTabSize で2以上を指定します。  
サロゲートぺアやIVSなどは一切考慮しません。  

●**１文字単位に分解**
------
**IEnumerable<string> CharacterEnumeratorBySurrogate(string pString)**  
**IEnumerable<string> CharacterEnumeratorByVS(string pString)**  
**IEnumerable<int> ConvertToUtf32(string pString)**  

文字列から1文字を切り出します。  
～BySurrogate() はサロゲートぺアを考慮しますがIVSや合成文字などは無視します。  
～ByVS()は サロゲートぺアもIVSも合成文字も考慮します。  
尚、合成文字の組み合わせは.netの StringInfo クラスの仕様に準じます。  

ConvertToUtf32() の動作は CharacterEnumeratorBySurrogate() と同じで、1文字をstring(サロゲートぺア)で返すかint(コードポイント)で返すかの違いのみです。  

3メソッド共に IEnumerable を返します。  
配列で欲しい場合は `List<int>(UnicodeUtility.ConvertToUtf32("ABC"))` などで受け取って下さい。  

    UnicodeUtility.CharacterEnumeratorBySurrogate("Aあ");  //[0]="A" [1]='あ'

    UnicodeUtility.CharacterEnumeratorBySurrogate("蝕󠄀");   //[0]="蝕"(U+8755) [1]='□'(0xDB40,0xDD00)
    UnicodeUtility.CharacterEnumeratorByVS("蝕󠄀");          //[0]="蝕󠄀"(U+8755,DB40,DD00)

無印(By無し)のメソッドが存在しないのは、単に `foreach(char c in "ABC"){～}` と書けば良いからです。

**List<string> MakeCodePointString(string pString, bool pSurrogatePair = false, bool pSurrogatePairCombine = false)**  
最小4桁のコードポイント16進数文字列を作ります。  

●**半角文字を全角文字に変換**
------
**public string ConvertHalfToFullWidthNumber(string pString)** '0'~'9'(U+30～U+39)のみを変換  
**public string ConvertHalfToFullWidthAlphabet(string pString)**  'A'~'Z','a'～'z'(U+41～U+5A,U+61～U+7A)のみを変換  
**public string ConvertHalfToFullWidthMark(string pString)** 半角記号のみを変換  
**public string ConvertHalfToFullWidth(string pString)**  上記変換に加えて片仮名も変換する  

半角文字を全角文字に変換した文字列を返します。  
円記号(U+5C及びU+A5)を全角に変換したく無い場合は、ReplaceFlag に YenSign_Ignore を設定して下さい。
円記号(U+A5)は全角へ変換したいが円記号(U+5C)は全角に変換したく無い場合は、ReplaceFlag に H2FJISYenSign_Ignore を設定して下さい。

●**全角文字を半角文字に変換**
------
**public string ConvertFullToHalfWidthNumber(string pString)**  
**public string ConvertFullToHalfWidthAlphabet(string pString)**  
**public string ConvertFullToHalfWidthMark(string pString)**  
**public string ConvertFullToHalfWidth(string pString)**  

●**全角平仮名<->全角片仮名変換**  
-----
**public string ConvertHiraganaToKatakana(string pString)**  
**public string ConvertKatakanaToHiragana(string pString)**  

●**左端から文字列を取り出す**
-----
**public string Left(string pString, int pHalfWidthLength)**  
**public string LeftBySurrogate(string pString, int pHalfWidthLength)**  
**public string LeftByVS(string pString, int pHalfWidthLength)**  

文字列 pString から、文字数 pHalfWidthLength 以下になるように文字を取り出します。

●**行分割**
-----
**public List<string> ToLine(string pString, int pHalfWidthLength)**  
**public List<string> ToLineBySurrogate(string pString, int pHalfWidthLength)**  
**public List<string> ToLineByVS(string pString, int pHalfWidthLength)**  

文字列 pString を文字数 pHalfWidthLength を超えないように分割、配列化して返します。  

●**文字付加**
-----
**public string PadLeft(string pString, int pHalfWidthLength, char pPaddingChar = ' ')**  
**public string PadLeftBySurrogate(string pString, int pHalfWidthLength, char pPaddingChar = ' ')**  
**public string PadLeftByVS(string pString, int pHalfWidthLength, char pPaddingChar = ' ')**  

**public string PadRight(string pString, int pHalfWidthLength, char pPaddingChar = ' ')**  
**public string PadRightBySurrogate(string pString, int pHalfWidthLength, char pPaddingChar = ' ')**  
**public string PadRightByVS(string pString, int pHalfWidthLength, char pPaddingChar = ' ')**  

文字列の左もしくは右へ、文字数が pHalfWidthLength になるように文字 pPaddingChar を追加します。  
pPaddingCharには半角文字を指定します。  

●**BOM判別**
-----
**static public BOMType GetBOMType(byte[] pDatas)**  

BOMが格納されている pPatas を元にBOMを特定し、そのエンコード・タイプを返します。  
pDatasには(ファイルなどの先頭から)4バイト以上のデータが格納されていること。  
4バイト未満ではBOMの判別に失敗する可能性があります。  
BOMが存在しない場合は Unknown が返ります。  

|BOMType||
----|----
|Unknown|不明|
|UTF8|UTF-8|
|UTF16BE|UTF-16BE|
|UTF16LE|UTF-16LE|
|UTF32BE|UTF-32BE|
|UTF32LE|UTF-32LE|

●**全コードポイント作成**
-----
**DEBUGモード時のみ使用可能**  
**static public IEnumerable<int> GetUnicodePoints(int pStart = UNICODE_LOW, int pEnd = UNICODE_HIGH, bool pUseSurrogate = false)**  


U+0000～10FFFFまでの有効なコードポイント(UTF-32)を返します。  
pUseSurrogate が true の場合はサロゲート用のコードポイント(0xD800～DFFF)も返します。  
