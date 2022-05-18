namespace Futurus.RemoteInput
{
    public enum ButtonDeltaState
    {
        /// <summary>
        /// No change since last time checked.
        /// </summary>
        NoChange = 0,

        /// <summary>
        /// Button pressed since last time checked.
        /// </summary>
        Pressed = 1 << 0,

        /// <summary>
        /// Button released since last time checked.
        /// </summary>
        Released = 1 << 1,
    }
}
