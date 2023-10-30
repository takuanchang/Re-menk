using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMaterials : MonoBehaviour
{
    private float _speed = 5.0f;

    [SerializeField]
    private Transform m_origin_Piece;
    private Transform target;

    public void InitializePiece()
    {
        var position = new Vector3((float)3.5, 5, (float)3.5);
        target = Instantiate(m_origin_Piece, position, Quaternion.identity, transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePiece();
    }

    // Update is called once per frame
    void Update()
    {
        //x軸方向、z軸方向の入力を取得
        //Horizontal、水平、横方向のイメージ
        var _input_x = Input.GetAxisRaw("Horizontal");
        //Vertical、垂直、縦方向のイメージ
        var _input_z = Input.GetAxisRaw("Vertical");

        //移動の向きなど座標関連はVector3で扱う
        Vector3 velocity = new Vector3(_input_x, 0, _input_z);
        //ベクトルの向きを取得
        Vector3 direction = velocity.normalized;

        //移動距離を計算
        float distance = _speed * Time.deltaTime;
        //移動先を計算
        Vector3 destination = target.position + direction * distance;

        //移動先に向けて回転
        //transform.LookAt(destination);
        //移動先の座標を設定
        target.position = destination;
    }
}
