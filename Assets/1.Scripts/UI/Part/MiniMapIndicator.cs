using Define;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapIndicator : MonoBehaviour
{
    [SerializeField]Image m_image;
    MiniMapSprite m_connect = null;

    public MiniMapSprite Cunnet { get => m_connect;set=> m_connect = value; }
    public void SetIndicatorImage()
    {
        switch ((Layer)transform.parent.gameObject.layer)
        {
            case Layer.Monster:
                m_image.sprite = Managers.Resource.Load<Sprite>(Path.MiniMapIndicator_Sprite + 1);
                break;
            case Layer.Door:
                m_image.sprite = Managers.Resource.Load<Sprite>(Path.MiniMapIndicator_Sprite + 2);
                break;
            default:
                break;
        }
    }
}
