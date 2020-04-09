using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    public Bullet bulletOrigin;
    public Bullet[] bulletsLevel;
    public float delay = 0.1f;
    public GameObject slot;

    private float timer = 0;
    private float powerPeriod = 10f;

    // Update is called once per frame
    void Update()   
    {
        if (timer > delay)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                Bullet bullet = GameObject.Instantiate<Bullet>(bulletOrigin);
                bullet.gameObject.SetActive(true);
                bullet.transform.position = child.position;
            }
            
            timer = 0;
        }

        if(powerPeriod > 0 && transform.childCount > 1) // 파워업 제한시간이 남아있는가?
        {
            powerPeriod -= Time.deltaTime;
            if(powerPeriod <= 0)
            {
                Destroy(transform.GetChild(transform.childCount-1).gameObject);
                powerPeriod = 10f;
            }
        }

        timer += Time.deltaTime;
    }

    private void LevelUp(int level)
    {
        bulletOrigin = bulletsLevel[Mathf.Min(level - 1, bulletsLevel.Length - 1)];
    }

    public void PowerUp()
    {
        var newSlot = GameObject.Instantiate(slot);
        newSlot.transform.SetParent(this.transform);
        powerPeriod = 10f;
    }
}
