<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Operation Clipboard

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 项目简介

**Operation Clipboard 操作粘贴板** - Unity 跨平台剪贴板访问插件。提供统一的系统剪贴板读写操作：
- **Editor/Standalone**：TextEditor API
- **WebGL**：抖音小游戏 SDK / 微信小游戏 SDK
- **Android**：JNI 桥接
- **iOS**：P/Invoke

包含 `link.xml` 用于防止 IL2CPP 裁剪。

## 快速开始

### 安装

选择以下任一方式：

1. 编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
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
       "com.gameframex.unity.operationclipboard": "1.2.1"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.operationclipboard": "https://github.com/gameframex/com.gameframex.unity.operationclipboard.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为：`https://github.com/gameframex/com.gameframex.unity.operationclipboard.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。
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
| WebGL（抖音小游戏） | 是 |
| WebGL（微信小游戏） | 是 |

## 文档与资源

- [文档](https://gameframex.doc.alianblank.com)
- [GitHub 仓库](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard)

## 社区与支持

- QQ群：通过 [二维码](https://qm.qq.com/cgi-bin/qm/qr?k=ikT9gA5m2sKwOyNOfYmQvSAPK_c3GmD6) 加入


## 依赖

| 包 | 说明 |
|----|------|
| (无) | - |


## 更新日志

查看 [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.operationclipboard/releases) 了解更新日志。
## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。
