using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonUI : MonoBehaviour
{
    
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;

    BattleControllerScript target;

    float characterControllerHeight;

    Transform targetTransform;

    Renderer targetRenderer;

    CanvasGroup _canvasGroup;

    Vector3 targetPosition;

    void Awake()
    {

        _canvasGroup = this.GetComponent<CanvasGroup>();

        //this.transform.SetParent(GameObject.Find("UICanvas").GetComponent<Transform>(), false);
    }

    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }


        // Reflect the Player Health
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = (float)target.health / (float)target.maxHealth;
        }
    }

    void LateUpdate()
    {

        
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y = this.target.GetComponent<Collider>().bounds.size.z - .5f;

            //this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;

           this.transform.position = GameObject.Find("ThirdPersonCamera").GetComponent<Camera>().WorldToScreenPoint(targetPosition) + screenOffset;
        }

    }

    public void SetTarget(BattleControllerScript _target)
    {

        // Cache references for efficiency because we are going to reuse them.
        this.target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();


        CharacterController _characterController = this.target.GetComponent<CharacterController>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }

        if (playerNameText != null)
        {
            playerNameText.text = this.target.photonView.Owner.NickName;
        }
    }
}
