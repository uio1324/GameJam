using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Editor.DataTableConverter;

public class Batchmode {
    static List<string> levels = new List<string>();
    private static string password = "autopixel";
    [MenuItem("Build/BuildAndroid")]
    public static void BuildAndroid()
    {
        DataTableConverter.ConvertDataTable();

        PlayerSettings.Android.keystorePass = password;
        PlayerSettings.Android.keyaliasPass = password;
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
            {
                continue;
            }
            levels.Add(scene.path);
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        var res = BuildPipeline.BuildPlayer(levels.ToArray(), "android.apk", BuildTarget.Android, BuildOptions.None);
        if (res.summary.totalErrors > 0)
        {
            throw new Exception();
        }
    }

    [MenuItem("Build/BuildiOS")]
    public static void BuildiOS()
    {
        DataTableConverter.ConvertDataTable();
        var privateScriptSymbols = Environment.GetEnvironmentVariable("ScriptingDefineSymbols");
        Debug.Log(privateScriptSymbols);
        privateScriptSymbols = privateScriptSymbols?.Replace('|', ';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, privateScriptSymbols);
        PlayerSettings.bundleVersion = "0.0.1";
        var ops = BuildOptions.None;

        foreach (var editorBuildSettingsScene in EditorBuildSettings.scenes)
        {
            if (!editorBuildSettingsScene.enabled)
            {
                continue;
            }
            levels.Add(editorBuildSettingsScene.path);
        }
        
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        BuildPipeline.BuildPlayer(levels.ToArray(), "iosProj", BuildTarget.iOS, ops);
    }
}
