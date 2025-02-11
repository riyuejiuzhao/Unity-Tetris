using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GrpcWindow : EditorWindow
{
    [MenuItem("Window/Grpc")]
    static void ShowWindow()
    {
        GetWindow<GrpcWindow>("Grpc窗口");
    }

    void Protoc()
    {
        var protoDir = Path.GetDirectoryName(protoFile);
        using (Process process = new Process())
        {
            // 配置ProcessStartInfo
            process.StartInfo = new ProcessStartInfo()
            {
                // 执行命令的程序（例如，cmd.exe）
                FileName = "protoc",
                // 要执行的命令
                Arguments = string.Join(" ", new string[] {
                    $"--proto_path={protoDir}",
                    $"--csharp-grpc_out={protoDir}",
                    $"--csharp_out={protoDir}",
                    $"{protoFile}"
                }),
                // 重定向输出和错误流
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                // 使用ShellExecute为false，以便重定向流
                UseShellExecute = false,
                // 不显示窗口
                CreateNoWindow = true,
                WorkingDirectory = Application.dataPath
            };
            // 启动进程
            process.Start();
            // 读取输出
            string error = process.StandardError.ReadToEnd();
            string output = process.StandardOutput.ReadToEnd();
            // 等待进程完成
            process.WaitForExit();
            if (error != null && error != "")
                Debug.LogError(error);
            Debug.Log(output);
        }
    }

    string protoKey = "协议文件路径";
    /// <summary>
    /// 相对与Application.dataPath的路径
    /// </summary>
    string protoFile;

    private void OnEnable()
    {
        protoFile = EditorPrefs.GetString(protoKey);
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label(protoKey);
        GUILayout.Label(protoFile);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("设置协议文件"))
        {
            var temp = EditorUtility.OpenFilePanel("选择协议文件", "", "");
            if(temp != "")
                protoFile = temp;
        }

        if (GUILayout.Button("重新生成"))
        {
            EditorPrefs.SetString(protoKey, protoFile);
            Protoc();
        }

        GUILayout.EndVertical();
    }
}

