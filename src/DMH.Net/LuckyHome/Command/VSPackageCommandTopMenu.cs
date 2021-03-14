using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace LuckyHome
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [Guid(VSPackageCommandTopMenu.CommandSet)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(InterfaceMapperWithClass))]
    [ProvideToolWindow(typeof(InputWindow))]
    public sealed class VSPackageCommandTopMenu : AsyncPackage
    {
        public const string CommandSet = "cd803dc7-0b76-4fb6-b92e-6415591b66b8";

        public VSPackageCommandTopMenu()
        {
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            //await AboutCommand.InitializeAsync(this);
            await CommandRunThisMethodContextMenu.InitializeAsync(this);
            //await InterfaceMapperWithClassCommand.InitializeAsync(this);
            //await InputWindowCommand.InitializeAsync(this);
        }
    }
}
