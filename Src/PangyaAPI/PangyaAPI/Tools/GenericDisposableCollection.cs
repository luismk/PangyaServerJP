using System.Collections.Generic;

namespace PangyaAPI.Tools
{
    public class GenericDisposableCollection<T> where T : class, IDisposeable
    {
        private List<T> _model;

        public List<T> Model
        {
            get
            {
                //Remove pessoas Disposed
                _model?.RemoveAll(p => p != null && p.Disposed);

                return _model;
            }
            set { _model = value; }
        }

        public T this[int index]
        {
            get
            {
                return Model[index];
            }
            set
            {
                Model[index] = value;
            }
        }

        public GenericDisposableCollection()
        {
            Model = new List<T>();
        }

        public void Add(T pessoa)
        {
            Model.Add(pessoa);
        }
        public bool Remove(T pessoa)
        {
            return Model.Remove(pessoa);
        }

        public List<T> ToList()
        {
            return Model;
        }

        public int Count
        {
            get
            {
                return Model.Count;
            }
        }

    }
}
