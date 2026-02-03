using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [InitializeOnLoad]
    public static class TweenSequencerSetupHelper
    {
        private static string SCRIPTING_DEFINE_SYMBOL = "TWEENER_ENABLED";
        private static string TWEENER_ASSEMBLY_NAME = "THEBADDEST.Tweener";
        static TweenSequencerSetupHelper()
        {
            Assembly[] availableAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);

            bool foundTweener = false;
            for (int i = availableAssemblies.Length - 1; i >= 0; i--)
            {
                if (availableAssemblies[i].name.IndexOf(TWEENER_ASSEMBLY_NAME, StringComparison.Ordinal) > -1)
                {
                    foundTweener = true;
                    break;
                }
            }

            if (foundTweener)
            {
                AddScriptingDefineSymbol();
            }
            else
            {
                RemoveScriptingDefineSymbol();
                Debug.LogWarning("No Tweener found, animation sequencer will be disabled until Tweener setup is complete and asmdef files are created");
            }
        }

        private static void AddScriptingDefineSymbol()
        {
            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (scriptingDefineSymbols.Contains(SCRIPTING_DEFINE_SYMBOL))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                $"{scriptingDefineSymbols};{SCRIPTING_DEFINE_SYMBOL}");
            
            Debug.Log($"Adding {SCRIPTING_DEFINE_SYMBOL} for {EditorUserBuildSettings.selectedBuildTargetGroup}");
        }

        private static void RemoveScriptingDefineSymbol()
        {
            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptingDefineSymbols.Contains(SCRIPTING_DEFINE_SYMBOL))
                return;

            scriptingDefineSymbols = scriptingDefineSymbols.Replace(SCRIPTING_DEFINE_SYMBOL, string.Empty);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                scriptingDefineSymbols);
        }
    }
}
