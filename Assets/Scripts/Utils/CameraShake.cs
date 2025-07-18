using UnityEngine;
#if CINEMACHINE
using Cinemachine;
#endif
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance { get; private set; }

	[Header("Defaults")]
	[Tooltip("Fréquence de jitter en sec^-1.")]
	public float defaultFrequency = 25f;
	[Tooltip("Courbe de décroissance de l'intensité (0..1 temps normalisé).")]
	public AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
	[Tooltip("Utiliser Time.unscaledDeltaTime pour les shakes ?")]
	public bool useUnscaledTime = false;

	private Transform camTransform;
	private Vector3 originalLocalPos;

#if CINEMACHINE
    // Cinemachine (optionnel)
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin perlin;
    private float cinemachineOriginalAmplitude;
    private float cinemachineOriginalFrequency;
#endif

	private Coroutine shakeRoutine;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		camTransform = GetComponent<Camera>() ? transform : Camera.main?.transform;
		if (camTransform == null)
		{
			Debug.LogError("[CameraShake] Aucune caméra trouvée.");
		}
		else
		{
			originalLocalPos = camTransform.localPosition;
		}

#if CINEMACHINE
        vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam == null && Camera.main != null)
            vcam = Camera.main.GetComponent<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin != null)
            {
                cinemachineOriginalAmplitude = perlin.m_AmplitudeGain;
                cinemachineOriginalFrequency = perlin.m_FrequencyGain;
            }
        }
#endif
	}

	private void OnDisable()
	{
		ResetCam();
	}

	public void Shake(float intensity, float duration)
	{
		Shake(intensity, duration, defaultFrequency);
	}
	public void Shake(float intensity, float duration, float frequency)
	{
		if (shakeRoutine != null) StopCoroutine(shakeRoutine);
		shakeRoutine = StartCoroutine(ShakeRoutine(intensity, duration, frequency));
	}

	private IEnumerator ShakeRoutine(float intensity, float duration, float frequency)
	{
		float elapsed = 0f;

#if CINEMACHINE
        bool usingCinemachine = perlin != null;
        if (usingCinemachine)
        {
            perlin.m_AmplitudeGain = intensity;
            perlin.m_FrequencyGain = frequency;
        }
#endif

		while (elapsed < duration)
		{
			float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
			elapsed += dt;

			float tNorm = Mathf.Clamp01(elapsed / duration);
			float falloff = falloffCurve.Evaluate(tNorm);

#if CINEMACHINE
            if (perlin != null)
            {
                perlin.m_AmplitudeGain = intensity * falloff;

            }
            else
#endif
			{
				if (camTransform != null)
				{

					float sampleX = (Random.value * 2f - 1f);
					float sampleY = (Random.value * 2f - 1f);
					float sampleZ = (Random.value * 2f - 1f);

					Vector3 offset = new Vector3(sampleX, sampleY, sampleZ) * intensity * falloff * 0.1f;
					camTransform.localPosition = originalLocalPos + offset;
				}
			}
			yield return null;
		}

		ResetCam();
		shakeRoutine = null;
	}

	private void ResetCam()
	{
#if CINEMACHINE
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = cinemachineOriginalAmplitude;
            perlin.m_FrequencyGain = cinemachineOriginalFrequency;
        }
#endif
		if (camTransform != null)
			camTransform.localPosition = originalLocalPos;
	}
}