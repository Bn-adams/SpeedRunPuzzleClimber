using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UISettingsMenu : MonoBehaviour
{
    [SerializeField] private SoundMixerManager _soundMixerManager;
    [SerializeField] private GameObject mainMenu;

    private bool controllerActive = false;
    private bool mouseActive = false;

    private VisualElement root;

    private Button backButton;
    private Button deleteButton;

    private Toggle Toggle1;
    private Toggle Toggle2;
    private Toggle Toggle3;

    private Slider Slider1;
    private Slider Slider2;
    private Slider Slider3;



    private float Slider1Value
    {
        get { return PlayerDataManager.Instance.GetMasterVolume(); }
        set { PlayerDataManager.Instance.SetMasterVolume(value); }
    }
    private float Slider2Value
    {
        get { return PlayerDataManager.Instance.GetMusicVolume(); }
        set { PlayerDataManager.Instance.SetMusicVolume(value); }
    }
    private float Slider3Value
    {
        get { return PlayerDataManager.Instance.GetSoundFXVolume(); }
        set { PlayerDataManager.Instance.SetSoundFXVolume(value); }
    }

    [SerializeField] private float _controllerSliderSpeed = 0.5f;


    private VisualElement[] _index = new VisualElement[8];

    [SerializeField] private int currentFocusing = 1;

    // Stores the vibration setting
    private bool vibrationEnabled = true;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("root null");
            return;
        }

        backButton = root.Q<Button>("BackButton");
        Debug.Log(backButton);
        deleteButton = root.Q<Button>("DeleteSavedDataButton");

        backButton.clicked += BackButtonPress;
        deleteButton.clicked += DeleteSavedDataPress;

        Toggle1 = root.Q<Toggle>("Checkbox1");
        Toggle2 = root.Q<Toggle>("Checkbox2");
        Toggle3 = root.Q<Toggle>("Checkbox3");

        Toggle1.RegisterValueChangedCallback(Toggle1Changed);
        Toggle2.RegisterValueChangedCallback(Toggle2Changed);
        Toggle3.RegisterValueChangedCallback(Toggle3Changed);

        Slider1 = root.Q<Slider>("VolumeSlider");
        Slider2 = root.Q<Slider>("MusicSlider");
        Slider3 = root.Q<Slider>("SoundFXSlider");

        Slider1.RegisterValueChangedCallback(evt => { ChangeVolume(1, evt.newValue); });
        Slider2.RegisterValueChangedCallback(evt => { ChangeVolume(2, evt.newValue); });
        Slider3.RegisterValueChangedCallback(evt => { ChangeVolume(3, evt.newValue); });

        for (int i = 1; i < 7; i++)
        {
            _index[i] = root.Q<VisualElement>("Index" + i);
        }
        

        // Set slider values
        Slider1.value = Slider1Value;
        Slider2.value = Slider2Value;
        Slider3.value = Slider3Value;
    }

    

    private void Update()
    {
        // Check for mouse movement
        if (Mouse.current != null &&
            Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f &&
            !mouseActive)
        {
            mouseActive = true;
            controllerActive = false;

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            RemoveControllerFocus();
        }

        // Get current gamepad
        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        // Detect controller movement
        if (gamepad.leftStick.ReadValue().sqrMagnitude > 0.1f ||
            gamepad.dpad.ReadValue() != Vector2.zero ||
            gamepad.buttonSouth.wasPressedThisFrame)
        {
            if (!controllerActive)
            {
                controllerActive = true;
                mouseActive = false;

                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;

                if (currentFocusing != 0) FocusButton(_index[currentFocusing]);
            }
        }

        // If mouse is being used, don't process controller navigation
        if (!controllerActive)
            return;

        // --------- Button Presses ---------

        // Pressing south button
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            // Back Button
            if (currentFocusing == 0)
            {
                Debug.Log("on the back button");
                BackButtonPress();
            }

            // Vibration Toggle
            if (currentFocusing == 1)
            {
                Toggle1.value = !Toggle1.value;
            }

            // Invert Controls Toggle
            if (currentFocusing == 2)
            {
                Toggle2.value = !Toggle2.value;
            }

            // Fullscreen Toggle
            if (currentFocusing == 3)
            {
                Toggle3.value = !Toggle3.value;
            }

            // Delete Saved Data
            if (currentFocusing == 7)
            {
                DeleteSavedDataPress();
            }
        }

        // Holding south button for sliders
        if (gamepad.buttonSouth.isPressed)
        {
            if (currentFocusing == 4)
            {
                Slider1Value = ChangeSliderValue(Slider1, Slider1Value, gamepad);
                _soundMixerManager.MasterVolume = Slider1Value;
                Debug.Log(Slider1Value);
            }

            if (currentFocusing == 5)
            {
                Slider2Value = ChangeSliderValue(Slider2, Slider2Value, gamepad);
                _soundMixerManager.MusicVolume = Slider2Value;
            }

            if (currentFocusing == 6)
            {
                Slider3Value = ChangeSliderValue(Slider3, Slider3Value, gamepad);
                _soundMixerManager.SoundFXVolume = Slider3Value;
            }
        }
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            Debug.Log("east");
            BackButtonPress();
        }

            // --------- Controller Navigation ---------

            // Left press, goes to back button
        if ((gamepad.dpad.left.wasPressedThisFrame ||
             gamepad.leftStick.left.wasPressedThisFrame) &&
            !gamepad.buttonSouth.isPressed)
        {
            RemoveControllerFocus();

            backButton.AddToClassList("LevelButtonsFocus");
            currentFocusing = 0;
        }

        // Right press, goes to first toggle
        if ((gamepad.dpad.right.wasPressedThisFrame ||
             gamepad.leftStick.right.wasPressedThisFrame) &&
            currentFocusing == 0)
        {
            FocusButton(_index[1]);
            currentFocusing = 1;
        }

        // Up
        if (gamepad.dpad.up.wasPressedThisFrame ||
            gamepad.leftStick.up.wasPressedThisFrame)
        {
            if (currentFocusing > 1)
            {
                currentFocusing--;

                FocusButton(_index[currentFocusing]);
            }
        }

        // Down
        if (gamepad.dpad.down.wasPressedThisFrame ||
            gamepad.leftStick.down.wasPressedThisFrame)
        {
            currentFocusing++;
            RemoveControllerFocus();

            if (currentFocusing < 7)
            {
                FocusButton(_index[currentFocusing]);
            }
            else if (currentFocusing == 7)
            {
                deleteButton.AddToClassList("LevelButtonsFocus");
            }
            else
            {
                currentFocusing = 1;
                FocusButton(_index[currentFocusing]);
            }
        }
    }

    private float ChangeSliderValue( Slider slider, float sliderValue, Gamepad gamepad)
    {
        Vector2 stick = gamepad.leftStick.ReadValue();

        if (gamepad.dpad.left.isPressed || stick.x <= -0.05f)
        {
            sliderValue = Mathf.Clamp(
                sliderValue - _controllerSliderSpeed * Time.deltaTime,
                0.0001f,
                1
            );

            slider.value = sliderValue;
        }

        if (gamepad.dpad.right.isPressed || stick.x >= 0.05f)
        {
            sliderValue = Mathf.Clamp(
                sliderValue + _controllerSliderSpeed * Time.deltaTime,
                0.0001f,
                1
            );

            slider.value = sliderValue;
        }
        return sliderValue;
    }

    private void FocusButton(VisualElement element)
    {
        RemoveControllerFocus();

        element.AddToClassList("SettingsFocused");
        element.AddToClassList("SettingsFocusedSlider");
    }

    private void RemoveControllerFocus()
    {
        backButton.RemoveFromClassList("LevelButtonsFocus");

        for (int i = 1; i <= 6; i++)
        {
            _index[i].RemoveFromClassList("SettingsFocused");
            _index[i].RemoveFromClassList("SettingsFocusedSlider");

        }

        deleteButton.RemoveFromClassList("LevelButtonsFocus");
    }

    private void BackButtonPress()
    {
        Debug.Log("backb");
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Toggle1Changed(ChangeEvent<bool> evt)
    {
        vibrationEnabled = evt.newValue;

        Debug.Log("Controller Vibration: " + vibrationEnabled);
    }

    private void Toggle2Changed(ChangeEvent<bool> evt)
    {
        Debug.Log("Invert Controls: " + evt.newValue);
    }

    private void Toggle3Changed(ChangeEvent<bool> evt)
    {
        Screen.fullScreen = evt.newValue;

        Debug.Log("Fullscreen: " + evt.newValue);
    }
    private void ChangeVolume(int slider, float v)
    {
        Debug.Log(v);
        if (slider == 1) _soundMixerManager.MasterVolume = v;
        if (slider == 2) _soundMixerManager.MusicVolume = v;
        if (slider == 3) _soundMixerManager.SoundFXVolume = v;
    }

    private void DeleteSavedDataPress()
    {
        PlayerDataManager.Instance.DeleteData();
    }
}