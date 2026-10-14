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

using System.Collections.Generic;
using GameFrameX.Runtime;

namespace GameFrameX.OperationClipboard.Runtime
{
    /// <summary>
    /// 粘贴板渠道注册表。
    /// </summary>
    /// <remarks>
    /// 渠道包通过自身的 Bootstrap（RuntimeInitializeOnLoadMethod）在启动时显式注册，未被引用的渠道包代码会被宏墙裁剪，
    /// 天然满足"引用即注册"。
    /// 互斥解析语义：粘贴板为"每运行环境至多一个后端"——<see cref="ResolveManager"/> 按注册顺序取首个渠道；
    /// 小游戏渠道宏墙保证同一构建内只有一个渠道注册，无支付域那样的按调用路由需求，
    /// 因此与支付（参数路由）、统计（广播）、YooAsset（Priority 互斥）、广告（互斥 + 显式 id 覆盖）并列但不含显式路由参数。
    /// 注册表在 SubsystemRegistration 阶段自动清空（早于各渠道的 BeforeSceneLoad 注册），关闭 Domain Reload 时不会产生重复注册。
    /// 注册表实例化的渠道管理器不参与 GameFrameworkEntry 的模块生命周期；静态门面 <see cref="OperationClipboard"/>
    /// 未解析到渠道时回落主包默认实现。
    /// The registry resolves mutually exclusively (first registered wins); mini-game macro walls guarantee a single channel per build.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public static class OperationClipboardRegistry
    {
        private static readonly Dictionary<string, OperationClipboardChannelRegistration> Registrations = new Dictionary<string, OperationClipboardChannelRegistration>();
        private static readonly Dictionary<string, IOperationClipboardManager> Managers = new Dictionary<string, IOperationClipboardManager>();

        /// <summary>
        /// 重置注册表（Domain Reload 关闭时避免重复注册残留）。
        /// </summary>
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        [UnityEngine.Scripting.Preserve]
        public static void Reset()
        {
            Registrations.Clear();
            Managers.Clear();
        }

        /// <summary>
        /// 注册粘贴板渠道（同标识重复注册时保留首个并告警）。
        /// </summary>
        /// <param name="registration">渠道注册描述</param>
        [UnityEngine.Scripting.Preserve]
        public static void Register(OperationClipboardChannelRegistration registration)
        {
            if (registration == null)
            {
                return;
            }

            if (Registrations.ContainsKey(registration.ChannelId))
            {
                Log.Warning("Operation clipboard channel '{0}' is already registered. The duplicate registration is ignored.", registration.ChannelId);
                return;
            }

            Registrations.Add(registration.ChannelId, registration);
        }

        /// <summary>
        /// 注销粘贴板渠道
        /// </summary>
        /// <param name="channelId">渠道唯一标识</param>
        [UnityEngine.Scripting.Preserve]
        public static void Unregister(string channelId)
        {
            Registrations.Remove(channelId);
            Managers.Remove(channelId);
        }

        /// <summary>
        /// 渠道是否已注册
        /// </summary>
        /// <param name="channelId">渠道唯一标识</param>
        /// <returns>已注册返回true，否则返回false</returns>
        [UnityEngine.Scripting.Preserve]
        public static bool Contains(string channelId)
        {
            return !string.IsNullOrEmpty(channelId) && Registrations.ContainsKey(channelId);
        }

        /// <summary>
        /// 获取渠道管理器（首次获取时通过工厂实例化并缓存）
        /// </summary>
        /// <param name="channelId">渠道唯一标识</param>
        /// <returns>渠道管理器；未注册返回null</returns>
        [UnityEngine.Scripting.Preserve]
        public static IOperationClipboardManager GetManager(string channelId)
        {
            if (string.IsNullOrEmpty(channelId) || !Registrations.TryGetValue(channelId, out var registration))
            {
                return null;
            }

            if (!Managers.TryGetValue(channelId, out var manager))
            {
                manager = registration.Factory();
                Managers[channelId] = manager;
            }

            return manager;
        }

        /// <summary>
        /// 互斥解析当前运行环境的粘贴板管理器。
        /// </summary>
        /// <remarks>
        /// 按注册顺序取首个渠道（小游戏宏墙保证天然互斥）；未注册任何渠道时返回 null，
        /// 由 <see cref="OperationClipboard"/> 门面回落主包默认实现。
        /// Resolves the first registered channel; returns null when no channel package is installed.
        /// </remarks>
        /// <returns>渠道管理器；无注册渠道时为null</returns>
        [UnityEngine.Scripting.Preserve]
        public static IOperationClipboardManager ResolveManager()
        {
            foreach (var registration in Registrations.Values)
            {
                return GetManager(registration.ChannelId);
            }

            return null;
        }
    }
}
