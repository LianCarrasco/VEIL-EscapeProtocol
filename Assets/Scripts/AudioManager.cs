using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;

    public AudioClip generadorDañadoSound;

    public AudioClip pantalla;
    public AudioClip cambio_camara;
    public AudioClip nariz_freddy;
    public AudioClip sonidoNormal;
  
    public AudioClip sonidoScreamer;
    public AudioClip generadorReparadoSound;

    public AudioClip sonidoLinternaNivel2;
    public AudioClip pasosIzquierda;
    public AudioClip pasosDerecha;
    public AudioClip pasosAtras;

    public AudioClip sonidoPuerta;
    public AudioClip linternaEncender;
    public AudioClip linternaAnimatronico;
    public AudioClip interferenciaCamara;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GeneradorDañado()
    {
        if (generadorDañadoSound != null)
        {
            audioSource.PlayOneShot(generadorDañadoSound);
        }
    }

    public void GeneradorReparado()
    {
        if (generadorReparadoSound != null)
        {
            audioSource.PlayOneShot(generadorReparadoSound);
        }
    }

    // Botones abrir/cerrar pantalla
    public void Boton()
    {
        audioSource.PlayOneShot(pantalla);
    }

    // Botón cámara
    public void Camara()
    {
        audioSource.PlayOneShot(cambio_camara);
    }

    // Botón llamar animatrónico
    public void LlamarAnimatronico()
    {
        audioSource.PlayOneShot(nariz_freddy);
    }

    // Botones normales
    public void NormalInter()
    {
        audioSource.PlayOneShot(sonidoNormal);
    }

    public void LinternaNivel2()
    {
        audioSource.PlayOneShot(sonidoLinternaNivel2);
    }

    public void Puerta()
    {
        audioSource.PlayOneShot(sonidoPuerta);
    }

    public void LinternaNormal()
    {
        audioSource.PlayOneShot(linternaEncender);
    }

    public void LinternaAnimatronico()
    {
        audioSource.PlayOneShot(linternaAnimatronico);
    }

    public void Interferencia()
    {
        audioSource.PlayOneShot(interferenciaCamara);
    }

    public void Screamer()
    {
        audioSource.PlayOneShot(sonidoScreamer);
    }

    public void PasosIzquierda()
    {
        audioSource.PlayOneShot(pasosIzquierda);
    }

    public void PasosDerecha()
    {
        audioSource.PlayOneShot(pasosDerecha);
    }

    public void PasosAtras()
    {
        audioSource.PlayOneShot(pasosAtras);
    }

}
