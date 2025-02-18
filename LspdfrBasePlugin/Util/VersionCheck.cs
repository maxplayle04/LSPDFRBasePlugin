﻿using System;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/* 
 * LspdfrBasePlugin
 * Helper project for new developers by Max Playle (thatmaxplayle | LCPDFR.com) 
 * 
 * This is a working base project, which you can use to create your own plugins. By all means, you can of course clone this and make it your own. 
 * All I ask is that you credit me in the final release. 
 * 
 * My LCPDFR profile: https://www.lcpdfr.com/profile/403804-thatmaxplayle/
 */

/*
 * LspdfrBasePlugin
 * 
 * Copyright 2022 Max Playle (thatmaxplayle)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace LspdfrBasePlugin.Util
{
    internal class VersionCheck
    {

        private static HttpClient _httpClient;

        public static void CheckForUpdates()
        {
            // This captures the installed version of the current assembly, so the version of your plugin that the user is running.
            var installedVersion = Assembly.GetAssembly(typeof(VersionCheck)).GetName().Version;

            // Initialise httpClient if this has not already been done.
            if (_httpClient is null)
                _httpClient = new HttpClient();

            // Call GetLspdfrServerVersion with the File ID assigned in the constant variable at the top of this class.
            var serverVersion = GetLspdfrServerVersion().GetAwaiter().GetResult();

            // Check if the server version is newer than the installed version.
            if (serverVersion != null && installedVersion < serverVersion)
            {
                Logger.DisplayToConsole($"Update detected! Make sure to downloaded the latest update as soon as possible.");
            }            
            else if (serverVersion == null)
            {
                Logger.DisplayToConsole("Could not contact LCPDFR servers to check version number! Check your internet connection and/or firewall.");
            }
        }

        static async Task<Version> GetLspdfrServerVersion()
        {
            try
            {
                // Call up the LCPDFR.com servers, and ask for a text-representation of the latest version you've uploaded using the unique file ID assigned by the website.
                var serverResult = await _httpClient.GetAsync($"http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId={Settings.LSPDFR_FILE_ID}&textOnly=1");

                // Read the content of the response the server gave into a string.
                string result = await serverResult.Content.ReadAsStringAsync();

                // Use regex to extract a version number (for example, 1.0.0.0) from the Version title
                Regex pattern = new Regex(@"\d+(\.\d+)+");
                Match m = pattern.Match(result);
                string version = m.Value;

                Logger.DisplayToConsole("Server returned version information: " + version);
                
                // Return the version given by the server in a Versio object.
                return Version.Parse(version);
            }
            catch (Exception e)
            {
                Logger.DisplayToConsole($"Error whilst fetching updated version number from server. {e}");
                return null;
            }
        }

    }
}
