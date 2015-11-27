using System;
using System.Diagnostics;

namespace Platron.Client.TestKit.Site
{
    public static class Browser
    {
        public static void Open(Uri address)
        {
            // http://stackoverflow.com/questions/58024/open-a-url-from-windows-forms
            var startInfo = new ProcessStartInfo(address.AbsoluteUri);
            Process.Start(startInfo);
        }
    }
}