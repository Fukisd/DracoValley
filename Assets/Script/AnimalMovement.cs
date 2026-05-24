using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float changeDirectionInterval = 2f; // Sau bao lâu thì tự đổi hướng
    public bool canMove = true;

    [Header("Sprites")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    [Header("Boundary")]
    public Collider2D barnCollider;

    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetRandomDirection();
        
        // Đảm bảo con vật luôn nằm cùng mặt phẳng Z với Barn nếu có thể
        if (barnCollider != null)
        {
            Vector3 pos = transform.position;
            pos.z = barnCollider.transform.position.z;
            transform.position = pos;
        }
    }

    void Update()
    {
        // if (barnCollider == null) return;

        if (barnCollider == null || !canMove) return; 

        // 1. Di chuyển
        Vector3 nextPosition = transform.position + (Vector3)moveDirection * moveSpeed * Time.deltaTime;

        // 2. Kiểm tra xem vị trí TIẾP THEO có nằm trong chuồng không
        // Sử dụng hàm CheckInsideBounds tùy chỉnh để bỏ qua trục Z
        if (IsInsideBarn(nextPosition))
        {
            transform.position = nextPosition;
        }
        else
        {
            // Nếu sắp đi ra ngoài thì đổi hướng ngay lập tức
            SetRandomDirection();
        }

        // 3. Lâu lâu tự đổi hướng cho sinh động
        timer += Time.deltaTime;
        if (timer >= changeDirectionInterval)
        {
            SetRandomDirection();
            timer = 0;
        }
    }

    // Hàm kiểm tra vị trí có nằm trong Collider không (Chỉ tính X và Y)
    bool IsInsideBarn(Vector3 position)
    {
        // Ép vị trí kiểm tra về cùng trục Z của Collider để tránh sai lệch
        position.z = barnCollider.bounds.center.z;
        return barnCollider.bounds.Contains(position);
    }

    void SetRandomDirection()
    {
        int random = Random.Range(0, 4);
        switch (random)
        {
            case 0: moveDirection = Vector2.up; spriteRenderer.sprite = upSprite; break;
            case 1: moveDirection = Vector2.down; spriteRenderer.sprite = downSprite; break;
            case 2: moveDirection = Vector2.left; spriteRenderer.sprite = leftSprite; break;
            case 3: moveDirection = Vector2.right; spriteRenderer.sprite = rightSprite; break;
        }
    }
}