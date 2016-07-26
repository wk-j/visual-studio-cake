using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace VisualStudio.Cake.Core
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(CakeCommandPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class CakeCommandPackage : Package
    {
        public const string PackageGuidString = "facf9730-92d7-466c-97fe-7ea00efdd319";

        public CakeCommandPackage()
        {
        }

        protected override void Initialize()
        {
            CakeCommand.Initialize(this);
            base.Initialize();
        }
    }
}
