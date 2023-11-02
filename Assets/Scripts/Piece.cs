using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    static float epsilon = 0.2f;
    public enum Status {Stand, Front, Back};
    private Rigidbody rb;

    // 今だけ
    [SerializeField]
    bool on;

    public Status GetStatus()
    {
        Vector3 up = transform.up.normalized;
        if (Mathf.Abs(up.y) < epsilon) return Status.Stand;
        else if (up.y > 0.0f) return Status.Front;
        else return Status.Back;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 1.56 && on)
        {
            GetColliders();
            on = false;
        }
    }


    // 本番はイベント実行時のみ呼ぶ
    void GetColliders()
    {
        Collider[] hitColliders = new Collider[1000];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 4.01f, hitColliders);
        for (int i = 0; i < numColliders; i++)
        {
            if(hitColliders[i].TryGetComponent<Piece>(out var p))
            {
                if (p == this)
                {
                    continue;
                }
                p.Futtobu(p.transform.position);
            }
            else if (hitColliders[i].TryGetComponent<Square>(out var m))
            {

            }
            else
            {
                continue;
            }
        }
    }

    // 今だけ
    public void Futtobu(Vector3 pos)
    {
        rb.AddForce(new Vector3(0.0f,5.0f, -2.0f), ForceMode.Impulse);
        rb.AddTorque(new Vector3(-6.0f, 0.0f, 0.0f), ForceMode.Impulse);
    }
}
