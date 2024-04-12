using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// 粘贴板的读写
/// </summary>
public static class BlankOperationClipboard
{
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern string GetClipBoard ();
	
	[DllImport("__Internal")]
	private static extern void SetClipBoard (string text);
#endif


    /// <summary>
    /// 获取粘贴板的值
    /// </summary>
    /// <returns></returns>
    public static string GetValue()
    {
#if UNITY_ANDROID
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
    /// 设置粘贴板的值
    /// </summary>
    /// <param name="text"></param>
    public static void SetValue(string text)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        TextEditor textEditor = new TextEditor();
        textEditor.text = text;
        textEditor.OnFocus();
        textEditor.Copy();
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