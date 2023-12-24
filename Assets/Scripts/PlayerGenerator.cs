using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    SettingManager setting;

    [SerializeField] private HumanPlayer humanPrefab;
    [SerializeField] private ComputerPlayer computerPrefab;

    void Start()
    {
        setting = FindObjectOfType<SettingManager>();

        int human = setting.HumanNum;
        int cpu = setting.ComputerNum;


        for(int i = 0; i < human; i++)
        {
            HumanPlayer humanPlayer = Instantiate(humanPrefab);
        }
        for (int i = 0; i < cpu; i++)
        {
            ComputerPlayer computerPlayer = Instantiate(computerPrefab);
        }

        // TODO : このあとカメラを生成し、各Playerに結びつける
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
