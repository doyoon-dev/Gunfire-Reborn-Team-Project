using Define;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MiniMapSprite : MonoBehaviour
{
    [SerializeField] MiniMapObjectType m_type;
    [SerializeField] SpriteRenderer m_renderer;
    public MiniMapObjectType Type => m_type;

    private void FixedUpdate()
    {
        if(Managers.Game.MiniMap!=null)
        {
            transform.rotation = Quaternion.LookRotation(Managers.Game.MiniMap.GetCamera.transform.up);
            transform.Rotate(90, 0, 0);
        }
    }
    public void Init()
    {
        switch (m_type)
        {
            case MiniMapObjectType.Player:
                break;
            case MiniMapObjectType.Monster:
                break;
            case MiniMapObjectType.Boss:
                break;
            case MiniMapObjectType.Shop:
                break;
            case MiniMapObjectType.Smith:
                break;
            case MiniMapObjectType.Door:
                break;
        }

    }
}