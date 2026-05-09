using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Class which manages the game
/// </summary>
public class GameManager : MonoBehaviour
{
    // The script that manages all others
    public static GameManager instance = null;

    [Tooltip("The player gameobject")]
    public GameObject player = null;

    [Header("Scores")]
    // The current player score in the game
    [Tooltip("The player's score")]
    [SerializeField] private int gameManagerScore = 0;

    // Static getter/setter for player score (for convenience)
    public static int score
    {
        get
        {
            return instance.gameManagerScore;
        }
        set
        {
            instance.gameManagerScore = value;
        }
    }

    // The highest score obtained by this player
    [Tooltip("The highest score acheived on this device")]
    public int highScore = 0;

    [Header("Game Progress / Victory Settings")]
    [Tooltip("Whether the game is winnable or not \nDefault: true")]
    public bool gameIsWinnable = true;
    [Tooltip("The number of enemies that must be defeated to win the game")]
    public int enemiesToDefeat = 10;
    
    // The number of enemies defeated in game
    private int enemiesDefeated = 0;

    [Tooltip("Whether or not to print debug statements about whether the game can be won or not according to the game manager's" +
        " search at start up")]
    public bool printDebugOfWinnableStatus = true;
    [Tooltip("Page index in the UIManager to go to on winning the game")]
    public int gameVictoryPageIndex = 0;
    [Tooltip("The effect to create upon winning the game")]
    public GameObject victoryEffect;

    //The number of enemies observed by the game manager in this scene at start up"
    private int numberOfEnemiesFoundAtStart;

    [Header("Assignment Improvement HUD")]
    [Tooltip("Adds objective, lives, and power-up text so the player understands the goal.")]
    public bool enableImprovementHUD = true;
    [Tooltip("Short objective shown during gameplay.")]
    public string objectiveText = "Defeat enemies and survive";
    [Tooltip("How long temporary feedback messages stay on screen.")]
    public float feedbackMessageDuration = 2.5f;

    private RectTransform improvementHudRoot;
    private TextMeshProUGUI objectiveDisplay;
    private TextMeshProUGUI livesDisplay;
    private TextMeshProUGUI powerUpDisplay;
    private TextMeshProUGUI feedbackDisplay;
    private TMP_FontAsset improvementHudFont;
    private string feedbackMessage = "";
    private float feedbackMessageUntil = 0;

    [Header("Rapid Fire Power-Up")]
    [Tooltip("Spawns a collectable that temporarily increases the player's fire rate.")]
    public bool spawnRapidFirePowerUps = true;
    [Tooltip("Delay before the first power-up appears.")]
    public float firstPowerUpDelay = 2.5f;
    [Tooltip("Delay before another power-up appears after one is collected.")]
    public float powerUpRespawnDelay = 12f;
    [Tooltip("How long rapid fire lasts after collecting the power-up.")]
    public float rapidFireDuration = 6f;
    [Tooltip("Multiplier applied to the player's fire rate. Lower means faster shots.")]
    [Range(0.1f, 1f)]
    public float rapidFireRateMultiplier = 0.35f;
    [Tooltip("Where the first power-up appears relative to the player.")]
    public Vector2 powerUpSpawnOffset = new Vector2(3f, 1.5f);

    private float nextPowerUpSpawnTime = Mathf.Infinity;
    private GameObject activePowerUp;
    private Coroutine rapidFirePowerUpRoutine;
    private readonly Dictionary<ShootingController, float> originalFireRates = new Dictionary<ShootingController, float>();

    /// <summary>
    /// Description:
    /// Standard Unity function called when the script is loaded, called before start
    /// 
    /// When this component is first added or activated, setup the global reference
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }

        if ((player == null) && (FindObjectOfType<Controller>() != null))
        {
            player = FindObjectOfType<Controller>().gameObject;
        }
        else if ((player == null) && (SceneManager.GetActiveScene().name!="MainMenu"))
        {
            Debug.Log("Player is not set and cannot find it in the scene. This is not a problem in non-playable scenes, such as the Main Menu.");
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Start()
    {
        HandleStartUp();
        nextPowerUpSpawnTime = Time.timeSinceLevelLoad + firstPowerUpDelay;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        if (enableImprovementHUD)
        {
            CreateImprovementHUDIfNeeded();
            UpdateImprovementHUD();
        }

        HandlePowerUpSpawning();
    }

    /// <summary>
    /// Description:
    /// Handles necessary activities on start up such as getting the highscore and score, updating UI elements, 
    /// and checking the number of enemies
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleStartUp()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }
        if (PlayerPrefs.HasKey("score"))
        {
            score = PlayerPrefs.GetInt("score");
        }
        UpdateUIElements();
        if (printDebugOfWinnableStatus)
        {
            FigureOutHowManyEnemiesExist();
        }
    }

    /// <summary>
    /// Description:
    /// Searches the level for all spawners and static enemies.
    /// Only produces debug messages / warnings if the game is set to be winnable
    /// If there are any infinite spawners a debug message will say so,
    /// If there are more enemies than the number of enemies to defeat to win
    /// then a debug message will say so
    /// If there are too few enemies to defeat to win then a debug warning will say so
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void FigureOutHowManyEnemiesExist()
    {
        List<EnemySpawner> enemySpawners = FindObjectsOfType<EnemySpawner>().ToList();
        List<Enemy> staticEnemies = FindObjectsOfType<Enemy>().ToList();

        int numberOfInfiniteSpawners = 0;
        int enemiesFromSpawners = 0;
        int enemiesFromStatic = staticEnemies.Count;
        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            if (enemySpawner.spawnInfinite)
            {
                numberOfInfiniteSpawners += 1;
            }
            else
            {
                enemiesFromSpawners += enemySpawner.maxSpawn;
            }
        }
        numberOfEnemiesFoundAtStart = enemiesFromSpawners + enemiesFromStatic;

        if (gameIsWinnable)
        {
            if (numberOfInfiniteSpawners > 0)
            {
                Debug.Log("There are " + numberOfInfiniteSpawners + " infinite spawners " + " so the level will always be winnable, "
                    + "\nhowever you sshould still playtest for timely completion");
            }
            else if (enemiesToDefeat > numberOfEnemiesFoundAtStart)
            {
                if (numberOfEnemiesFoundAtStart > 0)
                {
                    int originalGoal = enemiesToDefeat;
                    enemiesToDefeat = numberOfEnemiesFoundAtStart;
                    Debug.Log("Adjusted enemy goal from " + originalGoal + " to " + enemiesToDefeat +
                        " so the level can be completed.");
                }
                else
                {
                    Debug.LogWarning("There are " + enemiesToDefeat + " enemies to defeat but no enemies were found at start \nThe level can not be completed!");
                }
            }
            else
            {
                Debug.Log("There are " + enemiesToDefeat + " enemies to defeat and " + numberOfEnemiesFoundAtStart +
                    " enemies found at start \nThe level can completed");
            }
        }
    }

    /// <summary>
    /// Description:
    /// Increments the number of enemies defeated by 1
    /// Input:
    /// none
    /// Return:
    /// void (no returned value)
    /// </summary>
    public void IncrementEnemiesDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated >= enemiesToDefeat && gameIsWinnable)
        {
            LevelCleared();
        }
        else if (gameIsWinnable)
        {
            ShowFeedback("Enemy defeated: " + enemiesDefeated + "/" + enemiesToDefeat);
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function that gets called when the application (or playmode) ends
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveHighScore();
        ResetScore();
    }

    /// <summary>
    /// Description:
    /// Adds a number to the player's score stored in the gameManager
    /// Input: 
    /// int scoreAmount
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="scoreAmount">The amount to add to the score</param>
    public static void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        if (score > instance.highScore)
        {
            SaveHighScore();
        }
        UpdateUIElements();
    }
    
    /// <summary>
    /// Description:
    /// Resets the current player score
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void ResetScore()
    {
        PlayerPrefs.SetInt("score", 0);
        score = 0;
    }

    /// <summary>
    /// Description:
    /// Saves the player's highscore
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void SaveHighScore()
    {
        if (score > instance.highScore)
        {
            PlayerPrefs.SetInt("highscore", score);
            instance.highScore = score;
        }
        UpdateUIElements();
    }

    /// <summary>
    /// Description:
    /// Resets the high score in player preferences
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        if (instance != null)
        {
            instance.highScore = 0;
        }
        UpdateUIElements();
    }

    /// <summary>
    /// Description:
    /// Sends out a message to UI elements to update
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void UpdateUIElements()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateUI();
        }
    }

    /// <summary>
    /// Description:
    /// Ends the level, meant to be called when the level is complete (enough enemies have been defeated)
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void LevelCleared()
    {
        PlayerPrefs.SetInt("score", score);
        ShowFeedback("Objective complete!");
        if (UIManager.instance != null)
        {
            player.SetActive(false);
            UIManager.instance.allowPause = false;
            UIManager.instance.GoToPage(gameVictoryPageIndex);
            if (victoryEffect != null)
            {
                Instantiate(victoryEffect, transform.position, transform.rotation, null);
            }
        }     
    }

    [Header("Game Over Settings:")]
    [Tooltip("The index in the UI manager of the game over page")]
    public int gameOverPageIndex = 0;
    [Tooltip("The game over effect to create when the game is lost")]
    public GameObject gameOverEffect;

    // Whether or not the game is over
    [HideInInspector]
    public bool gameIsOver = false;

    /// <summary>
    /// Description:
    /// Displays game over screen
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void GameOver()
    {
        gameIsOver = true;
        ShowFeedback("Game Over");
        if (gameOverEffect != null)
        {
            Instantiate(gameOverEffect, transform.position, transform.rotation, null);
        }
        if (UIManager.instance != null)
        {
            UIManager.instance.allowPause = false;
            UIManager.instance.GoToPage(gameOverPageIndex);
        }
    }

    public void ShowFeedback(string message)
    {
        feedbackMessage = message;
        feedbackMessageUntil = Time.time + feedbackMessageDuration;
        if (enableImprovementHUD)
        {
            CreateImprovementHUDIfNeeded();
            UpdateImprovementHUD();
        }
    }

    public void ActivateRapidFirePowerUp(float duration, float fireRateMultiplier)
    {
        if (player == null)
        {
            return;
        }

        if (rapidFirePowerUpRoutine != null)
        {
            StopCoroutine(rapidFirePowerUpRoutine);
            RestorePlayerFireRates();
        }

        rapidFirePowerUpRoutine = StartCoroutine(RapidFirePowerUpRoutine(duration, fireRateMultiplier));
    }

    private IEnumerator RapidFirePowerUpRoutine(float duration, float fireRateMultiplier)
    {
        ShootingController[] playerGuns = player.GetComponentsInChildren<ShootingController>();
        foreach (ShootingController gun in playerGuns)
        {
            if (gun != null && gun.isPlayerControlled)
            {
                originalFireRates[gun] = gun.fireRate;
                gun.fireRate = Mathf.Max(0.01f, gun.fireRate * fireRateMultiplier);
            }
        }

        ShowFeedback("Rapid Fire activated!");
        float endTime = Time.time + duration;
        while (Time.time < endTime && !gameIsOver)
        {
            yield return null;
        }

        RestorePlayerFireRates();
        rapidFirePowerUpRoutine = null;
        ShowFeedback("Rapid Fire ended");
    }

    private void RestorePlayerFireRates()
    {
        foreach (KeyValuePair<ShootingController, float> originalFireRate in originalFireRates)
        {
            if (originalFireRate.Key != null)
            {
                originalFireRate.Key.fireRate = originalFireRate.Value;
            }
        }
        originalFireRates.Clear();
    }

    private void HandlePowerUpSpawning()
    {
        if (!spawnRapidFirePowerUps || gameIsOver || player == null || activePowerUp != null)
        {
            return;
        }

        if (Time.timeSinceLevelLoad >= nextPowerUpSpawnTime)
        {
            SpawnRapidFirePowerUp();
        }
    }

    private void SpawnRapidFirePowerUp()
    {
        Vector3 spawnPosition = player.transform.position + new Vector3(powerUpSpawnOffset.x, powerUpSpawnOffset.y, 0);
        activePowerUp = new GameObject("Rapid Fire Power-Up");
        activePowerUp.transform.position = spawnPosition;

        SpriteRenderer renderer = activePowerUp.AddComponent<SpriteRenderer>();
        renderer.sprite = CreatePowerUpSprite();
        renderer.color = new Color(1f, 0.82f, 0.18f, 1f);
        renderer.sortingOrder = 15;

        CircleCollider2D collider = activePowerUp.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.45f;

        Rigidbody2D body = activePowerUp.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0;

        RapidFirePowerUp powerUp = activePowerUp.AddComponent<RapidFirePowerUp>();
        powerUp.duration = rapidFireDuration;
        powerUp.fireRateMultiplier = rapidFireRateMultiplier;

        ShowFeedback("Rapid Fire power-up spawned");
    }

    public void NotifyPowerUpCollected()
    {
        activePowerUp = null;
        nextPowerUpSpawnTime = Time.timeSinceLevelLoad + powerUpRespawnDelay;
    }

    private Sprite CreatePowerUpSprite()
    {
        const int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        Color clear = new Color(0, 0, 0, 0);
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance < 22)
                {
                    texture.SetPixel(x, y, new Color(1f, 0.82f, 0.18f, 1f));
                }
                else if (distance < 28)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, clear);
                }
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Point;
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f), 64);
    }

    private void CreateImprovementHUDIfNeeded()
    {
        if (improvementHudRoot != null)
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Improvement HUD Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject rootObject = new GameObject("Assignment Improvement HUD", typeof(RectTransform));
        rootObject.transform.SetParent(canvas.transform, false);
        improvementHudRoot = rootObject.GetComponent<RectTransform>();
        improvementHudRoot.anchorMin = Vector2.zero;
        improvementHudRoot.anchorMax = Vector2.one;
        improvementHudRoot.offsetMin = Vector2.zero;
        improvementHudRoot.offsetMax = Vector2.zero;

        CacheExistingHudFont();

        objectiveDisplay = CreateHudText("Objective Text", new Vector2(-24, -24), new Vector2(760, 34), 20, TextAlignmentOptions.TopRight);
        livesDisplay = CreateHudText("Lives Text", new Vector2(-24, -58), new Vector2(360, 30), 18, TextAlignmentOptions.TopRight);
        powerUpDisplay = CreateHudText("Power-Up Text", new Vector2(-24, -88), new Vector2(420, 30), 18, TextAlignmentOptions.TopRight);
        feedbackDisplay = CreateHudText("Feedback Text", new Vector2(0, 72), new Vector2(640, 36), 20, TextAlignmentOptions.Center);

        SetHudAnchor(objectiveDisplay, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
        SetHudAnchor(livesDisplay, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
        SetHudAnchor(powerUpDisplay, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
        SetHudAnchor(feedbackDisplay, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
    }

    private TextMeshProUGUI CreateHudText(string name, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(improvementHudRoot, false);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        if (improvementHudFont != null)
        {
            text.font = improvementHudFont;
        }
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.outlineColor = Color.black;
        text.outlineWidth = 0.12f;
        text.raycastTarget = false;

        RectTransform rectTransform = text.rectTransform;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        return text;
    }

    private void SetHudAnchor(TextMeshProUGUI text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        RectTransform rectTransform = text.rectTransform;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
    }

    private void CacheExistingHudFont()
    {
        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>(true);
        TextMeshProUGUI scoreText = texts.FirstOrDefault(text => text != null && text.name == "Score Text");
        if (scoreText == null)
        {
            scoreText = texts.FirstOrDefault(text => text != null && text.font != null);
        }

        if (scoreText != null)
        {
            improvementHudFont = scoreText.font;
        }
    }

    private void UpdateImprovementHUD()
    {
        if (objectiveDisplay == null || livesDisplay == null || powerUpDisplay == null || feedbackDisplay == null)
        {
            return;
        }

        if (gameIsWinnable)
        {
            objectiveDisplay.text = "Objective: " + objectiveText + " (" + enemiesDefeated + "/" + enemiesToDefeat + ")";
        }
        else
        {
            objectiveDisplay.text = "Objective: " + objectiveText;
        }

        Health playerHealth = player != null ? player.GetComponent<Health>() : null;
        if (playerHealth != null && playerHealth.useLives)
        {
            livesDisplay.text = "Lives: " + playerHealth.currentLives + "/" + playerHealth.maximumLives;
        }
        else if (playerHealth != null)
        {
            livesDisplay.text = "Health: " + playerHealth.currentHealth + "/" + playerHealth.maximumHealth;
        }
        else
        {
            livesDisplay.text = "Lives: --";
        }

        if (rapidFirePowerUpRoutine != null)
        {
            powerUpDisplay.text = "Power-Up: Rapid Fire active";
        }
        else if (activePowerUp != null)
        {
            powerUpDisplay.text = "Power-Up: Collect Rapid Fire";
        }
        else
        {
            powerUpDisplay.text = "Power-Up: Waiting";
        }

        feedbackDisplay.text = Time.time < feedbackMessageUntil ? feedbackMessage : "";
    }
}
