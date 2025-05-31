using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 20f;

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

    [Header("# ĳ���� ���� UI")]
    public GameObject characterSelectionPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            Debug.Log("?? GameScene ���� �� ������Ʈ �翬�� �õ�");

            // ? ��Ȱ��ȭ�� ������Ʈ���� �����ؼ� �˻�
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    player = obj.GetComponent<Player>();
                    Debug.Log("? Player �翬�� ����");
                    break;
                }
            }

            pool = FindFirstObjectByType<PoolManager>();
            uiLevelUp = FindFirstObjectByType<LevelUp>();
            uiResult = FindFirstObjectByType<Result>();
            UiJoy = GameObject.Find("Joy")?.transform;
            enemyCleaner = GameObject.Find("EnemyCleaner");

            if (characterSelectionPanel == null)
            {
                GameObject foundPanel = GameObject.Find("Character Group");
                if (foundPanel != null)
                {
                    characterSelectionPanel = foundPanel;
                    Debug.Log("? CharacterSelectionPanel ���� �Ϸ�");
                }
            }

            if (characterSelectionPanel != null)
                characterSelectionPanel.SetActive(true);
        }
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        if (characterSelectionPanel != null)
            characterSelectionPanel.SetActive(false);

        if (player == null)
        {
            Debug.LogError("? GameStart: player�� null�Դϴ�! Player �±� Ȯ�� ���");
            return;
        }

        // ? ��Ȱ��ȭ�� HUD�� Canvas�κ��� ã�� Ȱ��ȭ
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform hudTransform = canvas.transform.Find("HUD");
            if (hudTransform != null)
            {
                hudTransform.gameObject.SetActive(true);
                Debug.Log("? HUD Ȱ��ȭ��");
            }
            else
            {
                Debug.LogWarning("? HUD Ʈ�������� ã�� ���߽��ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("? Canvas ������Ʈ�� ã�� �� �����ϴ�.");
        }

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2);
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
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
        if (UiJoy != null)
            UiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        if (UiJoy != null)
            UiJoy.localScale = Vector3.one;
    }

    public void SetUser(string uid) { }
}


