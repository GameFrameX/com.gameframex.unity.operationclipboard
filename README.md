<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX OperationClipboard Package

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Table of Contents

- [Project Overview](#project-overview)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
- [Documentation & Resources](#documentation--resources)
- [Community & Support](#community--support)
- [License](#license)

---

## Project Overview

GameFrameX OperationClipboard is a cross-platform clipboard component for the GameFrameX framework, providing unified clipboard read/write access in Unity projects through a callback-based contract and a channel registry.

### Key Features

- **Callback Contract** - `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`, one unified API across all platforms; synchronous platforms invoke the callback immediately on the calling stack
- **Channel Registry** - `OperationClipboardRegistry` with registration-order mutually exclusive resolution, idempotent `Register` (first wins with a warning), `Reset` at `SubsystemRegistration`, lazy factory instantiation with caching
- **Default Fallback** - falls back to the main package default implementation (Editor/Standalone `GUIUtility.systemCopyBuffer`, Android JNI, iOS P/Invoke) when no channel is registered
- **Zero Mini-Game SDK References** - the main package references no mini-game SDK assembly; platform adapters ship as separate channel packages that self-register at startup (referencing means registering)

### System Requirements

- Unity 2019.4 or higher

---

## Quick Start

### Installation

Choose one of the following methods:

1. Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:
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

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.operationclipboard": "https://github.com/gameframex/com.gameframex.unity.operationclipboard.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.operationclipboard.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.

## Usage Examples

### Basic Usage

```csharp
using GameFrameX.OperationClipboard.Runtime;

// Write to the clipboard
OperationClipboard.SetValue(text, success => { /* optional result */ });

// Read from the clipboard (synchronous platforms invoke the callback immediately)
OperationClipboard.GetValue((success, content) =>
{
    if (success)
    {
        // use content
    }
});
```

### Channel Registration (Bootstrap Self-Registration)

The discovery layer is registry-based: each channel package registers its channel factory into `OperationClipboardRegistry` via its own `XxxOperationClipboardBootstrap` (`RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`); uninstalled channel packages are stripped, so no registration happens (referencing means registering).

- **Mutually exclusive resolution**: `OperationClipboardRegistry.ResolveManager()` picks the first channel in registration order (mini-game channels are naturally mutually exclusive via platform macros);
- **No configuration surface**: clipboard APIs only take text, so channels have no config class; the registration entry carries the channel id, display name and `Func<IOperationClipboardManager>` factory;
- **Facade fallback**: the static `OperationClipboard` facade resolves the registry first and falls back to the default implementation when nothing is registered.

### Breaking Changes and Migration (v2.0.0)

- `BlankOperationClipboard.GetValue()` / `SetValue(string)` removed, replaced by `GameFrameX.OperationClipboard.Runtime.OperationClipboard.GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`;
- Assembly renamed from `BlankOperationClipboard.Runtime` to `GameFrameX.OperationClipboard.Runtime` (namespaces aligned to the family convention);
- The WebGL mini-game branches (`ENABLE_WECHAT_MINI_GAME` / `ENABLE_DOUYIN_MINI_GAME`) moved out of the main package - install the corresponding channel package (`com.gameframex.unity.operationclipboard.wechatminigame` / `.alipayminigame` / `.kuaishouminigame` / `.douyinminigame`) together with the mini-game SDK.
- Mini-game clipboard APIs (wx / my / ks / tt) are asynchronous callback APIs; the v1 synchronous `string GetValue()` returned an empty string on real devices before the JS bridge answered. v2 delivers results via callbacks on all platforms.

### Main Classes and Interfaces

- **`IOperationClipboardManager`**: core callback contract `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`.
- **`OperationClipboardRegistry`**: channel registry; `Register` / `Unregister` / `Contains` / `GetManager` / `ResolveManager()` (registration-order mutually exclusive resolution).
- **`OperationClipboardChannelRegistration`**: channel registration descriptor (channel id + display name + `Func<IOperationClipboardManager>` factory).
- **`DefaultOperationClipboardManager`**: main package default implementation (Editor/Standalone `GUIUtility.systemCopyBuffer`, Android JNI, iOS P/Invoke); synchronous platforms invoke callbacks immediately.
- **`OperationClipboard`**: static facade; resolves the registry first and falls back to the default implementation when nothing is registered.

### Platform Support

| Platform | Implementation |
|---|---|
| Editor / Standalone | `GUIUtility.systemCopyBuffer` |
| Android | JNI `com.alianhome.operationclipboard.MainActivity` |
| iOS | P/Invoke `BlankOperationClipboard.mm` |
| WeChat / Alipay / Kuaishou / Douyin Mini Games | channel packages |

---

## Documentation & Resources

- **Documentation**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **Issue Tracker**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/issues)

---

## Community & Support

- **QQ Group**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **Feature Requests**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/discussions)

---

## Dependencies

| Package | Description |
|----|------|
| com.gameframex.unity | GameFrameX core |


## Changelog

See [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases) for changelog.

## License

See [LICENSE.md](LICENSE.md) for license information.
