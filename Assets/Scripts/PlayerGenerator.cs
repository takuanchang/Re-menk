using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerGenerator : MonoBehaviour
{
    SettingManager setting;

    [SerializeField] private HumanPlayer m_HumanPrefab;
    [SerializeField] private ComputerPlayer m_ComputerPrefab;
    [SerializeField] private Camera m_MainCameraPrefab;
    [SerializeField] private GameObject m_SelectCameraPrefab;
    [SerializeField] private GameObject m_PieceCameraPrefab;
    [SerializeField] private GameObject m_FreeLookCameraPrefab;
    [SerializeField] private GameObject m_DollyCameraPrefab;

    [SerializeField] private TurnManager m_TurnManager;
    [SerializeField] private UiPrinter m_UiPrinter;

    void Start()
    {
        setting = FindObjectOfType<SettingManager>();
        int humanNum = setting.HumanNum;
        int cpuNum = setting.ComputerNum;

        GameObject piecesCollector = GameObject.Find("PiecesCollerctor");
        Transform board = GameObject.Find("Board").transform;

        int freeLookPriority = 8; // この実装おかしい。現状は1番はじめに生成されたプレイヤーが遊ぶようになっている。
                                  // 誰から開始か、どのタイミングで決める？

        for (int i = 0; i < humanNum; i++)
        {
            var myLayer = LayerMask.NameToLayer($"Player{i + 1}");

            HumanPlayer humanPlayer = Instantiate(m_HumanPrefab);

            Camera mainCamera =  Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << (myLayer));

            GameObject freeLook = Instantiate(m_FreeLookCameraPrefab);
            freeLook.layer = myLayer;
            Cinemachine.CinemachineVirtualCameraBase freeLookBase = freeLook.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            freeLookBase.Priority = freeLookPriority;

            GameObject pieceCamera = Instantiate(m_PieceCameraPrefab);
            freeLook.layer = myLayer;
            Cinemachine.CinemachineVirtualCamera pieceCameraBase = pieceCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            Assert.IsNotNull(pieceCameraBase);
            pieceCameraBase.Follow = board;
            pieceCameraBase.LookAt = board;

            humanPlayer.SetupCameras(mainCamera, freeLookBase, pieceCameraBase);

            freeLookPriority = 11;
        }

        for (int i = humanNum; i < cpuNum + humanNum; i++)
        {
            var myLayer = LayerMask.NameToLayer($"Player{i + 1}");

            ComputerPlayer computerPlayer = Instantiate(m_ComputerPrefab);

            Camera mainCamera = Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << myLayer);

            // これ本当はDolbyのカメラにした方がいい
            GameObject freeLook = Instantiate(m_FreeLookCameraPrefab);
            freeLook.layer = myLayer;
            Cinemachine.CinemachineVirtualCameraBase freeLookBase = freeLook.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            freeLookBase.Priority = freeLookPriority;

            GameObject pieceCamera = Instantiate(m_PieceCameraPrefab);
            pieceCamera.layer = myLayer;
            Cinemachine.CinemachineVirtualCamera pieceCameraBase = pieceCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            pieceCameraBase.Follow = board;
            pieceCameraBase.LookAt = board;

            computerPlayer.SetupCameras(mainCamera, freeLookBase, pieceCameraBase);
        }

        m_TurnManager.SendMessage("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
