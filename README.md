# Secret-Book-Safe-Reveal-System

Secret Book & Safe Reveal System
Bu script, tıklanabilir bir kitap aracılığıyla gizli bir objeyi (örneğin bir kasayı) ortaya çıkaran animasyonlu bir etkileşim sistemidir. Kendi kayıt (save) sisteminize kolayca entegre edilebilir bir yapıya sahiptir.

Özellikler:

Kitabın öne yatıp geri çekilme animasyonu (Kod üzerinden Slerp ile).

Gizli objenin belirlenen konuma pürüzsüzce çıkması.

Save/Load sistemleri için RestoreState metodu.

Kurulum:

BookInteraction.cs scriptini etkileşime girilecek kitabın (üzerinde Collider olan) üzerine atın.

Inspector'dan Safe Object kısmına ortaya çıkacak gizli kasayı sürükleyin.

Safe Target Local Y değerini, kasanın ulaşmasını istediğiniz yüksekliğe göre ayarlayın.

On Book Activated event'ine varsa kendi görev sisteminizi veya ses efektlerinizi bağlayın.
