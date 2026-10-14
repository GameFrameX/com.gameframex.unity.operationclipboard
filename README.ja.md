<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX OperationClipboard パッケージ

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## 目次

- [プロジェクト概要](#プロジェクト概要)
- [クイックスタート](#クイックスタート)
- [使用例](#使用例)
- [ドキュメントとリソース](#ドキュメントとリソース)
- [コミュニティとサポート](#コミュニティとサポート)
- [ライセンス](#ライセンス)

---

## プロジェクト概要

GameFrameX OperationClipboardは、GameFrameXフレームワークのクロスプラットフォーム クリップボードコンポーネントです。コールバック契約とチャネルレジストリにより、Unityプロジェクトに統一されたクリップボード読み書き機能を提供します。

### 主な機能

- **コールバック契約** - `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`、全プラットフォームで統一されたAPI。同期プラットフォームでは呼び出しスタック内で即時にコールバック
- **チャネルレジストリ** - `OperationClipboardRegistry`は登録順の相互排他解決、冪等な`Register`（同一idは先着優先＋警告）、`SubsystemRegistration`での`Reset`、ファクトリーの遅延インスタンス化とキャッシュ
- **デフォルトフォールバック** - チャネル未登録時はメインパッケージのデフォルト実装（Editor/Standalone `GUIUtility.systemCopyBuffer`、Android JNI、iOS P/Invoke）にフォールバック
- **ミニゲームSDKへの参照ゼロ** - メインパッケージはミニゲームSDKアセンブリを参照しません。プラットフォーム適応は独立したチャネルパッケージが起動時に自己登録します（参照即登録）

### システム要件

- Unity 2019.4以上

---

## クイックスタート

### インストール

以下のいずれかの方法を選択してください：

1. Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加してください：
   ```json
   {
     "scopedRegistries": [
       {
         "name": "GameFrameX",
         "url": "https://gameframex.upm.alianblank.uk",
         "scopes": [
           "com.gameframex"
         ]
       }
     ],
     "dependencies": {
       "com.gameframex.unity.operationclipboard": "2.0.0"
     }
   }
   ```

   `scopes` はどのパッケージがこのレジストリから解決されるかを制御します。`com.gameframex` で始まるパッケージのみこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に直接以下を追加してください：
   ```json
   {
      "com.gameframex.unity.operationclipboard": "https://github.com/gameframex/com.gameframex.unity.operationclipboard.git"
   }
   ```
3. Unity の `Package Manager` で **Git URL** を使って追加してください。URL： `https://github.com/gameframex/com.gameframex.unity.operationclipboard.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリに配置すると自動的に読み込まれます。

## 使用例

### 基本的な使い方

```csharp
using GameFrameX.OperationClipboard.Runtime;

// クリップボードへ書き込む
OperationClipboard.SetValue(text, success => { /* 任意の結果 */ });

// クリップボードから読み取る（同期プラットフォームは即時にコールバック）
OperationClipboard.GetValue((success, content) =>
{
    if (success)
    {
        // content を使用
    }
});
```

### チャネル登録（Bootstrap 自己登録）

チャネル発見層はレジストリベースです。各チャネルパッケージは自身の `XxxOperationClipboardBootstrap`（`RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`）でチャネルファクトリーを `OperationClipboardRegistry` に登録します。未インストールのチャネルパッケージのコードは除去され、登録は行われません（参照即登録）。

- **相互排他解決**：`OperationClipboardRegistry.ResolveManager()` は登録順で先頭のチャネルを取得します（ミニゲームチャネルはプラットフォームマクロにより自然に相互排他）；
- **設定面なし**：クリップボードAPIはテキストのみを受け取るため、チャネルに設定クラスはありません。登録項目はチャネルid・表示名・`Func<IOperationClipboardManager>` ファクトリーのみ；
- **ファサードフォールバック**：静的ファサード `OperationClipboard` はレジストリ解決を優先し、未登録時はデフォルト実装にフォールバックします。

### 破壊的変更と移行（v2.0.0）

- `BlankOperationClipboard.GetValue()` / `SetValue(string)` は削除され、`GameFrameX.OperationClipboard.Runtime.OperationClipboard.GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)` に置き換えられました；
- アセンブリ名が `BlankOperationClipboard.Runtime` から `GameFrameX.OperationClipboard.Runtime` に変更されました（名前空間は家族規約に整列）；
- WebGL ミニゲーム分岐（`ENABLE_WECHAT_MINI_GAME` / `ENABLE_DOUYIN_MINI_GAME`）はメインパッケージから移出されました。対応するチャネルパッケージ（`com.gameframex.unity.operationclipboard.wechatminigame` / `.alipayminigame` / `.kuaishouminigame` / `.douyinminigame`）とミニゲームSDKをインストールしてください；
- ミニゲームのクリップボードAPI（wx / my / ks / tt）は非同期コールバックAPIです。v1の同期 `string GetValue()` はJSブリッジの応答前に空文字を返していました。v2では全プラットフォームでコールバックにより結果を配信します。

### 主要なクラスとインターフェース

- **`IOperationClipboardManager`**：コアとなるコールバック契約 `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`。
- **`OperationClipboardRegistry`**：チャネルレジストリ。`Register` / `Unregister` / `Contains` / `GetManager` / `ResolveManager()`（登録順の相互排他解決）。
- **`OperationClipboardChannelRegistration`**：チャネル登録記述子（チャネルid + 表示名 + `Func<IOperationClipboardManager>` ファクトリー）。
- **`DefaultOperationClipboardManager`**：メインパッケージのデフォルト実装（Editor/Standalone `GUIUtility.systemCopyBuffer`、Android JNI、iOS P/Invoke）。同期プラットフォームでは即時にコールバック。
- **`OperationClipboard`**：静的ファサード。レジストリ解決を優先し、未登録時はデフォルト実装にフォールバック。

### プラットフォーム対応

| プラットフォーム | 実装 |
|---|---|
| Editor / Standalone | `GUIUtility.systemCopyBuffer` |
| Android | JNI `com.alianhome.operationclipboard.MainActivity` |
| iOS | P/Invoke `BlankOperationClipboard.mm` |
| WeChat / Alipay / Kuaishou / Douyin ミニゲーム | チャネルパッケージ |

---

## ドキュメントとリソース

- **ドキュメント**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **Issue トラッカー**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/issues)

---

## コミュニティとサポート

- **QQ グループ**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **機能リクエスト**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/discussions)

---

## 依存関係

| パッケージ | 説明 |
|----|------|
| com.gameframex.unity | GameFrameX コア |


## 変更履歴

更新履歴は [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases) をご参照ください。

## ライセンス

詳しくは [LICENSE.md](LICENSE.md) をご参照ください。
