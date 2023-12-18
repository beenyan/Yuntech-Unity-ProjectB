using UnityEngine;

public enum GemType {
    DIAMOND = 0,
    RUBY = 1,
    EMERALD = 2,
    LAPIS = 3,
    GOLD = 4,
}

public class Gem : MonoBehaviour {
    private Vector2 Pos;
    private GemType Type;
    private bool IsSelect = false;
    private static Sprite[] Sprites;
    private static GameObject Parent;
    private static GameController GameController;
    public Around ar;

    public Vector2 GetPos() {
        return Pos;
    }

    public GemType GetGemType() {
        return Type;
    }

    void Awake() {
        Parent = Parent != null ? Parent : GameObject.FindGameObjectWithTag(Utils.Tags.GameController.ToString());
        GameController = GameController != null ? GameController : Parent.GetComponent<GameController>();
        Sprites ??= Resources.LoadAll<Sprite>(Utils.Resources.GemMaterial.ToString());
    }

    public void Init(int y, int x) {
        Type = Utils.RandomEnumValue<GemType>();
        gameObject.transform.SetParent(Parent.transform);
        SetPosition(new(x, y));

        while (!GameController.IsAvailable(GameController.SameTypeAround(this))) {
            Type = Utils.RandomEnumValue<GemType>();
        }

        var renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.sprite = Sprites[(int)Type];
    }

    void Update() {
        if (IsSelect) {
            transform.Rotate(new Vector3(0, 0, 1));
        }
    }

    private void SetPosition(Vector2 pos) {
        int y = (int)pos.y;
        int x = (int)pos.x;
        Pos = new(x, y);

        var scale = new Vector2(1 / GameController.MapSize.x, 1 / GameController.MapSize.y);
        gameObject.name = $"({y}, {x})";
        gameObject.transform.localScale = scale;
        gameObject.transform.localPosition = new((x + 0.5f) * scale.x - 0.5f, -(y + 0.5f) * scale.y + 0.5f);
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

    public void MoveToPos(Vector2 pos) {
        SetPosition(pos);
    }
}
