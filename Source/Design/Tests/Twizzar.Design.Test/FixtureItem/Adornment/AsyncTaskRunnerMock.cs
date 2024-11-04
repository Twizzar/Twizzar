using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits

namespace Twizzar.Design.Test.FixtureItem.Adornment;

public class AsyncTaskRunnerMock : IAsyncTaskRunner
{
    #region Implementation of IHasLogger

    /// <inheritdoc />
    public ILogger Logger { get; set; }

    #endregion

    #region Implementation of IHasEnsureHelper

    /// <inheritdoc />
    public IEnsureHelper EnsureHelper { get; set; }

    #endregion

    #region Implementation of IAsyncTaskRunner

    /// <inheritdoc />
    public void Run(Func<Task> asyncMethod)
    {
        asyncMethod().ConfigureAwait(true);
    }

    /// <inheritdoc />
    public void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions)
    {
        asyncMethod().ConfigureAwait(true);
    }

    /// <inheritdoc />
    public T Run<T>(Func<Task<T>> asyncMethod)
    {
        var task = asyncMethod();
        task.Wait();
        return task.Result;
    }

    /// <inheritdoc />
    public T Run<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions)
    {
        var task = asyncMethod();
        task.Wait();
        return task.Result;
    }

    /// <inheritdoc />
    public JoinableTask RunAsync(Func<Task> asyncMethod) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public JoinableTask RunAsync(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
        throw new NotImplementedException();

    #endregion
}