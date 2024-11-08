﻿using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;

/// <inheritdoc cref="IConfigFileService"/>
[ExcludeFromCodeCoverage]
public sealed class ConfigFileService : BaseFileService, IConfigFileService
{
    /// <inheritdoc/>
    protected override string Default => """
                                        !! Configuration file
                                        
                                        !! These config is setup for the NUnit test framework.
                                        !! see https://github.com/Twizzar/Twizzar/tree/main/defaultConfigs for configs that can be used as a starting point for different test frameworks.
                                        
                                        !! Purpose:
                                        !! This configuration encompasses mapping configurations, along with various other settings, including the specification of NuGet packages to be included in newly created projects.
                                        
                                        
                                        !! Mapping Configurations
                                        !! ----------------------
                                        
                                        !! Purpose:
                                        !! This mapping configurations serves as a mapping between a "Member Under Test" (MUT) and its corresponding "Test Method".
                                        !! These variables enclosed in $ symbols will be dynamically filled by the Addin during the automated creation of unit tests.
                                        
                                        !! Usage:
                                        !! - All paths within this configuration file use forward slashes (/) as the directory separator.
                                        !! - The variables enclosed in $ symbols will be automatically populated by the Addin based on the context.
                                        !! - A mapping can have multiple pattern matches, separated by a newline. The first match will be used.
                                        !! - The syntax for a mapping is as follows: <pattern> : <replacement>
                                        !!   - The pattern can contain wildcards (*), which correspond to any number of characters (greedy matching).
                                        !!   - The replacement can contain back-references ($1, $2, etc.), which correspond to the wildcard matches.
                                        !!   - The pattern and replacement can contain variables enclosed in $ symbols.
                                        !! - For mapping everything to given replacement use the following syntax: <replacement>
                                        !!   - This is useful for example providing a default after other pattern matches.
                                        !!     For example:
                                        !!     *Infrastructure* : MySingleInfrastructureTestProject
                                        !!     $projectUnderTest$.Tests
                                        !! - The following variables are available:
                                        !!   - $solutionPath$ : The absolute path to the solution directory.
                                        !!   - $projectUnderTest$ : The name of the project under test.
                                        !!   - $fileUnderTest$ : The name of the file under test. Without the file extension.
                                        !!   - $namespaceUnderTest$ : The namespace of the class under test.
                                        !!   - $typeUnderTest$ : The name of the class under test.
                                        !!   - $memberUnderTest$ : The name of the method under test.
                                        
                                        !! Mapping from the source project name to the test project name.
                                        [testProject:]
                                        $projectUnderTest$.Tests

                                        !! Mapping from the source project path to the the test project path. This is the absolute path to the project directory.
                                        [testProjectPath:]
                                        $solutionPath$/$projectUnderTest$.Tests
                                        
                                        !! Mapping from the source file name to the test file name.
                                        !! This is the file name without the path, and without the file extension.
                                        [testFile:]
                                        $fileUnderTest$.Tests

                                        !! Mapping from the source file path to the test file path.
                                        !! This is the relative path to the test file directory.
                                        [testFilePath:]
                                        * : $1

                                        !! Mapping from the source namespace to the test namespace.
                                        [testNamespace:]
                                        $namespaceUnderTest$.Tests

                                        !! Mapping from the source class name to the test class name.
                                        [testClass:]
                                        $typeUnderTest$Tests
                                        
                                        !! Mapping from the source method name to the test method name.
                                        [testMethod:]
                                        $memberUnderTest$_Scenario_ExpectedBehavior

                                        !! Additional configurations
                                        !! -------------------------
                                        !!
                                        !! nuget packages to install when a new project is created
                                        !! PackageId : Version
                                        !! or for latest stable use only PackageId
                                        !! one nuget package per line
                                        [nugetPackages:]
                                        Microsoft.NET.Test.Sdk
                                        Twizzar.Api
                                        NUnit
                                        NUnit3TestAdapter
                                        """;
}