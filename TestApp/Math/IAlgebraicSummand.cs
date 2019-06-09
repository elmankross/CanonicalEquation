using System;

namespace TestApp.Math
{
    public interface IAlgebraicSummand : IEquatable<IAlgebraicSummand>
    {
        float Multiplier { get; }
    }
}