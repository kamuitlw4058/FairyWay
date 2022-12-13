using GameFramework;

namespace FairyWay
{
    public static class AssetUtility
    {
        public static string GetHotfixDLLEditorAsset()
        {
            return "Library/ScriptAssemblies/FairyWay.Hotfix.dll";
        }

        public static string GetHotfixPDBEditorAsset()
        {
            return "Library/ScriptAssemblies/FairyWay.Hotfix.pdb";
        }


    }
}