using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Player;
public class Exit : Interactable
{
    [Header("UI Objects")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _returnButton;

    [SerializeField] private SpriteRenderer _doorSpriteRender;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _doorSoundOpen;
    [SerializeField] private AudioClip _doorSoundClosed;

    public void Start()
    {
        _canvas.gameObject.SetActive(false);
        _doorSpriteRender.enabled = false;
    }

    public override void Act()
    {
        _canvas.gameObject.SetActive(true);

        //Disable Player
        player.GetComponent<PlayerMovement>().CanMove = false;
        player.GetComponent<PlayerActions>().enabled = false;
        player.GetComponentInChildren<PlayerVisuals>().enabled = false;

        //Disables the "InfoBubble"
        _infoBubble.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_returnButton);
    }

    public override void StopActing()
    {
        _canvas.gameObject.SetActive(false);

        //Enable Player
        player.GetComponent<PlayerMovement>().CanMove = true;
        player.GetComponent<PlayerActions>().enabled = true;
        player.GetComponentInChildren<PlayerVisuals>().enabled = true;

        _eventSystem.SetSelectedGameObject(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Gives the player as reference to the door
            player = collision.gameObject;

            //Enables the "OpenDoor" Sprite renderer
            _doorSpriteRender.enabled = true;
            //Plays the "OpenDoor" SFX
            _audioSource.clip = _doorSoundOpen;
            if (!_audioSource.isPlaying)
                _audioSource.Play();

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

            //Disables the "OpenDoor" Sprite renderer
            _doorSpriteRender.enabled = false;

            //Plays the "ClosedDoor" SFX
            _audioSource.clip = _doorSoundClosed;
            if (!_audioSource.isPlaying)
                _audioSource.Play();

        }

    }
}
