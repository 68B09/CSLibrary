# GraphicsStateStack

## 概要

Graphicsの状態(GraphicsState)のSave/Restoreをサポートするクラスです。  
C#のusing()を利用してスコープから抜けるときにRestoreを自動実行します。  
動作に関してはソースファイルも参照して下さい。

## 使い方

```sample
using (CSLibrary.GraphicsStateStack g = new CSLibrary.GraphicsStateStack(e.Graphics)) {
    using (g.Save()) {  // スコープから出たときに自動的にRestoreが実行される。
        ・・・
        using (g.Save()) {  // ネストも可能です。
            ・・・
        }
    }
}
```

●**コンストラクタ**  
**public GraphicsStateStack(Graphics pGraphics)**  
`GraphicsStateStack`を初期化します。  

●**ステート保存**  
**public IDisposable Save()**  
Gaphicsの状態を保存し、自動開放に必要なオブジェクトを返します。  
受け取ったオブジェクトはusing()に渡してください。  
(注意)Restoreは`GraphicsStateStack`に任せ、独自にRestoreを実行しないでください。  
