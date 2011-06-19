using System;
using System.Collections.Generic;
using NDesk.Options;

namespace Aqueduct.SitecorePackageConsoleApp
{
    class Program
    {
        static int _verbosity;

        static void Main(string[] args)
        {
            #region Declare options and installer variables
            // Installer variables
            string packagePath = null;
            string sitecoreWebUrl = null;
            string sitecoreDeployFolder = null;
            bool showHelp = args.Length == 0;

            // Options declaration
            var options = new OptionSet() {
   	            { "p|packagePath=", "The {PACKAGE PATH} is the path to the package. The package must be located in a folder reachable by the web server.\n",
                    v => packagePath = v },
                { "u|sitecoreUrl=", "The {SITECORE URL} is the url to the root of the Sitecore server.\n",
                    v => sitecoreWebUrl = v },
                { "f|sitecoreDeployFolder=", "The {SITECORE DEPLOY FOLDER} is the UNC path to the Sitecore web root.\n",
                    v => sitecoreDeployFolder = v },
   	            { "v", "Increase debug message verbosity.\n",
                    v => { if (v != null) ++_verbosity; } },
   	            { "h|help",  "Show this message and exit.", 
                    v => showHelp = v != null },
             };
            #endregion

            // Parse options - exit on error
            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                ShowError(e.Message);
                return;
            }

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            #region Validate and process parameters
            bool parameterMissing = false;

            if (string.IsNullOrEmpty(packagePath))
            {
                ShowError("Package Path is required.");

                parameterMissing = true;
            }

            if (string.IsNullOrEmpty(sitecoreWebUrl))
            {
                ShowError("Sitecore Web URL ie required.");

                parameterMissing = true;
            }

            if (!parameterMissing)
            {
                try
                {
                    Debug("Initializing update package installation: {0}", packagePath);

                    if (sitecoreWebUrl.LastIndexOf(@"/") != sitecoreWebUrl.Length - 1)
                    {
                        sitecoreWebUrl = sitecoreWebUrl + @"/";
                    }
      
                    var service = new localhost.PackageInstaller();
                    Debug("Initializing package installation ..");
                    service.Url = string.Concat(sitecoreWebUrl, "/__Admin/PackageInstaller.asmx");
                    service.InstallPackage(packagePath);
                    Debug("Update package installed successfully.");
                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}({1})\n{2}", ex.Message, ex.GetType().Name, ex.StackTrace);

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("\n\nInnerException: {0}({1})\n{2}", ex.InnerException.Message, ex.InnerException.GetType().Name, ex.InnerException.StackTrace);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Displays the help message
        /// </summary>
        /// <param name="opts"></param>
        static void ShowHelp(OptionSet opts)
        {
            Console.WriteLine("Usage: packageinstaller [OPTIONS]");
            Console.WriteLine("Installs a sitecore package.");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine(@"-v -sitecoreUrl ""http://mysite.com/"" -packagePath ""C:\Package1.update""");
            Console.WriteLine();
            Console.WriteLine("Options:");

            opts.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message"></param>
        static void ShowError(string message)
        {
            Console.Write("Error: ");
            Console.WriteLine(message);
            Console.WriteLine("Try `packageinstaller --help' for more information.");
        }

       
        /// <summary>
        /// Writes a debug message to the console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        static void Debug(string format, params object[] args)
        {
            if (_verbosity > 0)
            {
                Console.WriteLine();
                Console.Write(string.Format("[{0}] ", DateTime.Now.ToString("hh:MM:ss")));
                Console.WriteLine(format, args);
            }
        }
    }
}
