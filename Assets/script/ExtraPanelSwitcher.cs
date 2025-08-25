using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraPanelSwitcher : MonoBehaviour
{
    [Header("메인 패널 (첫 화면)")]
    [SerializeField] private GameObject mainPanel; 

    [Header("탭별 패널 (Episode, Ending, CG, Info 순서)")]
    [SerializeField] private GameObject episodePanel;   
    [SerializeField] private GameObject endingPanel;    
    [SerializeField] private GameObject cgPanel;        
    [SerializeField] private GameObject infoPanel;      

    [Header("선택: 탭 버튼들 (순서 동일)")]
    [SerializeField] private Button episodeBtn;
    [SerializeField] private Button endingBtn;
    [SerializeField] private Button cgBtn;
    [SerializeField] private Button infoBtn;

    void Start()
    {
        // 씬 시작 시: 메인만 true, 나머지 false
        ShowMain();
        if (episodeBtn) episodeBtn.onClick.AddListener(ShowEpisode);
        if (endingBtn) endingBtn.onClick.AddListener(ShowEnding);
        if (cgBtn) cgBtn.onClick.AddListener(ShowCG);
        if (infoBtn) infoBtn.onClick.AddListener(ShowInfo);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(mainPanel != null && mainPanel.activeInHierarchy) 
            {
                SceneManager.LoadScene("title");
            }
            else
            {
                ShowMain();
            }
        }
    }
    public void ShowMain()
    {
        SetActiveSafe(mainPanel, true);
        SetActiveSafe(episodePanel, false);
        SetActiveSafe(endingPanel, false);
        SetActiveSafe(cgPanel, false);
        SetActiveSafe(infoPanel, false);
    }

    public void ShowEpisode()
    {
        SetActiveSafe(mainPanel, false);
        SetActiveSafe(episodePanel, true);
        SetActiveSafe(endingPanel, false);
        SetActiveSafe(cgPanel, false);
        SetActiveSafe(infoPanel, false);
    }

    public void ShowEnding()
    {
        SetActiveSafe(mainPanel, false);
        SetActiveSafe(episodePanel, false);
        SetActiveSafe(endingPanel, true);
        SetActiveSafe(cgPanel, false);
        SetActiveSafe(infoPanel, false);
    }

    public void ShowCG()
    {
        SetActiveSafe(mainPanel, false);
        SetActiveSafe(episodePanel, false);
        SetActiveSafe(endingPanel, false);
        SetActiveSafe(cgPanel, true);
        SetActiveSafe(infoPanel, false);
    }

    public void ShowInfo()
    {
        SetActiveSafe(mainPanel, false);
        SetActiveSafe(episodePanel, false);
        SetActiveSafe(endingPanel, false);
        SetActiveSafe(cgPanel, false);
        SetActiveSafe(infoPanel, true);
    }

    private void SetActiveSafe(GameObject go, bool on)
    {
        if (go && go.activeSelf != on) go.SetActive(on);
    }
}
