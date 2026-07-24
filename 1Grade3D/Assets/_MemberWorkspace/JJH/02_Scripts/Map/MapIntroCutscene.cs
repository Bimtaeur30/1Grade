using _MemberWorkspace.JJW.Asset._02_Script.Events;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class MapIntroCutscene : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;

        [Header("Raining")]
        [SerializeField] private ParticleSystem rainParticle;
        [SerializeField] private Light sunLight;
        [SerializeField] private float rainyIntensity = 0.3f;
        [SerializeField] private Color rainyColor = new Color(0.7f, 0.75f, 0.8f);

        [Header("Camera")]
        [SerializeField] private CinemachineCamera introCamera;
        [SerializeField] private CinemachineCamera playerCamera;
        [SerializeField] private Transform rotatePivot;

        [Header("Rotate")]
        [SerializeField] private float rotateAngle = 180f;
        [SerializeField] private float rotateDuration = 5f;

        [Header("Spawn")]
        [SerializeField] private float spawnDelay = 0.15f;
        [SerializeField] private float endDelay = 1f;
        [SerializeField] private float minSpawnDelay = 0.05f;
        [SerializeField] private float maxSpawnDelay = 0.25f;

        private float defaultIntensity;
        private Color defaultColor;

        private void Awake()
        {
            defaultIntensity = sunLight.intensity;
            defaultColor = sunLight.color;
        }

        public void Play()
        {
            StartCoroutine(CutsceneRoutine());
        }

        private IEnumerator CutsceneRoutine()
        {
            rainParticle.Play();
            StartCoroutine(LerpLight(rainyIntensity, rainyColor, 1.5f));
            introCamera.Priority = 20;
            playerCamera.Priority = 10;

            Coroutine rotate = StartCoroutine(Rotate());

            foreach (GroundTile tile in mapGenerator.ItemTiles)
            {
                GroundItem item = tile.SpawnGroundItem();

                if (item != null)
                    item.PlayDropAnimation();

                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            }

            yield return rotate;

            yield return new WaitForSeconds(endDelay);

            rainParticle.Stop();
            yield return StartCoroutine(LerpLight(defaultIntensity, defaultColor, 1.5f));
            introCamera.Priority = 10;
            playerCamera.Priority = 20;

            mapGenerator.FlowChannel.RaiseEvent(FlowEvent.StormEndEvent);
        }

        private IEnumerator Rotate()
        {
            Quaternion startRotation = rotatePivot.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(0f, rotateAngle, 0f);

            float elapsed = 0f;

            while (elapsed < rotateDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / rotateDuration);

                rotatePivot.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                yield return null;
            }

            rotatePivot.rotation = targetRotation;
        }

        private IEnumerator LerpLight(float targetIntensity, Color targetColor, float duration)
        {
            float startIntensity = sunLight.intensity;
            Color startColor = sunLight.color;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                t = Mathf.SmoothStep(0f, 1f, t);

                sunLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                sunLight.color = Color.Lerp(startColor, targetColor, t);

                yield return null;
            }

            sunLight.intensity = targetIntensity;
            sunLight.color = targetColor;
        }
    }
}