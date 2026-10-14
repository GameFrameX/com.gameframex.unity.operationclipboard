// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相关法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root of the source tree for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any disputes or liabilities arising from secondary development based on this project
//   本项目组织及贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:    https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace GameFrameX.OperationClipboard.Runtime
{
    /// <summary>
    /// 主包默认粘贴板实现（渠道包未注册时的兜底）。
    /// </summary>
    /// <remarks>
    /// 覆盖非小游戏平台：Editor / Standalone / WebGL（未装渠道包时）走 <c>GUIUtility.systemCopyBuffer</c>，
    /// Android 走 JNI（Plugins/Android 的 operationclipboard.jar），iOS 走 P/Invoke（Plugins/iOS 的 BlankOperationClipboard.mm）。
    /// 全部为同步平台，回调在调用栈内立即触发——这是回调契约的同步侧实现，调用方无需区分平台。
    /// Synchronous fallback for non-mini-game platforms; callbacks fire immediately on the calling stack.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class DefaultOperationClipboardManager : IOperationClipboardManager
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GetClipBoard();

        [DllImport("__Internal")]
        private static extern void SetClipBoard(string text);
#endif

        /// <summary>
        /// 读取粘贴板内容（同步平台，立即回调）。
        /// </summary>
        /// <param name="callback">结果回调：成功标志与文本内容。</param>
        [UnityEngine.Scripting.Preserve]
        public void GetValue(Action<bool, string> callback)
        {
            if (callback == null)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            using (var androidJavaClass = new UnityEngine.AndroidJavaClass("com.alianhome.operationclipboard.MainActivity"))
            {
                callback.Invoke(true, androidJavaClass.CallStatic<string>("GetClipBoard") ?? string.Empty);
                return;
            }
#elif UNITY_IOS && !UNITY_EDITOR
            callback.Invoke(true, GetClipBoard() ?? string.Empty);
#else
            callback.Invoke(true, UnityEngine.GUIUtility.systemCopyBuffer ?? string.Empty);
#endif
        }

        /// <summary>
        /// 写入粘贴板内容（同步平台，立即回调）。
        /// </summary>
        /// <param name="text">要写入的文本内容。</param>
        /// <param name="callback">可选结果回调，调用栈内立即触发。</param>
        [UnityEngine.Scripting.Preserve]
        public void SetValue(string text, Action<bool> callback = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var androidJavaClass = new UnityEngine.AndroidJavaClass("com.alianhome.operationclipboard.MainActivity"))
            {
                androidJavaClass.CallStatic("SetClipBoard", text);
            }
#elif UNITY_IOS && !UNITY_EDITOR
            SetClipBoard(text);
#else
            UnityEngine.GUIUtility.systemCopyBuffer = text ?? string.Empty;
#endif
            callback?.Invoke(true);
        }
    }
}
