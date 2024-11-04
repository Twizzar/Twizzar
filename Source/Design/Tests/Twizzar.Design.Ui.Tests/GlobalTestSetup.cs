using System.Windows;
using Microsoft.VisualStudio.Sdk.TestFramework;
using NCrunch.Framework;
using NUnit.Framework;

[SetUpFixture]
[Atomic]
public class GlobalTestSetup
{
    private static GlobalServiceProvider _mockServiceProvider;
    
    [OneTimeSetUp]
    public static void OneTimeSetUp()
    {
        if (Application.Current == null)
        {
            _mockServiceProvider ??= new GlobalServiceProvider();
        }
    }
    
    [OneTimeTearDown]
    public static void OneTimeTearDown()
    {
        _mockServiceProvider?.Dispose();
        _mockServiceProvider = null;
    }
}