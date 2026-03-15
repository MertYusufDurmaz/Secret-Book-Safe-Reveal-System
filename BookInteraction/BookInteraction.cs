using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider))] // Bu scriptin çalışması için collider şart, otomatik ekler.
public class BookInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kitap çekildiğinde ortaya çıkacak obje (Kasa, gizli kapı vb.)")]
    public GameObject safeObject;

    [Header("Settings")]
    public float rotationSpeed = 2f;
    [Tooltip("Kitabın ne kadar eğileceği")]
    public Vector3 bookRotationOffset = new Vector3(0f, 20f, 0f);
    
    public float movementSpeed = 5f;
    [Tooltip("Kasanın ortaya çıktığında gideceği Y ekseni hedefi (Local)")]
    public float safeTargetLocalY = -6.245f;

    [Header("Events")]
    [Tooltip("Kitap çekildiğinde çalışacak olaylar (Örn: Görev tamamlama, ses çalma)")]
    public UnityEvent onBookActivated;

    private bool hasBeenActivated = false;
    private Vector3 initialSafePosition;
    private Quaternion initialBookRotation;
    private Quaternion targetBookRotation;
    private Collider bookCollider;

    void Start()
    {
        bookCollider = GetComponent<Collider>();

        if (safeObject != null)
        {
            initialSafePosition = safeObject.transform.localPosition;
        }

        // Kitabın rotasyonlarını hesapla (Hardcoded değer yerine değişkenden alıyoruz)
        initialBookRotation = transform.localRotation;
        targetBookRotation = initialBookRotation * Quaternion.Euler(bookRotationOffset);
    }

    void OnMouseDown()
    {
        if (hasBeenActivated || Time.timeScale == 0)
            return;

        ActivateBook();
    }

    private void ActivateBook()
    {
        hasBeenActivated = true;

        if (bookCollider != null)
            bookCollider.enabled = false;

        StartCoroutine(RotateBookSmoothly());
        StartCoroutine(RevealSafeSmoothly());

        // Kendi TaskManager'ın yerine UnityEvent kullandık.
        onBookActivated?.Invoke();
    }

    // --- SAVE SİSTEMİ İÇİN DURUM GERİ YÜKLEME ---
    public void RestoreState(bool isActivated)
    {
        if (isActivated)
        {
            hasBeenActivated = true;

            if (bookCollider != null) bookCollider.enabled = false;

            transform.localRotation = initialBookRotation;

            if (safeObject != null)
            {
                safeObject.SetActive(true);
                // Hardcoded değer yerine değişkeni kullanıyoruz
                Vector3 targetPos = new Vector3(
                    safeObject.transform.localPosition.x,
                    safeTargetLocalY,
                    safeObject.transform.localPosition.z
                );
                safeObject.transform.localPosition = targetPos;
            }

            Debug.Log("Kitap ve Kasa durumu geri yüklendi (Animasyonsuz).");
        }
    }

    public bool IsActivated()
    {
        return hasBeenActivated;
    }

    IEnumerator RotateBookSmoothly()
    {
        float timeElapsed = 0f;

        // --- İLERİ ---
        while (timeElapsed < 1f)
        {
            transform.localRotation = Quaternion.Slerp(initialBookRotation, targetBookRotation, timeElapsed / rotationSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = targetBookRotation;

        yield return new WaitForSeconds(0.5f);

        // --- GERİ ---
        timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            transform.localRotation = Quaternion.Slerp(targetBookRotation, initialBookRotation, timeElapsed / rotationSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = initialBookRotation;
    }

    IEnumerator RevealSafeSmoothly()
    {
        yield return new WaitForSeconds(0.5f);

        if (safeObject == null) yield break;

        Vector3 startPosition = safeObject.transform.localPosition;
        // Hardcoded değer yerine değişkeni kullanıyoruz
        Vector3 targetPosition = new Vector3(startPosition.x, safeTargetLocalY, startPosition.z);

        float timeElapsed = 0f;
        while (timeElapsed < 5f)
        {
            safeObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / movementSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        safeObject.transform.localPosition = targetPosition;
    }
}
