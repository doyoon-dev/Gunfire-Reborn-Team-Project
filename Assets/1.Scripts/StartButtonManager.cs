using UnityEngine;

public class StartButtonManager : MonoBehaviour    
{    
    public UnityEngine.UI.ToggleGroup m_toggleGroup;

    void Start()
    {
        m_toggleGroup = GetComponent<UnityEngine.UI.ToggleGroup>();        
    }           
    public void OnClickNewStart()
    {
        ChangeScene();
    }
    public void ChangeScene()
    {
        SceneLoader.Instance.LoadScene("MapT");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MapT");
    }
    public void OnClickExit()
    {
        ExitGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
