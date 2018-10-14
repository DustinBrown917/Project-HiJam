using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

    private static KeyCode[] VALID_KEYS =
    {
        KeyCode.A,
        KeyCode.B,
        KeyCode.C,
        KeyCode.D,
        KeyCode.E,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.I,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.N,
        KeyCode.O,
        KeyCode.P,
        KeyCode.Q,
        KeyCode.R,
        KeyCode.S,
        KeyCode.T,
        KeyCode.U,
        KeyCode.V,
        KeyCode.W,
        KeyCode.X,
        KeyCode.Y,
        KeyCode.Z,
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Minus,
        KeyCode.Equals,
        KeyCode.LeftBracket,
        KeyCode.RightBracket,
        KeyCode.Tab,
        KeyCode.Backslash,
        KeyCode.CapsLock,
        KeyCode.Semicolon,
        KeyCode.Quote,
        KeyCode.Return,
        KeyCode.LeftShift,
        KeyCode.RightShift,
        KeyCode.Comma,
        KeyCode.Period,
        KeyCode.Slash,
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftAlt,
        KeyCode.RightAlt,
        KeyCode.Space,
        KeyCode.Backspace,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow
    };

    [SerializeField] KeyCode desiredKeycode;
    [SerializeField] KeyCode pressedKey;
    [SerializeField] private GameObject textObject;

	// Use this for initialization
	void Start () {
        desiredKeycode = GetRandomValidKeyCode();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown) { HandleKeyPress(); }
	}

    private void HandleKeyPress()
    {
        foreach(KeyCode keyCode in VALID_KEYS)
        {
            if (Input.GetKeyDown(keyCode))
            {
                pressedKey = keyCode;
                CompareKeys();
                return;
            }
        }
    }

    private void CompareKeys()
    {
        if(desiredKeycode == pressedKey)
        {
            ClearAllText();
            AddText("Great job!");
        } else
        {
            AddText("You're a bloody dumb walnut, aren't you?");
        }
    }

    private KeyCode GetRandomValidKeyCode()
    {
        return VALID_KEYS[Random.Range(0, VALID_KEYS.Length)];
    }

    private void AddText(string text)
    {
        Text textBlock = Instantiate(textObject, transform).GetComponent<Text>();
        textBlock.text = text;
    }

    private void ClearAllText()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
