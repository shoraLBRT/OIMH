using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invector.vItemManager;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor
{
    [InitializeOnLoad]
    public class EadonRpgFinalizeImport
    {
        private const string DefineSymbol = "EADON_RPG_INVECTOR";

#if UNITY_EDITOR
        private static GUIStyle _style;

        static EadonRpgFinalizeImport()
        {
#if UNITY_2018
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
#elif UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
#endif
        }
        public static void OnScene(SceneView sceneView)
        {
            CheckInstallation();
        }

        public static void CheckInstallation()
        {
            Handles.BeginGUI();

            var rect = new Rect();
            var invalid = !ContainsSymbol(DefineSymbol);
            GUILayout.Space(-20);
            
            if (invalid)
            {
                if (_style == null)
                {
                    _style = new GUIStyle(EditorStyles.whiteLabel);
                    _style.fontSize = 20;
                    _style.alignment = TextAnchor.MiddleCenter;
                    _style.fontStyle = FontStyle.Bold;
                    _style.wordWrap = true;
                    _style.clipping = TextClipping.Overflow;
                }
                rect.width = 400;
                rect.height = 200;
                var myString = "Eadon RPG for Invector\nPress the button to finish installation";
                GUILayout.BeginArea(rect);
                GUILayout.Box("", EditorStyles.textField, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                rect = GUILayoutUtility.GetLastRect();
                GUI.Label(rect, myString, _style);
                if (GUILayout.Button("Finish Installation"))
                {
                    vItemEnumsBuilder.RefreshItemEnums();
                    AddDefineSymbol(DefineSymbol);
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }

        private static void AddDefineSymbol(string symbol)
        {
            var symbols = new[] {symbol};
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(symbols.Except(allDefines));
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
#endif
}
