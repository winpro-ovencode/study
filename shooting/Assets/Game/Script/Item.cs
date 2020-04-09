using UnityEngine;

public enum ItemType
{
    None,
    Coin,
    Magnet,
    Power,
    Gem
}

public class Item : MonoBehaviour
{
    private ItemType mItemType;
    private int mParameter;
    private PlayerController mPlayer;
    private Vector3 velocity;

    public ItemType Type {
        get {
            return mItemType;
        }
    }

    private void Start()
    {
        var rigid = GetComponent<Rigidbody2D>();

        int x = Random.Range(-50, 50);

        Vector2 vec;
        vec.x = x;
        vec.y = 100;

        rigid.AddForce(vec);

        var go = GameObject.FindGameObjectWithTag("Player");
        mPlayer = go.GetComponent<PlayerController>();
        Destroy(gameObject, 4.0f);
        
    }

    private void Update()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        viewPos.x = Mathf.Clamp(viewPos.x, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(viewPos);

        if(mPlayer.Magnet)
        {
            var force = mPlayer.transform.position - transform.position;
            force.z = 0;
            //force.y += 50;

            var r = force.magnitude;
            force.Normalize();
            force = force * 100;  // 방향벡터에 힘을 100정도 준다
            force = force / (r * r);
            velocity += force * Time.deltaTime;
            velocity.y = 0;
            transform.position += velocity * Time.deltaTime;
        }
    }

    public void SetItem(ItemType type, int parameter)
    {
        mItemType = type;
        mParameter = parameter;

        var spriteRender = GetComponent<SpriteRenderer>();
        var colli = GetComponent<BoxCollider2D>();
        Destroy(colli);
        gameObject.AddComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "PlayerBody")
            return;

        if(mItemType == ItemType.Coin)
        {
            mPlayer.AddCoin(mParameter);
        } else if(mItemType == ItemType.Magnet)
        {
            mPlayer.SetMagnet(true);
        } else if(mItemType == ItemType.Gem)
        {
            mPlayer.AddScore(mParameter);
        } else if(mItemType == ItemType.Power)
        {
            mPlayer.SetPowerUp();
        }

        Destroy(gameObject);
    }
}
