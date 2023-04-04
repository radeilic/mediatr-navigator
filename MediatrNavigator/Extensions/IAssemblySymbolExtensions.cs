using Microsoft.CodeAnalysis;

namespace MediatrNavigator.Extensions
{
    public static class IAssemblySymbolExtensions
    {
        public static bool IsEquivalentTo(this IAssemblySymbol thisAssembly, IAssemblySymbol otherAssembly)
        {
            if (thisAssembly == null || otherAssembly == null)
                return false;

            if (thisAssembly.Name != otherAssembly.Name)
                return false;

            AssemblyIdentity thisIdentity = thisAssembly.Identity;
            AssemblyIdentity otherIdentity = otherAssembly.Identity;

            if (thisIdentity.Name != otherIdentity.Name
                || thisIdentity.Version != otherIdentity.Version
                || !thisIdentity.PublicKeyToken.Equals(otherIdentity.PublicKeyToken))
                return false;

            return true;
        }
    }
}
