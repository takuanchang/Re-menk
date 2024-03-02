using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.AxisState;

public class JoyconInputProvider : MonoBehaviour, IInputAxisProvider
{
    private Joycon m_Joycon = null;
    public float GetAxisValue(int axis)
    {
        if (m_Joycon == null)
        {
            return 0;
        }

        var stick = m_Joycon.GetStick();
        switch (axis)
        {
            case 0:
                return stick[0];
            case 1:
                return stick[1];
            default:
                return 0;
        }
    }

    public void Initialize(Joycon joycon)
    {
        m_Joycon = joycon;
    }
}
