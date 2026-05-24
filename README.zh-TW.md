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
  獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使
</p>

<p align="center">
  <a href="https://gameframex.doc.alianblank.com">文檔</a> ·
  <a href="#快速開始">快速開始</a> ·
  <a href="https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6">QQ群</a> ·
  語言: <a href="README.md">English</a> | <a href="README.zh-CN.md">简体中文</a> | <strong>繁體中文</strong> | <a href="README.ja.md">日本語</a> | <a href="README.ko.md">한국어</a>
</p>

---

## 項目簡介

**Operation Clipboard 操作剪貼簿** - Unity 讀寫 Android 和 iOS 剪貼簿的外掛。包含 `link.xml` 用於防止 IL2CPP 裁剪。

## 快速開始

### 安裝方式

任選其一：

1. 直接在 `manifest.json` 文件中新增以下內容
   ```json
   {"com.gameframex.unity.operationclipboard": "https://github.com/AlianBlank/com.gameframex.unity.operationclipboard.git"}
   ```
2. 在 Unity 的 `Packages Manager` 中使用 `Git URL` 的方式新增庫，地址為：https://github.com/AlianBlank/com.gameframex.unity.operationclipboard.git
3. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。

### 使用範例

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

## 平台支援

| 平台 | 支援 |
|------|------|
| Windows | 是 |
| macOS | 是 |
| Android | 是 |
| iOS | 是 |

## 文檔與資源

- [文檔](https://gameframex.doc.alianblank.com)
- [GitHub 倉庫](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard)

## 社區與支援

- QQ群：透過 [二維碼](https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6) 加入

## 開源協議

本專案基於 [LICENSE](LICENSE) 文件中定義的條款授權。
