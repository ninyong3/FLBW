using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEditor;

public class DialogManager : MonoBehaviour
{
    bool gotoNext=false; // 다음 대사로 넘어가야 하는지 판단하는 변수
    int dialogCnt; // 대사 번호
    [SerializeField] TextMeshProUGUI name; // 이름 text
    [SerializeField] TextMeshProUGUI dialog; // 대사 text
    [SerializeField] Image background;
    [SerializeField] Image character;
    [SerializeField] Image character2;
    [SerializeField] GameObject Clickarea;
    [SerializeField] List<Sprite> JinYeinImageList;
    [SerializeField] List<Sprite> FreyjaImageList;
    [SerializeField] List<Sprite> RuImageList;
    [SerializeField] List<Sprite> backgroundList;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] GameObject Writeplayername;
    [SerializeField] RectTransform logContentRect;
    [SerializeField] GameObject Log;
    [SerializeField] GameObject ChoiceButton;
    [SerializeField] List<TextMeshProUGUI> choiceText;
    bool playerNameSelect=true;
    List<GameObject> logLineList=new List<GameObject>();
    void Start()
    {
        ChoiceButton.SetActive(false);
        character2.sprite = null;
        Color tempColor = character2.color;
        tempColor.a = 0f;
        character2.color = tempColor;
        if (GameManager.instance.playerName != "")
        {
            Writeplayername.SetActive(false);
            if (SceneManager.GetActiveScene().name == "ep0")
                ShowDialog();
        }
        if(SceneManager.GetActiveScene().name != "ep0")
            ShowDialog();
    }
    void Update()
    {
        if (GameManager.instance.IsLoading == false)
        {
            if (gotoNext && EventSystem.current.currentSelectedGameObject == Clickarea && GameManager.instance.printSetting == 1)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) // 엔터키 혹은 좌클릭 시
                {
                    dialogCnt++; // 다음 번호의 대사로
                    gotoNext = false;
                    StartCoroutine(PrintText());
                }
            }
            if (gotoNext && GameManager.instance.printSetting == 0)
            {
                dialogCnt++;
                gotoNext = false;
                StartCoroutine(PrintText());
            }
        }  
    }
    IEnumerator PrintText()
    {
        Dialogue tempDialog;
        if (DBManager.instance.dialogueDic.TryGetValue(dialogCnt, out tempDialog))
        {
            if (GameManager.instance.userChoice == tempDialog.choiceIndex || GameManager.instance.userChoice == null || tempDialog.choiceline == "-" || tempDialog.choiceIndex == "t")
            {
                GameManager.instance.dialogCount = dialogCnt;
                GameObject logLinePrefab = Resources.Load<GameObject>("Logline"); // 프리펩 받아옴
                GameObject logLineGo = GameObject.Instantiate(logLinePrefab, logContentRect); // 자식으로 만들기&클론 생성
                logLineList.Add(logLineGo);
                logLineGo.GetComponent<LogLine>().Count(logLineList.Count);
                Transform logLineText = logLineGo.transform.Find("Loglinetext"); // 프리펩의 텍스트 찾기
                Transform logNameText = logLineGo.transform.Find("Lognametext");
                logLineText.GetComponent<TextMeshProUGUI>().text = tempDialog.line; // 텍스트 내용 변환
                if (tempDialog.name == "Narration")
                {
                    name.text = "";
                    logNameText.GetComponent<TextMeshProUGUI>().text = "";
                }
                else if (tempDialog.name == "Player")
                {
                    name.text = GameManager.instance.playerName;
                    logNameText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.playerName;
                }
                else if (tempDialog.name == "Jin Yein")
                {
                    name.text = "진예인";
                    logNameText.GetComponent<TextMeshProUGUI>().text = "진예인";
                }
                else if (tempDialog.name == "Freyja")
                {
                    name.text = "프레이야";
                    logNameText.GetComponent<TextMeshProUGUI>().text = "프레이야";
                }
                else if (tempDialog.name == "Ru")
                {
                    name.text = "루";
                    logNameText.GetComponent<TextMeshProUGUI>().text = "루";
                }
                if (tempDialog.characterIndex[0] == 1)
                {
                    if (tempDialog.choiceIndex == "t" && SceneManager.GetActiveScene().name != "ep4_jinyein" || tempDialog.choiceIndex != "t" && SceneManager.GetActiveScene().name == "ep4_jinyein")
                    {
                        character.sprite = null;
                        Color tempColor = character.color;
                        tempColor.a = 0f;
                        character.color = tempColor;
                        character2.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                        tempColor = character2.color;
                        tempColor.a = 255f;
                        character2.color = tempColor;
                    }
                    else
                    {
                        character2.sprite = null;
                        Color tempColor = character2.color;
                        tempColor.a = 0f;
                        character2.color = tempColor;
                        character.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                        tempColor = character.color;
                        tempColor.a = 255f;
                        character.color = tempColor;
                    }
                }
                else if (tempDialog.characterIndex[0] == 2)
                {
                    if(tempDialog.choiceIndex == "t")
                    {
                        character.sprite = null;
                        Color tempColor = character.color;
                        tempColor.a = 0f;
                        character.color = tempColor;
                        character2.sprite = FreyjaImageList[tempDialog.characterIndex[1]];
                        tempColor = character2.color;
                        tempColor.a = 255f;
                        character2.color = tempColor;
                    }
                    else
                    {
                        character2.sprite = null;
                        Color tempColor = character2.color;
                        tempColor.a = 0f;
                        character2.color = tempColor;
                        character.sprite = FreyjaImageList[tempDialog.characterIndex[1]];
                        tempColor = character.color;
                        tempColor.a = 255f;
                        character.color = tempColor;
                    }
                }
                else if (tempDialog.characterIndex[0] == 3)
                {
                    if (tempDialog.choiceIndex == "t")
                    {
                        character.sprite = null;
                        Color tempColor = character.color;
                        tempColor.a = 0f;
                        character.color = tempColor;
                        character2.sprite = RuImageList[tempDialog.characterIndex[1]];
                        tempColor = character2.color;
                        tempColor.a = 255f;
                        character2.color = tempColor;
                    }
                    else
                    {
                        character2.sprite = null;
                        Color tempColor = character2.color;
                        tempColor.a = 0f;
                        character2.color = tempColor;
                        character.sprite = RuImageList[tempDialog.characterIndex[1]];
                        tempColor = character.color;
                        tempColor.a = 255f;
                        character.color = tempColor;
                    }
                }
                else if (tempDialog.characterIndex[0] == -1)
                {
                    character.sprite = null;
                    Color tempColor = character.color;
                    tempColor.a = 0f;
                    character.color = tempColor;
                    character2.sprite = null;
                    tempColor = character2.color;
                    tempColor.a = 0f;
                    character2.color = tempColor;
                }
                if (tempDialog.backgroundIndex != -1)
                {
                    background.sprite = backgroundList[tempDialog.backgroundIndex];
                    GameManager.instance.backgroundIndex = tempDialog.backgroundIndex;
                }
                dialog.text = "";
                for (int i = 0; i < tempDialog.line.Length; i++)
                {
                    dialog.text += tempDialog.line[i]; // 딕셔너리에서 대사 번호로 대사 가져오기
                    if (GameManager.instance.textPrintSpeed != 0)
                        yield return new WaitForSeconds(0.5f / (float)GameManager.instance.textPrintSpeed);
                    else
                        yield return new WaitForSeconds(0.02f);
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                    {
                        dialog.text = tempDialog.line;
                        break;
                    }
                }
                if (tempDialog.choiceIndex == "s")
                {
                    GameManager.instance.userChoice = null;
                    yield return StartCoroutine(WaitForChoice());
                }
            }
            else if(tempDialog.choiceIndex != GameManager.instance.userChoice && tempDialog.choiceIndex != "t")
            {
                dialogCnt++; 
                while (DBManager.instance.dialogueDic.TryGetValue(dialogCnt, out tempDialog) && tempDialog.choiceIndex != "-")
                {
                    dialogCnt++;
                }
                if (DBManager.instance.dialogueDic.TryGetValue(dialogCnt, out tempDialog))
                {
                    GameManager.instance.dialogCount = dialogCnt;
                    GameObject logLinePrefab = Resources.Load<GameObject>("Logline"); // 프리펩 받아옴
                    GameObject logLineGo = GameObject.Instantiate(logLinePrefab, logContentRect); // 자식으로 만들기&클론 생성
                    logLineList.Add(logLineGo);
                    logLineGo.GetComponent<LogLine>().Count(logLineList.Count);
                    Transform logLineText = logLineGo.transform.Find("Loglinetext"); // 프리펩의 텍스트 찾기
                    Transform logNameText = logLineGo.transform.Find("Lognametext");
                    logLineText.GetComponent<TextMeshProUGUI>().text = tempDialog.line; // 텍스트 내용 변환
                    if (tempDialog.name == "Narration")
                    {
                        name.text = "";
                        logNameText.GetComponent<TextMeshProUGUI>().text = "";
                    }
                    else if (tempDialog.name == "Player")
                    {
                        name.text = GameManager.instance.playerName;
                        logNameText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.playerName;
                    }
                    else if (tempDialog.name == "Jin Yein")
                    {
                        name.text = "진예인";
                        logNameText.GetComponent<TextMeshProUGUI>().text = "진예인";
                    }
                    else if (tempDialog.name == "Freyja")
                    {
                        name.text = "프레이야";
                        logNameText.GetComponent<TextMeshProUGUI>().text = "프레이야";
                    }
                    else if (tempDialog.name == "Ru")
                    {
                        name.text = "루";
                        logNameText.GetComponent<TextMeshProUGUI>().text = "루";
                    }
                    if (tempDialog.characterIndex[0] == 1)
                    {
                        if (tempDialog.choiceIndex == "t" && SceneManager.GetActiveScene().name != "ep4_jinyein" || tempDialog.choiceIndex != "t" && SceneManager.GetActiveScene().name == "ep4_jinyein")
                        {
                            character.sprite = null;
                            Color tempColor = character.color;
                            tempColor.a = 0f;
                            character.color = tempColor;
                            character2.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                            tempColor = character2.color;
                            tempColor.a = 255f;
                            character2.color = tempColor;
                        }
                        else
                        {
                            character2.sprite = null;
                            Color tempColor = character2.color;
                            tempColor.a = 0f;
                            character2.color = tempColor;
                            character.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                            tempColor = character.color;
                            tempColor.a = 255f;
                            character.color = tempColor;
                        }
                    }
                    else if (tempDialog.characterIndex[0] == 2)
                    {
                        character.sprite = FreyjaImageList[tempDialog.characterIndex[1]];
                        Color tempColor = character.color;
                        tempColor.a = 255f;
                        character.color = tempColor;
                    }
                    else if (tempDialog.characterIndex[0] == 3)
                    {
                        character.sprite = RuImageList[tempDialog.characterIndex[1]];
                        Color tempColor = character.color;
                        tempColor.a = 255f;
                        character.color = tempColor;
                    }
                    else if (tempDialog.characterIndex[0] == -1)
                    {
                        character.sprite = null;
                        Color tempColor = character.color;
                        tempColor.a = 0f;
                        character.color = tempColor;
                    }
                    if (tempDialog.backgroundIndex != -1)
                    {
                        background.sprite = backgroundList[tempDialog.backgroundIndex];
                        GameManager.instance.backgroundIndex = tempDialog.backgroundIndex;
                    }
                    dialog.text = "";
                    for (int i = 0; i < tempDialog.line.Length; i++)
                    {
                        dialog.text += tempDialog.line[i]; // 딕셔너리에서 대사 번호로 대사 가져오기
                        if (GameManager.instance.textPrintSpeed != 0)
                            yield return new WaitForSeconds(0.5f / (float)GameManager.instance.textPrintSpeed);
                        else
                            yield return new WaitForSeconds(0.02f);
                        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                        {
                            dialog.text = tempDialog.line;
                            break;
                        }
                    }
                }
                else
                {
                    GameManager.instance.userChoice = null;
                    GameManager.instance.dialogCount = 1;
                    GameManager.instance.IsLoading = true;
                    if (GameManager.instance.selectedHeroine == 1)
                        SceneManager.LoadScene("ep0_jinyein");
                    else if (GameManager.instance.selectedHeroine == 2)
                        SceneManager.LoadScene("ep0_freyja");
                    else if (GameManager.instance.selectedHeroine == 3)
                        SceneManager.LoadScene("ep0_ru");
                }
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().name != "ep0")
            {
                int checkIndex, index1, index2 = -1;
                index1 = SceneManager.GetActiveScene().name.IndexOf('_');
                if (SceneManager.GetActiveScene().name[index1 + 1] == 'j')
                    index1 = 0;
                else if (SceneManager.GetActiveScene().name[index1 + 1] == 'f')
                    index1 = 1;
                else if (SceneManager.GetActiveScene().name[index1 + 1] == 'r')
                    index1 = 2;
                if (SceneManager.GetActiveScene().name[0] == 'e')
                {
                    checkIndex = 1;
                    char epNum = SceneManager.GetActiveScene().name[2];
                    index2 = int.Parse(epNum.ToString());
                    GameManager.instance.ClearCheck(1, index1, index2);
                    GameManager.instance.userChoice = null;
                    GameManager.instance.dayCount++;
                    GameManager.instance.dialogCount = 1;
                    GameManager.instance.IsLoading = true;
                    SceneManager.LoadScene("main");
                }
                else
                {
                    checkIndex = 2;
                    if (SceneManager.GetActiveScene().name[0] == 'h')
                        GameManager.instance.ClearCheck(2, index1, index2);
                    else if (SceneManager.GetActiveScene().name[0] == 'b')
                        GameManager.instance.ClearCheck(2, 4, index2);
                    else if (SceneManager.GetActiveScene().name[0] == 'n')
                        GameManager.instance.ClearCheck(2, 3, index2);
                    GameManager.instance.dialogCount = 1;
                    GameManager.instance.IsLoading = true;
                    SceneManager.LoadScene("title");
                }
                
            }
            else
            {
               GameManager.instance.IsLoading= true;
                GameManager.instance.userChoice = null;
                if (GameManager.instance.selectedHeroine == 1)
                    SceneManager.LoadScene("ep0_jinyein");
                else if (GameManager.instance.selectedHeroine == 2)
                    SceneManager.LoadScene("ep0_freyja");
                else if (GameManager.instance.selectedHeroine == 3)
                    SceneManager.LoadScene("ep0_ru");
            }
        }  
        if (GameManager.instance.printSetting == 0 && GameManager.instance.IsLoading == false && (GameManager.instance.userChoice == tempDialog.choiceIndex || GameManager.instance.userChoice == null || tempDialog.choiceline == "-"))
            yield return new WaitForSeconds(3f);
        gotoNext = true;
        yield break;
    }
    public void ShowDialog()
    {
        dialog.text = "";
        name.text = "";
        dialogCnt= GameManager.instance.dialogCount;
        LogUpdate();
        StartCoroutine(PrintText());
    }
    public void PlayerNameDecide()
    {
        if (playerNameInputField.GetComponent<TMP_InputField>().text != "")
        {
            GameManager.instance.playerName = playerNameInputField.GetComponent<TMP_InputField>().text;
            Writeplayername.SetActive(false);
            ShowDialog();
        }

    }
    void LogUpdate()
    {
        for (int i = 1; i < dialogCnt; i++)
        {
            if (DBManager.instance.dialogueDic[i].choiceIndex == "-" || GameManager.instance.userChoice == DBManager.instance.dialogueDic[i].choiceIndex || DBManager.instance.dialogueDic[i].choiceIndex == "s" || DBManager.instance.dialogueDic[i].choiceIndex == "t")
            {
                GameObject logLinePrefab = Resources.Load<GameObject>("Logline"); // 프리펩 받아옴
                GameObject logLineGo = GameObject.Instantiate(logLinePrefab, logContentRect); // 자식으로 만들기&클론 생성
                logLineList.Add(logLineGo);
                logLineGo.GetComponent<LogLine>().Count(logLineList.Count);
                Transform logLineText = logLineGo.transform.Find("Loglinetext"); // 프리펩의 텍스트 찾기
                Transform logNameText = logLineGo.transform.Find("Lognametext"); // 프리펩의 텍스트 찾기
                Dialogue tempDialog;
                if (DBManager.instance.dialogueDic.TryGetValue(i, out tempDialog))
                {
                    logLineText.GetComponent<TextMeshProUGUI>().text = tempDialog.line; // 텍스트 내용 변환
                    if (tempDialog.name == "Narration")
                        logNameText.GetComponent<TextMeshProUGUI>().text = "";
                    else if (tempDialog.name == "Player")
                        logNameText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.playerName;
                    else if (tempDialog.name == "Jin Yein")
                        logNameText.GetComponent<TextMeshProUGUI>().text = "진예인";
                    else if (tempDialog.name == "Freyja")
                        logNameText.GetComponent<TextMeshProUGUI>().text = "프레이야";
                    else if (tempDialog.name == "Ru")
                        logNameText.GetComponent<TextMeshProUGUI>().text = "루";
                }
            }
        }
    }
    public void BackLog(int selectedLogIndex)
    {
        Log.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f, -4.4107e-06f);
        for (int i = logLineList.Count - 1; i >= selectedLogIndex-1; i--)
        {
            GameObject delete = logLineList[i];
            logLineList.RemoveAt(i);
            Destroy(delete);
        }
        dialogCnt = selectedLogIndex;
        gotoNext=false;
        StartCoroutine(PrintText());
    }
    IEnumerator WaitForChoice()
    {
        string Atext = null;
        string Btext = null;
        string Ctext = null;
        int cnt = dialogCnt + 1;
        int Astart=0;
        int Bstart=0;
        int Cstart=0;
        while (Atext == null || Btext == null || Ctext == null)
        {
            if (DBManager.instance.dialogueDic[cnt].choiceIndex == "a" && DBManager.instance.dialogueDic[cnt].choiceline != "-")
            {
                Atext = DBManager.instance.dialogueDic[cnt].choiceline;
                Astart = cnt;
            }
            else if (DBManager.instance.dialogueDic[cnt].choiceIndex == "b" && DBManager.instance.dialogueDic[cnt].choiceline != "-")
            {
                Btext = DBManager.instance.dialogueDic[cnt].choiceline;
                Bstart = cnt;
            }
            else if (DBManager.instance.dialogueDic[cnt].choiceIndex == "c" && DBManager.instance.dialogueDic[cnt].choiceline != "-")
            {
                Ctext = DBManager.instance.dialogueDic[cnt].choiceline;
                Cstart = cnt;
            }
            cnt++;
        }
        ChoiceButton.SetActive(true);
        choiceText[0].text = Atext;
        choiceText[1].text = Btext;
        choiceText[2].text = Ctext;
        yield return new WaitUntil(() => GameManager.instance.userChoice != null);
        if(GameManager.instance.userChoice == "a")
            cnt = Astart;
        else if(GameManager.instance.userChoice=="b")
            cnt = Bstart;
        else if(GameManager.instance.userChoice=="c")
            cnt = Cstart;
        Dialogue tempDialog = DBManager.instance.dialogueDic[cnt];
        GameManager.instance.dialogCount = cnt;
        dialogCnt = cnt;
        GameObject logLinePrefab = Resources.Load<GameObject>("Logline"); // 프리펩 받아옴
        GameObject logLineGo = GameObject.Instantiate(logLinePrefab, logContentRect); // 자식으로 만들기&클론 생성
        logLineList.Add(logLineGo);
        logLineGo.GetComponent<LogLine>().Count(logLineList.Count);
        Transform logLineText = logLineGo.transform.Find("Loglinetext"); // 프리펩의 텍스트 찾기
        Transform logNameText = logLineGo.transform.Find("Lognametext");
        logLineText.GetComponent<TextMeshProUGUI>().text = tempDialog.line; // 텍스트 내용 변환
        if (tempDialog.name == "Narration")
        {
            name.text = "";
            logNameText.GetComponent<TextMeshProUGUI>().text = "";
        }
        else if (tempDialog.name == "Player")
        {
            name.text = GameManager.instance.playerName;
            logNameText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.playerName;
        }
        else if (tempDialog.name == "Jin Yein")
        {
            name.text = "진예인";
            logNameText.GetComponent<TextMeshProUGUI>().text = "진예인";
        }
        else if (tempDialog.name == "Freyja")
        {
            name.text = "프레이야";
            logNameText.GetComponent<TextMeshProUGUI>().text = "프레이야";
        }
        else if (tempDialog.name == "Ru")
        {
            name.text = "루";
            logNameText.GetComponent<TextMeshProUGUI>().text = "루";
        }
        if (tempDialog.characterIndex[0] == 1)
        {
            if (tempDialog.choiceIndex == "t" && SceneManager.GetActiveScene().name != "ep4_jinyein" || tempDialog.choiceIndex != "t" && SceneManager.GetActiveScene().name == "ep4_jinyein")
            {
                character.sprite = null;
                Color tempColor = character.color;
                tempColor.a = 0f;
                character.color = tempColor;
                character2.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                tempColor = character2.color;
                tempColor.a = 255f;
                character2.color = tempColor;
            }
            else
            {
                character2.sprite = null;
                Color tempColor = character2.color;
                tempColor.a = 0f;
                character2.color = tempColor;
                character.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                tempColor = character.color;
                tempColor.a = 255f;
                character.color = tempColor;
            }
        }
        else if (tempDialog.characterIndex[0] == 2)
        {
            character.sprite = FreyjaImageList[tempDialog.characterIndex[1]];
            Color tempColor = character.color;
            tempColor.a = 255f;
            character.color = tempColor;
        }
        else if (tempDialog.characterIndex[0] == 3)
        {
            character.sprite = RuImageList[tempDialog.characterIndex[1]];
            Color tempColor = character.color;
            tempColor.a = 255f;
            character.color = tempColor;
        }
        else if (tempDialog.characterIndex[0] == -1)
        {
            character.sprite = null;
            Color tempColor = character.color;
            tempColor.a = 0f;
            character.color = tempColor;
        }
        if (tempDialog.backgroundIndex != -1)
        {
            background.sprite = backgroundList[tempDialog.backgroundIndex];
            GameManager.instance.backgroundIndex = tempDialog.backgroundIndex;
        }
        GameManager.instance.relationship_level += tempDialog.relationship_level;
        dialog.text = "";
        for (int i = 0; i < tempDialog.line.Length; i++)
        {
            dialog.text += tempDialog.line[i]; // 딕셔너리에서 대사 번호로 대사 가져오기
            if (GameManager.instance.textPrintSpeed != 0)
                yield return new WaitForSeconds(0.5f / (float)GameManager.instance.textPrintSpeed);
            else
                yield return new WaitForSeconds(0.02f);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                dialog.text = tempDialog.line;
                break;
            }
        }
    }
    public void SelectA()
    {
        ChoiceButton.SetActive(false);
        if (SceneManager.GetActiveScene().name == "ep0")
            GameManager.instance.selectedHeroine = 1;
        GameManager.instance.userChoice = "a";
    }
    public void SelectB()
    {
        ChoiceButton.SetActive(false);
        if (SceneManager.GetActiveScene().name == "ep0")
            GameManager.instance.selectedHeroine = 2;
        GameManager.instance.userChoice = "b";
    }
    public void SelectC()
    {
        ChoiceButton.SetActive(false);
        if (SceneManager.GetActiveScene().name == "ep0")
            GameManager.instance.selectedHeroine = 3;
        GameManager.instance.userChoice = "c";
    }

}
