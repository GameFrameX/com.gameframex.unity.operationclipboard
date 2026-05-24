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
  独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使
</p>

<p align="center">
  <a href="https://gameframex.doc.alianblank.com">文档</a> ·
  <a href="#快速开始">快速开始</a> ·
  <a href="https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6">QQ群</a> ·
  语言: <a href="README.md">English</a> | <strong>简体中文</strong> | <a href="README.zh-TW.md">繁體中文</a> | <a href="README.ja.md">日本語</a> | <a href="README.ko.md">한국어</a>
</p>

---

## 项目简介

**Operation Clipboard 操作粘贴板** - Unity 读写 Android 和 iOS 粘贴板的插件。包含 `link.xml` 用于防止 IL2CPP 裁剪。

## 快速开始

### 安装方式

任选其一：

1. 直接在 `manifest.json` 文件中添加以下内容
   ```json
   {"com.gameframex.unity.operationclipboard": "https://github.com/AlianBlank/com.gameframex.unity.operationclipboard.git"}
   ```
2. 在 Unity 的 `Packages Manager` 中使用 `Git URL` 的方式添加库，地址为：https://github.com/AlianBlank/com.gameframex.unity.operationclipboard.git
3. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

### 使用示例

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

## 平台支持

| 平台 | 支持 |
|------|------|
| Windows | 是 |
| macOS | 是 |
| Android | 是 |
| iOS | 是 |

## 文档与资源

- [文档](https://gameframex.doc.alianblank.com)
- [GitHub 仓库](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard)

## 社区与支持

- QQ群：通过 [二维码](https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6) 加入

## 开源协议

本项目基于 [LICENSE](LICENSE) 文件中定义的条款授权。
