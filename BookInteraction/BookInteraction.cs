using UnityEngine;
using System.Collections;

public class BookInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject safeObject;           // Kasa GameObject'i

    [Header("Settings")]
    public float rotationSpeed = 2f;        // Kitabın dönme hızı
    public float movementSpeed = 5f;        // Kasanın hareket hızı

    private bool hasBeenActivated = false;  // Tek seferlik çalışma kontrolü
    private Vector3 initialSafePosition;    // Kasanın başlangıç pozisyonu

    private Quaternion initialBookRotation; // Kitabın ilk rotasyonu
    private Quaternion targetBookRotation;  // Kitabın öne eğilmiş rotasyonu

    private Collider bookCollider;          // Kitabın collider'ı

    void Start()
    {
        // Collider referansı
        bookCollider = GetComponent<Collider>();

        // Kasanın ilk pozisyonu
        if (safeObject != null)
        {
            initialSafePosition = safeObject.transform.localPosition;
        }

        // Kitabın rotasyonları
        initialBookRotation = transform.localRotation;
        targetBookRotation = initialBookRotation * Quaternion.Euler(0f, 20f, 0f);
    }

    void OnMouseDown()
    {
        if (hasBeenActivated || Time.timeScale == 0)
            return;

        ActivateBook();
    }

    // Normal oynanışta tetiklenen fonksiyon
    private void ActivateBook()
    {
        hasBeenActivated = true;

        if (bookCollider != null)
            bookCollider.enabled = false;

        StartCoroutine(RotateBookSmoothly());
        StartCoroutine(RevealSafeSmoothly());

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.CompleteTask("task_trigger_secret_safe");
            Debug.Log("Görev tetiklendi");
        }
        else
        {
            Debug.LogError("TaskManager Instance NULL!");
        }
    }


    // --- SAVE SİSTEMİ İÇİN DURUM GERİ YÜKLEME ---
    public void RestoreState(bool isActivated)
    {
        if (isActivated)
        {
            hasBeenActivated = true; // Durumu "kullanıldı" yap

            // Collider'ı kapat (Tekrar tıklanamasın)
            if (bookCollider != null) bookCollider.enabled = false;

            // Kitap animasyonunun bitmiş hali (Başlangıç rotasyonu)
            transform.localRotation = initialBookRotation;

            // Kasa animasyonunun bitmiş hali (Açık pozisyon)
            if (safeObject != null)
            {
                safeObject.SetActive(true);
                // RevealSafeSmoothly coroutine'indeki hedef pozisyonu manuel veriyoruz:
                Vector3 targetPos = new Vector3(
                    safeObject.transform.localPosition.x,
                    -6.245f,
                    safeObject.transform.localPosition.z
                );
                safeObject.transform.localPosition = targetPos;
            }

            Debug.Log("Kitap ve Kasa durumu geri yüklendi (Animasyonsuz).");
        }
    }

    // SaveManager kaydederken bunu çağırır
    public bool IsActivated()
    {
        return hasBeenActivated;
    }

    // Kitap ileri → bekle → geri
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

    // Kasayı yukarı çıkar
    IEnumerator RevealSafeSmoothly()
    {
        yield return new WaitForSeconds(0.5f);

        if (safeObject == null) yield break;

        Vector3 startPosition = safeObject.transform.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, -6.245f, startPosition.z);

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