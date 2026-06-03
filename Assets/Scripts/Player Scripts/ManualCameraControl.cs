using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class ManualCameraControl : MonoBehaviour
{
    [SerializeField] CinemachineCamera virtualCamera;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public IEnumerator RepositionCamera(Transform target, float time, bool reposToPlayer, float focus = 10f)
    {
        virtualCamera.enabled = false;

        Debug.Log("Repositioning camera to " + target.name);
        float elapsedTime = 0f;
        Vector3 startingPosition = virtualCamera.transform.position;
        Vector3 targetPosition = target.position;
        float startingFov = mainCamera.orthographicSize;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            mainCamera.orthographicSize = Mathf.Lerp(startingFov, focus, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.transform.position = targetPosition;
        virtualCamera.Lens.OrthographicSize = focus;

        virtualCamera.enabled = true;
        virtualCamera.Follow = reposToPlayer ? target : null;
    }
}
