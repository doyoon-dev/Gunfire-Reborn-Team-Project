using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers m_instance = null;
    public static Managers Instance { get { Init(); return m_instance; } }

    #region Manager
    private ResourceManager m_resource = new ResourceManager();
    private PoolManager m_pool = new PoolManager();
    private InputManager m_input = new InputManager();
    private UIManager m_ui = new UIManager();
    private SoundManager m_sound = new SoundManager();
    private DataManager m_data = null;

    public static ResourceManager Resource { get => Instance.m_resource; }
    public static PoolManager Pool { get => Instance.m_pool; }
    public static InputManager Input { get => Instance.m_input; }
    public static UIManager UI { get => Instance.m_ui; }
    public static SoundManager Sound { get => Instance.m_sound; }
    public static DataManager Data { get => Instance.m_data; }
    #endregion
    #region Content
    private GameManager m_game = new GameManager();
    public static GameManager Game { get => Instance.m_game; }
    #endregion

    void Update()
    {
        m_instance.m_game.OnUpdate();
    }

    static void Init()
    {
        if (m_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.GetOrAddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            m_instance = go.GetOrAddComponent<Managers>();

            m_instance.m_pool.Init();
            m_instance.m_game.Init();
            //m_instance.m_input.Init();
            m_instance.m_sound.Init();
        }
    }

    public static void Clear()
    {
        m_instance.m_pool.Clear();
        m_instance.m_game.Clear();
        m_instance.m_ui.Clear();
        m_instance.m_sound.Clear();
    }
}
