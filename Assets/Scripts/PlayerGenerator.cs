using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

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

    [SerializeField] private GameObject m_TurnManager;
    [SerializeField] private UiPrinter m_UiPrinter;

    public List<IPlayer> GeneratePlayers(int humanNum, int cpuNum)
    {

        var piecesManager = GameObject.Find("PiecesManager").GetComponent<PiecesManager>();
        Transform board = GameObject.Find("Board").transform;

        var playersNum = humanNum + cpuNum;
        List<IPlayer> players = new List<IPlayer>();

        int freeLookPriority = 8; // この実装おかしい。現状は1番はじめに生成されたプレイヤーが遊ぶようになっている。
                                  // 誰から開始か、どのタイミングで決める？

        for (int i = 0; i < humanNum; i++)
        {
            var myLayer = LayerMask.NameToLayer($"Player{i + 1}");

            HumanPlayer humanPlayer = Instantiate(m_HumanPrefab);
            humanPlayer.Initialize((Team)(i % 2), m_TurnManager, piecesManager); // TODO:3人以上の時Team等要修正
            players.Add(humanPlayer);

            Camera mainCamera = Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << (myLayer));
            mainCamera.rect = new Rect(0.5f * (i % 2), 0.5f * (i / 2), 0.5f, (playersNum >= 3 ? 0.5f : 1.0f));

            GameObject selectCamera = Instantiate(m_SelectCameraPrefab);
            selectCamera.layer = myLayer;
            selectCamera.transform.rotation = Quaternion.Euler(90, 180 * (i % 2), 0); // TODO:3人以上の時要修正

            GameObject freeLook = Instantiate(m_FreeLookCameraPrefab);
            freeLook.layer = myLayer;
            Cinemachine.CinemachineVirtualCameraBase freeLookBase = freeLook.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            freeLookBase.Priority = freeLookPriority;
            freeLookBase.Follow = board;
            freeLookBase.LookAt = board;

            GameObject pieceCamera = Instantiate(m_PieceCameraPrefab);
            pieceCamera.layer = myLayer;
            Cinemachine.CinemachineVirtualCamera pieceCameraBase = pieceCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            Assert.IsNotNull(pieceCameraBase);

            humanPlayer.SetupCameras(mainCamera, freeLookBase, pieceCameraBase);

            freeLookPriority = 11;
        }

        CinemachineSmoothPath cinemachineSmoothPath = GameObject.Find("DollyTrack").GetComponent<CinemachineSmoothPath>();
        for (int i = humanNum; i < playersNum; i++)
        {
            var myLayer = LayerMask.NameToLayer($"Player{i + 1}");

            ComputerPlayer computerPlayer = Instantiate(m_ComputerPrefab);
            computerPlayer.Initialize((Team)(i % 2), m_TurnManager, piecesManager); // 3人以上の時Team等要修正
            players.Add(computerPlayer);

            Camera mainCamera = Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << myLayer);
            mainCamera.rect = new Rect(0.5f * (i % 2), 0.5f * (i / 2), 0.5f, (playersNum >= 3 ? 0.5f : 1.0f));

            GameObject selectCamera = Instantiate(m_SelectCameraPrefab);
            selectCamera.layer = myLayer;
            selectCamera.transform.rotation = Quaternion.Euler(90, 180 * (i % 2), 0); // TODO:3人以上の時要修正

            GameObject DollyCamera = Instantiate(m_DollyCameraPrefab);
            DollyCamera.layer = myLayer;
            DollyCamera.GetComponent<CinemachineDollyCart>().m_Path = cinemachineSmoothPath;
            Cinemachine.CinemachineVirtualCameraBase DollyCameraBase = DollyCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            DollyCameraBase.Priority = freeLookPriority;
            DollyCameraBase.Follow = board;
            DollyCameraBase.LookAt = board;

            GameObject pieceCamera = Instantiate(m_PieceCameraPrefab);
            pieceCamera.layer = myLayer;
            Cinemachine.CinemachineVirtualCamera pieceCameraBase = pieceCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();

            computerPlayer.SetupCameras(mainCamera, DollyCameraBase, pieceCameraBase);
        }
        return players;
    }
    
}
