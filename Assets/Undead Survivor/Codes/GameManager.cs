using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform UiJoy;
    public GameObject enemyCleaner;
    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }
    public string cachedUid;
    public int cachedCharacterId;
    public void SetUser(string uid)
    {
        cachedUid = uid;
    }

    public void SetCharacterId(int id)
    {
        cachedCharacterId = id;
    }
    public void GameStart()
    {
        playerId = cachedCharacterId;
        health = maxHealth;

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId);
        Resume();

        SaveUserData(cachedUid);
    }


    public void SaveUserData(string uid)
    {
        FirebaseDatabase db = FirebaseDatabase.GetInstance("https://undeadsurvivor-77af8-default-rtdb.firebaseio.com/");

        Dictionary<string, object> data = new Dictionary<string, object>
    {
        { "level", level },
        { "kill", kill },
        { "exp", exp },
        { "maxHealth", maxHealth },
        { "playerId", playerId }
    };

        db.RootReference.Child("users").Child(uid).UpdateChildrenAsync(data).ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogError("? Save Failed: " + task.Exception);
            else
                Debug.Log("? User data saved");
        });
    }
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }
    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
    public void GameQuit()
    {
        Application.Quit();
    }
    void Update()
    {
        if (!isLive)
            return;
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {

            gameTime = maxGameTime;
            GameVictory();
        }

    }
    public void GetExp()
    {
        if (!isLive)
            return;
        exp++;
        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        UiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        UiJoy.localScale = Vector3.one;
    }
}
