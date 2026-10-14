<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX OperationClipboard 粘贴板组件

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

## 目录导航

- [项目简介](#项目简介)
- [快速开始](#快速开始)
- [使用示例](#使用示例)
- [文档与资源](#文档与资源)
- [社区与支持](#社区与支持)
- [开源协议](#开源协议)

---

## 项目简介

GameFrameX OperationClipboard 是一个 GameFrameX 框架的跨平台粘贴板组件，通过回调式契约与渠道注册表，为 Unity 项目提供统一的粘贴板读写能力。

### 核心特性

- **回调契约** - `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`，全平台统一 API；同步平台在调用栈内立即回调
- **渠道注册表** - `OperationClipboardRegistry` 注册序互斥解析、`Register` 幂等（同 id 保留首个并告警）、`Reset` 挂 `SubsystemRegistration`、工厂懒实例化并缓存
- **默认兜底** - 未注册任何渠道时回落主包默认实现（Editor/Standalone `GUIUtility.systemCopyBuffer`、Android JNI、iOS P/Invoke）
- **主包零小游戏 SDK 引用** - 主包不引用任何小游戏 SDK 程序集；平台适配由独立渠道包在启动期自注册承载（引用即注册）

### 系统要求

- Unity 2019.4 或更高版本

---

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
       "com.gameframex.unity.operationclipboard": "2.0.0"
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
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为： `https://github.com/gameframex/com.gameframex.unity.operationclipboard.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

## 使用示例

### 基本使用

```csharp
using GameFrameX.OperationClipboard.Runtime;

// 写入粘贴板
OperationClipboard.SetValue(text, success => { /* 可选结果 */ });

// 读取粘贴板（同步平台在调用栈内立即回调）
OperationClipboard.GetValue((success, content) =>
{
    if (success)
    {
        // 使用 content
    }
});
```

### 渠道注册（Bootstrap 自注册）

渠道发现层基于注册表：各渠道包通过自身的 `XxxOperationClipboardBootstrap`（`RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`）把渠道工厂注册进 `OperationClipboardRegistry`；未安装的渠道包代码被裁剪，注册不会发生（引用即注册）。

- **互斥解析**：`OperationClipboardRegistry.ResolveManager()` 按注册顺序取首个渠道（小游戏渠道由平台宏保证天然互斥）；
- **无配置面**：粘贴板 API 仅收文本，渠道无配置类；注册项只携带渠道 id、展示名与 `Func<IOperationClipboardManager>` 工厂；
- **门面兜底**：静态门面 `OperationClipboard` 优先解析注册表，未注册任何渠道时回落主包默认实现。

### 破坏性变更与迁移（v2.0.0）

- `BlankOperationClipboard.GetValue()` / `SetValue(string)` 移除，改为 `GameFrameX.OperationClipboard.Runtime.OperationClipboard.GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`；
- 程序集 `BlankOperationClipboard.Runtime` 更名为 `GameFrameX.OperationClipboard.Runtime`（命名空间对齐家族规范）；
- WebGL 小游戏分支（`ENABLE_WECHAT_MINI_GAME` / `ENABLE_DOUYIN_MINI_GAME`）移出主包——安装对应渠道包（`com.gameframex.unity.operationclipboard.wechatminigame` / `.alipayminigame` / `.kuaishouminigame` / `.douyinminigame`）与小游戏 SDK 即可；
- 小游戏剪贴板 API（wx / my / ks / tt）为异步回调语义，v1 同步 `string GetValue()` 在 JS 桥应答前就返回、真机读到空串；v2 全平台以回调交付结果。

### 主要类和接口

- **`IOperationClipboardManager`**：核心回调契约 `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`。
- **`OperationClipboardRegistry`**：渠道注册表；`Register` / `Unregister` / `Contains` / `GetManager` / `ResolveManager()`（注册序互斥解析）。
- **`OperationClipboardChannelRegistration`**：渠道注册描述（渠道 id + 展示名 + `Func<IOperationClipboardManager>` 工厂）。
- **`DefaultOperationClipboardManager`**：主包默认实现（Editor/Standalone `GUIUtility.systemCopyBuffer`、Android JNI、iOS P/Invoke）；同步平台回调立即触发。
- **`OperationClipboard`**：静态门面；优先解析注册表，未注册时回落默认实现。

### 平台支持

| 平台 | 实现 |
|---|---|
| Editor / Standalone | `GUIUtility.systemCopyBuffer` |
| Android | JNI `com.alianhome.operationclipboard.MainActivity` |
| iOS | P/Invoke `BlankOperationClipboard.mm` |
| 微信 / 支付宝 / 快手 / 抖音小游戏 | 渠道包 |

---

## 文档与资源

- **完整文档**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **问题反馈**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/issues)

---

## 社区与支持

- **QQ 讨论群**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **功能建议**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/discussions)

---

## 依赖

| 包 | 说明 |
|----|------|
| com.gameframex.unity | GameFrameX 核心 |


## 更新日志

查看 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases) 了解更新日志。

## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。
