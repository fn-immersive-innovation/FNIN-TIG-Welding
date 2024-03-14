using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lovatto.SceneLoader;

public class NavigationUIController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject m_MainPanel;
    [SerializeField] GameObject m_GameModePanel;
    [SerializeField] bl_SceneLoader m_SceneLoader;

    [SerializeField] string m_SceneToSwitch;

    public string CurrentSceneName
    {
        get { return m_SceneToSwitch; }
    }

    public void SetCurrentSceneReference(string sceneName)
    {
        m_SceneToSwitch = sceneName;

    }
    public void SwitchToGameModePanel()
    {
        m_GameModePanel.SetActive(true);
        m_MainPanel.SetActive(false);

    }

    public void SwitchToMainPanel()
    {
        m_GameModePanel.SetActive(false);
        m_MainPanel.SetActive(true);

    }

    public void SwitchToLoadingPanel(string header, string tooltip)
    {
        bl_SceneLoaderInfo info = new bl_SceneLoaderInfo();

        info.DisplayName = header;
        info.Description = tooltip;

        m_GameModePanel.gameObject.SetActive(true);

        m_SceneLoader.SetupUI(info);
    }

    public void LoadSelectedScene()
    {
        m_SceneLoader.gameObject.SetActive(true);
        m_SceneLoader.LoadLevel(m_SceneToSwitch);
    }
}
