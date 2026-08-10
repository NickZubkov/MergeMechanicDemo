using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MergeMechanic.Editor
{
    public static class BuildScript
    {
        [MenuItem("MergeMechanic/Build WebGL")]
        public static void BuildWebGL()
        {
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/_Project/Scenes/Game.unity" },
                locationPathName = "Builds/WebGL",
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"WebGL build finished: {report.summary.result}, size {report.summary.totalSize} bytes");

            if (report.summary.result != BuildResult.Succeeded)
                EditorApplication.Exit(1);
        }
    }
}
