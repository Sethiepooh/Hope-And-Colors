using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
    public Image blackFade;
    int fade = 0;
    bool restart = false;

    private void Start()
    {
        fade = 0;
    }

    private void Update()
    {
        switch(fade)
        {
            case 0:

                if (blackFade.color.a > 0)
                {
                    Color color = blackFade.color;
                    color.a -= Time.deltaTime / 2;
                    blackFade.color = color;
                    
                }
                else
                {
                    blackFade.gameObject.SetActive(false);
                    fade = 2;
                }
                break;
            case 1:
                if(blackFade.color.a < 1)
                {
                    if(!blackFade.gameObject.activeSelf)
                    {
                        blackFade.gameObject.SetActive(true);
                    }
                    Color color = blackFade.color;
                    color.a += Time.deltaTime / 2;
                    blackFade.color = color;
                }
                else
                {
                    if (restart)
                    {
                        SceneManager.LoadScene(0);
                        return;
                    }
                    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                    SceneManager.LoadScene(currentSceneIndex + 1);
                }
                break;
            case 2:
                break;
        }
    }
    public void StartGame()
    {
        restart = true;
        fade = 1;
    }

    public void LoadNextLevel()
    {
        fade = 1;
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
            LoadNextLevel();
        }
    }
}
