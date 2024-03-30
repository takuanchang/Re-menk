using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PiecesManager : MonoBehaviour
{
    [SerializeField]
    private TurnManager m_TurnManager;

    List<Piece> m_Pieces = new List<Piece>();
    [SerializeField]
    private Piece m_OriginalPiece;

    private static readonly Vector3 InitialPosition = new Vector3(3.5f, 5.0f, 3.5f);
    public Piece CreatePiece(Team team)
    {
        var position = InitialPosition;
        var piece = Instantiate(m_OriginalPiece, position, Quaternion.identity, transform);
        piece.Initialize(team, this);

        m_Pieces.Add(piece);
        return piece;  
    }

    public bool IsStableAll()
    {
        foreach (var piece in m_Pieces)
        {
            if (!piece.IsStable()) return false;
        }
        return true;
    }

    public void StopPiecesMove()
    {
        foreach (var piece in m_Pieces)
        {
            piece.Stop();
        }
    }

    public void RequestResetEndTurn()
    {
        m_TurnManager.ResetEndTurn();
    }

    // TODO : 重そうなので後で修正することを考える
    public List<int> CountPiecesNums()
    {
        int listSize = System.Enum.GetNames(typeof(Team)).Length - 1; // Noneがあるので一つ減らす
        List<int> piecesNums = new List<int>(listSize);
        for (int i = 0; i < listSize; i++)
        {
            piecesNums.Add(0);
        }

        foreach(Piece piece in m_Pieces)
        {
            Team team = piece.Team;
            if(team == Team.None)
            {
                continue;
            }

            piecesNums[((int)team)]++;
        }

        return piecesNums;
    }
}
