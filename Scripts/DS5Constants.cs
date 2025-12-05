/// <summary>
/// Constants used in DS5Bridge, extracted from dsw header file.
/// </summary>
public static class DS5Constants
{
    // Face buttons + dpad in buttonsAndDpad byte (per ds5w.h defines)
    public const byte BTX_SQUARE = 0x10; // DS5W_ISTATE_BTX_SQUARE
    public const byte BTX_CROSS = 0x20; // DS5W_ISTATE_BTX_CROSS
    public const byte BTX_CIRCLE = 0x40; // DS5W_ISTATE_BTX_CIRCLE
    public const byte BTX_TRIANGLE = 0x80; // DS5W_ISTATE_BTX_TRIANGLE


    public const byte DPAD_LEFT = 0x01; // DS5W_ISTATE_DPAD_LEFT
    public const byte DPAD_DOWN = 0x02; // DS5W_ISTATE_DPAD_DOWN
    public const byte DPAD_RIGHT = 0x04; // DS5W_ISTATE_DPAD_RIGHT
    public const byte DPAD_UP = 0x08; // DS5W_ISTATE_DPAD_UP


    // "A" button group (secondary button byte) -- mapping from ds5w.h
    public const byte BTN_A_LEFT_BUMPER = 0x01; // DS5W_ISTATE_BTN_A_LEFT_BUMPER
    public const byte BTN_A_RIGHT_BUMPER = 0x02; // DS5W_ISTATE_BTN_A_RIGHT_BUMPER
    public const byte BTN_A_LEFT_TRIGGER = 0x04; // DS5W_ISTATE_BTN_A_LEFT_TRIGGER
    public const byte BTN_A_RIGHT_TRIGGER = 0x08; // DS5W_ISTATE_BTN_A_RIGHT_TRIGGER
    public const byte BTN_A_SELECT = 0x10; // DS5W_ISTATE_BTN_A_SELECT
    public const byte BTN_A_MENU = 0x20; // DS5W_ISTATE_BTN_A_MENU
    public const byte BTN_A_LEFT_STICK = 0x40; // DS5W_ISTATE_BTN_A_LEFT_STICK
    public const byte BTN_A_RIGHT_STICK = 0x80; // DS5W_ISTATE_BTN_A_RIGHT_STICK


    // "B" button group (secondary) -- mapping from ds5w.h
    public const byte BTN_B_PLAYSTATION_LOGO = 0x01; // DS5W_ISTATE_BTN_B_PLAYSTATION_LOGO
    public const byte BTN_B_PAD_BUTTON = 0x02; // DS5W_ISTATE_BTN_B_PAD_BUTTON
    public const byte BTN_B_MIC_BUTTON = 0x04; // DS5W_ISTATE_BTN_B_MIC_BUTTON


    // Output / player leds
    public const byte PLAYER_LED_LEFT = 0x01; // DS5W_OSTATE_PLAYER_LED_LEFT
    public const byte PLAYER_LED_MIDDLE_LEFT = 0x02; // DS5W_OSTATE_PLAYER_LED_MIDDLE_LEFT
    public const byte PLAYER_LED_MIDDLE = 0x04; // DS5W_OSTATE_PLAYER_LED_MIDDLE
    public const byte PLAYER_LED_MIDDLE_RIGHT = 0x08; // DS5W_OSTATE_PLAYER_LED_MIDDLE_RIGHT
    public const byte PLAYER_LED_RIGHT = 0x10; // DS5W_OSTATE_PLAYER_LED_RIGHT


    // Useful helper masks
    public const byte FACE_BUTTONS_MASK = 0xF0; // bits 4..7
    public const byte DPAD_MASK = 0x0F; // bits 0..3


    // Deadzone / thresholds (defaults — tweak as needed)
    public static float DefaultStickThreshold = 0.5f; // analog stick axis threshold
    public static float DefaultTriggerThreshold = 0.5f; // trigger combined threshold
}