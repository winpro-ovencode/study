using System.Collections.Generic;
using UnityEngine;

public class BackgroundRepeat : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float distance;

    List<SpriteRenderer> mChild;
    float mHeight;
    

    // Start is called before the first frame update
    void Start()
    {
        mChild = new List<SpriteRenderer>();
        var list = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer item in list)
        {
            mChild.Add(item);
        }

        mHeight = mChild[1].transform.position.y - mChild[0].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float moveY = Time.deltaTime * moveSpeed;

        foreach(var bg in mChild)
        {
            bg.transform.Translate(0, -moveY, 0);
        }

        distance += moveY;

        foreach(var bg in mChild)
        {
            Vector3 targetScreenPos = Camera.main.WorldToViewportPoint(bg.transform.position);
            
            if(targetScreenPos.y < 0)
            {
                var pos = bg.transform.position;
                pos.y += mHeight * 3;
                bg.transform.position = pos;
            }
        }
    }
}
