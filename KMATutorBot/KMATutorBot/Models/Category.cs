using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Models
{
    /// <summary>
    /// Represents category or subject (math, physics...)
    /// </summary>
    public class Category: IEquatable<Category>, IComparable<Category>
    {
        private static int _currentId = 1;
        public int Id { get; init; }
        public string Name { get; init; }
        public Category(string name)
        {
            this.Id = _currentId++;
            this.Name = name;
        }

        public bool Equals(Category that)
        {
            if (that == null) return false;
            return this.Id == that.Id;
        }

        public int CompareTo(Category that)
        {
            return this.Id - that.Id;
        }
        public override bool Equals(object that)
        {
            if (that == null || that is not Category thatCat) return false;
            return this.Equals(thatCat);
        }
        public override int GetHashCode()
        {
            return Id + Name.GetHashCode();
        }
        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}
