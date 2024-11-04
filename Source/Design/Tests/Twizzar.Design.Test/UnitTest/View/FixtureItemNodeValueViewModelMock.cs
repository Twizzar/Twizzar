using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

using Moq;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.Design.Ui.Messaging;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.View.RichTextBox;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Test.UnitTest.View;

/// <summary>
/// Mock class implements the <see cref="IFixtureItemNodeValueViewModel"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class FixtureItemNodeValueViewModelMock : ViewModelBase, IFixtureItemNodeValueViewModel
{
    #region fields

    private string _fullText;

    #endregion

    #region ctors

    public FixtureItemNodeValueViewModelMock()
    {
        this.Commit = new RelayCommand(this.CommitChanges);
    }

    #endregion

    #region events

    public event Action<IViToken> ValueChanged;

    #endregion

    #region properties

    /// <inheritdoc />
    public string FullText
    {
        get => this._fullText;
        set
        {
            if (this._fullText != value)
            {
                this._fullText = value;

                this.ItemValueSegments = new List<ItemValueSegment>
                {
                    new ItemValueSegment(value, SegmentFormat.Type, true),
                };

                this.OnPropertyChanged(nameof(this.ItemValueSegments));

                this.ValueChanged?.Invoke(ViStringToken.Create(ParserPoint.New("a"), ParserPoint.New("b")));
            }
        }
    }

    /// <inheritdoc />
    public string DefaultValue => Guid.NewGuid().ToString();

    /// <inheritdoc />
    public string Tooltip { get; }

    /// <inheritdoc />
    public string AdornerText { get; }

    /// <inheritdoc />
    public bool HasFocus { get; set; }

    /// <inheritdoc />
    public IEnumerable<ItemValueSegment> ItemValueSegments { get; private set; } = new List<ItemValueSegment>();

    /// <inheritdoc />
    public IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; } = new List<AutoCompleteEntry>
    {
        new AutoCompleteEntry("ExampleCode.ICar", AutoCompleteFormat.Type),
        new AutoCompleteEntry("ExampleCode.IBike", AutoCompleteFormat.Type),
        new AutoCompleteEntry("ExampleCode.IVehicle", AutoCompleteFormat.Type),
        new AutoCompleteEntry("ExampleCode.IBus", AutoCompleteFormat.Type),
        new AutoCompleteEntry("ExampleCode.IBus #School bus", AutoCompleteFormat.TypeAndId),
        new AutoCompleteEntry("ExampleCode.IBus #Company bus", AutoCompleteFormat.TypeAndId),
        new AutoCompleteEntry("ExampleCode.IBus #Family bus", AutoCompleteFormat.TypeAndId),
        new AutoCompleteEntry(KeyWords.Default, AutoCompleteFormat.Keyword),
        new AutoCompleteEntry(KeyWords.Undefined, AutoCompleteFormat.Keyword),
        new AutoCompleteEntry(KeyWords.Null, AutoCompleteFormat.Keyword),
        new AutoCompleteEntry(KeyWords.Unique, AutoCompleteFormat.Keyword),
    };

    /// <inheritdoc />
    public bool IsReadOnly { get; } = false;

    /// <inheritdoc />
    public IUiEventHub UiEventHub { get; }

    /// <inheritdoc />
    public IVsColorService VsColorService { get; }

    /// <inheritdoc />
    public ICommand Commit { get; }

    /// <inheritdoc />
    public ICommand ExpandCollapseCommand { get; }

    /// <inheritdoc />
    public IFixtureItemNodeViewModel FixtureNodeVM { get; set; }

    /// <inheritdoc />
    public string ExpandAndCollapseShortcut { get; }

    /// <inheritdoc />
    public NodeId Id { get; }

    /// <summary>
    /// Gets the count of commits
    /// </summary>
    public int CommitsCount { get; private set; }

    public IScopedServiceProvider ServiceProvider
    {
        get
        {
            var mock = new Mock<IScopedServiceProvider>();

            mock
                .Setup(provider => provider.GetService<IUiEventHub>())
                .Returns(() => new UiEventHub());

            mock
                .Setup(provider => provider.GetService<IItemValueSegmentToRunConverter>())
                .Returns(() => new ItemValueSegmentToRunConverter(new DummyValueSegmentColorPicker()));

            return mock.Object;
        }

    }   
            

    #endregion

    #region members

    /// <inheritdoc />
    public Task LoadAutoCompleteElementsAsync() =>
        Task.CompletedTask;

    private void CommitChanges()
    {
        this.CommitsCount += 1;
    }

    #endregion
}