using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using Task = System.Threading.Tasks.Task;

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Mocks;

public class AsyncTaskRunnerMock : IAsyncTaskRunner
{
    #region properties

    /// <inheritdoc />
    public ILogger Logger { get; set; }

    /// <inheritdoc />
    public IEnsureHelper EnsureHelper { get; set; }

    #endregion

    #region members

    /// <inheritdoc />
    public void Run(Func<Task> asyncMethod)
    {
        asyncMethod().Forget();
    }

    /// <inheritdoc />
    public void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions)
    {
        asyncMethod().Forget();
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
    public JoinableTask RunAsync(Func<Task> asyncMethod) => throw new NotImplementedException();

    /// <inheritdoc />
    public JoinableTask RunAsync(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod) => throw new NotImplementedException();

    /// <inheritdoc />
    public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions) =>
        throw new NotImplementedException();

    #endregion
}