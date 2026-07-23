using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class MapIntroCutscene : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;

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

        public void Play()
        {
            StartCoroutine(CutsceneRoutine());
        }

        private IEnumerator CutsceneRoutine()
        {
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

            introCamera.Priority = 10;
            playerCamera.Priority = 20;
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
    }
}