using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffectCollector : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_BrokenEffectOriginal;

    const int m_Length = 8;
    public List<ParticleSystem> m_BrokenEffects;
    private Vector3 m_CenterOffset = new Vector3(-3.5f, 0, -3.5f);

    public void InitializeEffects()
    {
        int count = m_Length * m_Length;
        for (int i = 0; i < count; i++)
        {
            var row = i % m_Length;
            var column = i / m_Length;
            var position = new Vector3(row, 0, column) + m_CenterOffset;
            var effect = Instantiate(m_BrokenEffectOriginal, position, Quaternion.identity, transform);
            m_BrokenEffects.Add(effect);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("エフェクトプールの初期化")]
    public void InitializeEffectsInEditor()
    {
        InitializeEffects();
    }
#endif

    public void PlayEffect(Vector3 position)
    {
        var adjustedPosition = position - m_CenterOffset;
        var floatIndex = adjustedPosition.z * m_Length + adjustedPosition.x;
        int index = ((int)Mathf.Round(floatIndex));
        m_BrokenEffects[index].Play();
    }
}
