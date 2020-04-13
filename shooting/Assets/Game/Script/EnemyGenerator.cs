using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public BackgroundRepeat background;
    public EnemyGroup enemyOrigin;
    public float delay = 3f;
    private float timer = 0;
    private List<DataLevel> levelData;
    private Enemy currentEnemy;

    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        levelData = DataLevel.GetAll();
        ChangeLevel(1);
    }

    void ChangeLevel(int level)
    {
        currentLevel = level;
        var dataLevel = DataLevel.GetGet(currentLevel);
        var enemy = DataEnemy.GetGet(dataLevel.enemy);
        var enemyPrefab = Resources.Load<Enemy>(enemy.prefab);
        currentEnemy = GameObject.Instantiate<Enemy>(enemyPrefab);
        currentEnemy.SetData(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var i in levelData)
        {
            float realDistance = background.distance * 100;
            if (realDistance < i.distance)
            {
                if (currentLevel == i.id)
                    break;

                ChangeLevel(i.id);
                break;
            }
        }

        if (timer > delay)
        {
            EnemyGroup go = Instantiate<EnemyGroup>(enemyOrigin);
            go.gameObject.SetActive(true);
            go.transform.position = transform.position;
            timer = 0;
            go.enemyTemplete = currentEnemy.gameObject;
        }

        timer += Time.deltaTime;
    }
}
