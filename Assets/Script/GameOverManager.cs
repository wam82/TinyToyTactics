using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameOverManager : MonoBehaviour
{
    private UIDocument _document;

    private Button _startButton;

    private Button _menuButton;


    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _startButton = _document.rootVisualElement.Q("RestartButton") as Button;
        _startButton.RegisterCallback<ClickEvent>(OnStartClick);
        _menuButton = _document.rootVisualElement.Q("MenuButton") as Button;
        _menuButton.RegisterCallback<ClickEvent>(OnMenuClick);
    }

    private void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnStartClick);
    }

    private void OnStartClick(ClickEvent ce)
    {
        SceneManager.LoadScene("PlayArea");
    }
    
    private void OnMenuClick(ClickEvent ce)
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
