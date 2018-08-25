using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] AudioClip[] clip;
    [SerializeField] int layerFilter = 0;
    [SerializeField] float triggerRadius = 5f;
    [SerializeField] bool isOneTimeOnly = true;

    [SerializeField] bool hasPlayed = false;
    AudioSource audioSource;

    void Start()
    {
        gameObject.layer = 0;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0.7f;
        audioSource.playOnAwake = false;
        audioSource.clip = clip[0];

        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = triggerRadius;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerFilter)
        {
            audioSource.clip = clip[GenerateRandomInt()];
            RequestPlayAudioClip();
        }
    }

    void RequestPlayAudioClip()
    {
        if (isOneTimeOnly && hasPlayed)
        {
            return;
        }
        else if (audioSource.isPlaying == false)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }

    int GenerateRandomInt() {
        return (int) Random.Range(0, clip.Length);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 255f, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}