using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Archive
{
    public class Property<T> : IEquatable<Property<T>> where T : new()
    {
        private T value;
        public T Value
        {
            get => Value;
            set
            {
                if (Equals(this.value, value)) return;
                Console.WriteLine($"Assigning value to {value}");
                this.value = value;
            }
        }

        public Property() : this(default(T))
        {

        }

        public Property(T value)
        {
            this.value = value;
        }

        public bool Equals(Property<T>? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }

        //public override bool Equals(object? obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((Property<T>) obj);
        //}

        //public override int GetHashCode()
        //{
        //    return value.GetHashCode();
        //}

        //public static bool operator ==(Property<T> left, Property<T> right)
        //{
        //    return Equals(left, right);
        //}

        //public static bool operator !=(Property<T> left, Property<T> right)
        //{
        //    return !Equals(left, right);
        //}VV
    }

    public class Creature
    {
        private Property<int> agility = new Property<int>();
        public int Agility
        {
            get => agility.Value;
            set => agility.Value = value;
        }


    }

    public class Sentence
    {
        private string PlainText;
        private WordToken[] WordTokens;
        public Sentence(string plainText)
        {
            // todo
            PlainText = plainText;
            WordTokens = plainText.Split(' ')
                .Select(p => new WordToken())
                .ToArray();
        }

        public WordToken this[int index]
        {
            get
            {
                // todo
                return WordTokens[index];
            }
        }

        public override string ToString()
        {
            // output formatted text here
            var sb = new StringBuilder();
            var pt = PlainText.Split(' ').ToList();
            for (int i = 0; i < pt.Count; i++)
            {
                if (WordTokens[i].Capitalize)
                {
                    sb.Append(pt[i].ToUpper());
                }
                else
                {
                    sb.Append(pt[i]);
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }

        public class WordToken
        {
            public bool Capitalize = false;
        }
    }
}
