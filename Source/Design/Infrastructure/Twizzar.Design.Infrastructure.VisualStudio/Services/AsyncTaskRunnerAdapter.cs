using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using Task = System.Threading.Tasks.Task;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <summary>
    /// Uses the implementation of <see cref="JoinableTaskFactory"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AsyncTaskRunnerAdapter : IAsyncTaskRunner
    {
        #region fields

        private readonly JoinableTaskFactory _joinableTaskFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTaskRunnerAdapter"/> class.
        /// </summary>
        public AsyncTaskRunnerAdapter()
        {
            this._joinableTaskFactory = ThreadHelper.JoinableTaskFactory;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Run(Func<Task> asyncMethod) =>
            this._joinableTaskFactory.Run(asyncMethod);

        /// <inheritdoc />
        public void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions)
        {
            this._joinableTaskFactory.Run(asyncMethod, creationOptions);
        }

        /// <inheritdoc />
        public T Run<T>(Func<Task<T>> asyncMethod) =>
            this._joinableTaskFactory.Run(asyncMethod);

        /// <inheritdoc />
        public T Run<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
            this._joinableTaskFactory.Run(asyncMethod, creationOptions);

        /// <inheritdoc />
        public JoinableTask RunAsync(Func<Task> asyncMethod) =>
            this._joinableTaskFactory.RunAsync(asyncMethod);

        /// <inheritdoc />
        public JoinableTask RunAsync(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
            this._joinableTaskFactory.RunAsync(asyncMethod, creationOptions);

        /// <inheritdoc />
        public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod) =>
            this._joinableTaskFactory.RunAsync(asyncMethod);

        /// <inheritdoc />
        public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
            this._joinableTaskFactory.RunAsync(asyncMethod, creationOptions);

        #endregion
    }
}