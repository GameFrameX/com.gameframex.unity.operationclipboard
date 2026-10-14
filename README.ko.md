<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX OperationClipboard 패키지

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationclipboard)](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 목차

- [프로젝트 개요](#프로젝트-개요)
- [빠른 시작](#빠른-시작)
- [사용 예시](#사용-예시)
- [문서 및 자료](#문서-및-자료)
- [커뮤니티 및 지원](#커뮤니티-및-지원)
- [라이선스](#라이선스)

---

## 프로젝트 개요

GameFrameX OperationClipboard는 GameFrameX 프레임워크의 크로스 플랫폼 클립보드 컴포넌트입니다. 콜백 기반 계약과 채널 레지스트리를 통해 Unity 프로젝트에 통일된 클립보드 읽기/쓰기를 제공합니다.

### 주요 기능

- **콜백 계약** - `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`, 모든 플랫폼에서 통일된 API. 동기 플랫폼은 호출 스택 내에서 즉시 콜백
- **채널 레지스트리** - `OperationClipboardRegistry`는 등록 순 상호 배타적 해석, 멱등한 `Register`(같은 id는 선등록 우선 + 경고), `SubsystemRegistration` 시점의 `Reset`, 팩토리 지연 인스턴스화 및 캐싱
- **기본 폴백** - 등록된 채널이 없으면 메인 패키지 기본 구현(Editor/Standalone `GUIUtility.systemCopyBuffer`, Android JNI, iOS P/Invoke)으로 폴백
- **미니게임 SDK 참조 없음** - 메인 패키지는 미니게임 SDK 어셈블리를 참조하지 않습니다. 플랫폼 적응은 독립 채널 패키지가 시작 시 자가 등록합니다(참조 곧 등록)

### 시스템 요구사항

- Unity 2019.4 이상

---

## 빠른 시작

### 설치

다음 방법 중 하나를 선택하세요:

1. Unity 프로젝트의 `Packages/manifest.json`을 편집하여 `scopedRegistries` 섹션을 추가하세요:
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

   `scopes`는 이 레지스트리를 통해 해석되는 패키지를 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 다음을 직접 추가하세요:
   ```json
   {
      "com.gameframex.unity.operationclipboard": "https://github.com/gameframex/com.gameframex.unity.operationclipboard.git"
   }
   ```
3. Unity의 `Package Manager`에서 **Git URL**로 추가하세요. 주소: `https://github.com/gameframex/com.gameframex.unity.operationclipboard.git`
4. 저장소를 Unity 프로젝트의 `Packages` 디렉토리에 복사하면 자동으로 로드됩니다.

## 사용 예시

### 기본 사용법

```csharp
using GameFrameX.OperationClipboard.Runtime;

// 클립보드에 쓰기
OperationClipboard.SetValue(text, success => { /* 선택적 결과 */ });

// 클립보드에서 읽기 (동기 플랫폼은 즉시 콜백)
OperationClipboard.GetValue((success, content) =>
{
    if (success)
    {
        // content 사용
    }
});
```

### 채널 등록 (Bootstrap 자가 등록)

채널 발견 계층은 레지스트리 기반입니다. 각 채널 패키지는 자체 `XxxOperationClipboardBootstrap`(`RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`)으로 채널 팩토리를 `OperationClipboardRegistry`에 등록합니다. 설치되지 않은 채널 패키지의 코드는 제거되어 등록이 일어나지 않습니다(참조 곧 등록).

- **상호 배타적 해석**: `OperationClipboardRegistry.ResolveManager()`는 등록 순서대로 첫 채널을 선택합니다(미니게임 채널은 플랫폼 매크로로 자연스럽게 상호 배타적);
- **설정 영역 없음**: 클립보드 API는 텍스트만 받으므로 채널에 설정 클래스가 없습니다. 등록 항목은 채널 id, 표시 이름, `Func<IOperationClipboardManager>` 팩토리만 포함;
- **퍼사드 폴백**: 정적 퍼사드 `OperationClipboard`는 레지스트리 해석을 우선하고, 등록이 없으면 기본 구현으로 폴백합니다.

### 파괴적 변경 및 마이그레이션 (v2.0.0)

- `BlankOperationClipboard.GetValue()` / `SetValue(string)`가 제거되고 `GameFrameX.OperationClipboard.Runtime.OperationClipboard.GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`로 대체되었습니다;
- 어셈블리 이름이 `BlankOperationClipboard.Runtime`에서 `GameFrameX.OperationClipboard.Runtime`으로 변경되었습니다(네임스페이스가 패밀리 규약에 정렬);
- WebGL 미니게임 분기(`ENABLE_WECHAT_MINI_GAME` / `ENABLE_DOUYIN_MINI_GAME`)가 메인 패키지에서 이전되었습니다. 해당 채널 패키지(`com.gameframex.unity.operationclipboard.wechatminigame` / `.alipayminigame` / `.kuaishouminigame` / `.douyinminigame`)와 미니게임 SDK를 설치하세요;
- 미니게임 클립보드 API(wx / my / ks / tt)는 비동기 콜백 API입니다. v1의 동기 `string GetValue()`는 JS 브리지 응답 전에 빈 문자열을 반환했습니다. v2는 모든 플랫폼에서 콜백으로 결과를 전달합니다.

### 주요 클래스와 인터페이스

- **`IOperationClipboardManager`**: 핵심 콜백 계약 `GetValue(Action<bool,string>)` / `SetValue(string, Action<bool>)`.
- **`OperationClipboardRegistry`**: 채널 레지스트리. `Register` / `Unregister` / `Contains` / `GetManager` / `ResolveManager()`(등록 순 상호 배타적 해석).
- **`OperationClipboardChannelRegistration`**: 채널 등록 설명자(채널 id + 표시 이름 + `Func<IOperationClipboardManager>` 팩토리).
- **`DefaultOperationClipboardManager`**: 메인 패키지 기본 구현(Editor/Standalone `GUIUtility.systemCopyBuffer`, Android JNI, iOS P/Invoke). 동기 플랫폼은 즉시 콜백.
- **`OperationClipboard`**: 정적 퍼사드. 레지스트리 해석 우선, 미등록 시 기본 구현으로 폴백.

### 플랫폼 지원

| 플랫폼 | 구현 |
|---|---|
| Editor / Standalone | `GUIUtility.systemCopyBuffer` |
| Android | JNI `com.alianhome.operationclipboard.MainActivity` |
| iOS | P/Invoke `BlankOperationClipboard.mm` |
| WeChat / Alipay / Kuaishou / Douyin 미니게임 | 채널 패키지 |

---

## 문서 및 자료

- **문서**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **이슈 트래커**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/issues)

---

## 커뮤니티 및 지원

- **QQ 그룹**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **기능 요청**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/discussions)

---

## 의존성

| 패키지 | 설명 |
|----|------|
| com.gameframex.unity | GameFrameX 코어 |


## 변경 로그

변경 로그는 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationclipboard/releases)를 참조하세요.

## 라이선스

자세한 내용은 [LICENSE.md](LICENSE.md) 파일을 참조하세요.
