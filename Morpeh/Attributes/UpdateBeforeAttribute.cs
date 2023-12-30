using System;

namespace Scellecs.Morpeh.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateBeforeAttribute : Attribute
    {
        public Type Type { get; private set; }
        public UpdateBeforeAttribute(Type type) { Type = type; }
    }
}
