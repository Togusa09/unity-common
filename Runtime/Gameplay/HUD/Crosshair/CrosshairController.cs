using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Anarks2.UnityCommon.Gameplay.HUD.Crosshair
{
    public class CrosshairController : MonoBehaviour
    {
       [Header("UI Toolkit")]
    [SerializeField] private PanelRenderer panelRenderer;

    [Header("Input System")]
    [SerializeField] private InputActionReference fireAction;

    [Header("Crosshair Spread")]
    [SerializeField] private float baseSpread = 10f;
    [SerializeField] private float recoilKick = 14f;
    [SerializeField] private float recoverySpeed = 12f;

    private VectorCrosshair _crosshair;
    private float _currentSpread;

    private void Awake()
    {
        if (panelRenderer == null)
            panelRenderer = GetComponent<PanelRenderer>();

        _currentSpread = baseSpread;
    }

    private void OnEnable()
    {
        if (panelRenderer != null)
            panelRenderer.RegisterUIReloadCallback(OnUIReload);

        if (fireAction != null && fireAction.action != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed += OnFirePerformed;
        }
    }

    private void OnDisable()
    {
        if (panelRenderer != null)
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        if (fireAction != null && fireAction.action != null)
        {
            fireAction.action.performed -= OnFirePerformed;
            fireAction.action.Disable();
        }

        _crosshair = null;
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        _crosshair = root?.Q<VectorCrosshair>();

        if (_crosshair != null)
            _crosshair.SetDynamicSpread(_currentSpread);
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        _currentSpread += recoilKick;
    }

    private void Update()
    {
        if (_crosshair == null)
            return;

        _currentSpread = Mathf.Lerp(_currentSpread, baseSpread, Time.deltaTime * recoverySpeed);
        _crosshair.SetDynamicSpread(_currentSpread);
    }
    }
}