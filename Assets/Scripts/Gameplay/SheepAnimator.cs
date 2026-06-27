using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SheepAnimator : MonoBehaviour
{
    public float frameRate = 0.15f; // Thời gian đổi khung hình (giây)

    private SpriteRenderer sr;
    private Player player;
    private NPCWander npc;

    private Sprite[] leftSprites = new Sprite[2];
    private Sprite[] rightSprites = new Sprite[2];
    private Sprite[] downSprites = new Sprite[2];
    private Sprite[] upSprites = new Sprite[3];

    private enum Direction { Down, Up, Left, Right }
    private Direction lastDirection = Direction.Down;
    
    private int currentFrame = 0;
    private float animationTimer = 0f;
    private bool isMoving = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
        npc = GetComponent<NPCWander>();

        LoadSprites();
    }

    void LoadSprites()
    {
        // Tải các sprite theo đúng tên file thực tế trong Assets/Resources/Sheep
        leftSprites[0] = Resources.Load<Sprite>("Sheep/s5ToTheLeft");
        leftSprites[1] = Resources.Load<Sprite>("Sheep/s6ToTheLeft");

        rightSprites[0] = Resources.Load<Sprite>("Sheep/s3ToTheRight");
        rightSprites[1] = Resources.Load<Sprite>("Sheep/s4ToTheRight");

        downSprites[0] = Resources.Load<Sprite>("Sheep/s1DownwardEndDefault");
        downSprites[1] = Resources.Load<Sprite>("Sheep/s2Downward");

        upSprites[0] = Resources.Load<Sprite>("Sheep/s7GoUp");
        upSprites[1] = Resources.Load<Sprite>("Sheep/s8GoUp");
        upSprites[2] = Resources.Load<Sprite>("Sheep/s9GoUp");

        // Kiểm tra xem có lỗi tải file nào không
        CheckSprite(leftSprites[0], "s5ToTheLeft");
        CheckSprite(leftSprites[1], "s6ToTheLeft");
        CheckSprite(rightSprites[0], "s3ToTheRight");
        CheckSprite(rightSprites[1], "s4ToTheRight");
        CheckSprite(downSprites[0], "s1DownwardEndDefault");
        CheckSprite(downSprites[1], "s2Downward");
        CheckSprite(upSprites[0], "s7GoUp");
        CheckSprite(upSprites[1], "s8GoUp");
        CheckSprite(upSprites[2], "s9GoUp");

        // Gán sprite mặc định ban đầu là hướng nhìn xuống (2 xuống - s2Downward)
        if (downSprites[1] != null)
        {
            sr.sprite = downSprites[1];
        }
    }

    void CheckSprite(Sprite sprite, string name)
    {
        if (sprite == null)
        {
            Debug.LogError($"SheepAnimator: Không tìm thấy file Sheep/{name} trong thư mục Resources!");
        }
    }

    void Update()
    {
        Vector2 moveDir = Vector2.zero;

        // Lấy hướng di chuyển từ Player hoặc NPC
        if (player != null)
        {
            moveDir = player.MoveDirection;
        }
        else if (npc != null)
        {
            moveDir = npc.MoveDirection;
        }

        isMoving = moveDir.sqrMagnitude > 0.001f;

        if (isMoving)
        {
            // Cập nhật bộ đếm thời gian hoạt ảnh
            animationTimer += Time.deltaTime;
            if (animationTimer >= frameRate)
            {
                animationTimer = 0f;
                currentFrame++;
            }

            // Xác định hướng di chuyển chính và lặp khung hình tương ứng với độ dài mảng
            if (Mathf.Abs(moveDir.x) >= Mathf.Abs(moveDir.y))
            {
                sr.flipX = false; // Không cần lật do đã có sprite riêng cho trái/phải
                if (moveDir.x < 0)
                {
                    lastDirection = Direction.Left;
                    SetSprite(leftSprites[currentFrame % 2]);
                }
                else
                {
                    lastDirection = Direction.Right;
                    SetSprite(rightSprites[currentFrame % 2]);
                }
            }
            else
            {
                sr.flipX = false;
                if (moveDir.y > 0)
                {
                    lastDirection = Direction.Up;
                    SetSprite(upSprites[currentFrame % 3]);
                }
                else
                {
                    lastDirection = Direction.Down;
                    SetSprite(downSprites[currentFrame % 2]);
                }
            }
        }
        else
        {
            // Trạng thái đứng yên (Idle) - sử dụng khung hình mặc định tương ứng với hướng cuối cùng
            animationTimer = 0f;
            currentFrame = 0;
            sr.flipX = false;

            switch (lastDirection)
            {
                case Direction.Left:
                    SetSprite(leftSprites[1]); // 2 sang trái (s6ToTheLeft)
                    break;
                case Direction.Right:
                    SetSprite(rightSprites[1]); // 2 phải (s4ToTheRight)
                    break;
                case Direction.Up:
                    SetSprite(upSprites[2]); // 3 đi lên (s9GoUp)
                    break;
                case Direction.Down:
                default:
                    SetSprite(downSprites[1]); // 2 xuống (s2Downward)
                    break;
            }
        }
    }

    void SetSprite(Sprite newSprite)
    {
        if (newSprite != null)
        {
            sr.sprite = newSprite;
        }
    }
}
