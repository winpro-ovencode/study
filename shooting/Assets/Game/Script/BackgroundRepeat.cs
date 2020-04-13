using System.Collections.Generic;
using UnityEngine;

public class BackgroundRepeat : MonoBehaviour
{
    public float moveSpeed = 1f;

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
        foreach(var bg in mChild)
        {
            float moveY = moveSpeed * Time.deltaTime;
            bg.transform.Translate(0, -moveY, 0);
        }

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
