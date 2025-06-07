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

# 大阪万博向け関連リポジトリ一覧

大阪万博向けのイベントで利用するリポジトリは以下の通りです。

- https://github.com/toppers/hakoniwa-drone-core
- https://github.com/toppers/hakoniwa-ar-bridge
- https://github.com/toppers/hakoniwa-webserver
- https://github.com/toppers/hakoniwa-pdu-csharp
- https://github.com/toppers/hakoniwa-sim-csharp
- https://github.com/toppers/hakoniwa-core-cpp-client
- https://github.com/toppers/hakoniwa-ros2pdu

`world-expo2025` でタグ付けされています。

# サンプル・モデル

このプロジェクトでは、以下のモデルが含まれています：

## `SimpleDrone`：基本的なドローンモデル
- 3Dモデル：`drone-quadcopter.dae`
  - ベースモデル(実機)：https://holybro.com/products/px4-development-kit-x500-v2?variant=43018371596477
  - 取得元：[OnShape](https://www.onshape.com/en/)
    - 参照URL：https://cad.onshape.com/documents/309acdd0886d0292a98383c2/w/cf26e885b6bdbeacdfee62cf/e/f5458a8dd2d6f5c8dc2574a3

## `Rover`：基本的なドローンモデル
- 3Dモデル：`Turtlebot3.dae`
  - 取得元：[OnShape](https://www.onshape.com/en/)
    - 参照URL：https://cad.onshape.com/documents/58a2bdd2a263420f7a316285/w/01c383d9ab503ce7a7c42e3c/e/16a05a97d362a47b16a8f117

## `DJIAvatar`: ドローンモデル
- 3Dモデル：`dji_avatar2.dae`
  - 取得元：[OnShape](https://www.onshape.com/en/)
    - 参照URL：https://cad.onshape.com/documents/8302790419ef6b56cd1eb03c/w/dad3a9c868c42504811a0d26/e/c8d69131de8ac6383b17593b

## `corn`：三角コーン
- 3dモデル：`corn.fbx`
  - 取得元：[3D屋さん](https://3dyasan.com/)
    - 参照URL：https://3dyasan.com/2021/05/10/pylon

## モデルの用途
本リポジトリでは、上述のパブリックモデルを **シミュレーション環境でのビジュアライゼーション用途として** 使用しています。これらのモデルは物理演算やシミュレーションのテストに活用されますが、再配布や改変後の利用については保証されません。

## 注意
これらのモデルはOnShape上のパブリックドキュメントから取得しています。本プロジェクトでは研究・非商用目的で使用していますが、OnShapeのパブリックドキュメントにはライセンス表記がないため、著作権や利用規約については各モデルの作成者に依存する可能性があります。

そのため、商用利用や再配布を行う場合は、各モデルの作成者にライセンスを確認する必要があります。
