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
using NUnit.Framework;

namespace GameFrameX.OperationClipboard.Tests
{
    using GameFrameX.OperationClipboard.Runtime;
    // 门面类名与命名空间片段 GameFrameX.OperationClipboard 同名：文件级 using 会被外层命名空间遮蔽，
    // 必须在命名空间体内用别名导入（见 notes.md「门面命名遮蔽」）。
    using ClipboardFacade = GameFrameX.OperationClipboard.Runtime.OperationClipboard;

    /// <summary>
    /// 粘贴板渠道注册表语义用例。
    /// </summary>
    /// <remarks>
    /// 覆盖：懒实例化与缓存、重复注册保留首个、null 注册拒绝、Reset 后可重注册、
    /// 互斥解析按注册序取一、未注册时静态门面回落默认实现（Editor 下 systemCopyBuffer 同步回调）。
    /// </remarks>
    public class OperationClipboardRegistryTests
    {
        private class StubClipboardManager : IOperationClipboardManager
        {
            public int GetValueCallCount;
            public string LastSetValue;

            public void GetValue(Action<bool, string> callback)
            {
                GetValueCallCount++;
                callback?.Invoke(true, "stub-content");
            }

            public void SetValue(string text, Action<bool> callback = null)
            {
                LastSetValue = text;
                callback?.Invoke(true);
            }
        }

        private sealed class OtherClipboardManager : StubClipboardManager
        {
        }

        [SetUp]
        public void SetUp()
        {
            OperationClipboardRegistry.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            OperationClipboardRegistry.Reset();
        }

        [Test]
        public void Register_ThenGetManager_CreatesInstanceLazilyAndCaches()
        {
            var factoryCallCount = 0;
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "存根渠道", () =>
            {
                factoryCallCount++;
                return new StubClipboardManager();
            }));

            Assert.IsTrue(OperationClipboardRegistry.Contains("stub"));
            Assert.AreEqual(0, factoryCallCount, "工厂在首次获取前不应被调用");

            var first = OperationClipboardRegistry.GetManager("stub");
            var second = OperationClipboardRegistry.GetManager("stub");

            Assert.IsNotNull(first);
            Assert.IsInstanceOf<StubClipboardManager>(first);
            Assert.AreSame(first, second, "同一渠道应复用实例");
            Assert.AreEqual(1, factoryCallCount);
        }

        [Test]
        public void Register_DuplicateChannelId_KeepsFirstRegistration()
        {
            var first = new StubClipboardManager();
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "第一个", () => first));
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "第二个", () => new OtherClipboardManager()));

            Assert.AreSame(first, OperationClipboardRegistry.GetManager("stub"), "重复注册应保留首个");
        }

        [Test]
        public void Register_NullArguments_AreIgnoredOrThrown()
        {
            OperationClipboardRegistry.Register(null);
            Assert.IsFalse(OperationClipboardRegistry.Contains("stub"));

            Assert.Throws<ArgumentNullException>(() => new OperationClipboardChannelRegistration(null, "无效", () => new StubClipboardManager()));
            Assert.Throws<ArgumentNullException>(() => new OperationClipboardChannelRegistration("stub", "无效", null));
        }

        [Test]
        public void Unregister_RemovesChannelAndCachedManager()
        {
            var manager = new StubClipboardManager();
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "存根渠道", () => manager));
            Assert.AreSame(manager, OperationClipboardRegistry.GetManager("stub"));

            OperationClipboardRegistry.Unregister("stub");

            Assert.IsFalse(OperationClipboardRegistry.Contains("stub"));
            Assert.IsNull(OperationClipboardRegistry.GetManager("stub"));
        }

        [Test]
        public void Reset_ClearsRegistrations_AllowingReRegister()
        {
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "存根渠道", () => new StubClipboardManager()));
            OperationClipboardRegistry.Reset();

            Assert.IsFalse(OperationClipboardRegistry.Contains("stub"));
            Assert.IsNull(OperationClipboardRegistry.ResolveManager());

            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "存根渠道", () => new StubClipboardManager()));
            Assert.IsInstanceOf<StubClipboardManager>(OperationClipboardRegistry.GetManager("stub"), "重新注册后可再次获取");
        }

        [Test]
        public void ResolveManager_NoRegistration_ReturnsNull()
        {
            Assert.IsNull(OperationClipboardRegistry.ResolveManager());
        }

        [Test]
        public void ResolveManager_ResolvesFirstRegisteredChannel()
        {
            var first = new StubClipboardManager();
            var second = new OtherClipboardManager();
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("channel-a", "渠道A", () => first));
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("channel-b", "渠道B", () => second));

            Assert.AreSame(first, OperationClipboardRegistry.ResolveManager(), "互斥解析应取首个注册渠道");
            Assert.AreSame(second, OperationClipboardRegistry.GetManager("channel-b"), "指定渠道 id 仍可显式获取该渠道管理器");
        }

        [Test]
        public void Facade_WithoutChannel_FallsBackToDefaultImplementation()
        {
            var invoked = false;
            var success = false;
            var received = string.Empty;
            ClipboardFacade.SetValue("facade-default-probe");
            ClipboardFacade.GetValue((succeed, content) =>
            {
                invoked = true;
                success = succeed;
                received = content;
            });

            Assert.IsTrue(invoked, "默认实现应在调用栈内同步回调");
            Assert.IsTrue(success, "默认实现应回调成功");
            Assert.AreEqual("facade-default-probe", received, "Editor 下 systemCopyBuffer 应读写一致");
        }

        [Test]
        public void Facade_WithChannel_ForwardsToResolvedManager()
        {
            var stub = new StubClipboardManager();
            OperationClipboardRegistry.Register(new OperationClipboardChannelRegistration("stub", "存根渠道", () => stub));

            var received = string.Empty;
            ClipboardFacade.GetValue((success, content) => received = content);
            ClipboardFacade.SetValue("facade-channel-probe");

            Assert.AreEqual("stub-content", received, "门面应转发互斥解析结果");
            Assert.AreEqual("facade-channel-probe", stub.LastSetValue);
            Assert.AreEqual(1, stub.GetValueCallCount);
        }


    }
}
