using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Example : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    private void Update()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);
        }
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 36;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }

        GUILayout.BeginHorizontal(GUILayout.Width(1080));

        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z キー" : "X キー";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = joycon.GetStick();
            var gyro = joycon.GetGyro();
            var accel = joycon.GetAccel();
            var orientation = joycon.GetVector();

            //var initial_rot = Quaternion.LookRotation(Vector3.back, Vector3.up);
            //var rot = orientation * Quaternion.Inverse(initial_rot);
            var vec = accel;

            vec = Quaternion.Inverse(orientation) * vec;
            (vec.x, vec.y, vec.z) = (-vec.y, vec.z, -vec.x);
            vec = Quaternion.AngleAxis(180f, Vector3.right) * vec;
            //vec = Quaternion.Inverse(orientation) * vec;

            // UI表示
            {
                GUILayout.BeginVertical(GUILayout.Width(640));
                GUILayout.Label(name);
                //GUILayout.Label(key + "：振動");
                //GUILayout.Label("押されているボタン：" + button);
                //GUILayout.Label(string.Format("スティック：({0}, {1})", stick[0], stick[1]));
                //Debug.Log(MathF.Atan2(stick[1], stick[0]));
                //GUILayout.Label("ジャイロ向き：" + gyro.normalized);
                //GUILayout.Label("ジャイロ大きさ：" + gyro.magnitude);
                GUILayout.Label("加速度向き(local)：" + accel.normalized);
                GUILayout.Label("加速度向き(world)：" + vec.normalized);
                GUILayout.Label("加速度大きさ：" + accel.magnitude);
                GUILayout.Label("傾き：" + orientation.eulerAngles);
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndHorizontal();
    }
}
