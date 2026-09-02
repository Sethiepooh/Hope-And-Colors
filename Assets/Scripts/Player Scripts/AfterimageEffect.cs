using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class AfterimageEffect : MonoBehaviour
{
    [SerializeField] GameObject afterImage;
    [SerializeField] GameObject player;
    [SerializeField] float fadeStep;


    [SerializeField] List<GameObject> afterImagePool = new List<GameObject>();
    List<GameObject> activeAfterImages = new List<GameObject>();
    SpriteRenderer playerSpriteRenderer;
    [SerializeField] Color initColor;
    [SerializeField] Color fadedColor;

    private void Awake()
    {
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
    }

    public void GenerateAfterimage()
    {
        GameObject targetImage = afterImage;
        SpriteRenderer spriteRenderer = targetImage.GetComponent<SpriteRenderer>();
        int imageCount = 0;

        foreach (GameObject image in afterImagePool.ToArray())
        {
            if(image.GetComponent<SpriteRenderer>().color.a > 0)
            {
                imageCount++;
                if(imageCount == afterImagePool.Count)
                {
                    targetImage = Instantiate(afterImage);
                    afterImagePool.Add(targetImage);
                }
                continue;
            }
            else
            {
                targetImage = image;               
                break;
            }
        }

        spriteRenderer = targetImage.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        targetImage.transform.position = player.transform.position;
        targetImage.transform.localScale = player.transform.localScale;
        spriteRenderer.color = initColor;
        activeAfterImages.Add(targetImage);
    }

    private void Update()
    {
        if (activeAfterImages.Count == 0) return;
        foreach (GameObject image in activeAfterImages.ToArray())
        {
            SpriteRenderer spriteRenderer = image.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - fadeStep);
            if(spriteRenderer.color.a <= 0)
            {
                spriteRenderer.color = fadedColor;
                activeAfterImages.Remove(image);
                continue;
            }
        }
    }
}
