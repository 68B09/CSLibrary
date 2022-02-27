# JISCharacterCode
**概要**
==========
JIS文字コード系を扱うクラス群です。  
JISCharacterCode.ShiftJISは純粋なJIS X 0208を、JISCharacterCode.CP932はMicrosoft-CP932を扱います。  
動作に関してはソースファイルも参照して下さい。

●**ShiftJIS**
------
本クラスはJIS X 0208を基準とする文字変換メソッドなどを提供します。  
CP932を対象とする場合はCP932クラスを使用して下さい。  

**int JIStoSJIS(ushort pJISCode)**  
**int SJIStoJIS(ushort pSJISCode)**  

JIStoSJISはJIS漢字コードをShift_JISへ、SJIStoJISはその逆の変換を行います。  
JIS漢字コード範囲は0x2121～7E7E、Shift_JISコードの範囲は0x8140～EFFCです。  

●**CP932**
------
本クラスはCP932を基準とする文字変換メソッドなどを提供します。  
JIS X 0208CP932を対象とする場合はShiftJISクラスを使用して下さい。  

**int JIStoCP932(ushort pJISCode)**  
**int CP932toJIS(ushort pCP932Code)**  

JIStoCP932はJIS漢字コードをCP932へ、CP932toJISはその逆の変換を行います。  
JIS漢字コード範囲は0x2121～987E、Shift_JISコードの範囲は0x8140～FCFCです。  