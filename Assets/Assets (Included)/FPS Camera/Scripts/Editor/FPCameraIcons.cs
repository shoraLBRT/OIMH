// SJM Tech
// www.sjmtech3d.com
//

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class FPCameraIcons
{

    static FPCameraIcons()
    {
        EditorApplication.hierarchyWindowItemOnGUI += FPCameraRoot;
    }

    static void FPCameraRoot(int instanceId, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        if (go == null) return;

        var fPCRoot = go.GetComponent<vFPCameraRoot>();
        if (fPCRoot != null) DrawIcon("FPCameraIcon", selectionRect);
        var fPCExtraC = go.GetComponent<vExtraCams>();
        if (fPCExtraC != null) DrawIcon("FPCameraExtraCIcon", selectionRect);
        var fPCHCollider = go.GetComponent<vFPCameraHeadCollider>();
        if (fPCHCollider != null) DrawIcon("FPCameraCollisionIcon", selectionRect);
        var fPCUI = go.GetComponent<vFPCameraUI>();
        if (fPCUI != null) DrawIcon("FPCameraUI", selectionRect);
    }


    private static void DrawIcon(string texName, Rect rect)
    {
        Rect r = new Rect(rect.x + rect.width - 16f, rect.y, 16f, 16f);
        GUI.DrawTexture(r, (Texture2D)Resources.Load(texName));
    }

}
