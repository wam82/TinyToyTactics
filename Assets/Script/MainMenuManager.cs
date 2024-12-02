using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MainMenuManager : MonoBehaviour
{
    private UIDocument _document;

    private Button _startButton;


    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _startButton = _document.rootVisualElement.Q("PlayButton") as Button;
        _startButton.RegisterCallback<ClickEvent>(OnStartClick);
    }

    private void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnStartClick);
    }

    private void OnStartClick(ClickEvent ce)
    {
        SceneManager.LoadScene("PlayArea");
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
