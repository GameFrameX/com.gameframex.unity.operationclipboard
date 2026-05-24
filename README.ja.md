<p align="center">
  <img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160" />
</p>

<h1 align="center">Game Frame X Operation Clipboard</h1>

<p align="center">
  <a href="https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases">
    <img src="https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationclipboard" alt="Version" />
  </a>
  <a href="https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationclipboard" alt="License" />
  </a>
  <a href="https://gameframex.doc.alianblank.com">
    <img src="https://img.shields.io/badge/Documentation-online-blue" alt="Documentation" />
  </a>
</p>

<p align="center">
  インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援
</p>

<p align="center">
  <a href="https://gameframex.doc.alianblank.com">ドキュメント</a> ·
  <a href="#クイックスタート">クイックスタート</a> ·
  <a href="https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6">QQグループ</a> ·
  言語: <a href="README.md">English</a> | <a href="README.zh-CN.md">简体中文</a> | <a href="README.zh-TW.md">繁體中文</a> | <strong>日本語</strong> | <a href="README.ko.md">한국어</a>
</p>

---

## プロジェクト概要

**Operation Clipboard** - Unity で Android と iOS のクリップボードの読み書きを行うプラグイン。IL2CPP ストリッピング防止用の `link.xml` を含みます。

## クイックスタート

### インストール

以下のいずれかの方法を選択してください：

1. プロジェクトの `manifest.json` ファイルに以下を追加：
   ```json
   {"com.gameframex.unity.operationclipboard": "https://github.com/AlianBlank/com.gameframex.unity.operationclipboard.git"}
   ```
2. Unity の `Package Manager` で `Git URL` を使用して追加：https://github.com/AlianBlank/com.gameframex.unity.operationclipboard.git
3. リポジトリをダウンロードして Unity プロジェクトの `Packages` ディレクトリに配置（自動的に読み込まれます）。

### 使用例

```csharp
using UnityEngine;

public class ClipboardDemo : MonoBehaviour
{
    private string text = "demoText";
    private string result = "";

    void OnGUI()
    {
        text = GUILayout.TextField(text, GUILayout.Width(500), GUILayout.Height(100));

        if (GUILayout.Button("SetValue", GUILayout.Width(500), GUILayout.Height(100)))
        {
            BlankOperationClipboard.SetValue(text);
        }

        if (GUILayout.Button("GetValue", GUILayout.Width(500), GUILayout.Height(100)))
        {
            result = BlankOperationClipboard.GetValue();
        }

        GUILayout.Label(result);
    }
}
```

## プラットフォーム対応

| プラットフォーム | 対応 |
|-----------------|------|
| Windows | はい |
| macOS | はい |
| Android | はい |
| iOS | はい |

## ドキュメントとリソース

- [ドキュメント](https://gameframex.doc.alianblank.com)
- [GitHub リポジトリ](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard)

## コミュニティとサポート

- QQグループ：[QRコード](https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6)から参加

## ライセンス

このプロジェクトは [LICENSE](LICENSE) ファイルに定義された条件に基づいてライセンスされています。
