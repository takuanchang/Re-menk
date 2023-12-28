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
        int humanNum = setting.HumanNum;
        int cpuNum = setting.ComputerNum;

        GameObject piecesCollector = GameObject.Find("PiecesCollerctor");
        Transform board = GameObject.Find("Board").transform;

        Camera mainCameraOrigin = new Camera();
        mainCameraOrigin.tag = "MainCamera";
        mainCameraOrigin.cullingMask = (1 << 8 - 1); // プレイヤー系レイヤー以外をすべてONにする

        Cinemachine.CinemachineFreeLook freeLookOrigin = new Cinemachine.CinemachineFreeLook();
        freeLookOrigin.m_Orbits[0] = new Cinemachine.CinemachineFreeLook.Orbit(14, 5);
        freeLookOrigin.m_Orbits[1] = new Cinemachine.CinemachineFreeLook.Orbit(9, 9);
        freeLookOrigin.m_Orbits[2] = new Cinemachine.CinemachineFreeLook.Orbit(3, 10);
        freeLookOrigin.Follow = board;
        freeLookOrigin.LookAt = board;

        Cinemachine.CinemachineVirtualCamera pieceCameraOrigin = new Cinemachine.CinemachineVirtualCamera();

        int freeLookPriority = 8; // この実装おかしい。現状は1番はじめに生成されたプレイヤーが遊ぶようになっている。
                                  // 誰から開始か、どのタイミングで決める？

        for (int i = 0; i < humanNum; i++)
        {
            HumanPlayer humanPlayer = Instantiate(humanPrefab);

            Camera mainCamera =  Instantiate(mainCameraOrigin);
            mainCamera.cullingMask |= (1 << (8 + i));

            Cinemachine.CinemachineFreeLook freeLook = Instantiate(freeLookOrigin);
            freeLook.Priority = freeLookPriority;

            Cinemachine.CinemachineVirtualCamera pieceCamera = Instantiate(pieceCameraOrigin);

            humanPlayer.SetupCameras(mainCamera, freeLook, pieceCamera);

            freeLookPriority = 11;
        }

        for (int i = 0; i < cpuNum; i++)
        {
            ComputerPlayer computerPlayer = Instantiate(computerPrefab);

            Camera mainCamera = Instantiate(mainCameraOrigin);
            mainCamera.cullingMask |= (1 << (8 + humanNum + i));

            // これ本当はDolbyのカメラにした方がいい
            Cinemachine.CinemachineFreeLook freeLook = Instantiate(freeLookOrigin);
            freeLook.Priority = freeLookPriority;

            Cinemachine.CinemachineVirtualCamera pieceCamera = Instantiate(pieceCameraOrigin);

            computerPlayer.SetupCameras(mainCamera, freeLook, pieceCamera);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
