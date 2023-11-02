using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMaterials : MonoBehaviour
{
    private float _speed = 5.0f;

    [SerializeField]
    private GravityController m_OriginPiece;
    private GravityController m_Target;

    public void InitializePiece()
    {
        var position = new Vector3((float)3.5, 5, (float)3.5);
        m_Target = Instantiate(m_OriginPiece, position, Quaternion.identity, transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePiece();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Target.UseGravity();
            m_Target = null;

            return;
        }

        //x軸方向、z軸方向の入力を取得
        //Horizontal、水平、横方向のイメージ
        var _input_x = Input.GetAxisRaw("Horizontal");
        //Vertical、垂直、縦方向のイメージ
        var _input_z = Input.GetAxisRaw("Vertical");

        //移動の向きなど座標関連はVector3で扱う
        Vector3 velocity = new Vector3(_input_x, 0, _input_z);
        //ベクトルの向きを取得
        Vector3 direction = velocity.normalized;

        var transform = m_Target.transform;

        //移動距離を計算
        float distance = _speed * Time.deltaTime;

        //移動先に向けて回転
        //transform.LookAt(destination);
        //移動先の座標を設定
        transform.position += direction * distance;
    }
}
