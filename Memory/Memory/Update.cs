﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory
{
    class Update
    {
        public static void runUpdate()
        {
            string tmpVer = "";
            if (RemoteFileExists(tmpVer))
            {
                //get version tag of latest version
                string newUrl = GetFinalRedirect("https://github.com/Rn11/Memory-Csharp/releases/latest");
                string latestVer = newUrl.Substring(51, 6);
                MessageBox.Show(latestVer);
                if (latestVer != FormHauptmenue.currVer)
                {
                    if (MessageBox.Show("Ein neuere Version ist verfügbar! Jetzt neue Version runterladen?", "Update verfügbar!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        string downloadUrl = "https://github.com/Rn11/Memory-Csharp/releases/latest";
                        System.Diagnostics.Process.Start(downloadUrl);
                    }
                }

            }
        }
        private static bool RemoteFileExists(string version)
        {
            try
            {
                //combine git url and version
                HttpWebRequest request = WebRequest.Create("https://github.com/Rn11/Memory-Csharp/releases/latest") as HttpWebRequest;
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }

            catch
            {
                return false;
            }
        }

        public static string GetFinalRedirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 9;
            string newUrl = url;
            do
            {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    return newUrl;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
    }
}