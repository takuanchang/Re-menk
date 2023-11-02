using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePiece : MonoBehaviour
{
    private const float deleteHeight = -3.5f;
    void Update()
    {
        if(transform.position.y <= deleteHeight)
        {
            Debug.Log("delete");
            Destroy(this.gameObject);
        }
    }
}
