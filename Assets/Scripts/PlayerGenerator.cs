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

    /// <summary>
    /// ピースを管理しているゲームオブジェクト名
    /// </summary>
    const string PiecesManager = "PiecesManager";

    /// <summary>
    /// マスを管理しているゲームオブジェクト名
    /// </summary>
    const string BoardObjectName = "Board";

    /// <summary>
    /// 操作可能な時のfreelookカメラの設定
    /// </summary>
    const int PlayableFreeLookPriority = 9;
    /// <summary>
    /// 操作不可能な時のfreelookカメラの設定
    /// </summary>
    const int NonplayableFreeLookPriority = 11;

    /// <summary>
    /// 駒の総数
    /// </summary>
    const int AllPiecesNum = 4;

    /// <summary>
    /// チームの総数
    /// </summary>
    //public int m_TeamNum { get; private set; } = 2;

    public List<IPlayer> GeneratePlayers(int humanNum, int cpuNum, int teamNum)
    {

        var piecesManager = GameObject.Find(PiecesManager).GetComponent<PiecesManager>();
        var boardObject = GameObject.Find(BoardObjectName);
        var boardRoot = boardObject.transform;
        Board board = boardObject.GetComponent<Board>();

        var playersNum = humanNum + cpuNum;
        List<IPlayer> players = new List<IPlayer>();

        var individualPiecesNum = AllPiecesNum / playersNum;
        var remainPiecesNum = AllPiecesNum % playersNum;

        int cameraRowNum = 1, cameraColumnNum = 1; // カメラの配置数
        for(int i = 0; cameraColumnNum * cameraRowNum < playersNum; i++)
        {
            if (i % 2 == 0)
            {
                cameraRowNum++;
            }
            else
            {
                cameraColumnNum++;
            }
        }
        // 各カメラ表示の幅と高さ
        var width = 1f / cameraRowNum;
        var height = 1f / cameraColumnNum;

        for (int i = 0; i < humanNum; i++)
        {
            var myLayer = LayerMask.NameToLayer($"Player{i + 1}");

            HumanPlayer humanPlayer = Instantiate(m_HumanPrefab);
            humanPlayer.Initialize((Team)(i % teamNum), m_TurnManager, piecesManager, individualPiecesNum + (i < remainPiecesNum ? 1 : 0)); // TODO:3人以上の時Team等要修正
            players.Add(humanPlayer);

            Camera mainCamera = Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << (myLayer));
            mainCamera.rect = new Rect(width * (i % cameraRowNum), height * (i / cameraRowNum), width, height);

            GameObject selectCamera = Instantiate(m_SelectCameraPrefab);
            selectCamera.layer = myLayer;
            selectCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            Cinemachine.CinemachineVirtualCameraBase selectCameraBase = selectCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            selectCameraBase.Follow = boardRoot;
            selectCameraBase.LookAt = boardRoot;

            GameObject freeLook = Instantiate(m_FreeLookCameraPrefab);
            freeLook.layer = myLayer;
            Cinemachine.CinemachineVirtualCameraBase freeLookBase = freeLook.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            freeLookBase.Follow = boardRoot;
            freeLookBase.LookAt = boardRoot;

            GameObject pieceCamera = Instantiate(m_PieceCameraPrefab);
            pieceCamera.layer = myLayer;
            Cinemachine.CinemachineVirtualCamera pieceCameraBase = pieceCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            Assert.IsNotNull(pieceCameraBase);

            humanPlayer.SetupCameras(mainCamera, selectCameraBase, freeLookBase, pieceCameraBase);
        }

        CinemachineSmoothPath cinemachineSmoothPath = GameObject.Find("DollyTrack").GetComponent<CinemachineSmoothPath>();
        for (int i = humanNum; i < playersNum; i++)
        {
            var myLayer = LayerMask.NameToLayer($"Player{i + 1}");

            ComputerPlayer computerPlayer = Instantiate(m_ComputerPrefab);
            computerPlayer.Initialize((Team)(i % teamNum), m_TurnManager, piecesManager, individualPiecesNum + (i < remainPiecesNum ? 1 : 0)); // 3人以上の時Team等要修正
            computerPlayer.RegisterBoard(board);
            players.Add(computerPlayer);

            Camera mainCamera = Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << myLayer);
            mainCamera.rect = new Rect(width * (i % cameraRowNum), height * (i / cameraRowNum), width, height);

            GameObject selectCamera = Instantiate(m_SelectCameraPrefab);
            selectCamera.layer = myLayer;
            selectCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            Cinemachine.CinemachineVirtualCameraBase selectCameraBase = selectCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            selectCameraBase.Follow = boardRoot;
            selectCameraBase.LookAt = boardRoot;

            GameObject DollyCamera = Instantiate(m_DollyCameraPrefab);
            DollyCamera.layer = myLayer;
            DollyCamera.GetComponent<CinemachineDollyCart>().m_Path = cinemachineSmoothPath;
            Cinemachine.CinemachineVirtualCameraBase DollyCameraBase = DollyCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            DollyCameraBase.Follow = boardRoot;
            DollyCameraBase.LookAt = boardRoot;

            GameObject pieceCamera = Instantiate(m_PieceCameraPrefab);
            pieceCamera.layer = myLayer;
            Cinemachine.CinemachineVirtualCamera pieceCameraBase = pieceCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();

            computerPlayer.SetupCameras(mainCamera, selectCameraBase, DollyCameraBase, pieceCameraBase);
        }

        for (int i = playersNum; i < cameraRowNum * cameraColumnNum; i++)
        {
            var myLayer = LayerMask.NameToLayer("Excess");
            Debug.Log(myLayer);
            Camera mainCamera = Instantiate(m_MainCameraPrefab);
            mainCamera.cullingMask |= (1 << myLayer);
            mainCamera.rect = new Rect(width * (i % cameraRowNum), height * (i / cameraRowNum), width, height);

            GameObject DollyCamera = Instantiate(m_DollyCameraPrefab);
            DollyCamera.layer = myLayer;
            DollyCamera.GetComponent<CinemachineDollyCart>().m_Path = cinemachineSmoothPath;
            Cinemachine.CinemachineVirtualCameraBase DollyCameraBase = DollyCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            DollyCameraBase.Follow = boardRoot;
            DollyCameraBase.LookAt = boardRoot;
        }
        return players;
    }
    
}
