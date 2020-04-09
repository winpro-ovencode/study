using System;
using UnityEngine;  // namespace

namespace Global
{
    public class Animal
    {
        public int age;
        public string name;
    }

    public class Human : Animal
    {
        string address;
        int money;
    }

    internal class KangWook : Human
    {
        bool classes;
    }

    public class Suhyun : Human
    {
        bool car;
    }
}

public struct Data1
{
    public bool value;
}

public class Data2
{
    public bool value;
}

public class PlayerController : MonoBehaviour
{
    public bool Magnet;
    public int mLevel = 1;
    public BulletGenerator mGenerator;
    public ScoreUI Score;

    Vector3 mOffset;
    int mCoin;
    int mScore;
    float timeMagnet;
    int mExp;

    private Vector2 screenBounds;
    
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        SQLiteLoaderClient loader = new SQLiteLoaderClient();
        if(loader.Load("Assets/GameData/_game.db3"))
        {
            LocalDataTable.LoadAll(loader);
            LocalDataTable.LoadComplete();
            loader.Unload();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 왼쪽버튼을 눌렀을때
        if(Input.GetMouseButtonDown(0))
        {
            mOffset = transform.position - GetMouseWorldPos();
            mOffset.z = 0;
            mOffset.y = 0;
        } else if(Input.GetMouseButton(0)) //Move
        {
            var pos = GetMouseWorldPos() * 1.3f;
            pos.z = transform.position.z;
            pos.y = transform.position.y;
            
            Vector3 posOrigin = pos + mOffset;
            transform.position = posOrigin;

        }

        if(timeMagnet > 0.0f)
        {
            timeMagnet -= Time.deltaTime;

            if (timeMagnet <= 0)
                SetMagnet(false);

            //모든 코인
            //현재 자기위치로 조금씩 끌어당김
        }
    }

    internal void AddCoin(int parameter)
    {
        mCoin += parameter;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 input = Input.mousePosition;
        input.y = 0;
        //main 카메라가 속한 좌표로 변환
        return Camera.main.ScreenToWorldPoint(input);
    }

    internal void SetMagnet(bool active)
    {
        if(active)
        {
            timeMagnet = 10.0f;
            Magnet = true;
            Debug.Log("자석 효과 온");
        } else
        {
            Magnet = false;
            Debug.Log("자석 효과 오프");
        }
    }

    internal void SetPowerUp()
    {
        mGenerator.PowerUp();
    }

    internal void AddScore(int mParameter)
    {
        mScore += mParameter;
        mExp += mParameter;
        if(mExp >= 10)
        {
            LevelUp();
            mExp = 0;
        }
        Score.SendMessage("AddScore", mScore);
        //BroadcastMessage("AddScore", mScore);

    }

    private void LevelUp()
    {
        mLevel = mLevel + 1;
        Debug.Log("LevelUp");

        mGenerator.SendMessage("LevelUp", mLevel);
        //mGenerator.LevelUp(mLevel);
    }
}
