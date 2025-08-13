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
    [SerializeField] InputField playerNameInputField;
    [SerializeField] GameObject Writeplayername;
    bool playerNameSelect=true;
    void Start()
    {
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
        if (GameManager.instance.printSetting == 0 && dialogCnt != 1)
            yield return new WaitForSeconds(3f);
        Dialogue tempDialog;
        if (DBManager.instance.dialogueDic.TryGetValue(dialogCnt, out tempDialog))
        {
            GameManager.instance.dialogCount=dialogCnt;
            dialog.text = tempDialog.line; // 딕셔너리에서 대사 번호로 대사 가져오기
            if (tempDialog.name == "Narration")
                name.text = "";
            else if(tempDialog.name == "Player")
                name.text = GameManager.instance.playerName;
            else
                name.text = tempDialog.name; // 딕셔너리에서 대사 번호로 이름 가져오기
            if (tempDialog.characterIndex[0] == 1)
                character.sprite = JinYeinImageList[tempDialog.characterIndex[1]];
            else if (tempDialog.characterIndex[0] == 2)
                character.sprite = FreyjaImageList[tempDialog.characterIndex[1]];
            else if (tempDialog.characterIndex[0] == 3)
                character.sprite = RuImageList[tempDialog.characterIndex[1]];
            background.sprite = backgroundList[tempDialog.backgroundIndex];
        }
        else
            SceneManager.LoadScene("main");
            gotoNext = true;
        yield break;
    }
    public void ShowDialog()
    {
        dialog.text = "";
        name.text = "";
        dialogCnt= GameManager.instance.dialogCount;
        Debug.Log(dialogCnt);
        StartCoroutine(PrintText());
    }
    public void PlayerNameDecide()
    {
        GameManager.instance.playerName = playerNameInputField.GetComponent<InputField>().text;
        Writeplayername.SetActive(false);
        ShowDialog();
    }
}
