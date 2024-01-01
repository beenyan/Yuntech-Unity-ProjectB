using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;

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

class GameInitData {
    public int[,] Map = new int[(int)GameController.MapSize.y, (int)GameController.MapSize.x];
    public Utils.Images Scene = Utils.Images.GraveD;
    public int RandomSeed;
    public uint enemyuid;
    public uint uuid;
    public bool Request = true;
    public GameInitData() { }
    public GameInitData(GameObject[,] Map, Utils.Images scene, int randomSeed, uint uuid, bool request = true) {
        this.Map = new int[Map.GetLength(0), Map.GetLength(1)];
        for (int y = 0; y < Map.GetLength(0); y++) {
            for (int x = 0; x < Map.GetLength(1); x++) {
                this.Map[y, x] = (int)Map[y, x].GetComponent<Gem>().GetGemType();
            }
        }

        Scene = scene;
        RandomSeed = randomSeed;
        Request = request;
        this.uuid = uuid;
    }
}

public class GameController: MonoBehaviour {
    public static Vector2 MapSize = new(10, 10);
    public GameObject[,] Map = new GameObject[(int)MapSize.y, (int)MapSize.x];
    private readonly HashSet<Vector2> SelectedGem = new();
    public Status Status = Status.Idle;
    public int MoveCount = 0;
    private Gem[] RemovePrepare;
    private readonly HashSet<Vector2> FallDownPosSet = new();
    private PlayerController PlayerController;
    private AGCC CloudController;
    private AudioSource AudioS;

    private void Awake() {
        PlayerController = PlayerController != null ? PlayerController : Utils.FindByTag(Utils.Tags.Player).GetComponent<PlayerController>();
        CloudController = CloudController != null ? CloudController : FindObjectOfType<AGCC>();
        AudioS = gameObject.GetComponent<AudioSource>();
        if (transform.parent.CompareTag(Utils.Tags.PlayerPlace.ToString())) {
            Init();
        }
    }

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

        AGCC.PlayerMap = Map;
        AGCC.PlayerRandomSeed = UnityEngine.Random.Range(6, 59);
        if (CloudController == null) {
            Utils.Scenes.AGCC.Load();
        }
        Utils.Images Scene = Utils.RandomEnumValue<Utils.Images>();
        var data = new GameInitData(AGCC.PlayerMap, Scene, AGCC.PlayerRandomSeed, CloudController.ag.poid);
        CloudController.chatSn.Send(JsonConvert.SerializeObject(data));
        Utils.FindByTag(Utils.Tags.Background).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>($"Images/{Scene}");
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
        if (Status == Status.Remove || Status == Status.FallDown) {
            return false;
        }
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


        if (!transform.parent.CompareTag(Utils.Tags.EnemyPlace.ToString())) {
            Vector2[] tempVec2 = new Vector2[] { firstPos, pos };
            string json = JsonConvert.SerializeObject(tempVec2, new Vector2Converter());
            Debug.Log(json);
            CloudController.ag.PrivacySend(json, CloudController.EnemyUID);
        }

        // Switch
        var firstGem = Map[(int)firstPos.y, (int)firstPos.x].GetComponent<Gem>();
        var secondGem = Map[(int)pos.y, (int)pos.x].GetComponent<Gem>();
        firstGem.SetSelect(false);
        AudioS.Play();
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
            AudioS.Play();
            var around = SameTypeAround(gem);
            int posY = (int)gem.GetPos().y;
            int posX = (int)gem.GetPos().x;
            int destroyCount = 0;
            if (around.Right + around.Left >= 2) {
                for (int x = posX - around.Left; x < posX + around.Right + 1; ++x) {
                    destroyCount += 1;
                    Destroy(Map[posY, x]);
                    Map[posY, x] = null;
                    isExcuteRemove = true;
                }
            }

            if (around.Top + around.Down >= 2) {
                for (int y = posY - around.Top; y < posY + around.Down + 1; ++y) {
                    destroyCount += 1;
                    Destroy(Map[y, posX]);
                    Map[y, posX] = null;
                    isExcuteRemove = true;
                }
            }

            bool isPlayer = transform.parent.CompareTag(Utils.Tags.PlayerPlace.ToString());

            // If Gem Be Remove
            if (destroyCount != 0) {
                var gemType = gem.GetComponent<Gem>().GetGemType();
                switch (gemType) {
                    case GemType.ATTACK_FIRE:
                    case GemType.ATTACK_WOOD:
                    case GemType.ATTACK_WATER:
                        if (!isPlayer) {
                            PlayerController.Attack(gemType, destroyCount);
                        }
                        break;
                    case GemType.DEFENSE:
                        if (isPlayer) {
                            PlayerController.Def(destroyCount);
                        }
                        break;
                    case GemType.HEAL:
                        if (isPlayer) {
                            PlayerController.Heal(destroyCount);
                        }
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
                    Map[y, x] = SummonGem(y - 1, x);
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

    GameObject SummonGem(int y, int x) {
        int gemType;
        GameObject parent;
        if (transform.parent.CompareTag(Utils.Tags.EnemyPlace.ToString())) {
            gemType = (AGCC.EnemyRandomSeed / 10 + AGCC.EnemyRandomSeed % 10) % Enum.GetValues(typeof(GemType)).Length;
            AGCC.EnemyRandomSeed = AGCC.EnemyRandomSeed % 10 * 10 + gemType;
            parent = Utils.FindByTag(Utils.Tags.EnemyPlace);
        } else {
            gemType = (AGCC.PlayerRandomSeed / 10 + AGCC.PlayerRandomSeed % 10) % Enum.GetValues(typeof(GemType)).Length;
            AGCC.PlayerRandomSeed = AGCC.PlayerRandomSeed % 10 * 10 + gemType;
            parent = Utils.FindByTag(Utils.Tags.PlayerPlace);
        }

        var gem = Instantiate(Resources.Load<GameObject>(Utils.Resources.Gem.ToString()));
        gem.GetComponent<Gem>().Init(y, x, parent.transform.GetChild(0).gameObject, (GemType)gemType);
        return gem;
    }
}
