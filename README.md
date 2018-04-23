# BlankOperationClipboard

DEMO

```csharp

using UnityEngine;

public class BlankOperationClipboardDemo : MonoBehaviour
{
    private string text = "demoText";
    private string result = "";
    void OnGUI()
    {
        text = GUILayout.TextField(text, GUILayout.Width(500), GUILayout.Height(100));
        if (GUILayout.Button("SetValue", GUILayout.Width(500), GUILayout.Height(100)))
        {
            BlankOperationClipboard.SetValue(text);
        }
        if (GUILayout.Button("GetValue", GUILayout.Width(500), GUILayout.Height(100)))
        {
            result = BlankOperationClipboard.GetValue();
        }
        GUILayout.Label(result);
    }
}

```

#  Important Tips (Android)
##  需要添加下面的Activity：

```xml

 <activity android:name="com.alianhome.clipboardoperation.MainActivity"/> 
 ```

* Email: wangfj11@foxmail.com
* by Blank
* by blankOperationClipboard.alianhome.com (waiting...)