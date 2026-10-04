using UnityEngine;

public class PowerBarHandling : MonoBehaviour
{
    [SerializeField] SpriteSwitch[] bars;
    
    public void SwitchBar(bool b)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].SwitchSprite(b);
        }
    }
}
