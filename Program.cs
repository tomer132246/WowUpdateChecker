﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace WowUpdateChecker
{
    class Program
    {
        private static string DiscordWebhookURL = "https://discordapp.com/api/webhooks/103636102431784/GqsdIOYv-nIcEtRyBrUh-srxdfcg_TroLoLlolETxrcytgvBkjn"; //Enter your discord webhook URL here
        private static string BattleNetUpdateURL = "http://us.patch.battle.net:1119/wow_classic/versions";
        private static TimeSpan CheckDelay = TimeSpan.FromMinutes(1); //1 min interval
        
        static void Main(string[] args)
        {
            //print some art
            PrintBanner();

            //init variables
            DateTime LastCheck = DateTime.Now.Subtract(CheckDelay);
            List<VersionFile> LastVersionFiles = GetCurrentVersion();
            List<VersionFile> CurrentVersionFiles = LastVersionFiles;
            DiscordWebhookHandler discordWebhook = new DiscordWebhookHandler(DiscordWebhookURL);

            while (true)
            {
                if(DateTime.Now.Subtract(CheckDelay) >= LastCheck)
                {
                    DateLog("Checking for updates... ");
                    CurrentVersionFiles = GetCurrentVersion();
                    for(int i = 0; i < CurrentVersionFiles.Count; i++)
                    {
                        if(LastVersionFiles[i].BuildNumber != CurrentVersionFiles[i].BuildNumber)
                        {
                            //Update detected
                            string NotificationMsg = LastVersionFiles[i].Location + " updated to version " + LastVersionFiles[i].Version;
                            DateLog(NotificationMsg);
                            discordWebhook.SendMessage(NotificationMsg);
                        }
                    }
                    LastVersionFiles = CurrentVersionFiles.ToList();
                    CurrentVersionFiles.Clear();
                    LastCheck = DateTime.Now;
                }
                else
                {
                    Thread.Sleep(DateTime.Now.Add(CheckDelay) - LastCheck);
                }
            }
        }

        static void PrintBanner()
        {
            Console.WriteLine("                   _       _                   ___ _               _             ");
            Console.WriteLine("   /\\ /\\ _ __   __| | __ _| |_ ___            / __\\ |__   ___  ___| | _____ _ __ ");
            Console.WriteLine("  / / \\ \\ '_ \\ / _` |/ _` | __/ _ \\  _____   / /  | '_ \\ / _ \\/ __| |/ / _ \\ '__|");
            Console.WriteLine("  \\ \\_/ / |_) | (_| | (_| | ||  __/ |_____| / /___| | | |  __/ (__|   <  __/ |   ");
            Console.WriteLine("   \\___/| .__/ \\__,_|\\__,_|\\__\\___|         \\____/|_| |_|\\___|\\___|_|\\_\\___|_|   ");
            Console.WriteLine("        |_|                                                                      ");
            Console.WriteLine("                                                    Author: Ferib Hellscream");
        }

        static List<VersionFile> GetCurrentVersion()
        {
            WebClient client = new WebClient();
            string VersionContent = client.DownloadString(BattleNetUpdateURL);
            string VersionContentLine = "";
            List<VersionFile> FileVersionList = new List<VersionFile>();
            VersionFile CurrentVersion = new VersionFile();

            //Region!STRING:0|BuildConfig!HEX:16|CDNConfig!HEX:16|KeyRing!HEX:16|BuildId!DEC:4|VersionsName!String:0|ProductConfig!HEX:16

            for (int i = 2; i < VersionContent.Split('\n').Count(); i++)//Skip first 2 lines
            {
                CurrentVersion = new VersionFile();
                VersionContentLine = VersionContent.Split('\n')[i];
                if (VersionContentLine.Count() < 1)
                    break;
                CurrentVersion.Location = VersionContentLine.Split('|')[0];
                CurrentVersion.hash1 = VersionContentLine.Split('|')[1];
                CurrentVersion.hash2 = VersionContentLine.Split('|')[2];
                CurrentVersion.hash3 = VersionContentLine.Split('|')[6];
                CurrentVersion.BuildNumber = VersionContentLine.Split('|')[4];
                CurrentVersion.Version = VersionContentLine.Split('|')[5];
                FileVersionList.Add(CurrentVersion);
            }
            return FileVersionList;
        }

        static void DateLog(string data)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss") + "]: " + data); //F*ck Amaricans, fight me IRL
        }
    }
    class VersionFile
    {
        public string Location { get; set; }
        public string hash1 { get; set; }
        public string hash2 { get; set; }
        public string hash3 { get; set; }
        public string BuildNumber { get; set; }
        public string Version { get; set; }

        public VersionFile()
        {
            Location = "xx";
            hash1 = "N/A";
            hash2 = "N/A";
            hash3 = "N/A";
            BuildNumber = "00000";
            Version = "0.0.0.00000";
        }
        public VersionFile(string location, string Hash1, string Hash2, string Hash3, string buildNumber, string version)
        {
            Location = location;
            hash1 = Hash1;
            hash2 = Hash2;
            hash3 = Hash3;
            BuildNumber = buildNumber;
            Version = version;
        }
    }
}
