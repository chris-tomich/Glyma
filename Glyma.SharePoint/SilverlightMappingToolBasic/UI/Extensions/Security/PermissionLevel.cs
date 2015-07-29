using System.Collections.Generic;
using SilverlightMappingToolBasic.GlymaSecurityService;

namespace SilverlightMappingToolBasic.UI.Extensions.Security
{
    public sealed class PermissionLevel
    {

        #region Constants - Permission Level Names
        // The names of the permissions used by Glyma
        public const string OldReaderRoleName = "Glyma Reader";
        public const string OldAuthorRoleName = "Glyma Author";
        public const string SecurityMagagerRoleName = "Glyma Security Manager";
        public const string ProjectManagerRoleName = "Glyma Project Manager";
        public const string MapManagerRoleName = "Glyma Map Manager";
        public const string AuthorRoleName = "Glyma Map Author";
        public const string ReaderRoleName = "Glyma Map Reader";
        public const string NoneRoleName = "None";
        #endregion


        private bool Equals(PermissionLevel other)
        {
            return string.Equals(_name, other._name) && _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is PermissionLevel && Equals((PermissionLevel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_name != null ? _name.GetHashCode() : 0)*397) ^ _value;
            }
        }

        private readonly string _name;
        private readonly int _value;

        public static readonly PermissionLevel OldAuthor = new PermissionLevel(4, OldAuthorRoleName);
        public static readonly PermissionLevel OldReader = new PermissionLevel(2, OldReaderRoleName);

        public static readonly PermissionLevel None = new PermissionLevel(0, NoneRoleName);
        public static readonly PermissionLevel Reader = new PermissionLevel(1, ReaderRoleName);
        public static readonly PermissionLevel Author = new PermissionLevel(3, AuthorRoleName);
        public static readonly PermissionLevel MapManager = new PermissionLevel(5, MapManagerRoleName);
        public static readonly PermissionLevel ProjectManager = new PermissionLevel(6, ProjectManagerRoleName);
        public static readonly PermissionLevel SecurityManager = new PermissionLevel(7, SecurityMagagerRoleName);

        private PermissionLevel(int value, string name)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return _name;
        }

        public static bool operator <(PermissionLevel x, PermissionLevel y)
        {
            return x._value < y._value;
        }

        public static bool operator >(PermissionLevel x, PermissionLevel y)
        {
            return x._value > y._value;
        }

        public static bool operator == (PermissionLevel x, PermissionLevel y)
        {
            return x._value == y._value;
        }

        public static bool operator != (PermissionLevel x, PermissionLevel y)
        {

            return x._value != y._value;
        }


        public static bool operator >=(PermissionLevel x, PermissionLevel y)
        {
            return x._value >= y._value;
        }

        public static bool operator <=(PermissionLevel x, PermissionLevel y)
        {
            return x._value <= y._value;
        }

        public static PermissionLevel Convert(string name)
        {
            switch (name)
            {
                case NoneRoleName:
                    return None;
                case OldReaderRoleName:
                    return OldReader;
                case OldAuthorRoleName:
                    return OldAuthor;
                case ReaderRoleName:
                    return Reader;
                case AuthorRoleName:
                    return Author;
                case SecurityMagagerRoleName:
                    return SecurityManager;
                case ProjectManagerRoleName:
                    return ProjectManager;
                case MapManagerRoleName:
                    return MapManager;
                default:
                    return None;
            }
        }


        public static PermissionLevel Convert(GlymaPermissionLevel value)
        {
            switch (value)
            {
                case GlymaPermissionLevel.GlymaMapAuthor:
                    return Author;
                case GlymaPermissionLevel.GlymaMapReader:
                    return Reader;
                case GlymaPermissionLevel.GlymaMapManager:
                    return MapManager;
                case GlymaPermissionLevel.GlymaProjectManager:
                    return ProjectManager;
                case GlymaPermissionLevel.GlymaSecurityManager:
                    return SecurityManager;
                default:
                    return None;
            }
        }

        public static bool IsOldPermission(PermissionLevel value)
        {
            return value == OldAuthor || value == OldReader;
        }

        public static PermissionLevel GetHighestPermission(List<string> names)
        {
            var current = None;
            if (names != null && names.Count > 0)
            {
                foreach (var name in names)
                {
                    var item = Convert(name);
                    if (item > current)
                    {
                        current = item;
                    }
                }
            }
            return current;
        }
    }
}
