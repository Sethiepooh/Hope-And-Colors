using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
    public Image blackFade;


    private void Start()
    {
        StartCoroutine(FadeScreen(true, false));
    }

    public IEnumerator FadeScreen(bool fadeIn, bool nextLevel)
    {
        if (fadeIn)
        {
            while (blackFade.color.a > 0)
            {
                Color color = blackFade.color;
                color.a -= Time.deltaTime / 2;
                blackFade.color = color;
                yield return null;
            }
            blackFade.gameObject.SetActive(false);
        }
        else
        {
            while (blackFade.color.a < 1)
            {
                if (!blackFade.gameObject.activeSelf)
                {
                    blackFade.gameObject.SetActive(true);
                }
                Color color = blackFade.color;
                color.a += Time.deltaTime / 2;
                blackFade.color = color;
                yield return null;
            }
        }
        if (nextLevel)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(FadeScreen(false, true));
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeScreen(false, true));
        }
    }
}
