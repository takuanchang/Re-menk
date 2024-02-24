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
}
