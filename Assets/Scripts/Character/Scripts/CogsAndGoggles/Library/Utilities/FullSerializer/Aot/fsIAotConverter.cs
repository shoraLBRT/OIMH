using System;

namespace CogsAndGoggles.Library.Utilities.FullSerializer.Aot {
    /// <summary>
    /// Interface that AOT generated converters extend. Used to check to see if
    /// the AOT converter is up to date.
    /// </summary>
    public interface fsIAotConverter {
        Type ModelType { get; }
        fsAotVersionInfo VersionInfo { get; }
    }
}