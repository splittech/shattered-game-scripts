using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] AudioClip buttonHoverAudio;

    [Header("Player")]
    [SerializeField] PlayerMovement playerController;
    [SerializeField] AudioClip playerDashAudio;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (playerController != null)
        {
            playerController.OnPlayerMovementStateChanged += PlayDashSFX;
        }
    }

    private void PlayDashSFX(PlayerMovement.MovementStates movementState)
    {
        if (movementState == PlayerMovement.MovementStates.Dashing)
        {
            PlaySFX(playerDashAudio);
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
