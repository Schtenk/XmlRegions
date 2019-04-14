using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using XmlRegions;

namespace JavaScriptRegions
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuidString)]
    public sealed class RegionsPackage : AsyncPackage
    {
        #region Constants

        public const string PackageGuidString = "919fc714-2481-4f4f-a916-06eacc702380";

        #endregion Constants
    }
}