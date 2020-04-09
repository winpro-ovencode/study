using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraResolution : MonoBehaviour
{
    public Text value;
    public SpriteRenderer renderer1;
    private int screenSizeX = 0;
    private int screenSizeY = 0;

    private void OnDrawGizmosSelected()
    {
        Vector3 center = renderer1.bounds.center;
        float radius = renderer1.bounds.extents.magnitude;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(center, radius);
    }
    // Start is called before the first frame update
    void Start()
    {
        float targetAspect = 9.0f / 16.0f; // 우리가 개발하는 해상도 비율
        float windowAspect = (float)Screen.width / (float)Screen.safeArea.height; // 디바이스에 해당하는 해상도 비율
        float scaleHeight = windowAspect / targetAspect;
        Camera camera = GetComponent<Camera>();
        

        value.text = string.Format("{0} : {1}", Screen.safeArea.width, Screen.safeArea.height);

        if(scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
            camera.orthographicSize = renderer1.bounds.size.x * Screen.safeArea.height / Screen.width * 0.5f;
        } else
        {
            float scaleWidth = 1.0f;
            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }

        screenSizeX = Screen.width;
        screenSizeY = Screen.height;
    }

    //빈 공간 출력 금지
    private void OnPreCull()
    {
        if (Application.isEditor)
            return;

        Rect wp = Camera.main.rect;
        Rect nr = new Rect(0, 0, 1, 1);

        Camera.main.rect = nr;
        GL.Clear(true, true, Color.black);
        Camera.main.rect = wp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
