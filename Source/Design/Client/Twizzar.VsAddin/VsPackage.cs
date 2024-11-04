using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.Ui.View.About;
using Twizzar.VsAddin.Commands;
using Twizzar.VsAddin.Interfaces.CompositionRoot;
using Task = System.Threading.Tasks.Task;

namespace Twizzar.VsAddin
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ExcludeFromCodeCoverage]
    public sealed class VsPackage : AsyncPackage
    {
        #region static fields and constants

        /// <summary>
        /// InfoCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "eaa3803d-1f6b-456d-99c0-df8b2a931a1e";

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsPackage"/> class.
        /// </summary>
        public VsPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #endregion

        #region members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var maybeComponentModel = await this.GetServiceAsync(typeof(SComponentModel));
            progress.Report(new ServiceProgressData("Initializing Twizzar"));

            if (maybeComponentModel is IComponentModel componentModel)
            {
                var iocContainer = componentModel.GetService<IIocOrchestrator>();
                await AboutWindowCommand.InitializeAsync(this, iocContainer.Resolve<AboutWindow>());
                await OpenOrCloseShortcutCommand.InitializeAsync(this, iocContainer.Resolve<IUiEventHub>());
                await CreateUnitTestCommand.InitializeAsync(this, iocContainer.Resolve<ICommandBus>(), iocContainer.Resolve<ILocationService>());
                await UnitTestNavigationCommand.InitializeAsync(this, iocContainer.Resolve<ICommandBus>(), iocContainer.Resolve<ILocationService>());
            }
        }

        #endregion
    }
}