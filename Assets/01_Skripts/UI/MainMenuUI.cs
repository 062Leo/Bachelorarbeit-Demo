using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject checkpointSelectPanel;
    public GameObject levelSelectPanel;
    public GameObject loadingPanel;

    [Header("Level Buttons")]
    public Button[] levelButtons;       // Alle 13 Level-Buttons
    private string[] sceneNames;         // 13 Szenen-Namen

     [Header("Level Buttons")]
    public Button button_AllLevel;     
    public Button button_DRC;     
    public Button button_OC;     

    [Header("Other Buttons")]
    public Button backButton;           // Back-Button zum Hauptmenü
    public Button backButton2;           // Back-Button zum Hauptmenü
    public Button watchAIButton;        // Optional: Watch AI Button
    public Button playYourselfButton;        // Optional: Watch AI Button

    void Start()
    {
        sceneNames = new string[13];
        for (int i = 0; i < 13; i++)
        {
            sceneNames[i] = $"Level {i + 1}";
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i; // lokale Kopie für Listener
            levelButtons[i].onClick.AddListener(() => StartCoroutine(LoadLevel(index)));
        }

        backButton.onClick.AddListener(BackToMain);
        backButton2.onClick.AddListener(BackToMain);
        playYourselfButton.onClick.AddListener(OnPlayYourself);
        watchAIButton.onClick.AddListener(OnWatchAI);

        button_AllLevel.onClick.AddListener(() => On_All_Level());
        button_DRC.onClick.AddListener(() => On_DRC_Level());
        button_OC.onClick.AddListener(() => On_OC_Level());
    }

    // Buttons
    public void OnPlayYourself()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnWatchAI()
    {
        mainPanel.SetActive(false);
        checkpointSelectPanel.SetActive(true);
    }


    
    public void BackToMain()
    {
        checkpointSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    public IEnumerator LoadSceneSetup()
    {
        Time.timeScale = 1f;
        loadingPanel.SetActive(true);
        checkpointSelectPanel.SetActive(false);
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        // Wait for one frame to ensure UI updates are processed
        yield return null;
    }

    public void On_All_Level()
    {
        StartCoroutine(LoadLevelRoutine("AI_Level_Full"));
    }
    
    public void On_DRC_Level()
    {
        StartCoroutine(LoadLevelRoutine("AI_Level_DRC"));
    }
    
    public void On_OC_Level()
    {
        StartCoroutine(LoadLevelRoutine("AI_Level_OC"));
    }

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        // First, set up the UI
        yield return StartCoroutine(LoadSceneSetup());
        // Then load the scene
        yield return StartCoroutine(LoadSceneWithCleanup(sceneName));
    }


    private IEnumerator LoadLevel(int index)
    {
        if (index >= 0 && index < sceneNames.Length)
        {
            yield return StartCoroutine(LoadSceneSetup());

            yield return StartCoroutine(LoadSceneWithCleanup(sceneNames[index]));
        }
    }

    private IEnumerator LoadSceneWithCleanup(string sceneName)
    {
        DestroyExistingAcademy();

        yield return null;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void DestroyExistingAcademy()
    {
        try
        {
            var academy = Academy.Instance;
            if (academy != null)
            {
                academy.Dispose();
            }
        } catch { }
    }
}
