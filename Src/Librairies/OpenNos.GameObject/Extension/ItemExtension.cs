namespace OpenNos.GameObject.Extension
{
    public static class ItemExtension
    {
        #region Methods

        public static void ClearShell(this ItemInstance i)
        {
            i.ShellEffects.Clear();
            i.RuneEffects.Clear();
        }

        #endregion
    }
}