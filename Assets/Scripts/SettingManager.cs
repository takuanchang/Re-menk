using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private int m_HumanNum;
    private int m_ComputerNum;

    public int HumanNum
    {
        get => m_HumanNum;

        private set
        {
            m_HumanNum = value;
        }
    }

    public int ComputerNum
    {
        get => m_ComputerNum;

        private set
        {
            m_ComputerNum = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetGameMode((int human, int cpu) player)
    {
        m_HumanNum = player.human;
        m_ComputerNum = player.cpu;
    }
}
