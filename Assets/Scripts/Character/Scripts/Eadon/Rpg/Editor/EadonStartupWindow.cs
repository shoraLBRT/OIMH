#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor
{
    /*
    [InitializeOnLoad]
    public static class ProjectOpenEvent
    {
        private static readonly string[] Symbols =
        {
            "EADON_RPG_INVECTOR"
        };

        static ProjectOpenEvent()
        {
            string defineString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = defineString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }
    }
    */

    public class EadonStartupWindow : EditorWindow
    {
        private bool _useSurvival;
        private bool _useMalbers;
        private bool _useEmerald;
        private bool _useRfx1;
        private bool _useRfx3;
        private bool _useRfx4;
        private bool _useAe;

        private const string SurvivalDefine = "EADON_USE_SURVIVAL";
        private const string MalbersDefine = "EADON_USE_MALBERS";
        private const string EmeraldDefine = "EADON_USE_EMERALD";
        private const string Rfx1Define = "EADON_USE_RFX1";
        private const string Rfx3Define = "EADON_USE_RFX3";
        private const string Rfx4Define = "EADON_USE_RFX4";
        private const string AeDefine = "EADON_USE_AE";
        private FontStyle _origFontStyle;

        [MenuItem("Invector/Eadon RPG/Settings", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<EadonStartupWindow>();
            window.titleContent = new GUIContent("Eadon Controller Settings");
            window.minSize = new Vector2(512, 600);
            window.maxSize = new Vector2(512, 600);

            window._useSurvival = ContainsSymbol(SurvivalDefine);
            window._useMalbers = ContainsSymbol(MalbersDefine);
            window._useEmerald = ContainsSymbol(EmeraldDefine);
            window._useRfx1 = ContainsSymbol(Rfx1Define);
            window._useRfx3 = ContainsSymbol(Rfx3Define);
            window._useRfx4 = ContainsSymbol(Rfx4Define);
            window._useAe = ContainsSymbol(AeDefine);

            window.Show();
        }

        private void OnGUI()
        {
            _origFontStyle = EditorStyles.label.fontStyle;

            var useSurvivalOld = ContainsSymbol(SurvivalDefine);
            var useMalbersOld = ContainsSymbol(MalbersDefine);
            var useEmeraldOld = ContainsSymbol(EmeraldDefine);
            var useRfx1Old = ContainsSymbol(Rfx1Define);
            var useRfx3Old = ContainsSymbol(Rfx3Define);
            var useRfx4Old = ContainsSymbol(Rfx4Define);
            var useAeOld = ContainsSymbol(AeDefine);

            var splashTexture = (Texture2D)Resources.Load("Textures/eadon_settings", typeof(Texture2D));
            GUILayoutUtility.GetRect(1f, 3f, GUILayout.ExpandWidth(false));
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100f));
            GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleToFit, true, 0f);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tick to enable integrations with assets imported in project",
                EditorStyles.boldLabel);

            EditorGUILayout.Space();

            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 400;

            _useSurvival = EditorGUILayout.Toggle("Enable Eadon Survival for Invector", useSurvivalOld);
            if (_useSurvival != useSurvivalOld)
            {
                SetupSymbol(SurvivalDefine, _useSurvival);
            }
            
            EditorGUILayout.Space();

            _useMalbers = EditorGUILayout.Toggle("Enable Malbers Animal Controller", useMalbersOld);
            if (_useMalbers != useMalbersOld)
            {
                SetupSymbol(MalbersDefine, _useMalbers);
            }

            _useEmerald = EditorGUILayout.Toggle("Enable Emerald AI", useEmeraldOld);
            if (_useEmerald != useEmeraldOld)
            {
                SetupSymbol(EmeraldDefine, _useEmerald);
            }

            EditorGUILayout.Space();

            _useRfx1 = EditorGUILayout.Toggle("Enable Kripto289 Realistic Effects Pack 1", useRfx1Old);
            if (_useRfx1 != useRfx1Old)
            {
                SetupSymbol(Rfx1Define, _useRfx1);
            }

            _useRfx3 = EditorGUILayout.Toggle("Enable Kripto289 Realistic Effects Pack 3", useRfx3Old);
            if (_useRfx3 != useRfx3Old)
            {
                SetupSymbol(Rfx3Define, _useRfx3);
            }

            _useRfx4 = EditorGUILayout.Toggle("Enable Kripto289 Realistic Effects Pack 4", useRfx4Old);
            if (_useRfx4 != useRfx4Old)
            {
                SetupSymbol(Rfx4Define, _useRfx4);
            }

            _useAe = EditorGUILayout.Toggle("Enable Kripto289 Archer Effects", useAeOld);
            if (_useAe != useAeOld)
            {
                SetupSymbol(AeDefine, _useAe);
            }

            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = oldWidth;
        }

        private static void SetupSymbol(string symbol, bool status)
        {
            if (status)
            {
                AddDefineSymbol(symbol);
            }
            else
            {
                DeleteDefineSymbol(symbol);
            }
        }

        private static void AddDefineSymbol(string symbol)
        {
            var symbols = new[] { symbol };
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        private static void DeleteDefineSymbol(string symbol)
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            allDefines.Remove(symbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        private static bool ContainsSymbol(string symbol)
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            return allDefines.Contains(symbol);
        }
    }
}
#endif
