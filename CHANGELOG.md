# [2.0.0](https://github.com/gameframex/com.gameframex.unity.operationclipboard/compare/1.2.2...2.0.0) (2026-10-15)


* feat(operationclipboard)!: 粘贴板拆渠道注册表，回调式 API 修复小游戏空返回 (v2.0.0) ([a731817](https://github.com/gameframex/com.gameframex.unity.operationclipboard/commit/a731817c7d016889bb50fa50630940450121c8b2))


### Bug Fixes

* **operationclipboard:** Preserve 改为全限定特性引用 ([9b0c3b7](https://github.com/gameframex/com.gameframex.unity.operationclipboard/commit/9b0c3b7c1d068e1e417e7f290882745bc8a7abe1))


### BREAKING CHANGES

* 全局 BlankOperationClipboard 改为 GameFrameX.OperationClipboard.Runtime.OperationClipboard 回调门面；asmdef 更名 GameFrameX.OperationClipboard.Runtime；小游戏适配拆至渠道包，主包零 SDK 依赖

- 新增 IOperationClipboardManager 回调契约、OperationClipboardRegistry 互斥解析注册表（注册序取一，无显式路由）、默认实现兜底（systemCopyBuffer/JNI/P-Invoke）与静态门面
- 删除 WeChatWASM/TTSDK 内联小游戏分支与 asmdef "Wx" 引用；Editor/Standalone 改 GUIUtility.systemCopyBuffer
- 命名空间/asmdef/license 头/CroppingHelper/link.xml/Tests 对齐家族；README ×5 按家族模板重写并附迁移说明；EditMode 9/9

# [2.0.0](https://github.com/gameframex/com.gameframex.unity.operationclipboard/compare/1.2.2...2.0.0) (2026-10-14)


### ⚠ BREAKING CHANGES

* 粘贴板 API 改为回调式契约（小游戏剪贴板 API 为异步回调，同步返回值在真机拿不到数据）；`BlankOperationClipboard.GetValue()/SetValue(string)` 移除，迁移到 `GameFrameX.OperationClipboard.Runtime.OperationClipboard.GetValue(Action<bool,string>)/SetValue(string, Action<bool>)`
* 命名空间与程序集对齐家族命名：`BlankOperationClipboard.Runtime` → `GameFrameX.OperationClipboard.Runtime`
* 小游戏适配（微信/支付宝/快手/抖音）移出主包，拆分为独立渠道包 `com.gameframex.unity.operationclipboard.wechatminigame` / `.alipayminigame` / `.kuaishouminigame` / `.douyinminigame`；主包零小游戏 SDK 依赖

### Features

* 新增 OperationClipboardRegistry 互斥解析注册表（注册序取一、重复注册保留首个并告警、SubsystemRegistration 自动清空、工厂懒实例化缓存）
* 新增 DefaultOperationClipboardManager 兜底（Editor/Standalone systemCopyBuffer、Android JNI、iOS P-Invoke，同步平台立即回调）
* 新增 OperationClipboard 静态门面：未注册渠道时回落默认实现，安装渠道包 + SDK 后自动转发渠道实现
* Editor/Standalone 由 TextEditor 旧法迁移到 GUIUtility.systemCopyBuffer；修复 link.xml 程序集名错误（BlankGetChannel.Runtime 残留）
