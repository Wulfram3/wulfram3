using UnityEngine;
using System.Collections;
using System.IO;

namespace MHLab.PATCH.Settings
{
    public class Settings : Singleton<Settings>
    {

        /**
         * Directories routing settings
         */
        public static string APP_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
        // ASSETS_PATH determines where static Assets are stored
        public static string ASSETS_PATH = "Assets" + Path.DirectorySeparatorChar + "MHLab" + Path.DirectorySeparatorChar + "PATCH" + Path.DirectorySeparatorChar;

        /**
         * Language settings
         */
        // LANGUAGE_PATH determines where language files are stored
        public static string LANGUAGE_PATH = Settings.ASSETS_PATH + "Resources" + Path.DirectorySeparatorChar + "Localizatron" + Path.DirectorySeparatorChar + "Locale" + Path.DirectorySeparatorChar;
        public static string SAVING_LANGUAGE_PATH = Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Localizatron" + Path.DirectorySeparatorChar + "Locale" + Path.DirectorySeparatorChar;
        // LANGUAGE_EXTENSION determines the language files extension
        public static string LANGUAGE_EXTENSION = ".txt";
        // LANGUAGE_DEFAULT determines the default language code
        public static string LANGUAGE_DEFAULT = "en_EN";
    }
}