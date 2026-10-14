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

namespace GameFrameX.OperationClipboard.Runtime
{
    /// <summary>
    /// 粘贴板统一静态门面。
    /// </summary>
    /// <remarks>
    /// 优先转发 <see cref="OperationClipboardRegistry.ResolveManager"/> 的互斥解析结果（渠道包安装 + 小游戏 SDK 就绪时），
    /// 未注册任何渠道时回落主包默认实现 <see cref="DefaultOperationClipboardManager"/>。
    /// 粘贴板无状态、无 Update/Shutdown 生命周期需求，因此不做 GameFrameworkComponent / 模块槽位，
    /// 游戏侧直接调用本门面即可。
    /// Forwards to the registry-resolved channel manager and falls back to the default implementation when no channel is registered.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public static class OperationClipboard
    {
        private static IOperationClipboardManager s_DefaultManager;

        private static IOperationClipboardManager Manager
        {
            get
            {
                var manager = OperationClipboardRegistry.ResolveManager();
                if (manager != null)
                {
                    return manager;
                }

                return s_DefaultManager ??= new DefaultOperationClipboardManager();
            }
        }

        /// <summary>
        /// 读取粘贴板内容。
        /// </summary>
        /// <param name="callback">结果回调：成功标志与文本内容（失败或为空时内容为空字符串）；同步平台立即回调。</param>
        [UnityEngine.Scripting.Preserve]
        public static void GetValue(Action<bool, string> callback)
        {
            Manager.GetValue(callback);
        }

        /// <summary>
        /// 写入粘贴板内容。
        /// </summary>
        /// <param name="text">要写入的文本内容。</param>
        /// <param name="callback">可选结果回调；同步平台在调用栈内立即触发。</param>
        [UnityEngine.Scripting.Preserve]
        public static void SetValue(string text, Action<bool> callback = null)
        {
            Manager.SetValue(text, callback);
        }
    }
}
