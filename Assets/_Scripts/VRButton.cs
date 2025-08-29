using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A simple VR button that works with both hand tracking and controllers.
/// Uses Unity UI button for compatibility and adds VR-specific interaction zones.
/// </summary>
public class VRButton : MonoBehaviour
{
    [Header("Button Configuration")]
    [SerializeField] private Button _uiButton;
    [SerializeField] private bool _useVRInteraction = true;
    [SerializeField] private float _pressDistance = 0.02f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject _buttonVisual;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoveredColor = Color.cyan;
    [SerializeField] private Color _pressedColor = Color.blue;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hoverSound;
    [SerializeField] private AudioClip _pressSound;
    
    [Header("Interaction Zones")]
    [SerializeField] private Collider _proximityZone;
    [SerializeField] private Collider _contactZone;
    
    private MeshRenderer _buttonRenderer;
    private Vector3 _originalPosition;
    private Vector3 _pressedPosition;
    private bool _isHovered = false;
    private bool _isPressed = false;
    private int _interactingTools = 0;
    
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;
    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;
    
    private void Awake()
    {
        if (_uiButton == null)
            _uiButton = GetComponent<Button>();
            
        if (_buttonVisual == null)
            _buttonVisual = gameObject;
            
        _buttonRenderer = _buttonVisual.GetComponent<MeshRenderer>();
        _originalPosition = _buttonVisual.transform.localPosition;
        _pressedPosition = _originalPosition - Vector3.forward * _pressDistance;
        
        SetupInteractionZones();
        SetButtonColor(_normalColor);
    }
    
    private void SetupInteractionZones()
    {
        // Setup proximity zone for hover detection
        if (_proximityZone == null)
        {
            GameObject proximityObj = new GameObject("ProximityZone");
            proximityObj.transform.parent = transform;
            proximityObj.transform.localPosition = Vector3.zero;
            proximityObj.transform.localRotation = Quaternion.identity;
            proximityObj.transform.localScale = Vector3.one * 1.2f;
            
            _proximityZone = proximityObj.AddComponent<SphereCollider>();
            _proximityZone.isTrigger = true;
            
            var proximityTrigger = proximityObj.AddComponent<VRButtonTrigger>();
            proximityTrigger.SetButton(this);
            proximityTrigger.SetTriggerType(VRButtonTrigger.TriggerType.Proximity);
        }
        
        // Setup contact zone for press detection
        if (_contactZone == null)
        {
            GameObject contactObj = new GameObject("ContactZone");
            contactObj.transform.parent = transform;
            contactObj.transform.localPosition = Vector3.zero;
            contactObj.transform.localRotation = Quaternion.identity;
            contactObj.transform.localScale = Vector3.one;
            
            _contactZone = contactObj.AddComponent<BoxCollider>();
            _contactZone.isTrigger = true;
            
            var contactTrigger = contactObj.AddComponent<VRButtonTrigger>();
            contactTrigger.SetButton(this);
            contactTrigger.SetTriggerType(VRButtonTrigger.TriggerType.Contact);
        }
    }
    
    public void OnProximityEnter()
    {
        if (!_isHovered)
        {
            _isHovered = true;
            SetButtonColor(_hoveredColor);
            PlaySound(_hoverSound);
            OnHoverEnter.Invoke();
        }
    }
    
    public void OnProximityExit()
    {
        if (_isHovered && _interactingTools <= 0)
        {
            _isHovered = false;
            SetButtonColor(_normalColor);
            OnHoverExit.Invoke();
        }
    }
    
    public void OnContactEnter()
    {
        _interactingTools++;
        if (!_isPressed)
        {
            _isPressed = true;
            SetButtonColor(_pressedColor);
            _buttonVisual.transform.localPosition = _pressedPosition;
            PlaySound(_pressSound);
            OnButtonPressed.Invoke();
            
            // Also trigger the UI button if available
            if (_uiButton != null)
            {
                _uiButton.onClick.Invoke();
            }
        }
    }
    
    public void OnContactExit()
    {
        _interactingTools--;
        if (_isPressed && _interactingTools <= 0)
        {
            _isPressed = false;
            _buttonVisual.transform.localPosition = _originalPosition;
            
            if (_isHovered)
                SetButtonColor(_hoveredColor);
            else
                SetButtonColor(_normalColor);
                
            OnButtonReleased.Invoke();
        }
        
        // Check if we should exit hover state
        if (_interactingTools <= 0 && _isHovered)
        {
            OnProximityExit();
        }
    }
    
    private void SetButtonColor(Color color)
    {
        if (_buttonRenderer != null && _buttonRenderer.material != null)
        {
            _buttonRenderer.material.color = color;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
    
    // Public methods for setting up the button programmatically
    public void SetUIButton(Button uiButton)
    {
        _uiButton = uiButton;
    }
    
    public void AddClickListener(UnityAction action)
    {
        OnButtonPressed.AddListener(action);
        if (_uiButton != null)
        {
            _uiButton.onClick.AddListener(action);
        }
    }
}

/// <summary>
/// Helper component for detecting VR interaction with buttons.
/// Handles both hand tracking and controller input.
/// </summary>
public class VRButtonTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Proximity,
        Contact
    }
    
    private VRButton _button;
    private TriggerType _triggerType;
    
    public void SetButton(VRButton button)
    {
        _button = button;
    }
    
    public void SetTriggerType(TriggerType type)
    {
        _triggerType = type;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_button == null) return;
        
        // Check if the collider is from a hand or controller
        if (IsValidInteractor(other))
        {
            if (_triggerType == TriggerType.Proximity)
            {
                _button.OnProximityEnter();
            }
            else if (_triggerType == TriggerType.Contact)
            {
                _button.OnContactEnter();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_button == null) return;
        
        if (IsValidInteractor(other))
        {
            if (_triggerType == TriggerType.Proximity)
            {
                _button.OnProximityExit();
            }
            else if (_triggerType == TriggerType.Contact)
            {
                _button.OnContactExit();
            }
        }
    }
    
    private bool IsValidInteractor(Collider other)
    {
        // Check for OVR hand tracking components
        if (other.GetComponent<OVRHand>() != null)
            return true;
            
        // Check for OVR controller components
        if (other.GetComponent<OVRControllerHelper>() != null)
            return true;
            
        // Check for generic hand/controller tags
        if (other.CompareTag("Hand") || other.CompareTag("Controller"))
            return true;
            
        // Check for InteractableTool (from Meta SDK)
        if (other.GetComponent<OculusSampleFramework.InteractableTool>() != null)
            return true;
            
        // Check for common VR interaction component names
        var componentNames = new[] { "Hand", "Controller", "Finger", "InteractionTool", "PokeInteractor", "RayInteractor" };
        foreach (var name in componentNames)
        {
            if (other.name.Contains(name))
                return true;
        }
        
        return false;
    }
}