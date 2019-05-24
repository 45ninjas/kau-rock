public static class Time {
    // The time the last update call took scaled by Scale.
    public static float Delta {private set; get; }
    // The accumulation of delta scaled by Scale since the game started.
    public static float GameTime {private set; get; }

    // The time since the game started.
    public static float UnscaledGameTime {private set; get; }
    // The time the last update call took.
    public static float UnscaledDelta {private set; get; }

    // The scale to change the time by.
    public static float Scale = 1;

    internal static void SetTime(float delta) {

        // Set the unscaled time.
        UnscaledDelta = delta;
        UnscaledGameTime += delta;

        // Scale the delta.
        delta *= Scale;

        // Set the scaled time.
        GameTime += delta;
        Delta = delta;
    }
}