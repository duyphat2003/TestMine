using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{

    [SerializeField] AudioClip[] audioClips;
    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = false;
        source.PlayOneShot(audioClips[index]);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(waitForSound());
    }

     IEnumerator waitForSound()
    {
        //Wait Until Sound has finished playing
        while (source.isPlaying)
        {
            yield return null;
        }

        index++;
        if(index > audioClips.Length - 1)
        {
            index = 0;
        }
       //Auidio has finished playing, disable GameObject
        source.PlayOneShot(audioClips[index]);
    }
}
