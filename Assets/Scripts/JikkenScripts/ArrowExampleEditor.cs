using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JoyconDemo))]
public class ArrowExampleEditor : Editor
{
    float size = 3f;

    protected virtual void OnSceneGUI()
    {
        var demo = (JoyconDemo)target;
        Transform transform = demo.transform;
        Handles.color = Handles.xAxisColor;
        //var vec = demo.accel;
        var vec = Vector3.left;
        vec = transform.TransformPoint(vec);
        Handles.ArrowHandleCap(
            0,
            transform.position,
            //Quaternion.
            //Quaternion.LookRotation(vec),
            Quaternion.FromToRotation(Vector3.forward, vec),
            //Quaternion.identity,
            //rot,
            size,
            EventType.Repaint
        );
    }
}
