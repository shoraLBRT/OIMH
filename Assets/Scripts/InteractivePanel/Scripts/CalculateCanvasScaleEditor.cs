/**************************************************************************/
/** 	© 2016 NULLcode Studio. License: CC 0.
/** 	Разработано специально для http://null-code.ru/
/** 	WebMoney: R209469863836, Z126797238132, E274925448496, U157628274347
/** 	Яндекс.Деньги: 410011769316504
/**************************************************************************/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CalculateCanvasScale))]

public class CalculateCanvasScaleEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		CalculateCanvasScale t = (CalculateCanvasScale)target;
		GUILayout.Label("Расчет масштаба холста:", EditorStyles.boldLabel);
		if(GUILayout.Button("Calculate")) t.Calculate();
	}
}
#endif