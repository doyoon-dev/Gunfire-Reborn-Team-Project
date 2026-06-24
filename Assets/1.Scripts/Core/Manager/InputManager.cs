using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

class KeyInfo
{
    public UserKey key = UserKey.End;
    public List<KeyCode> listKey = new List<KeyCode>();
    public bool down = false;
    public bool press = false;
    public bool up = false;
}

public class InputManager : MonoBehaviour
{
    private Dictionary<UserKey, KeyInfo> m_dicKeys = new Dictionary<UserKey, KeyInfo>();

    public Action KeyEvent = null;
    public Action<Define.Mouse> MouseEvent = null;

    public void Init()
    {
        m_dicKeys.Clear();

        AddKey(UserKey.Forward, KeyCode.W);
        AddKey(UserKey.Backward, KeyCode.S);
        AddKey(UserKey.Right, KeyCode.D);
        AddKey(UserKey.Left, KeyCode.A);
    }
    public void Clear()
    {
    }

    private bool GetKeyCode(List<KeyCode> _keyCode)
    {
        foreach (KeyCode code in _keyCode)
        {
            if (Input.GetKey(code) == true)
            {
                return true;
            }
        }

        return false;
    }
    public bool GetKey(UserKey p_key)
    {
        if (m_dicKeys.ContainsKey(p_key) == false) return false; 
        if (m_dicKeys[p_key].down == true || m_dicKeys[p_key].press == true)  return true;
        return false;
    }

    public bool GetKeyDown(UserKey _key)
    {
        if (m_dicKeys.ContainsKey(_key) == false) return false;
        if (m_dicKeys[_key].down == true)  return true; 
        return false;
    }

    public bool GetKeyUp(UserKey _key)
    { 
        if (m_dicKeys.ContainsKey(_key) == false) return false; 
        if (m_dicKeys[_key].up == true) return true; 
        return false;
    }
    public void RegisterKeyEvent(Action _keyEvent)
    {
        KeyEvent -= _keyEvent;
        KeyEvent += _keyEvent;
    }

    public void RegisterMouseEvent(Action<Define.Mouse> _mouseEvent)
    {
        MouseEvent -= _mouseEvent;
        MouseEvent += _mouseEvent;
    }

    public KeyCode GetKeyData(UserKey p_key)
    {
        return m_dicKeys[p_key].listKey[0];
    }

    public void ChangeKey(UserKey _userKey, KeyCode _keyCode)
    {
        if (m_dicKeys.ContainsKey(_userKey) == true)
        {
            int l_size = m_dicKeys[_userKey].listKey.Count;
            List<KeyCode> l_keys = m_dicKeys[_userKey].listKey;
            l_keys[0] = _keyCode;
        }
    }

    public void AddKey(UserKey _key, KeyCode _keycode)
    {
        if (m_dicKeys.ContainsKey(_key) == false)
        {
            m_dicKeys[_key] = new KeyInfo();
            m_dicKeys[_key].key = _key;
        }

        KeyInfo l_info = m_dicKeys[_key];

        if (IsListKeyCode(l_info.listKey, _keycode) == true)
        {
            return;
        }

        l_info.listKey.Add(_keycode);
    }
    public void DelKey(UserKey _key, KeyCode _keycode = KeyCode.None)
    {
        if (m_dicKeys.ContainsKey(_key) == false)
        {
            return;
        }

        KeyInfo l_info = m_dicKeys[_key];
        if (_keycode == KeyCode.None)
        {
            l_info.listKey.RemoveRange(0, l_info.listKey.Count);
        }
        else
        {
            l_info.listKey.Remove(_keycode);
        }
    }
    private bool IsListKeyCode(List<KeyCode> _listKey, KeyCode _keyCode)
    {
        foreach (KeyCode key in _listKey)
        {
            if (key == _keyCode)
            {
                return true;
            }
        }
        return false;
    }
}
