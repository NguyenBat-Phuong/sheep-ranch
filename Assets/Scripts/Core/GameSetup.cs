using UnityEngine;
using TMPro;

public class GameSetup : MonoBehaviour
{
    private static GameSetup instance;
    private PhysicsMaterial2D sheepPhysicsMaterial;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // Tránh tạo trùng lặp
        if (FindObjectOfType<GameSetup>() != null) return;

        GameObject manager = new GameObject("GameSetupManager");
        instance = manager.AddComponent<GameSetup>();
        instance.InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("GameSetup: Đang khởi tạo bản đồ và các thực thể...");

        // 1. Cấu hình Player
        SetupPlayer();

        // 2. Sinh các NPC Cừu
        SpawnNPCs(5);

        // 3. Thiết lập teleporter nếu tồn tại object Tele trong scene
        SetupTeleporter();
    }

    private void SetupPlayer()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            playerObj = new GameObject("Player");
            playerObj.transform.position = new Vector3(15.56f, 3.84f, 0f);
        }

        playerObj.tag = "Player";

        // Thiết lập hiển thị và vật lý giống với NPC
        SetupSheepGraphicsAndPhysics(playerObj, RigidbodyType2D.Dynamic);

        // Thiết lập logic Player di chuyển
        Player playerScript = playerObj.GetComponent<Player>();
        if (playerScript == null) playerScript = playerObj.AddComponent<Player>();
        playerScript.moveSpeed = 4f;

        // Thiết lập Animator hoạt ảnh hướng đi của cừu
        SheepAnimator animator = playerObj.GetComponent<SheepAnimator>();
        if (animator == null) animator = playerObj.AddComponent<SheepAnimator>();

        // Thiết lập thoại cho cừu
        SheepSpeech speech = playerObj.GetComponent<SheepSpeech>();
        if (speech == null) speech = playerObj.AddComponent<SheepSpeech>();

        // Thiết lập nhãn tên trên đầu
        CreateNameTag(playerObj, "Bạn");

        Debug.Log("GameSetup: Đã thiết lập xong nhân vật người chơi (Player).");
    }

private CircleCollider2D SetupSheepGraphicsAndPhysics(GameObject sheepObj, RigidbodyType2D bodyType = RigidbodyType2D.Dynamic)
    {
        // Thiết lập SpriteRenderer
        SpriteRenderer sr = sheepObj.GetComponent<SpriteRenderer>();
        if (sr == null) sr = sheepObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 9; // Hiển thị thấp hơn hàng rào (8)

        // Thiết lập Rigidbody2D
        Rigidbody2D rb = sheepObj.GetComponent<Rigidbody2D>();
        if (rb == null) rb = sheepObj.AddComponent<Rigidbody2D>();
        rb.bodyType = bodyType;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        rb.sharedMaterial = GetSheepPhysicsMaterial();

        // Thiết lập CircleCollider2D (Collider cho chân cừu)
        CircleCollider2D col = sheepObj.GetComponent<CircleCollider2D>();
        if (col == null) col = sheepObj.AddComponent<CircleCollider2D>();
        col.offset = new Vector2(0f, -0.2f);
        col.radius = 0.35f;

        return col;
    }

    private PhysicsMaterial2D GetSheepPhysicsMaterial()
    {
        if (sheepPhysicsMaterial != null)
            return sheepPhysicsMaterial;

        sheepPhysicsMaterial = new PhysicsMaterial2D("SheepNoBounce")
        {
            friction = 0.5f,
            bounciness = 0f
        };
        return sheepPhysicsMaterial;
    }

    private void SetupTeleporter()
    {
        GameObject teleObj = GameObject.Find("Tele");
        if (teleObj == null)
        {
            Debug.LogWarning("GameSetup: Không tìm thấy object 'Tele' trong scene.");
            return;
        }

        Teleporter teleporter = teleObj.GetComponent<Teleporter>();
        if (teleporter == null)
            teleporter = teleObj.AddComponent<Teleporter>();

        teleporter.teleportDistance = 3.5f;
        teleporter.safeMargin = 0.4f;
        teleporter.cooldown = 0.15f;

        Debug.Log("GameSetup: Đã gắn Teleporter lên object Tele.");
    }

    private void SpawnNPCs(int count)
    {
        Vector2 center = new Vector2(15.56f, 3.84f);
        float spawnRangeX = 5.0f;
        float spawnRangeY = 2.5f;

        for (int i = 0; i < count; i++)
        {
            GameObject npcObj = new GameObject("NPC_Sheep_" + (i + 1));
            
            // Đặt vị trí ngẫu nhiên quanh tâm nông trại
            float randomX = center.x + Random.Range(-spawnRangeX, spawnRangeX);
            float randomY = center.y + Random.Range(-spawnRangeY, spawnRangeY);
            npcObj.transform.position = new Vector3(randomX, randomY, 0f);

            // Thiết lập đồ họa và vật lý giống Player
            SetupSheepGraphicsAndPhysics(npcObj, RigidbodyType2D.Dynamic);

            // Thiết lập di chuyển tự do (Wander)
            NPCWander wander = npcObj.AddComponent<NPCWander>();
            wander.moveSpeed = 1.2f;
            wander.wanderRadius = 3.5f;
            wander.changeDirectionTime = 2.5f;

            // Thiết lập Animator hoạt ảnh
            npcObj.AddComponent<SheepAnimator>();

            // Thiết lập thoại cho cừu
            npcObj.AddComponent<SheepSpeech>();

            // Thiết lập nhãn tên ngẫu nhiên
            CreateNameTag(npcObj, "");

            Debug.Log($"GameSetup: Đã sinh NPC {npcObj.name} tại vị trí {npcObj.transform.position}");
        }
    }

    private void CreateNameTag(GameObject parent, string nameText)
    {
        GameObject tagObj = new GameObject("NameTag");
        tagObj.transform.parent = parent.transform;
        tagObj.transform.localPosition = new Vector3(0f, 0.8f, 0f); // Phía trên đầu cừu

        // Tạo TextMeshPro dạng 3D World Space Text
        TextMeshPro tm = tagObj.AddComponent<TextMeshPro>();
        tm.fontSize = 3.5f;
        tm.alignment = TextAlignmentOptions.Center;
        tm.color = Color.white;
        
        // Thiết lập thứ tự hiển thị (Sorting Order) để nằm trên cùng (cao hơn ground=7, fence=8, cừu=10)
        MeshRenderer mr = tagObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingOrder = 12;
        }

        // Thêm viền chữ đen để dễ nhìn trên nền cỏ xanh
        tm.outlineColor = Color.black;
        tm.outlineWidth = 0.2f;

        // Tải Font Asset mặc định của TMP từ Resources
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null)
        {
            tm.font = font;
        }

        // Đặt nhãn NameLabel
        NameLabel nameLabel = tagObj.AddComponent<NameLabel>();
        if (!string.IsNullOrEmpty(nameText))
        {
            nameLabel.characterName = nameText;
        }
    }
}
