using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float floatSpeed = 50f;
    public float lifetime = 1f;

    private TMP_Text text;
    private RectTransform rect;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.unscaledDeltaTime;
        lifetime -= Time.unscaledDeltaTime;
        if (lifetime <= 0f) Destroy(gameObject);
    }

    public void SetDamage(int amount)
    {
        if (text != null)
            text.text = "-" + amount;
    }
}
