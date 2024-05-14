using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private int m_HumanNum;
    private int m_ComputerNum;
    private int m_TeamNum = 2;

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

    public int TeamNum
    {
        get => m_TeamNum;

        private set
        {
            m_TeamNum = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetGameMode((int human, int cpu, int team) set)
    {
        m_HumanNum = set.human;
        m_ComputerNum = set.cpu;
        m_TeamNum = set.team;
    }
}
