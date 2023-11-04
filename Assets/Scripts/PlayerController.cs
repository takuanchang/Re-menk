using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private bool isBlack = true;

    /// <summary>
    /// このプレイヤーが操作可能かどうか
    /// </summary>
    public bool IsPlayable { get; set; }

    private int m_SquareLayerMask;

    private int m_RemainingPieces = 32;

    [SerializeField]
    private Piece m_OriginPiece;
    private Piece m_Target;

    public void PrepareNextPiece()
    {
        if(m_RemainingPieces <= 0) {
            return;
        }
        m_RemainingPieces--;

        var position = new Vector3(3.5f, 5.0f, 3.5f);
        if (isBlack)
        {
            m_Target = Instantiate(m_OriginPiece, position, Quaternion.identity, transform);
        }
        else
        {
            m_Target = Instantiate(m_OriginPiece, position, Quaternion.Euler(180, 0, 0), transform);
        }

        IsPlayable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_SquareLayerMask = LayerMask.GetMask("Square");
        PrepareNextPiece();
    }

    private bool isDropping = false;
    float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        //要調整
        if (isDropping)
        {
            timer += Time.deltaTime;
            if (timer >= 2.0f)
            {
                PrepareNextPiece();
                isDropping = false;
                timer = 0.0f;
            }
            return;
        }
        //要調整
        if (!IsPlayable)
        {
            return;
        }

        // 選択したマスに落とす
        if (Input.GetMouseButtonDown(0))
        {
            // マウスからレイを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hit, 10.0f, m_SquareLayerMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 pos = hit.collider.transform.position;
                pos.y = 5.0f;
                Drop(pos);
            }
        }
    }

    public void Drop(Vector3 pos)
    {
        if (m_Target == null)
        {
            Debug.Log("Error");
            return;
        }
        m_Target.transform.position = pos;
        m_Target.UseGravity();
        m_Target = null;
        isDropping = true;
        return;
    }

    public void SetBlack()
    {
        isBlack = true;
    }
    public void SetWhite()
    {
        isBlack = false;
    }
}
