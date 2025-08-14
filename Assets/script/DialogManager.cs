using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

public class DialogManager : MonoBehaviour
{
    bool gotoNext=false; // 다음 대사로 넘어가야 하는지 판단하는 변수
    int dialogCnt; // 대사 번호
    [SerializeField] TextMeshProUGUI name; // 이름 text
    [SerializeField] TextMeshProUGUI dialog; // 대사 text
    [SerializeField] Image background;
    [SerializeField] Image character;
    [SerializeField] GameObject Clickarea;
    [SerializeField] List<Sprite> JinYeinImageList;
    [SerializeField] List<Sprite> FreyjaImageList;
    [SerializeField] List<Sprite> RuImageList;
    [SerializeField] List<Sprite> backgroundList;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] GameObject Writeplayername;
    bool playerNameSelect=true;
    void Start()
    {
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
        if (gotoNext && EventSystem.current.currentSelectedGameObject == Clickarea && GameManager.instance.printSetting == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) // 엔터키 혹은 좌클릭 시
            {
                dialogCnt++; // 다음 번호의 대사로
                gotoNext = false;
                StartCoroutine(PrintText());
            }
        }
        if(gotoNext && GameManager.instance.printSetting == 0)
        {
            dialogCnt++;
            gotoNext = false;
            StartCoroutine(PrintText());
        }
        
    }
    IEnumerator PrintText()
    {
        Dialogue tempDialog;
        if (DBManager.instance.dialogueDic.TryGetValue(dialogCnt, out tempDialog))
        {
            GameManager.instance.dialogCount = dialogCnt;
            dialog.text = tempDialog.line; // 딕셔너리에서 대사 번호로 대사 가져오기
            if (tempDialog.name == "Narration")
                name.text = "";
            else if (tempDialog.name == "Player")
                name.text = GameManager.instance.playerName;
            else if (tempDialog.name == "Jin Yein")
                name.text = "진예인";
            else if (tempDialog.name == "Freyja")
                name.text = "프레이야";
            else if (tempDialog.name == "Ru")
                name.text = "루";
            if (tempDialog.characterIndex[0] == 1)
            {
                character.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
                Color tempColor = character.color;
                tempColor.a = 255f;
                character.color = tempColor;
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
            else if (GameManager.instance.backgroundIndex != 0)
                background.sprite = backgroundList[GameManager.instance.backgroundIndex];
        }
        else
        {
            SceneManager.LoadScene("main");
        }  
        if (GameManager.instance.printSetting == 0)
            yield return new WaitForSeconds(3f);
        gotoNext = true;
        yield break;
    }
    public void ShowDialog()
    {
        dialog.text = "";
        name.text = "";
        dialogCnt= GameManager.instance.dialogCount;
        StartCoroutine(PrintText());
    }
    public void PlayerNameDecide()
    {
        GameManager.instance.playerName = playerNameInputField.GetComponent<TMP_InputField>().text;
        Writeplayername.SetActive(false);
        ShowDialog();
    }
}
