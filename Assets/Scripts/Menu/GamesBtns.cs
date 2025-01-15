using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GamesBtns : MonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;

    [SerializeField]  Transform menuPanel;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject button = Instantiate(buttonPrefab);

            string slotStr;
            if (PlayerPrefs.HasKey("Slot" + i + 1))
            {
                slotStr = "Slot ";
                Debug.Log("debug partie " + slotStr);
                button.GetComponent<Button>().onClick.AddListener(
                    () => { loadGame(i + 1); }
                );
            }
            else
            {
                slotStr = "Empty slot ";
                Debug.Log("debug partie vide " + slotStr);
                button.GetComponent<Button>().onClick.AddListener(
                    () => { createGame(); }
                );
            }

            button.gameObject.name = "Slot" + (i + 1);
            GameObject textMesh = button.transform.GetChild(0).gameObject;
            textMesh.GetComponent<TextMeshProUGUI>().text = slotStr + (i + 1);

            button.transform.SetParent(menuPanel, false);
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            float decalage = rectTransform.rect.height * rectTransform.lossyScale.y;
            button.transform.Translate(0f, -i * decalage, 0f, Space.World);
        }
    }

    void loadGame(int i)
    {
        SceneManager.LoadScene("Game" + i);
    }

    void createGame()
    {
        SceneManager.LoadScene("NewGame");
    }
}
