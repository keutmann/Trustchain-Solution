using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustchainCore.Configuration;
using TrustchainCore.Extensions;

namespace TrustchainCore.Business
{

    public class AppDirectory
    {
        /// <summary>
        /// Root path of the Trustchain Application folder
        /// </summary>
        public static string TrustchainPath { get; set; }
        public static string TrustchainName { get; set; }

        /// <summary>
        /// Data folder of the Trustchain Application folder
        /// </summary>
        public static string DataPath { get; set; }
        public static string DataName { get; set; }

        /// <summary>
        /// All Trust packages are located here.
        /// </summary>
        public static string LibraryPath { get; set; }
        public static string LibraryName { get; set; }

        /// <summary>
        /// Trustbuild data folder, where files are located when building.
        /// </summary>
        public static string BuildPath { get; set; }
        public static string BuildName { get; set; }

        public static string TorrentName { get; set; }
        /// <summary>
        /// TrustTorrent folder for torrent files, the actual shared file is in the library folder.
        /// </summary>
        public static string TorrentPath { get; set; }

        public static string StampName { get; set; }
        /// <summary>
        /// Truststamp folder for Stamp database files.
        /// </summary>
        public static string StampPath { get; set; }


        static AppDirectory()
        {
        }

        public static void Setup()
        {
            TrustchainName = App.Config["trustchainfoldername"].ToStringValue("Trustchain");
            TrustchainPath = Path.Combine(App.Config["trustchainfolderpath"].ToStringValue(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)), TrustchainName);
            EnsureDirectory(TrustchainPath);

            DataName = App.Config["datafoldername"].ToStringValue("Data");
            DataPath = App.Config["datafolderpath"].ToStringValue(Path.Combine(TrustchainPath, DataName));
            EnsureDirectory(DataPath);

            LibraryName = App.Config["libraryfoldername"].ToStringValue("Library");
            LibraryPath = App.Config["libraryfolderpath"].ToStringValue(Path.Combine(DataPath, LibraryName));
            EnsureDirectory(LibraryPath);

            BuildName = App.Config["buildfoldername"].ToStringValue("Build");
            BuildPath = App.Config["buildfolderpath"].ToStringValue(Path.Combine(DataPath, BuildName));
            EnsureDirectory(BuildPath);

            TorrentName = App.Config["torrentfoldername"].ToStringValue("Torrent");
            TorrentPath = App.Config["torrentfolderpath"].ToStringValue(Path.Combine(DataPath, TorrentName));
            EnsureDirectory(TorrentPath);

            StampName = App.Config["stampfoldername"].ToStringValue("Stamp");
            StampPath = App.Config["stampfolderpath"].ToStringValue(Path.Combine(DataPath, StampName));
            EnsureDirectory(StampPath);
        }

        public static void EnsureDirectory(string path)
        {
            // Check if folder exists and if not, create it
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }



        //public DirectoryManager()
        //{
        //}
        

        //public string GetLibraryFolder()
        //{
        //    return Path.Combine(TrustchainPath, "Library");
        //}

        //public string GetTorrentFolder()
        //{
        //    return Path.Combine(TrustchainPath, "Torrent");
        //}

    }
}
