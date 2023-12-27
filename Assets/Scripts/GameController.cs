using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Around {
    public int Left = 0;
    public int Right = 0;
    public int Top = 0;
    public int Down = 0;
};

public enum Status {
    Idle,
    FallDown,
    Remove,
    Drag
}

public class GameController: MonoBehaviour {
    public static Vector2 MapSize = new(10, 10);
    private GameObject[,] Map = new GameObject[(int)MapSize.y, (int)MapSize.x];
    private readonly HashSet<Vector2> SelectedGem = new();
    public Status Status = Status.Idle;
    public int MoveCount = 0;
    private Gem[] RemovePrepare;
    private readonly HashSet<Vector2> FallDownPosSet = new();
    private PlayerController PlayerController;

    private void Awake() {
        PlayerController = PlayerController != null ? PlayerController : Utils.FindByTag(Utils.Tags.Player).GetComponent<PlayerController>();
        Init();
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (Status == Status.Remove && MoveCount == 0) {
            FallDownPosSet.Clear();
            RemoveSameType(RemovePrepare);
            Array.Clear(RemovePrepare, 0, RemovePrepare.Length);
        } else if (Status == Status.FallDown && MoveCount == 0) {
            FallDown();
        }
    }

    void Init() {
        for (int y = 0; y < MapSize.y; ++y) {
            for (int x = 0; x < MapSize.x; ++x) {
                Map[y, x] = Instantiate(Resources.Load<GameObject>(Utils.Resources.Gem.ToString()));
                Map[y, x].GetComponent<Gem>().Init(y, x, gameObject);
            }
        }
    }

    public Around SameTypeAround(Gem gem) {
        var around = new Around();
        var type = gem.GetGemType();
        int posY = Math.Max(0, (int)gem.GetPos().y);
        int posX = (int)gem.GetPos().x;

        for (int x = posX - 1; x >= 0; x--) {
            if (Map[posY, x] == null || type != Map[posY, x].GetComponent<Gem>().GetGemType())
                break;
            ++around.Left;
        }

        for (int x = posX + 1; x < MapSize.x; x++) {
            if (Map[posY, x] == null || type != Map[posY, x].GetComponent<Gem>().GetGemType())
                break;
            ++around.Right;
        }

        for (int y = posY - 1; y >= 0; y--) {
            if (Map[y, posX] == null || type != Map[y, posX].GetComponent<Gem>().GetGemType())
                break;
            ++around.Top;
        }

        for (int y = posY + 1; y < MapSize.y; y++) {
            if (Map[y, posX] == null || type != Map[y, posX].GetComponent<Gem>().GetGemType())
                break;
            ++around.Down;
        }

        return around;
    }

    public bool IsAvailable(Around around) {
        return !(around.Right + around.Left >= 2 || around.Top + around.Down >= 2);
    }

    public bool GemClick(Vector2 pos) {
        if (transform.parent.CompareTag(Utils.Tags.EnemyPlace.ToString())) {
            return false;
        }

        if (Status == Status.Remove || Status == Status.FallDown)
            return false;

        // Select
        if (SelectedGem.Contains(pos)) {
            SelectedGem.Remove(pos);
            return false;
        }

        // Select First
        if (SelectedGem.Count == 0) {
            SelectedGem.Add(pos);
            return true;
        }

        // Select Second
        var firstPos = SelectedGem.Single();

        var diff = Utils.Abs(firstPos - pos);
        if (diff.x >= 2 || diff.y >= 2) {
            // Too far
            return false;
        }

        // Switch
        var firstGem = Map[(int)firstPos.y, (int)firstPos.x].GetComponent<Gem>();
        var secondGem = Map[(int)pos.y, (int)pos.x].GetComponent<Gem>();
        firstGem.SetSelect(false);
        firstGem.MoveToPos(pos);
        secondGem.MoveToPos(firstPos);
        (Map[(int)firstPos.y, (int)firstPos.x], Map[(int)pos.y, (int)pos.x]) = (Map[(int)pos.y, (int)pos.x], Map[(int)firstPos.y, (int)firstPos.x]);
        SelectedGem.Clear();
        Status = Status.Remove;
        MoveCount = 2;

        // Remove Same Type Gem
        RemovePrepare = new[] { firstGem, secondGem };

        return false;
    }

    void RemoveSameType(Gem[] gemArray) {
        bool isExcuteRemove = false;
        foreach (var gem in gemArray.Distinct().NotNull()) {
            var around = SameTypeAround(gem);
            int posY = (int)gem.GetPos().y;
            int posX = (int)gem.GetPos().x;
            int destroyCount = 1;
            if (around.Right + around.Left >= 2) {
                for (int x = posX - around.Left; x < posX + around.Right + 1; ++x) {
                    destroyCount += around.Right + around.Left;
                    Destroy(Map[posY, x]);
                    Map[posY, x] = null;
                    isExcuteRemove = true;
                }
            }

            if (around.Top + around.Down >= 2) {
                for (int y = posY - around.Top; y < posY + around.Down + 1; ++y) {
                    destroyCount += around.Top + around.Down;
                    Destroy(Map[y, posX]);
                    Map[y, posX] = null;
                    isExcuteRemove = true;
                }
            }

            // If Gem Be Remove
            if (destroyCount != 1) {
                var gemType = gem.GetComponent<Gem>().GetGemType();
                switch (gemType) {
                    case GemType.ATTACK_FIRE:
                    case GemType.ATTACK_WOOD:
                    case GemType.ATTACK_WATER:
                        PlayerController.Attack(gemType, destroyCount);
                        break;
                    case GemType.DEFENSE:
                        PlayerController.Def(destroyCount);
                        break;
                    case GemType.HEAL:
                        PlayerController.Heal(destroyCount);
                        break;
                }
            }
        }

        if (!isExcuteRemove) {
            // No Gem Remove
            Status = Status.Idle;
            return;
        }

        Status = Status.FallDown;
    }

    void FallDown() {
        bool isFall = false;
        for (int y = (int)MapSize.y - 1; y >= 0; --y) {
            for (int x = (int)MapSize.x - 1; x >= 0; --x) {
                if (Map[y, x] != null)
                    continue;
                FallDownPosSet.Add(new Vector2(x, y));

                if (y == 0) {
                    // Top - Generate
                    Map[y, x] = Instantiate(Resources.Load<GameObject>(Utils.Resources.Gem.ToString()));
                    Map[y, x].GetComponent<Gem>().Init(y - 1, x, gameObject);
                    Map[y, x].GetComponent<Gem>().MoveToPos(new Vector2(x, y));
                    isFall = true;
                    ++MoveCount;
                } else if (Map[y - 1, x] != null) {
                    // Not Top - Fall
                    Map[y, x] = Map[y - 1, x];
                    Map[y, x].GetComponent<Gem>().MoveToPos(new Vector2(x, y));
                    Map[y - 1, x] = null;
                    isFall = true;
                    ++MoveCount;
                }
            }
        }


        if (!isFall) {
            var posArray = new Vector2[FallDownPosSet.Count];
            FallDownPosSet.CopyTo(posArray);
            RemovePrepare = Array.ConvertAll(posArray, pos => Map[(int)pos.y, (int)pos.x].GetComponent<Gem>());
            Status = Status.Remove;
            return;
        }
    }
}
