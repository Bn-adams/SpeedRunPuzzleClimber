using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UISettingsMenu : MonoBehaviour
{
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

    private float slider1Value = 50;
    private float slider2Value = 50;
    private float slider3Value = 50;

    private VisualElement[] Index = new VisualElement[8];

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

        Toggle1 = root.Q<Toggle>("ControllerVibrationToggle");
        Toggle2 = root.Q<Toggle>("InvertControllsToggle");
        Toggle3 = root.Q<Toggle>("FullscreenToggle");

        Toggle1.RegisterValueChangedCallback(Toggle1Changed);
        Toggle2.RegisterValueChangedCallback(Toggle2Changed);
        Toggle3.RegisterValueChangedCallback(Toggle3Changed);

        Slider1 = root.Q<Slider>("VolumeSlider");
        Slider2 = root.Q<Slider>("MusicSlider");
        Slider3 = root.Q<Slider>("SoundFXSlider");

        //for (int i = 1; i < 8; i++)
        //{
        //    Index[i] = root.Q<VisualElement>("Index" + i);
        //}

        // Set slider values
        Slider1.value = slider1Value;
        Slider2.value = slider2Value;
        Slider3.value = slider3Value;
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

                FocusButton(Index[currentFocusing]);
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
                ChangeSliderValue(
                    Slider1,
                    ref slider1Value,
                    gamepad
                );
            }

            if (currentFocusing == 5)
            {
                ChangeSliderValue(
                    Slider2,
                    ref slider2Value,
                    gamepad
                );
            }

            if (currentFocusing == 6)
            {
                ChangeSliderValue(
                    Slider3,
                    ref slider3Value,
                    gamepad
                );
            }
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
            FocusButton(Index[1]);
            currentFocusing = 1;
        }

        // Up
        if (gamepad.dpad.up.wasPressedThisFrame ||
            gamepad.leftStick.up.wasPressedThisFrame)
        {
            if (currentFocusing > 1)
            {
                currentFocusing--;

                FocusButton(Index[currentFocusing]);
            }
        }

        // Down
        if (gamepad.dpad.down.wasPressedThisFrame ||
            gamepad.leftStick.down.wasPressedThisFrame)
        {
            if (currentFocusing < 7)
            {
                currentFocusing++;

                FocusButton(Index[currentFocusing]);

                if (currentFocusing == 7)
                {
                    deleteButton.AddToClassList("LevelButtonsFocus");
                }
            }
        }
    }

    private void ChangeSliderValue(
        Slider slider,
        ref float sliderValue,
        Gamepad gamepad)
    {
        Vector2 stick = gamepad.leftStick.ReadValue();

        if (gamepad.dpad.left.isPressed || stick.x <= -0.05f)
        {
            sliderValue = Mathf.Clamp(
                sliderValue - 15f * Time.deltaTime,
                0,
                100
            );

            slider.value = sliderValue;
        }

        if (gamepad.dpad.right.isPressed || stick.x >= 0.05f)
        {
            sliderValue = Mathf.Clamp(
                sliderValue + 15f * Time.deltaTime,
                0,
                100
            );

            slider.value = sliderValue;
        }
    }

    private void FocusButton(VisualElement element)
    {
        RemoveControllerFocus();

        element.AddToClassList("SettingsFocused");
    }

    private void RemoveControllerFocus()
    {
        //backButton.RemoveFromClassList("LevelButtonsFocus");

        //for (int i = 1; i < 8; i++)
        //{
        //    Index[i].RemoveFromClassList("SettingsFocused");
        //}

        //deleteButton.RemoveFromClassList("LevelButtonsFocus");
    }

    private void BackButtonPress()
    {
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

    private void DeleteSavedDataPress()
    {
        Debug.Log("Delete Saved Data Button Pressed");
    }
}