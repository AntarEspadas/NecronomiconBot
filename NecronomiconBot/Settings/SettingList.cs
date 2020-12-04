using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NecronomiconBot.Settings
{
    public class SettingList : IList<string>
    {
        private LinkedList<string> list;
        private TypeConverter converter;
        public readonly Type type;

        int ICollection<string>.Count => list.Count;

        bool ICollection<string>.IsReadOnly { get { return false;  } }
        public string this[int index] { get => GetNode(index).Value; set => SetValue(index, value); }
        public SettingList(LinkedList<string> list, Type type)
        {
            this.list = list;
            this.type = type;
            converter = TypeDescriptor.GetConverter(this.type);
        }

        public SettingList(Type type) : this(new LinkedList<string>(), type)
        {
        }

        private void ValidateValue(string value)
        {
            if (!converter.IsValid(value))
                throw new TypeMissmatchException($"Value {value} may not be used on a list of underlying type {type}");
        }

        private void SetValue(int index, string item)
        {
            VerifyIndex(index);
            ValidateValue(item);
            GetNode(index).Value = item;
        }

        private LinkedListNode<string> GetNode(int index)
        {
            VerifyIndex(index);
            LinkedListNode<string> result = list.First;
            for (int i = 0; i < index; i++)
            {
                result = result.Next;
            }
            return result;
        }

        private void VerifyIndex(int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new IndexOutOfRangeException($"Index {index} was outside of range for the SettingList");
            }
        }

        void ICollection<string>.Add(string item)
        {
            ValidateValue(item);
            list.AddLast(item);
        }

        void ICollection<string>.Clear()
        {
            list.Clear();
        }

        bool ICollection<string>.Contains(string item)
        {
            return list.Contains(item);
        }

        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        int IList<string>.IndexOf(string item)
        {
            int index = 0;
            foreach (var value in list)
            {
                if (value == item)
                    return index;
                index++;
            }
            return -1;
        }

        void IList<string>.Insert(int index, string item)
        {
            ValidateValue(item);
            LinkedListNode<string> node = GetNode(index);
            list.AddBefore(node, item);
        }

        bool ICollection<string>.Remove(string item)
        {
            return list.Remove(item);
        }

        void IList<string>.RemoveAt(int index)
        {
            LinkedListNode<string> node = GetNode(index);
            list.Remove(node);
        }

    }
}
