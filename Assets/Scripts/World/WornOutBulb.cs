using UnityEngine;

public class WornOutBulb : MonoBehaviour
{
    [SerializeField] private float minOnTime = 0.8f;
    [SerializeField] private float maxOnTime = 2.8f;
    [SerializeField] private float minOffTime = 0.08f;
    [SerializeField] private float maxOffTime = 0.35f;

    private Light bulbLight;
    private Renderer bulbRenderer;
    private float timer;
    private bool isOn = true;

    private void Awake()
    {
        bulbLight = GetComponent<Light>();
        bulbRenderer = GetComponentInChildren<Renderer>();
        timer = Random.Range(minOnTime, maxOnTime);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        isOn = !isOn;
        bulbLight.enabled = isOn;
        if (bulbRenderer != null)
            bulbRenderer.enabled = true;

        timer = isOn
            ? Random.Range(minOnTime, maxOnTime)
            : Random.Range(minOffTime, maxOffTime);
    }
}
