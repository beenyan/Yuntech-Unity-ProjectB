using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StringValueAttribute: Attribute {
    public string Value { get; }

    public StringValueAttribute(string value) {
        Value = value;
    }
}

public class Utils {
    public enum Axis {
        [StringValue("Vertical")]
        Vertical,
        [StringValue("Horizontal")]
        Horizontal,
    }

    public enum Scenes {
        [StringValue("Register")]
        Register = 0,
        [StringValue("Lobby")]
        Lobby = 1,
        [StringValue("Play")]
        Play = 2,
    }

    public enum MouseButton {
        Left = 0,
        Right = 1,
        Middle = 2,
    }

    public enum Resources {
        [StringValue("Gem")]
        Gem,
        [StringValue("GemMaterial")]
        GemMaterial,
    }

    public enum Tags {
        [StringValue("GameController")]
        GameController,
        [StringValue("Player")]
        Player,
        [StringValue("PlayerPlace")]
        PlayerPlace,
        [StringValue("EnemyPlace")]
        EnemyPlace,
    }

    public static GameObject FindByTag(Tags tag) {
        return GameObject.FindGameObjectWithTag(tag.ToString());
    }

    public static T RandomEnumValue<T>() where T : Enum {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[new System.Random().Next(values.Length)];
    }
}

public static class ScenesExtensions {
    public static void Load(this Utils.Scenes scene) {
        SceneManager.LoadScene((int)scene);
    }
}
