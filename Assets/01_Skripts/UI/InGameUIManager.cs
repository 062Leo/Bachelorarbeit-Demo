using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    [Header("InGameInfo Text Obj")]
    [SerializeField] private TMP_Text totalAgentsInScene;
    [SerializeField] private TMP_Text survivedText;
    [SerializeField] private TMP_Text diedText;
    [SerializeField] private TMP_Text survivalRateText;
    [SerializeField] private TMP_Text AverageRewardText;

    [Header("PauseMenu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button backToMenuButton;

    [Header("TimeScalePanel")]
    [SerializeField] private GameObject timeScalePanel;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Slider timeScaleSlider;

    [Header("InfoBoxes")]
    [SerializeField] private GameObject aiControlsInfo;
    [SerializeField] private GameObject playerControlsInfo;


    private int totalSurvived = 0;
    private int totalDied = 0;
    private float totalReward = 0f;
    private int totalAgents = 0;
    private bool isPaused = false;

    private const float MIN_TIME_SCALE = 0.5f;
    private const float DEFAULT_TIME_SCALE = 1.0f;
    private const float MAX_TIME_SCALE = 2.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateUI();

        // Button-Listener registrieren
        returnButton.onClick.AddListener(TogglePauseMenu);
        backToMenuButton.onClick.AddListener(BackToMainMenu);

        // TimeScalePanel nur in "AlleLevels_AI" Scene aktivieren
        bool watchAiScene = SceneManager.GetActiveScene().name == "AI_Level_Full" || SceneManager.GetActiveScene().name == "AI_Level_DRC" || SceneManager.GetActiveScene().name == "AI_Level_OC";
        timeScalePanel.SetActive(watchAiScene);
        if(watchAiScene)
        {
            aiControlsInfo.SetActive(true);
            playerControlsInfo.SetActive(false);
        }
        else
        {
            aiControlsInfo.SetActive(false);
            playerControlsInfo.SetActive(true);
        }


        // Slider konfigurieren
        timeScaleSlider.minValue = MIN_TIME_SCALE;
        timeScaleSlider.maxValue = MAX_TIME_SCALE;
        timeScaleSlider.value = DEFAULT_TIME_SCALE;
        timeScaleSlider.wholeNumbers = false;
        timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);

        // Speed Text initial aktualisieren
        UpdateSpeedText(DEFAULT_TIME_SCALE);

        // Pause-Menü initial deaktivieren
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        //Tab - Taste zum Öffnen/ Schließen des Pause-Menüs
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePauseMenu();
        }
    }

    private void OnDestroy()
    {
        // Listener entfernen, um Memory Leaks zu vermeiden
        if (returnButton != null)
            returnButton.onClick.RemoveListener(TogglePauseMenu);
        if (backToMenuButton != null)
            backToMenuButton.onClick.RemoveListener(BackToMainMenu);
        if (timeScaleSlider != null)
            timeScaleSlider.onValueChanged.RemoveListener(OnTimeScaleChanged);
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        bool isAlleLevelsAIScene = SceneManager.GetActiveScene().name == "AI_Level_Full" || SceneManager.GetActiveScene().name == "AI_Level_DRC" || SceneManager.GetActiveScene().name == "AI_Level_OC";
        float timeScaleValue = isAlleLevelsAIScene ? timeScaleSlider.value : DEFAULT_TIME_SCALE;
        Time.timeScale = isPaused ? 0f : timeScaleValue;
    }

    private void OnTimeScaleChanged(float newTimeScale)
    {
        bool isAlleLevelsAIScene = SceneManager.GetActiveScene().name == "AI_Level_Full" || SceneManager.GetActiveScene().name == "AI_Level_DRC" || SceneManager.GetActiveScene().name == "AI_Level_OC";
        float timeScaleValue = isAlleLevelsAIScene ? newTimeScale : DEFAULT_TIME_SCALE;
        Time.timeScale = isPaused ? 0f : timeScaleValue;
        UpdateSpeedText(newTimeScale);
    }

    private void UpdateSpeedText(float timeScale)
    {
        speedText.text = $"{timeScale:F1}x";
    }

    private void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Wird aufgerufen, wenn ein Agent überlebt hat
    /// </summary>
    public void OnAgentSurvived(float agentReward)
    {
        totalSurvived++;
        totalReward += agentReward;
        totalAgents++;
        UpdateUI();
    }

    /// <summary>
    /// Wird aufgerufen, wenn ein Agent gestorben ist
    /// </summary>
    public void OnAgentDied(float agentReward)
    {
        totalDied++;
        totalAgents++;
        totalReward += agentReward;
        UpdateUI();
    }

    /// <summary>
    /// Aktualisiert die UI mit den aktuellen Statistiken
    /// </summary>
    private void UpdateUI()
    {
        // Überlebte Agenten
        survivedText.text = $"Survived: {totalSurvived}";

        // Gestorbene Agenten
        diedText.text = $"Died: {totalDied}";

        // Überlebensrate in %
        float survivalRate = totalAgents > 0 ? (totalSurvived / (float)totalAgents) * 100f : 0f;
        survivalRateText.text = $"Survival Rate: {survivalRate:F1}%";

        // Durchschnittliche Belohnung
        float averageReward = totalAgents > 0 ? totalReward / totalAgents : 0f;
        AverageRewardText.text = $"Average Reward: {averageReward:F2}";

        // Aktuelle Anzahl der Agenten in der Scene
        MLAgentController[] agentsInScene = FindObjectsOfType<MLAgentController>();
        totalAgentsInScene.text = $"Total Agents in Scene: {agentsInScene.Length}";
    }

    /// <summary>
    /// Setzt alle Statistiken zurück
    /// </summary>
    public void ResetStats()
    {
        totalSurvived = 0;
        totalDied = 0;
        totalReward = 0f;
        totalAgents = 0;
        UpdateUI();
    }
}
