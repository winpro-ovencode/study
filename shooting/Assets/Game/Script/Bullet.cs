using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveY = moveSpeed * Time.deltaTime;
        transform.Translate(0, moveY, 0);

        var viewPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPos.y > 1.1f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Enemy")
            return;

        Destroy(gameObject);
    }
}
