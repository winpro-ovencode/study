using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGroup : MonoBehaviour
{
    public float moveSpeed = 1.3f;
    public GameObject enemyTemplete;
    // Start is called before the first frame update
    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i); //slot
            var go = GameObject.Instantiate<GameObject>(enemyTemplete);
            go.SetActive(true);
            go.transform.SetParent(child);
            go.transform.localPosition = Vector3.zero;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        float moveY = moveSpeed * Time.deltaTime;
        transform.Translate(0, -moveY, 0);

        var viewPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPos.y < 0f)
            Destroy(gameObject);
    }
}