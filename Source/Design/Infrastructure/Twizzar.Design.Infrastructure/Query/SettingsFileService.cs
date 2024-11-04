using System;
using System.Collections.Generic;
using System.Linq;

using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.Query;

/// <summary>
/// Reads and writes settings to a file in a simple format.
/// </summary>
internal class SettingsFileService : ISettingsQuery, ISettingsWriter
{
    #region static fields and constants

    private const string EnableAnalyticsKey = "EnableAnalitics";
    private const string SettingsFilePath = @"%AppData%\vi-sit\twizzar\twizzar.config";
    private static readonly object Lock = new();

    #endregion

    #region fields

    private readonly IFileService _fileService;

    private Dictionary<string, string> _settings = new();

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsFileService"/> class.
    /// </summary>
    /// <param name="fileService"></param>
    public SettingsFileService(IFileService fileService)
    {
        this.EnsureParameter(fileService, nameof(fileService)).ThrowWhenNull();
        this._fileService = fileService;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public ILogger Logger { get; set; }

    /// <inheritdoc />
    public IEnsureHelper EnsureHelper { get; set; }

    private string SettingsFile { get; } = Environment.ExpandEnvironmentVariables(SettingsFilePath);

    #endregion

    #region members

    /// <inheritdoc />
    public bool GetAnalyticsEnabled() =>
        this._settings.GetMaybe(EnableAnalyticsKey)
            .Bind(s => bool.TryParse(s, out var result) ? result : Maybe.None<bool>())
            .SomeOrProvided(() => true);

    /// <inheritdoc />
    public void Initialize()
    {
        static string Process(string s) => s.Trim();
        lock (Lock)
        {
            try
            {
                if (!this._fileService.Exists(this.SettingsFile))
                {
                    this._fileService.Create(this.SettingsFile);
                }

                this._settings = this._fileService.ReadAllLines(this.SettingsFile)
                    .Select(s => s.Split(':'))
                    .Where(a => a.Length == 2)
                    .Select(a => (Process(a[0]), Process(a[1])))
                    .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }
    }

    /// <inheritdoc />
    public void SetAnalyticsEnabled(bool enabled)
    {
        lock (Lock)
        {
            this._settings[EnableAnalyticsKey] = enabled.ToString().ToLowerInvariant();
            this._fileService.WriteAllLines(this.SettingsFile, this._settings.Select(pair => $"{pair.Key}: {pair.Value}").ToArray());
        }
    }

    #endregion
}