using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
public abstract class Interactable : MonoBehaviour
{
    //Player Ref
    protected GameObject player;
    //Interaction Check
    protected bool hasInteracted = false;

    [Header("Game Objects")]
    [SerializeField] protected GameObject _infoBubble;
    [Header("UI Library")]
    [SerializeField] protected UIButtonsSO _uIButtonsLibrary;

    public void Awake()
    {
        _infoBubble.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;

            //Enables the "InfoBubble"
            _infoBubble.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null)
        {
            StopActing();
            player = null;
            hasInteracted = false;
            //Disables the "InfoBubble"
            _infoBubble.SetActive(false);
        }

    }
    private void Update()
    {
        if(player != null)
        {
            UpdateUIButton();

            if (player.GetComponent<PlayerController>().Head && !hasInteracted)
            {
                Act();
                hasInteracted = true;
            }
        }
    }

    /// <summary>
    /// Acts with the Interactable
    /// </summary>
    public abstract void Act();

    /// <summary>
    /// Stops acting with the Interacable
    /// </summary>
    public abstract void StopActing();

    /// <summary>
    /// Update UI Button SpriteRenderer according to PlayerController.ControlScheme
    /// </summary>
    protected void UpdateUIButton()
    {
        switch (player.GetComponent<PlayerController>().ControlScheme)
        {
            case "Keyboard":
                _infoBubble.GetComponent<SpriteRenderer>().sprite =
                    _uIButtonsLibrary.keyboardSprites[(int)BODYPART.HEAD];
                break;
            case "Gamepad":
                _infoBubble.GetComponent<SpriteRenderer>().sprite =
                    _uIButtonsLibrary.gamepadSprites[(int)BODYPART.HEAD];
                break;
        }
    }
}
