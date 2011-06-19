using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using Sitecore.Update.Installer;
using Sitecore.SecurityModel;

using Sitecore.Update.Installer.Utils;
using Sitecore.Update;
using Sitecore.Update.Metadata;
using System.Configuration;
using Sitecore.Update.Utils;

namespace Aqueduct.SitecorePackageInstaller
{
    [WebService(Namespace = "http://sitecorepackages.aquepreview.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class PackageInstaller
    {

        [WebMethod(Description = "Installs a Sitecore Update Package.")]
        public void InstallPackage(string path)
        {
            using (new SecurityDisabler())
            {
                var installer = new DiffInstaller(UpgradeAction.Upgrade);
                var view = UpdateHelper.LoadMetadata(path);
                string historyPath;
                bool hasPostAction;
                var entries = installer.InstallPackage(path, InstallMode.Install, null, out hasPostAction, out historyPath);
                installer.ExecutePostInstallationInstructions(path, historyPath, InstallMode.Install, view, null, ref entries);
                UpdateHelper.SaveInstallationMessages(entries, historyPath);
            }
        }
    }
}
