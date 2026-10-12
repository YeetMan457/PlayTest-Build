using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartTransition : MonoBehaviour
{
    public Vector3 targetRotation = new Vector3(0, 0, 10);
    public Vector3 targetScale = new Vector3(3.7f, 3.7f, 1);
    public Vector3 targetPosition = new Vector3(-55, 530, 0);
    public float duration = 5f; // Time to complete in seconds

    private Quaternion startRotation;
    private Vector3 startScale;
    private float elapsedTime; 

    private Vector3 startPosition;

    private bool isAnimating;
    public bool targetReached;
    public bool transitionStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimating) return;

        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / duration);

        transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.Euler(targetRotation), t);

        transform.localScale = Vector3.Lerp(startScale,  targetScale, t);


        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

        if (t >= 1f)
        {
            transform.localRotation = Quaternion.Euler(targetRotation);
            transform.localScale = targetScale;
            transform.localPosition = targetPosition;

            isAnimating = false;
            targetReached = true;
            SceneManager.LoadScene("Game");
        }
    }

    public void Animate()
    {
        startRotation = transform.localRotation;
        startScale = transform.localScale;
        startPosition = transform.localPosition;

        elapsedTime = 0f;
        isAnimating = true;
        transitionStarted = true;
        targetReached = false;
    }
}
