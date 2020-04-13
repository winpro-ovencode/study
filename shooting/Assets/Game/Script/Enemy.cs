using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject Bomb;
    public Item CoinItem;
    public Item MagnetItem;
    public Item PowerItem;
    public Item GemItem;
    private PlayerController mPlayer;
    int HP = 2;

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        mPlayer = go.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "PlayerBody" && collision.tag != "Bullet")
            return;

        HP = HP - 1;

        if(HP <= 0)
        {
            var go = GameObject.Instantiate<GameObject>(Bomb);
            go.transform.position = transform.position;
            go.SetActive(true);
            Destroy(go, 2.0f);

            CreateRandomItem();
            Destroy(gameObject);

            mPlayer.AddScore(1);
        }
    }

    private void CreateRandomItem()
    {
        int value = UnityEngine.Random.Range(0, 10);

        Item go;

        if (value == 0)
        {
            go = GameObject.Instantiate<Item>(GemItem);
            go.SetItem(ItemType.Gem, 0);
            
        } else if(value == 1)
        {
            go = GameObject.Instantiate<Item>(MagnetItem);
            go.SetItem(ItemType.Magnet, 0);
        } else if(value == 2)
        {
             go = GameObject.Instantiate<Item>(PowerItem);
            go.SetItem(ItemType.Power, 0);
        } else
        {
            go = GameObject.Instantiate<Item>(CoinItem);
            int coin = UnityEngine.Random.Range(1, 5);
            go.SetItem(ItemType.Coin, coin);
        }

        go.transform.position = transform.position;
        var enemyColi = GetComponent<BoxCollider2D>();
        var itemColi = go.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(enemyColi, itemColi);
        Destroy(go, 2.0f);
    }

    public void SetData(DataEnemy enemy)
    {
        HP = enemy.hp;
    }
}
