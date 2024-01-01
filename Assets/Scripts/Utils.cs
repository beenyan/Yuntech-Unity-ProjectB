using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
        [StringValue("AGCC")]
        AGCC = 0,
        [StringValue("Login")]
        Login = 1,
        [StringValue("Lobby")]
        Lobby = 2,
        [StringValue("Play")]
        Play = 3,
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

    public enum Images {
        [StringValue("MoonlitDancer")]
        FieldD,
        [StringValue("MoonlitDancer")]
        FieldL,
        [StringValue("CrimsonMoon")]
        GraveD,
        [StringValue("CrimsonMoon")]
        GraveL,
        [StringValue("PrayOrgan")]
        SanctuaryD,
        [StringValue("PrayOrgan")]
        SanctuaryL,
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
        [StringValue("Waiting")]
        Waiting,
        [StringValue("Background")]
        Background,
        [StringValue("Character")]
        Character,
    }

    public static GameObject FindByTag(Tags tag) {
        return GameObject.FindGameObjectWithTag(tag.ToString());
    }

    public static T RandomEnumValue<T>() where T : Enum {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[new System.Random().Next(values.Length)];
    }

}

public static class EnumExtraExtensions {
    public static string GetDescription<T>(this T source) where T : Enum {
        System.Reflection.FieldInfo fi = source.GetType().GetField(source.ToString());
        System.ComponentModel.DescriptionAttribute[] attributes = (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(System.ComponentModel.DescriptionAttribute), false);
        if (attributes.Length > 0)
            return attributes[0].Description;
        else
            return source.ToString();
    }
}

public static class ScenesExtensions {
    public static void Load(this Utils.Scenes scene) {
        SceneManager.LoadScene((int)scene);
    }
}

public static class UnityVector2Extensions {
    public static Vector2 Abs(this Vector2 v2) {
        v2.x = Mathf.Abs(v2.x);
        v2.y = Mathf.Abs(v2.y);
        return v2;
    }
}



public class Vector2Converter: JsonConverter {
    public override bool CanConvert(Type objectType) {
        if (objectType == typeof(Vector2[]))
            objectType = typeof(Vector2Converter);
        return (objectType == typeof(Vector2) || objectType == typeof(Vector2[]));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        string[] v2Str = reader.Value.ToString().Split(',');
        v2Str[0] = v2Str[0].Replace("(", "");
        v2Str[1] = v2Str[1].Replace(")", "");
        return new Vector2(int.Parse(v2Str[0]), int.Parse(v2Str[1]));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        Vector2 v2 = (Vector2)value;
        serializer.Serialize(writer, "(" + v2.x + "," + v2.y + ")");
    }
}


