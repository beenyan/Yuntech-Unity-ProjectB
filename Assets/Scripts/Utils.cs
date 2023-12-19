using System;

public class StringValueAttribute : Attribute {
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

    public enum MouseButton {
        Left = 0,
        Right = 1,
        Middle = 2,
    }

    public enum Resources {
        [StringValue("Gem")]
        Gem,
        [StringValue("GemMaterial")]
        GemMaterial
    }

    public enum Tags {
        [StringValue("GameController")]
        GameController
    }

    public static T RandomEnumValue<T>() where T : Enum {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[new Random().Next(values.Length)];
    }
}
