<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX OperationClipboard 粘貼板組件

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目錄導航

- [項目簡介](#項目簡介)
- [快速開始](#快速開始)
- [使用範例](#使用範例)
- [文檔與資源](#文檔與資源)
- [社區與支援](#社區與支援)
- [開源協議](#開源協議)

---

## 項目簡介

GameFrameX OperationClipboard 是一個 GameFrameX 框架的跨平台粘貼板組件，通過回調式契約與渠道註冊表，為 Unity 項目提供統一的粘貼板讀寫能力。

### 核心特性

- **回調契約** - `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`，全平台統一 API；同步平台在調用棧內立即回調
- **渠道註冊表** - `OperationClipboardRegistry` 註冊序互斥解析、`Register` 冪等（同 id 保留首個並告警）、`Reset` 掛 `SubsystemRegistration`、工廠懶實例化並快取
- **預設兜底** - 未註冊任何渠道時回落主包預設實現（Editor/Standalone `GUIUtility.systemCopyBuffer`、Android JNI、iOS P/Invoke）
- **主包零小遊戲 SDK 引用** - 主包不引用任何小遊戲 SDK 程式集；平台適配由獨立渠道包在啟動期自註冊承載（引用即註冊）

### 系統需求

- Unity 2019.4 或更高版本

---

## 快速開始

### 安裝

選擇以下任一方式：

1. 編輯 Unity 專案的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
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

   `scopes` 控制哪些包通過此註冊表解析。只有以 `com.gameframex` 開頭的包才會從這個註冊表獲取。

2. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {
      "com.gameframex.unity.operationclipboard": "https://github.com/gameframex/com.gameframex.unity.operationclipboard.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加庫，地址為： `https://github.com/gameframex/com.gameframex.unity.operationclipboard.git`
4. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動加載識別。

## 使用範例

### 基本使用

```csharp
using GameFrameX.OperationClipboard.Runtime;

// 寫入粘貼板
OperationClipboard.SetValue(text, success => { /* 可選結果 */ });

// 讀取粘貼板（同步平台在調用棧內立即回調）
OperationClipboard.GetValue((success, content) =>
{
    if (success)
    {
        // 使用 content
    }
});
```

### 渠道註冊（Bootstrap 自註冊）

渠道發現層基於註冊表：各渠道包通過自身的 `XxxOperationClipboardBootstrap`（`RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`）把渠道工廠註冊進 `OperationClipboardRegistry`；未安裝的渠道包代碼被裁剪，註冊不會發生（引用即註冊）。

- **互斥解析**：`OperationClipboardRegistry.ResolveManager()` 按註冊順序取首個渠道（小遊戲渠道由平台宏保證天然互斥）；
- **無配置面**：粘貼板 API 僅收文本，渠道無配置類；註冊項只攜帶渠道 id、展示名與 `Func<IOperationClipboardManager>` 工廠；
- **門面兜底**：靜態門面 `OperationClipboard` 優先解析註冊表，未註冊任何渠道時回落主包預設實現。

### 破壞性變更與遷移（v2.0.0）

- `BlankOperationClipboard.GetValue()` / `SetValue(string)` 移除，改為 `GameFrameX.OperationClipboard.Runtime.OperationClipboard.GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`；
- 程式集 `BlankOperationClipboard.Runtime` 更名為 `GameFrameX.OperationClipboard.Runtime`（命名空間對齊家族規範）；
- WebGL 小遊戲分支（`ENABLE_WECHAT_MINI_GAME` / `ENABLE_DOUYIN_MINI_GAME`）移出主包——安裝對應渠道包（`com.gameframex.unity.operationclipboard.wechatminigame` / `.alipayminigame` / `.kuaishouminigame` / `.douyinminigame`）與小遊戲 SDK 即可；
- 小遊戲剪貼板 API（wx / my / ks / tt）為非同步回調語義，v1 同步 `string GetValue()` 在 JS 橋應答前就返回、真機讀到空串；v2 全平台以回調交付結果。

### 主要類和介面

- **`IOperationClipboardManager`**：核心回調契約 `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`。
- **`OperationClipboardRegistry`**：渠道註冊表；`Register` / `Unregister` / `Contains` / `GetManager` / `ResolveManager()`（註冊序互斥解析）。
- **`OperationClipboardChannelRegistration`**：渠道註冊描述（渠道 id + 展示名 + `Func<IOperationClipboardManager>` 工廠）。
- **`DefaultOperationClipboardManager`**：主包預設實現（Editor/Standalone `GUIUtility.systemCopyBuffer`、Android JNI、iOS P/Invoke）；同步平台回調立即觸發。
- **`OperationClipboard`**：靜態門面；優先解析註冊表，未註冊時回落預設實現。

### 平台支援

| 平台 | 實現 |
|---|---|
| Editor / Standalone | `GUIUtility.systemCopyBuffer` |
| Android | JNI `com.alianhome.operationclipboard.MainActivity` |
| iOS | P/Invoke `BlankOperationClipboard.mm` |
| 微信 / 支付寶 / 快手 / 抖音小遊戲 | 渠道包 |

---

## 文檔與資源

- **完整文檔**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **問題反饋**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/issues)

---

## 社區與支援

- **QQ 討論群**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **功能建議**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/discussions)

---

## 依賴

| 包 | 說明 |
|----|------|
| com.gameframex.unity | GameFrameX 核心 |


## 更新日誌

查看 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases) 了解更新日誌。

## 開源協議

詳見 [LICENSE.md](LICENSE.md) 檔案。
