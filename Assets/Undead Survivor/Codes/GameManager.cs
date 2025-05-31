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

    [Header("# 캐릭터 선택 UI")]
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
            Debug.Log("?? GameScene 진입 → 오브젝트 재연결 시도");

            // ? 비활성화된 오브젝트까지 포함해서 검색
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    player = obj.GetComponent<Player>();
                    Debug.Log("? Player 재연결 성공");
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
                    Debug.Log("? CharacterSelectionPanel 연결 완료");
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
            Debug.LogError("? GameStart: player가 null입니다! Player 태그 확인 요망");
            return;
        }

        // ? 비활성화된 HUD를 Canvas로부터 찾아 활성화
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform hudTransform = canvas.transform.Find("HUD");
            if (hudTransform != null)
            {
                hudTransform.gameObject.SetActive(true);
                Debug.Log("? HUD 활성화됨");
            }
            else
            {
                Debug.LogWarning("? HUD 트랜스폼을 찾지 못했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("? Canvas 오브젝트를 찾을 수 없습니다.");
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


