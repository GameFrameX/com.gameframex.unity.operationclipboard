using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// 粘贴板的读写操作工具类。
/// </summary>
/// <remarks>
/// Provides clipboard read and write operations across different platforms.
/// Supports Editor, Standalone, WebGL (Douyin Mini Game), Android, and iOS platforms.
/// </remarks>
public static class BlankOperationClipboard
{
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern string GetClipBoard ();
	
	[DllImport("__Internal")]
	private static extern void SetClipBoard (string text);
#endif


    /// <summary>
    /// 获取粘贴板的值。
    /// </summary>
    /// <remarks>
    /// Gets the current value from the system clipboard.
    /// </remarks>
    /// <returns>粘贴板中的文本内容；如果为空则返回空字符串 / The text content from clipboard; returns empty string if empty</returns>
    public static string GetValue()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        TextEditor textEditor = new TextEditor();
        textEditor.OnFocus();
        textEditor.Paste();
        var content = textEditor.text;
        return content;
#elif UNITY_WEBGL
#if ENABLE_DOUYIN_MINI_GAME
        string content = string.Empty;
        TTSDK.TT.GetClipboardData((b, text) =>
        {
            if (b)
            {
                content = text;
            }
        });
        return content;
#endif
#elif UNITY_ANDROID
        using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.alianhome.operationclipboard.MainActivity"))
        {
            return androidJavaClass.CallStatic<string>("GetClipBoard");
        }
#elif UNITY_IOS
	    return GetClipBoard ();
#else
        return UnityEngine.GUIUtility.systemCopyBuffer ?? string.Empty;
#endif
    }

    /// <summary>
    /// 设置粘贴板的值。
    /// </summary>
    /// <remarks>
    /// Sets the specified text to the system clipboard.
    /// </remarks>
    /// <param name="text">要设置到粘贴板的文本内容 / Text content to set to clipboard</param>
    public static void SetValue(string text)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        TextEditor textEditor = new TextEditor();
        textEditor.text = text;
        textEditor.OnFocus();
        textEditor.Copy();
#elif UNITY_WEBGL
#if ENABLE_DOUYIN_MINI_GAME
        TTSDK.TT.SetClipboardData(text);
#endif
#elif UNITY_ANDROID
        using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.alianhome.operationclipboard.MainActivity"))
        {
             androidJavaClass.CallStatic("SetClipBoard", text);
        }
#elif UNITY_IOS
		SetClipBoard (text);
#else
        UnityEngine.GUIUtility.systemCopyBuffer = text;
#endif
    }
}