# hakoniwa-unity-drone
このリポジトリでは、Unity上で箱庭ドローンの物理モデルをビジュアライズ・操作できる環境を提供します。

## ディレクトリ構成

このプロジェクトのディレクトリ構成は以下のとおりです：

```tree
hakoniwa-unity-drone/
├── LICENSE
├── README.md
└── simulation/
    ├── Assets/
    │   ├── Configs/        # 設定ファイル
    │   ├── Models/         # FBXファイルなどの3Dモデル
    │   ├── Materials/      # マテリアルファイル
    │   ├── Textures/       # テクスチャファイル
    │   ├── Prefabs/        # プレハブ
    │   ├── Scripts/        # スクリプト
    │   └── Scenes/         # シーンファイル
    └── [その他のディレクトリ]
```

## シーン名

ドローンの基本的な飛行テストを行うシーンとして、`SimpleFlightTest` があり、実機と同等の設定パラメータでシミュレーションの動作確認を行うことができます。

## プラグインのディレクトリ構成

クロスプラットフォーム対応を考慮し、各プラットフォームおよびCPUアーキテクチャに対応したプラグインを以下のように配置しています：
```tree
simulation/
├── Assets/
│   ├── Plugins/
│   │   ├── Android/
│   │   │   ├── ARMv7/
│   │   │   ├── ARM64/
│   │   │   └── x86_64/
│   │   ├── macOS/
│   │   │   ├── x86_64/
│   │   │   └── ARM64/
│   │   ├── Windows/
│   │   │   ├── x86/
│   │   │   └── x86_64/
│   │   ├── Linux/
│   │   │   ├── x86/
│   │   │   └── x86_64/
│   │   └── [共通プラグイン]
```

# サンプル・ドローン

このプロジェクトでは、以下のドローンモデルが含まれています：

## `SimpleDrone`：基本的なドローンモデル
- 3Dモデル：`drone-quadcopter.dae`
  - ベースモデル(実機)：https://holybro.com/products/px4-development-kit-x500-v2?variant=43018371596477
  - 取得元：[OnShape](https://www.onshape.com/en/)
    - 参照URL：https://cad.onshape.com/documents/309acdd0886d0292a98383c2/w/cf26e885b6bdbeacdfee62cf/e/f5458a8dd2d6f5c8dc2574a3

<img width="750" alt="スクリーンショット 2025-02-07 16 59 42" src="https://github.com/user-attachments/assets/f0a03247-a2bf-43a0-a0b1-47409a62e0b9" />
