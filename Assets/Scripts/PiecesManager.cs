using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PiecesManager : MonoBehaviour
{
    List<Piece> m_Pieces = new List<Piece>();
    [SerializeField]
    private Piece m_OriginalPiece;
    public Piece CreatePiece(Team team)
    {
        var position = new Vector3(3.5f, 5.0f, 3.5f);
        var piece = Instantiate(m_OriginalPiece, position, Quaternion.identity, transform);
        piece.Initialize(team);

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
}
