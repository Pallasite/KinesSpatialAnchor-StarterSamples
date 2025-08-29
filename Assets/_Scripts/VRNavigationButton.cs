/*
 * VR Navigation Button
 * 
 * This script extends the Meta SDK ButtonController to provide navigation functionality
 * for the trial system. It works with ray casting, poke interactions, and hand tracking.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
    public class VRNavigationButton : ButtonController
    {
        [Header("Navigation Button Settings")]
        public UnityEvent OnButtonPressed = new UnityEvent();
        public AudioClip buttonPressSound;
        public float pressDepth = 0.02f;
        public float animationSpeed = 5.0f;
        
        [Header("Visual Feedback")]
        public Color normalColor = Color.white;
        public Color highlightColor = Color.cyan;
        public Color pressedColor = Color.green;
        public Renderer buttonRenderer;
        
        private AudioSource audioSource;
        private Vector3 initialPosition;
        private bool isPressed = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Setup audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && buttonPressSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = buttonPressSound;
            }
            
            // Get button renderer if not assigned
            if (buttonRenderer == null)
            {
                buttonRenderer = GetComponent<Renderer>();
            }
            
            // Store initial position
            initialPosition = transform.localPosition;
            
            // Subscribe to button state changes
            InteractableStateChanged += OnStateChanged;
        }
        
        private void OnStateChanged(InteractableStateArgs args)
        {
            switch (args.NewInteractableState)
            {
                case InteractableState.Default:
                    SetButtonColor(normalColor);
                    SetButtonPressed(false);
                    break;
                    
                case InteractableState.ProximityState:
                    SetButtonColor(highlightColor);
                    SetButtonPressed(false);
                    break;
                    
                case InteractableState.ContactState:
                    SetButtonColor(highlightColor);
                    SetButtonPressed(false);
                    break;
                    
                case InteractableState.ActionState:
                    SetButtonColor(pressedColor);
                    SetButtonPressed(true);
                    TriggerButtonPress();
                    break;
            }
        }
        
        private void SetButtonColor(Color color)
        {
            if (buttonRenderer != null)
            {
                buttonRenderer.material.color = color;
            }
        }
        
        private void SetButtonPressed(bool pressed)
        {
            if (isPressed == pressed) return;
            
            isPressed = pressed;
            
            // Animate button press
            Vector3 targetPosition = initialPosition;
            if (pressed)
            {
                targetPosition += Vector3.down * pressDepth;
            }
            
            StopAllCoroutines();
            StartCoroutine(AnimateToPosition(targetPosition));
        }
        
        private IEnumerator AnimateToPosition(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.localPosition;
            float elapsed = 0f;
            float duration = 1f / animationSpeed;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            
            transform.localPosition = targetPosition;
        }
        
        private void TriggerButtonPress()
        {
            // Play sound
            if (audioSource != null && buttonPressSound != null)
            {
                audioSource.PlayOneShot(buttonPressSound);
            }
            
            // Invoke button press event
            OnButtonPressed?.Invoke();
            
            Debug.Log($"[VRNavigationButton] {gameObject.name} pressed!");
        }
        
        public void SetButtonText(string text)
        {
            var textComponent = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            }
        }
        
        private void OnDestroy()
        {
            InteractableStateChanged -= OnStateChanged;
        }
    }
}