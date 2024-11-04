using System;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Notifies the user over the progress of the test creation.
/// </summary>
public interface ITestCreationProgress : IProgress<TestCreationProgressData>, IDisposable
{
}