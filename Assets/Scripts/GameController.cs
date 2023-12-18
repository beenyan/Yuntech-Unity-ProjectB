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

public class GameController : MonoBehaviour {
    public static Vector2 MapSize = new(10, 10);
    static readonly GameObject[,] Map = new GameObject[(int)MapSize.y, (int)MapSize.x];
    private static readonly HashSet<Vector2> SelectedGem = new();
    private static bool IsMoved = false;

    private void Awake() {
        Init();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

    void Init() {
        for (int y = 0; y < MapSize.y; ++y) {
            for (int x = 0; x < MapSize.x; ++x) {
                Map[y, x] = Instantiate(Resources.Load<GameObject>(Utils.Resources.Gem.ToString()));
                Map[y, x].GetComponent<Gem>().Init(y, x);
            }
        }
    }

    public Around SameTypeAround(Gem gem) {
        var around = new Around();
        var type = gem.GetGemType();
        int posY = (int)gem.GetPos().y;
        int posX = (int)gem.GetPos().x;

        for (int x = posX - 1; x >= 0; x--) {
            if (Map[posY, x] == null || type != Map[posY, x].GetComponent<Gem>().GetGemType()) break;
            ++around.Left;
        }

        for (int x = posX + 1; x < MapSize.x; x++) {
            if (Map[posY, x] == null || type != Map[posY, x].GetComponent<Gem>().GetGemType()) break;
            ++around.Right;
        }

        for (int y = posY - 1; y >= 0; y--) {
            if (Map[y, posX] == null || type != Map[y, posX].GetComponent<Gem>().GetGemType()) break;
            ++around.Top;
        }

        for (int y = posY + 1; y < MapSize.y; y++) {
            if (Map[y, posX] == null || type != Map[y, posX].GetComponent<Gem>().GetGemType()) break;
            ++around.Down;
        }

        return around;
    }

    public bool IsAvailable(Around around) {
        return !(around.Right + around.Left >= 2 || around.Top + around.Down >= 2);
    }

    public bool GemClick(Vector2 pos) {
        if (IsMoved) return false;

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
        var diff = (firstPos - pos).Abs();
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
        IsMoved = true;

        // Remove Same Type Gem
        RemoveSameType(new[] { firstGem, secondGem });

        return false;
    }

    void RemoveSameType(Gem[] gemArray) {
        bool isExcuteRemove = false;

        foreach (var gem in gemArray) {
            var around = SameTypeAround(gem);
            int posY = (int)gem.GetPos().y;
            int posX = (int)gem.GetPos().x;
            if (around.Right + around.Left >= 2) {
                for (int x = posX - around.Left; x < posX + around.Right + 1; x++) {
                    Destroy(Map[posY, x]);
                    Map[posY, x] = null;
                    isExcuteRemove = true;
                }
            }

            if (around.Top + around.Down >= 2) {
                for (int y = posY - around.Top; y < posY + around.Down + 1; y++) {
                    Destroy(Map[y, posX]);
                    Map[y, posX] = null;
                    isExcuteRemove = true;
                }
            }
        }

        if (!isExcuteRemove) {
            // No Gem Remove
            IsMoved = false;
            return;
        }

        FallDown();
    }

    void FallDown() {
        HashSet<Vector2> movedPos = new();

        while (true) {
            bool isFall = false;
            for (int y = (int)MapSize.y - 1; y >= 0; --y) {
                for (int x = (int)MapSize.x - 1; x >= 0; --x) {
                    if (Map[y, x] != null) continue;
                    movedPos.Add(new Vector2(x, y));

                    if (y == 0) {
                        // Top - Generate
                        Map[y, x] = Instantiate(Resources.Load<GameObject>(Utils.Resources.Gem.ToString()));
                        Map[y, x].GetComponent<Gem>().Init(y, x);
                    } else if (Map[y - 1, x] != null) {
                        // Not Top - Fall
                        Map[y - 1, x].GetComponent<Gem>().MoveToPos(new Vector2(x, y));
                        Map[y, x] = Map[y - 1, x];
                        Map[y - 1, x] = null;
                        isFall = true;
                    }
                }
            }

            if (!isFall) break;
        }

        var posArray = new Vector2[movedPos.Count];
        movedPos.CopyTo(posArray);
        RemoveSameType(Array.ConvertAll(posArray, pos => Map[(int)pos.y, (int)pos.x].GetComponent<Gem>()));
    }
}
