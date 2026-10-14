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


