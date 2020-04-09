using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyOrigin;
    public float delay = 3f;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > delay)
        {
            GameObject go = GameObject.Instantiate<GameObject>(enemyOrigin);
            go.gameObject.SetActive(true);
            go.transform.position = transform.position;
            timer = 0;
        }

        timer += Time.deltaTime;
    }
}
