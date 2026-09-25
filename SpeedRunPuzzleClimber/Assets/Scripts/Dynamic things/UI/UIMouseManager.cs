using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class UIMouseManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private InputActionReference _mousePosition;

    [SerializeField] private InputActionReference _mouseDelta;
    [SerializeField] private int _sampleCount = 10;

    private Queue<float> _samples = new Queue<float>();
    private float _averageSpeed;

    [SerializeField] private float minScalar = 0;
    [SerializeField] private float maxScalar = 14;

    public float speedScalar;

    private void Update()
    {
        SetPosition();
        MouseDeltaSmooth();

    }


    private void SetPosition()
    {
        Vector2 screenPosition = _mousePosition.action.ReadValue<Vector2>();

        Vector3 worldPosition = _camera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, 10f)
        );

        transform.position = worldPosition;
    }

    private void MouseDeltaSmooth()
    {
        Vector2 delta = _mouseDelta.action.ReadValue<Vector2>();

        float speed = delta.magnitude;

        _samples.Enqueue(speed);

        if (_samples.Count > _sampleCount)
            _samples.Dequeue();

        float total = 0f;

        foreach (float sample in _samples)
            total += sample;

        _averageSpeed = total / _samples.Count;


        speedScalar = Mathf.Clamp(_averageSpeed / maxScalar, 0, 1);
    }
}
