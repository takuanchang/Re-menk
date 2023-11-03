using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    private bool isYourTurn = true;
    [SerializeField]
    private bool isBlack = true;
    int remain_pieces;

    [SerializeField]
    private Piece m_OriginPiece;
    private Piece m_Target;

    public void InitializePiece()
    {
        var position = new Vector3((float)3.5, 5, (float)3.5);
        if (isBlack)
        {
            m_Target = Instantiate(m_OriginPiece, position, Quaternion.identity, transform);
        }
        else
        {
            m_Target = Instantiate(m_OriginPiece, position, Quaternion.Euler(180, 0, 0), transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePiece();
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
                InitializePiece();
                isDropping = false;
                timer = 0.0f;
            }
            return;
        }
        //要調整
        if (!isYourTurn)
        {
            return;
        }

        // 選択したマスに落とす
        if (Input.GetMouseButtonDown(0))
        {
            // マウスからレイを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layer_mask = 1 << 7;
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10.0f, layer_mask, QueryTriggerInteraction.Ignore))
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

    public void SetTurnTrue()
    {
        isYourTurn = true;
    }
    public void SetTurnFalse()
    {
        isYourTurn = false;
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
