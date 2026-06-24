using UnityEngine;
using UnityEngine.UI;

public class UI_ShopItemInfo : MonoBehaviour
{
    [SerializeField] Image m_icon;
    [SerializeField] Text m_name;
    [SerializeField] Text m_note;

    public void SetInfo(ShopItem item)
    {
        m_icon.sprite = item.Icon;
        m_name.text = item.Name;
        m_note.text = item.Note;
    }
}
