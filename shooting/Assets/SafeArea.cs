using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeArea : MonoBehaviour
{
    public RectTransform Panel1;
    // Start is called before the first frame update
    void Start()
    {
        var r = Screen.safeArea;

        Debug.Log(r.position);
        Debug.Log(r.size);

        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;

        anchorMin.x /= Screen.width; 
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        Panel1.anchorMin = anchorMin;
        Panel1.anchorMax = anchorMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
