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

■**簡易半角全角判定**
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

■**外字判定**
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

■**非文字判定**
------
**bool IsHimoji(int pUTF32)**  
**bool IsHimoji(char pChar)**  
**bool IsHimoji(string pString)**  

どのメソッドにも1文字を渡して下さい。  
渡された文字が非文字であるかを判定し、非文字であれば true を返します。  
非文字とは下記の文字を指します。  
+ U+FDD0～FDEF
+ U+xxFFFE～xxFFFF (各面の下位16bit)
