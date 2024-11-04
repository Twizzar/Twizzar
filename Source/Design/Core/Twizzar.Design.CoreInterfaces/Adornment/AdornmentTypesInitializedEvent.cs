using Twizzar.Design.CoreInterfaces.Common.Util;

namespace Twizzar.Design.CoreInterfaces.Adornment;

/// <summary>
/// Event fired when the type system is initialized. Used for example in the validators.
/// </summary>
public record AdornmentTypesInitializedEvent : IUiEvent;