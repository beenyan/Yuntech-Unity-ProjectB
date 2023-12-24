using UnityEngine;

public enum GemType {
    ATTACK_WOOD = 0,
    ATTACK_FIRE = 1,
    ATTACK_WATER = 2,
    DEFENSE = 3,
    HEAL = 4,
}

public class Gem: MonoBehaviour {
    private Vector2 Pos;
    private Vector2 StartPos;
    private float StartTick = 0.0f;
    private GemType Type;
    private bool IsSelect = false;
    private bool IsMoved = false;
    private static Sprite[] Sprites;
    private GameObject Parent;
    private GameController GameController;

    public Vector2 GetPos() {
        return Pos;
    }

    public GemType GetGemType() {
        return Type;
    }

    void Awake() {
        Sprites ??= Resources.LoadAll<Sprite>(Utils.Resources.GemMaterial.ToString());
    }

    public void Init(int y, int x, GameObject parent) {
        Parent = parent;
        GameController = Parent.GetComponent<GameController>();
        Type = Utils.RandomEnumValue<GemType>();
        gameObject.transform.SetParent(Parent.transform);
        SetPosition(new(x, y));

        while (!GameController.IsAvailable(GameController.SameTypeAround(this))) {
            Type = Utils.RandomEnumValue<GemType>();
        }

        var renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.sprite = Sprites[(int)Type];
        gameObject.transform.localPosition = CalcTargetPos(x, y);
    }

    void Update() {
        if (IsSelect) {
            transform.Rotate(new Vector3(0, 0, 1));
        }

        if (IsMoved) {
            StartTick += Time.deltaTime * 4;
            var newPos = Vector2.Lerp(StartPos, Pos, StartTick);
            gameObject.transform.localPosition = CalcTargetPos(newPos);
            // Move End
            if (StartTick >= 1.0f) {
                --GameController.MoveCount;
                IsMoved = false;
                StartTick = 0.0f;
            }
        }
    }

    private void SetPosition(Vector2 pos) {
        int y = (int)pos.y;
        int x = (int)pos.x;
        Pos = new(x, y);

        gameObject.name = $"({y}, {x})";
    }

    private Vector2 CalcTargetPos(float x, float y) {
        var scale = new Vector2(1 / GameController.MapSize.x, 1 / GameController.MapSize.y);
        gameObject.transform.localScale = scale;
        return new((x + 0.5f) * scale.x - 0.5f, -(y + 0.5f) * scale.y + 0.5f);
    }

    private Vector2 CalcTargetPos(Vector2 pos) {
        return CalcTargetPos(pos.x, pos.y);
    }

    public void SetSelect(bool isSelect) {
        IsSelect = isSelect;
        if (IsSelect == false) {
            transform.rotation = new(0, 0, 0, 0);
        }
    }

    public void OnMouseDownEvent() {
        var isSelect = GameController.GemClick(Pos);
        SetSelect(isSelect);
    }

    public void OnMouseDragEvent() {
        if (GameController.Status != Status.Remove && GameController.Status != Status.FallDown)
            GameController.Status = Status.Drag;
    }

    public void OnMouseEnterEvent() {
        if (GameController.Status == Status.Drag) {
            GameController.GemClick(Pos);
        }
    }

    public void MoveToPos(Vector2 pos) {
        IsMoved = true;
        StartPos = Pos;
        SetPosition(pos);
    }
}
