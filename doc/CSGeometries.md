# CSGeometries
**概要**
==========
https://github.com/68B09/CSGeometries をCSLibraryに移動させたものです。  
線分の長さや線交差座標等を求めるための幾何計算クラスや、RGB-HSV相互変換などのカラーモデル変換クラスを実装しています。  
1. 座標は直交座標系です  
1. 座標の管理には独自のDouble型 **PointD** クラスを使用しています  
1. 条件を満たさないパラメータを与えると不正な結果を返すメソッドもあることに注意してください  
1. ColorDやHSV構造体の多くのメンバ変数が取る値が0.0～1.0であることに注意が必要です

動作に関してはソースファイルも参照して下さい。  

------
# CSGeometries.Units
**概要**
==========
SIなどの単位に関するクラス群です。  
1. SIUnit … 10を底とするSI接頭辞(u,m,k,M,...)  
1. BinUnit … 2を底とする2進接頭辞(Ki,Mi,Gi...)  
1. DPIUnit … DotPerInchを基準とした各種単位への変換  
     
double型の精度や演算誤差などにより必ずしも正確な変換が行えるとは限らないことに注意して下さい。  
特に大きな単位や小さな単位では誤差が生じやすくなります。  

動作に関してはソースファイルも参照して下さい。  
  
●**単位変換(正規化)**
------
**double Normalize(double pValue, out string pUnitName)**  
  
正規化後の値とその単位文字列を返します。  
SIUnitは1000単位、BinUnitは1024単位で正規化します。  
```
SIUnit unit = new SIUnit();
string unitName;
double ans = unit.Normalize(1234, out unitName);
// ans = 1.234, unitName = "k"
```

```
BinUnit unit = new BinUnit();
string unitName;
double ans = unit.Normalize(1263.616, out unitName);
// ans = 1.234, unitName = "Ki"
```
  
●**冪指数取得**  
------
**int GetBekisisu(string pUnitName)**  
  
単位名に対応する冪指数を返します。  
SIUnitは10^n^の、BinUnitは2^n^のnを返します。  
単位名の大文字小文字は厳密に扱われます。  
```
SIUnit unit = new SIUnit();
int beki = unit.GetBekisisu("k");
// beki = 3
```

```
BinUnit unit = new BinUnit();
int beki = unit.GetBekisisu("Ki");
// beki = 10
```

●**DPIUnit**  
------
DPIを基準として各種単位への変換を行います。  

・ミリメートルをドット(ピクセル)数へ変換。  
```
DPIUnit dpiUnit = new DPIUnit(p_graphics.DpiX);
float width = dpiUnit.MMToDot<float>(10);    // 10mmをドット数へ変換
```
