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
    public SpriteRenderer WingLeft;
    public SpriteRenderer WingRight;
    public SpriteRenderer EyeRight;
    public SpriteRenderer EyeLeft;
    private SpriteRenderer Body;

    int HP = 2;

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        mPlayer = go.GetComponent<PlayerController>();
        Body = GetComponent<SpriteRenderer>();
    }

    public void SetEnemy(int id)
    {
        var enemy = DataEnemy.GetAll().Find(w => w.id == id);
        HP = enemy.hp;

        var texture = Resources.Load<Sprite>($"Assets/Resources/Sprite/{enemy.image}_0");
        Body.sprite = texture;
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
        int value = Random.Range(0, 10);

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
            int coin = Random.Range(1, 5);
            go.SetItem(ItemType.Coin, coin);
        }

        go.transform.position = transform.position;
        var enemyColi = GetComponent<BoxCollider2D>();
        var itemColi = go.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(enemyColi, itemColi);
        Destroy(go, 2.0f);
    }
}
