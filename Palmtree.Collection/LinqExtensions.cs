/*
  LinqExtensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Palmtree.Collection
{
    /// <summary>
    /// Linqをサポートする拡張メソッドのクラスです。
    /// </summary>
    public static class LinqExtensions
    {
        #region 階乗進数カウンタ の定義

        private class 階乗進数カウンタ
        {
            #region プライベートフィールド

            private int[] _counter;

            #endregion

            #region コンストラクタ

            public 階乗進数カウンタ(int length)
            {
                _counter = new int[length + 1];
            }

            #endregion

            #region パブリックメソッド

            public void Increment()
            {
                int index = 1;
                _counter[index] += 1;
                while (_counter[index] >= index + 1)
                {
                    _counter[index] -= index + 1;
                    _counter[index + 1] += 1;
                    ++index;
                }
            }

            #endregion

            #region パブリックプロパティ

            public bool EndOfCounter
            {
                get
                {
                    return (_counter[_counter.Length - 1] > 0);
                }
            }

            public IEnumerable<int> Value
            {
                get
                {
                    return (_counter.Reverse().Skip(1));
                }
            }

            #endregion
        }

        #endregion

        #region  ChunkEnumerable<ELEMENT_T> の定義

        private class ChunkEnumerable<ELEMENT_T>
            : IEnumerable<IEnumerable<ELEMENT_T>>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private int _chunk_size;

            #endregion

            #region コンストラクタ

            public ChunkEnumerable(IEnumerable<ELEMENT_T> source, int chunk_size)
            {
                _source = source;
                _chunk_size = chunk_size;
            }

            #endregion

            #region IEnumerable<IEnumerable<ELEMENT_T>> のメンバ

            public IEnumerator<IEnumerable<ELEMENT_T>> GetEnumerator()
            {
                return (new ChunkEnumerator<ELEMENT_T>(_source, _chunk_size));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ChunkEnumerator<ELEMENT_T> の定義

        private class ChunkEnumerator<ELEMENT_T>
            : IEnumerator<IEnumerable<ELEMENT_T>>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private int _chunk_size;
            private bool _disposed;
            private IEnumerator<ELEMENT_T> _imp;
            private IEnumerable<ELEMENT_T> _current_value;

            #endregion

            #region コンストラクタ

            public ChunkEnumerator(IEnumerable<ELEMENT_T> source, int chunk_size)
            {
                _source = source;
                _chunk_size = chunk_size;
                _disposed = false;
                _imp = source.GetEnumerator();
                _current_value = null;
            }

            ~ChunkEnumerator()
            {
                Dispose(false);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        ReleaseSourceEnumerator();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void ReleaseSourceEnumerator()
            {
                if (_imp != null)
                {
                    _imp.Dispose();
                    _imp = null;
                }
            }

            #endregion

            #region IEnumerator<IEnumerable<ELEMENT_T>> のメンバ

            public IEnumerable<ELEMENT_T> Current
            {
                get
                {
                    if (_current_value == null)
                        throw (new InvalidOperationException());
                    return (_current_value);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (Current);
                }
            }

            public bool MoveNext()
            {
                var value = new List<ELEMENT_T>();
                for (int count = 0; count < _chunk_size && _imp.MoveNext(); ++count)
                    value.Add(_imp.Current);
                _current_value = value;
                return (value.Count > 0);
            }

            public void Reset()
            {
                ReleaseSourceEnumerator();
                _imp = _source.GetEnumerator();
                _current_value = null;
            }

            #endregion

            #region IDisposable のメンバ

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        #endregion

        #region  AsEnumerableEnumerable<ELEMENT_T> の定義

        private class AsEnumerableEnumerable<ELEMENT_T>
            : IEnumerable<ELEMENT_T>
        {
            #region プライベートフィールド

            private IEnumerable _source;
            private Func<object, ELEMENT_T> _element_converter;

            #endregion

            #region コンストラクタ

            public AsEnumerableEnumerable(IEnumerable source, Func<object, ELEMENT_T> element_converter)
            {
                _source = source;
                _element_converter = element_converter;
            }

            #endregion

            #region IEnumerable<IEnumerable<ELEMENT_T>> のメンバ

            public IEnumerator<ELEMENT_T> GetEnumerator()
            {
                return (new AsEnumerableEnumerator<ELEMENT_T>(_source, _element_converter));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region AsEnumerableEnumerator<ELEMENT_T> の定義

        private class AsEnumerableEnumerator<ELEMENT_T>
            : IEnumerator<ELEMENT_T>
        {
            #region プライベートフィールド

            private IEnumerable _source;
            private Func<object, ELEMENT_T> _element_converter;
            private bool _disposed;
            private IEnumerator _imp;

            #endregion

            #region コンストラクタ

            public AsEnumerableEnumerator(IEnumerable source, Func<object, ELEMENT_T> element_converter)
            {
                _source = source;
                _element_converter = element_converter;
                _disposed = false;
                _imp = source.GetEnumerator();
            }

            ~AsEnumerableEnumerator()
            {
                Dispose(false);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        ReleaseSourceEnumerator();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void ReleaseSourceEnumerator()
            {
                if (_imp is IDisposable)
                {
                    ((IDisposable)_imp).Dispose();
                    _imp = null;
                }
            }

            #endregion

            #region IEnumerator<ELEMENT_T> のメンバ

            public ELEMENT_T Current
            {
                get
                {
                    return (_element_converter(_imp.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (Current);
                }
            }

            public bool MoveNext()
            {
                return (_imp.MoveNext());
            }

            public void Reset()
            {
                _imp.Reset();
            }

            #endregion

            #region IDisposable のメンバ

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        #endregion

        #region KeyComparer<ELEMENT_T, KEY_T> の定義

        private class KeyComparer<ELEMENT_T, KEY_T>
            : IEqualityComparer<ELEMENT_T>
        {
            #region プライベートフィールド

            private Func<ELEMENT_T, KEY_T> _key_selecter;

            #endregion

            #region コンストラクタ

            public KeyComparer(Func<ELEMENT_T, KEY_T> key_selecter)
            {
                _key_selecter = key_selecter;
            }

            #endregion

            #region IEqualityComparer<ELEMENT_T> のメンバ

            bool IEqualityComparer<ELEMENT_T>.Equals(ELEMENT_T x, ELEMENT_T y)
            {
                if (x == null)
                    return (y == null);
                else if (y == null)
                    return (false);
                else
                {
                    var key_x = _key_selecter(x);
                    var key_y = _key_selecter(y);
                    if (key_x == null)
                        return (key_y == null);
                    else if (key_y == null)
                        return (false);
                    else
                        return (key_x.Equals(key_y));
                }
            }

            int IEqualityComparer<ELEMENT_T>.GetHashCode(ELEMENT_T o)
            {
                if (o == null)
                    return (0);
                else
                    return (_key_selecter(o).GetHashCode());
            }

            #endregion
        }

        #endregion

        #region  SequenceEnumerable<ELEMENT_T> の定義

        private class SequenceEnumerable<ELEMENT_T>
            : IEnumerable<ELEMENT_T>
        {
            #region プライベートフィールド

            private ELEMENT_T _first_term;
            private Func<int, ELEMENT_T, ELEMENT_T> _general_term;
            private Func<int, ELEMENT_T, bool> _continuous_condition;

            #endregion

            #region コンストラクタ

            public SequenceEnumerable(ELEMENT_T first_term, Func<int, ELEMENT_T, ELEMENT_T> general_term, Func<int, ELEMENT_T, bool> continuous_condition)
            {
                _first_term = first_term;
                _general_term = general_term;
                _continuous_condition = continuous_condition;
            }

            #endregion

            #region IEnumerable<IEnumerable<ELEMENT_T>> のメンバ

            public IEnumerator<ELEMENT_T> GetEnumerator()
            {
                return (new SequenceEnumerator<ELEMENT_T>(_first_term, _general_term, _continuous_condition));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region SequenceEnumerator<ELEMENT_T> の定義

        private class SequenceEnumerator<ELEMENT_T>
            : IEnumerator<ELEMENT_T>
        {
            #region SequenceState の定義

            private enum SequenceState
            {
                未計算,
                計算中,
                終わりの項に達した,
            }

            #endregion

            #region プライベートフィールド

            private ELEMENT_T _first_term;
            private Func<int, ELEMENT_T, ELEMENT_T> _general_term;
            private Func<int, ELEMENT_T, bool> _continuous_condition;
            private bool _disposed;
            private SequenceState _state;
            private int _n;
            private ELEMENT_T _current_term;

            #endregion

            #region コンストラクタ

            public SequenceEnumerator(ELEMENT_T first_term, Func<int, ELEMENT_T, ELEMENT_T> general_term, Func<int, ELEMENT_T, bool> continuous_condition)
            {
                _first_term = first_term;
                _general_term = general_term;
                _continuous_condition = continuous_condition;
                _disposed = false;
                _state = SequenceState.未計算;
                _n = -1;
                _current_term = default(ELEMENT_T);
            }

            ~SequenceEnumerator()
            {
                Dispose(false);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            #endregion

            #region IEnumerator<ELEMENT_T> のメンバ

            public ELEMENT_T Current
            {
                get
                {
                    if (_state != SequenceState.計算中)
                        throw (new InvalidOperationException());
                    return (_current_term);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (Current);
                }
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case SequenceState.未計算:
                        _state = SequenceState.計算中;
                        _n = 0;
                        _current_term = _first_term;
                        if (!_continuous_condition(_n, _current_term))
                            _state = SequenceState.終わりの項に達した;
                        break;
                    case SequenceState.計算中:
                        _current_term = _general_term(_n, _current_term);
                        ++_n;
                        if (!_continuous_condition(_n, _current_term))
                            _state = SequenceState.終わりの項に達した;
                        break;
                    case SequenceState.終わりの項に達した:
                    default:
                        break;
                }
                return (_state != SequenceState.終わりの項に達した);
            }

            public void Reset()
            {
                _state = SequenceState.未計算;
                _current_term = default(ELEMENT_T);
            }

            #endregion

            #region IDisposable のメンバ

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        #endregion

        #region MinMaxAccumulator<ELEMENT_T, VALUE_T> の定義

        private class MinMaxAccumulator<ELEMENT_T, VALUE_T>
        {
            #region コンストラクタ

            public MinMaxAccumulator(ELEMENT_T item, VALUE_T value)
            {
                Item = item;
                Value = value;
            }

            #endregion

            #region パブリックプロパティ

            public ELEMENT_T Item { get; private set; }
            public VALUE_T Value { get; private set; }

            #endregion
        }

        #endregion

        #region ZipEnumerable3 の定義

        private class ZipEnumerable3<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T>
            : IEnumerable<RESULT_T>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T> _result_selector;

            #endregion

            #region コンストラクタ

            public ZipEnumerable3(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T> result_selector)
            {
                if (source == null)
                    throw new ArgumentNullException();
                if (source2 == null)
                    throw new ArgumentNullException();
                if (source3 == null)
                    throw new ArgumentNullException();
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _result_selector = result_selector;
            }

            #endregion

            #region IEnumerable<RESULT_T> の定義

            public IEnumerator<RESULT_T> GetEnumerator()
            {
                return (new ZipEnumerator3<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T>(_source, _source2, _source3, _result_selector));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ZipEnumerator3 の定義

        private class ZipEnumerator3<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T>
            : IEnumerator<RESULT_T>
        {
            #region プライベートフィールド

            private bool _disposed;
            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T> _result_selector;
            private IEnumerator<ELEMENT_T> _enumerator;
            private IEnumerator<ELEMENT2_T> _enumerator2;
            private IEnumerator<ELEMENT3_T> _enumerator3;
            private RESULT_T _current_object;
            private bool _is_valid_current_object;

            #endregion

            #region コンストラクタ

            public ZipEnumerator3(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, RESULT_T> result_selector)
            {
                _disposed = false;
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _result_selector = result_selector;
                InitializeImp();
            }

            ~ZipEnumerator3()
            {
                Dispose(false);
            }

            #endregion

            #region パブリックメソッド

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        DisposeImp();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void InitializeImp()
            {
                _enumerator = null;
                _enumerator2 = null;
                _enumerator3 = null;
                _current_object = default(RESULT_T);
                _is_valid_current_object = false;
            }

            private void DisposeImp()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                if (_enumerator2 != null)
                {
                    _enumerator2.Dispose();
                    _enumerator = null;
                }
                if (_enumerator3 != null)
                {
                    _enumerator3.Dispose();
                    _enumerator = null;
                }
            }

            #endregion

            #region IEnumerator<RESULT_T> の定義

            public RESULT_T Current
            {
                get
                {
                    if (!_is_valid_current_object)
                        throw new InvalidOperationException();
                    return (_current_object);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (_current_object);
                }
            }

            public bool MoveNext()
            {
                if (_enumerator == null)
                    _enumerator = _source.GetEnumerator();
                if (_enumerator2 == null)
                    _enumerator2 = _source2.GetEnumerator();
                if (_enumerator3 == null)
                    _enumerator3 = _source3.GetEnumerator();
                _is_valid_current_object = _enumerator.MoveNext() &&
                                           _enumerator2.MoveNext() &&
                                           _enumerator3.MoveNext();
                if (_is_valid_current_object)
                    _current_object = _result_selector(_enumerator.Current, _enumerator2.Current, _enumerator3.Current);
                return (_is_valid_current_object);
            }

            public void Reset()
            {
                DisposeImp();
                InitializeImp();
            }

            #endregion
        }

        #endregion

        #region ZipEnumerable4 の定義

        private class ZipEnumerable4<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T>
            : IEnumerable<RESULT_T>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T> _result_selector;

            #endregion

            #region コンストラクタ

            public ZipEnumerable4(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T> result_selector)
            {
                if (source == null)
                    throw new ArgumentNullException();
                if (source2 == null)
                    throw new ArgumentNullException();
                if (source3 == null)
                    throw new ArgumentNullException();
                if (source4 == null)
                    throw new ArgumentNullException();
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _result_selector = result_selector;
            }

            #endregion

            #region IEnumerable<RESULT_T> の定義

            public IEnumerator<RESULT_T> GetEnumerator()
            {
                return (new ZipEnumerator4<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T>(_source, _source2, _source3, _source4, _result_selector));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ZipEnumerator4 の定義

        private class ZipEnumerator4<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T>
            : IEnumerator<RESULT_T>
        {
            #region プライベートフィールド

            private bool _disposed;
            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T> _result_selector;
            private IEnumerator<ELEMENT_T> _enumerator;
            private IEnumerator<ELEMENT2_T> _enumerator2;
            private IEnumerator<ELEMENT3_T> _enumerator3;
            private IEnumerator<ELEMENT4_T> _enumerator4;
            private RESULT_T _current_object;
            private bool _is_valid_current_object;

            #endregion

            #region コンストラクタ

            public ZipEnumerator4(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T> result_selector)
            {
                _disposed = false;
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _result_selector = result_selector;
                InitializeImp();
            }

            ~ZipEnumerator4()
            {
                Dispose(false);
            }

            #endregion

            #region パブリックメソッド

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        DisposeImp();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void InitializeImp()
            {
                _enumerator = null;
                _enumerator2 = null;
                _enumerator3 = null;
                _enumerator4 = null;
                _current_object = default(RESULT_T);
                _is_valid_current_object = false;
            }

            private void DisposeImp()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                if (_enumerator2 != null)
                {
                    _enumerator2.Dispose();
                    _enumerator = null;
                }
                if (_enumerator3 != null)
                {
                    _enumerator3.Dispose();
                    _enumerator = null;
                }
                if (_enumerator4 != null)
                {
                    _enumerator4.Dispose();
                    _enumerator = null;
                }
            }

            #endregion

            #region IEnumerator<RESULT_T> の定義

            public RESULT_T Current
            {
                get
                {
                    if (!_is_valid_current_object)
                        throw new InvalidOperationException();
                    return (_current_object);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (_current_object);
                }
            }

            public bool MoveNext()
            {
                if (_enumerator == null)
                    _enumerator = _source.GetEnumerator();
                if (_enumerator2 == null)
                    _enumerator2 = _source2.GetEnumerator();
                if (_enumerator3 == null)
                    _enumerator3 = _source3.GetEnumerator();
                if (_enumerator4 == null)
                    _enumerator4 = _source4.GetEnumerator();
                _is_valid_current_object = _enumerator.MoveNext() &&
                                           _enumerator2.MoveNext() &&
                                           _enumerator3.MoveNext() &&
                                           _enumerator4.MoveNext();
                if (_is_valid_current_object)
                    _current_object = _result_selector(_enumerator.Current, _enumerator2.Current, _enumerator3.Current, _enumerator4.Current);
                return (_is_valid_current_object);
            }

            public void Reset()
            {
                DisposeImp();
                InitializeImp();
            }

            #endregion
        }

        #endregion

        #region ZipEnumerable5 の定義

        private class ZipEnumerable5<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T>
            : IEnumerable<RESULT_T>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T> _result_selector;

            #endregion

            #region コンストラクタ

            public ZipEnumerable5(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T> result_selector)
            {
                if (source == null)
                    throw new ArgumentNullException();
                if (source2 == null)
                    throw new ArgumentNullException();
                if (source3 == null)
                    throw new ArgumentNullException();
                if (source4 == null)
                    throw new ArgumentNullException();
                if (source5 == null)
                    throw new ArgumentNullException();
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _result_selector = result_selector;
            }

            #endregion

            #region IEnumerable<RESULT_T> の定義

            public IEnumerator<RESULT_T> GetEnumerator()
            {
                return (new ZipEnumerator5<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T>(_source, _source2, _source3, _source4, _source5, _result_selector));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ZipEnumerator5 の定義

        private class ZipEnumerator5<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T>
            : IEnumerator<RESULT_T>
        {
            #region プライベートフィールド

            private bool _disposed;
            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T> _result_selector;
            private IEnumerator<ELEMENT_T> _enumerator;
            private IEnumerator<ELEMENT2_T> _enumerator2;
            private IEnumerator<ELEMENT3_T> _enumerator3;
            private IEnumerator<ELEMENT4_T> _enumerator4;
            private IEnumerator<ELEMENT5_T> _enumerator5;
            private RESULT_T _current_object;
            private bool _is_valid_current_object;

            #endregion

            #region コンストラクタ

            public ZipEnumerator5(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T> result_selector)
            {
                _disposed = false;
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _result_selector = result_selector;
                InitializeImp();
            }

            ~ZipEnumerator5()
            {
                Dispose(false);
            }

            #endregion

            #region パブリックメソッド

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        DisposeImp();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void InitializeImp()
            {
                _enumerator = null;
                _enumerator2 = null;
                _enumerator3 = null;
                _enumerator4 = null;
                _enumerator5 = null;
                _current_object = default(RESULT_T);
                _is_valid_current_object = false;
            }

            private void DisposeImp()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                if (_enumerator2 != null)
                {
                    _enumerator2.Dispose();
                    _enumerator = null;
                }
                if (_enumerator3 != null)
                {
                    _enumerator3.Dispose();
                    _enumerator = null;
                }
                if (_enumerator4 != null)
                {
                    _enumerator4.Dispose();
                    _enumerator = null;
                }
                if (_enumerator5 != null)
                {
                    _enumerator5.Dispose();
                    _enumerator = null;
                }
            }

            #endregion

            #region IEnumerator<RESULT_T> の定義

            public RESULT_T Current
            {
                get
                {
                    if (!_is_valid_current_object)
                        throw new InvalidOperationException();
                    return (_current_object);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (_current_object);
                }
            }

            public bool MoveNext()
            {
                if (_enumerator == null)
                    _enumerator = _source.GetEnumerator();
                if (_enumerator2 == null)
                    _enumerator2 = _source2.GetEnumerator();
                if (_enumerator3 == null)
                    _enumerator3 = _source3.GetEnumerator();
                if (_enumerator4 == null)
                    _enumerator4 = _source4.GetEnumerator();
                if (_enumerator5 == null)
                    _enumerator5 = _source5.GetEnumerator();
                _is_valid_current_object = _enumerator.MoveNext() &&
                                           _enumerator2.MoveNext() &&
                                           _enumerator3.MoveNext() &&
                                           _enumerator4.MoveNext() &&
                                           _enumerator5.MoveNext();
                if (_is_valid_current_object)
                    _current_object = _result_selector(_enumerator.Current, _enumerator2.Current, _enumerator3.Current, _enumerator4.Current, _enumerator5.Current);
                return (_is_valid_current_object);
            }

            public void Reset()
            {
                DisposeImp();
                InitializeImp();
            }

            #endregion
        }

        #endregion

        #region ZipEnumerable6 の定義

        private class ZipEnumerable6<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T>
            : IEnumerable<RESULT_T>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private IEnumerable<ELEMENT6_T> _source6;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T> _result_selector;

            #endregion

            #region コンストラクタ

            public ZipEnumerable6(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T> result_selector)
            {
                if (source == null)
                    throw new ArgumentNullException();
                if (source2 == null)
                    throw new ArgumentNullException();
                if (source3 == null)
                    throw new ArgumentNullException();
                if (source4 == null)
                    throw new ArgumentNullException();
                if (source5 == null)
                    throw new ArgumentNullException();
                if (source6 == null)
                    throw new ArgumentNullException();
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _source6 = source6;
                _result_selector = result_selector;
            }

            #endregion

            #region IEnumerable<RESULT_T> の定義

            public IEnumerator<RESULT_T> GetEnumerator()
            {
                return (new ZipEnumerator6<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T>(_source, _source2, _source3, _source4, _source5, _source6, _result_selector));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ZipEnumerator6 の定義

        private class ZipEnumerator6<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T>
            : IEnumerator<RESULT_T>
        {
            #region プライベートフィールド

            private bool _disposed;
            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private IEnumerable<ELEMENT6_T> _source6;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T> _result_selector;
            private IEnumerator<ELEMENT_T> _enumerator;
            private IEnumerator<ELEMENT2_T> _enumerator2;
            private IEnumerator<ELEMENT3_T> _enumerator3;
            private IEnumerator<ELEMENT4_T> _enumerator4;
            private IEnumerator<ELEMENT5_T> _enumerator5;
            private IEnumerator<ELEMENT6_T> _enumerator6;
            private RESULT_T _current_object;
            private bool _is_valid_current_object;

            #endregion

            #region コンストラクタ

            public ZipEnumerator6(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T> result_selector)
            {
                _disposed = false;
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _source6 = source6;
                _result_selector = result_selector;
                InitializeImp();
            }

            ~ZipEnumerator6()
            {
                Dispose(false);
            }

            #endregion

            #region パブリックメソッド

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        DisposeImp();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void InitializeImp()
            {
                _enumerator = null;
                _enumerator2 = null;
                _enumerator3 = null;
                _enumerator4 = null;
                _enumerator5 = null;
                _enumerator6 = null;
                _current_object = default(RESULT_T);
                _is_valid_current_object = false;
            }

            private void DisposeImp()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                if (_enumerator2 != null)
                {
                    _enumerator2.Dispose();
                    _enumerator = null;
                }
                if (_enumerator3 != null)
                {
                    _enumerator3.Dispose();
                    _enumerator = null;
                }
                if (_enumerator4 != null)
                {
                    _enumerator4.Dispose();
                    _enumerator = null;
                }
                if (_enumerator5 != null)
                {
                    _enumerator5.Dispose();
                    _enumerator = null;
                }
                if (_enumerator6 != null)
                {
                    _enumerator6.Dispose();
                    _enumerator = null;
                }
            }

            #endregion

            #region IEnumerator<RESULT_T> の定義

            public RESULT_T Current
            {
                get
                {
                    if (!_is_valid_current_object)
                        throw new InvalidOperationException();
                    return (_current_object);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (_current_object);
                }
            }

            public bool MoveNext()
            {
                if (_enumerator == null)
                    _enumerator = _source.GetEnumerator();
                if (_enumerator2 == null)
                    _enumerator2 = _source2.GetEnumerator();
                if (_enumerator3 == null)
                    _enumerator3 = _source3.GetEnumerator();
                if (_enumerator4 == null)
                    _enumerator4 = _source4.GetEnumerator();
                if (_enumerator5 == null)
                    _enumerator5 = _source5.GetEnumerator();
                if (_enumerator6 == null)
                    _enumerator6 = _source6.GetEnumerator();
                _is_valid_current_object = _enumerator.MoveNext() &&
                                           _enumerator2.MoveNext() &&
                                           _enumerator3.MoveNext() &&
                                           _enumerator4.MoveNext() &&
                                           _enumerator5.MoveNext() &&
                                           _enumerator6.MoveNext();
                if (_is_valid_current_object)
                    _current_object = _result_selector(_enumerator.Current, _enumerator2.Current, _enumerator3.Current, _enumerator4.Current, _enumerator5.Current, _enumerator6.Current);
                return (_is_valid_current_object);
            }

            public void Reset()
            {
                DisposeImp();
                InitializeImp();
            }

            #endregion
        }

        #endregion

        #region ZipEnumerable7 の定義

        private class ZipEnumerable7<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T>
            : IEnumerable<RESULT_T>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private IEnumerable<ELEMENT6_T> _source6;
            private IEnumerable<ELEMENT7_T> _source7;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T> _result_selector;

            #endregion

            #region コンストラクタ

            public ZipEnumerable7(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, IEnumerable<ELEMENT7_T> source7, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T> result_selector)
            {
                if (source == null)
                    throw new ArgumentNullException();
                if (source2 == null)
                    throw new ArgumentNullException();
                if (source3 == null)
                    throw new ArgumentNullException();
                if (source4 == null)
                    throw new ArgumentNullException();
                if (source5 == null)
                    throw new ArgumentNullException();
                if (source6 == null)
                    throw new ArgumentNullException();
                if (source7 == null)
                    throw new ArgumentNullException();
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _source6 = source6;
                _source7 = source7;
                _result_selector = result_selector;
            }

            #endregion

            #region IEnumerable<RESULT_T> の定義

            public IEnumerator<RESULT_T> GetEnumerator()
            {
                return (new ZipEnumerator7<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T>(_source, _source2, _source3, _source4, _source5, _source6, _source7, _result_selector));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ZipEnumerator7 の定義

        private class ZipEnumerator7<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T>
            : IEnumerator<RESULT_T>
        {
            #region プライベートフィールド

            private bool _disposed;
            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private IEnumerable<ELEMENT6_T> _source6;
            private IEnumerable<ELEMENT7_T> _source7;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T> _result_selector;
            private IEnumerator<ELEMENT_T> _enumerator;
            private IEnumerator<ELEMENT2_T> _enumerator2;
            private IEnumerator<ELEMENT3_T> _enumerator3;
            private IEnumerator<ELEMENT4_T> _enumerator4;
            private IEnumerator<ELEMENT5_T> _enumerator5;
            private IEnumerator<ELEMENT6_T> _enumerator6;
            private IEnumerator<ELEMENT7_T> _enumerator7;
            private RESULT_T _current_object;
            private bool _is_valid_current_object;

            #endregion

            #region コンストラクタ

            public ZipEnumerator7(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, IEnumerable<ELEMENT7_T> source7, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T> result_selector)
            {
                _disposed = false;
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _source6 = source6;
                _source7 = source7;
                _result_selector = result_selector;
                InitializeImp();
            }

            ~ZipEnumerator7()
            {
                Dispose(false);
            }

            #endregion

            #region パブリックメソッド

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        DisposeImp();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void InitializeImp()
            {
                _enumerator = null;
                _enumerator2 = null;
                _enumerator3 = null;
                _enumerator4 = null;
                _enumerator5 = null;
                _enumerator6 = null;
                _enumerator7 = null;
                _current_object = default(RESULT_T);
                _is_valid_current_object = false;
            }

            private void DisposeImp()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                if (_enumerator2 != null)
                {
                    _enumerator2.Dispose();
                    _enumerator = null;
                }
                if (_enumerator3 != null)
                {
                    _enumerator3.Dispose();
                    _enumerator = null;
                }
                if (_enumerator4 != null)
                {
                    _enumerator4.Dispose();
                    _enumerator = null;
                }
                if (_enumerator5 != null)
                {
                    _enumerator5.Dispose();
                    _enumerator = null;
                }
                if (_enumerator6 != null)
                {
                    _enumerator6.Dispose();
                    _enumerator = null;
                }
                if (_enumerator7 != null)
                {
                    _enumerator7.Dispose();
                    _enumerator = null;
                }
            }

            #endregion

            #region IEnumerator<RESULT_T> の定義

            public RESULT_T Current
            {
                get
                {
                    if (!_is_valid_current_object)
                        throw new InvalidOperationException();
                    return (_current_object);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (_current_object);
                }
            }

            public bool MoveNext()
            {
                if (_enumerator == null)
                    _enumerator = _source.GetEnumerator();
                if (_enumerator2 == null)
                    _enumerator2 = _source2.GetEnumerator();
                if (_enumerator3 == null)
                    _enumerator3 = _source3.GetEnumerator();
                if (_enumerator4 == null)
                    _enumerator4 = _source4.GetEnumerator();
                if (_enumerator5 == null)
                    _enumerator5 = _source5.GetEnumerator();
                if (_enumerator6 == null)
                    _enumerator6 = _source6.GetEnumerator();
                if (_enumerator7 == null)
                    _enumerator7 = _source7.GetEnumerator();
                _is_valid_current_object = _enumerator.MoveNext() &&
                                           _enumerator2.MoveNext() &&
                                           _enumerator3.MoveNext() &&
                                           _enumerator4.MoveNext() &&
                                           _enumerator5.MoveNext() &&
                                           _enumerator6.MoveNext() &&
                                           _enumerator7.MoveNext();
                if (_is_valid_current_object)
                    _current_object = _result_selector(_enumerator.Current, _enumerator2.Current, _enumerator3.Current, _enumerator4.Current, _enumerator5.Current, _enumerator6.Current, _enumerator7.Current);
                return (_is_valid_current_object);
            }

            public void Reset()
            {
                DisposeImp();
                InitializeImp();
            }

            #endregion
        }

        #endregion

        #region ZipEnumerable8 の定義

        private class ZipEnumerable8<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T>
            : IEnumerable<RESULT_T>
        {
            #region プライベートフィールド

            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private IEnumerable<ELEMENT6_T> _source6;
            private IEnumerable<ELEMENT7_T> _source7;
            private IEnumerable<ELEMENT8_T> _source8;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T> _result_selector;

            #endregion

            #region コンストラクタ

            public ZipEnumerable8(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, IEnumerable<ELEMENT7_T> source7, IEnumerable<ELEMENT8_T> source8, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T> result_selector)
            {
                if (source == null)
                    throw new ArgumentNullException();
                if (source2 == null)
                    throw new ArgumentNullException();
                if (source3 == null)
                    throw new ArgumentNullException();
                if (source4 == null)
                    throw new ArgumentNullException();
                if (source5 == null)
                    throw new ArgumentNullException();
                if (source6 == null)
                    throw new ArgumentNullException();
                if (source7 == null)
                    throw new ArgumentNullException();
                if (source8 == null)
                    throw new ArgumentNullException();
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _source6 = source6;
                _source7 = source7;
                _source8 = source8;
                _result_selector = result_selector;
            }

            #endregion

            #region IEnumerable<RESULT_T> の定義

            public IEnumerator<RESULT_T> GetEnumerator()
            {
                return (new ZipEnumerator8<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T>(_source, _source2, _source3, _source4, _source5, _source6, _source7, _source8, _result_selector));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region ZipEnumerator8 の定義

        private class ZipEnumerator8<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T>
            : IEnumerator<RESULT_T>
        {
            #region プライベートフィールド

            private bool _disposed;
            private IEnumerable<ELEMENT_T> _source;
            private IEnumerable<ELEMENT2_T> _source2;
            private IEnumerable<ELEMENT3_T> _source3;
            private IEnumerable<ELEMENT4_T> _source4;
            private IEnumerable<ELEMENT5_T> _source5;
            private IEnumerable<ELEMENT6_T> _source6;
            private IEnumerable<ELEMENT7_T> _source7;
            private IEnumerable<ELEMENT8_T> _source8;
            private Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T> _result_selector;
            private IEnumerator<ELEMENT_T> _enumerator;
            private IEnumerator<ELEMENT2_T> _enumerator2;
            private IEnumerator<ELEMENT3_T> _enumerator3;
            private IEnumerator<ELEMENT4_T> _enumerator4;
            private IEnumerator<ELEMENT5_T> _enumerator5;
            private IEnumerator<ELEMENT6_T> _enumerator6;
            private IEnumerator<ELEMENT7_T> _enumerator7;
            private IEnumerator<ELEMENT8_T> _enumerator8;
            private RESULT_T _current_object;
            private bool _is_valid_current_object;

            #endregion

            #region コンストラクタ

            public ZipEnumerator8(IEnumerable<ELEMENT_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, IEnumerable<ELEMENT7_T> source7, IEnumerable<ELEMENT8_T> source8, Func<ELEMENT_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T> result_selector)
            {
                _disposed = false;
                _source = source;
                _source2 = source2;
                _source3 = source3;
                _source4 = source4;
                _source5 = source5;
                _source6 = source6;
                _source7 = source7;
                _source8 = source8;
                _result_selector = result_selector;
                InitializeImp();
            }

            ~ZipEnumerator8()
            {
                Dispose(false);
            }

            #endregion

            #region パブリックメソッド

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region プロテクテッドメソッド

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        DisposeImp();
                    }
                    _disposed = true;
                }
            }

            #endregion

            #region プライベートメソッド

            private void InitializeImp()
            {
                _enumerator = null;
                _enumerator2 = null;
                _enumerator3 = null;
                _enumerator4 = null;
                _enumerator5 = null;
                _enumerator6 = null;
                _enumerator7 = null;
                _enumerator8 = null;
                _current_object = default(RESULT_T);
                _is_valid_current_object = false;
            }

            private void DisposeImp()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }
                if (_enumerator2 != null)
                {
                    _enumerator2.Dispose();
                    _enumerator = null;
                }
                if (_enumerator3 != null)
                {
                    _enumerator3.Dispose();
                    _enumerator = null;
                }
                if (_enumerator4 != null)
                {
                    _enumerator4.Dispose();
                    _enumerator = null;
                }
                if (_enumerator5 != null)
                {
                    _enumerator5.Dispose();
                    _enumerator = null;
                }
                if (_enumerator6 != null)
                {
                    _enumerator6.Dispose();
                    _enumerator = null;
                }
                if (_enumerator7 != null)
                {
                    _enumerator7.Dispose();
                    _enumerator = null;
                }
                if (_enumerator8 != null)
                {
                    _enumerator8.Dispose();
                    _enumerator = null;
                }
            }

            #endregion

            #region IEnumerator<RESULT_T> の定義

            public RESULT_T Current
            {
                get
                {
                    if (!_is_valid_current_object)
                        throw new InvalidOperationException();
                    return (_current_object);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (_current_object);
                }
            }

            public bool MoveNext()
            {
                if (_enumerator == null)
                    _enumerator = _source.GetEnumerator();
                if (_enumerator2 == null)
                    _enumerator2 = _source2.GetEnumerator();
                if (_enumerator3 == null)
                    _enumerator3 = _source3.GetEnumerator();
                if (_enumerator4 == null)
                    _enumerator4 = _source4.GetEnumerator();
                if (_enumerator5 == null)
                    _enumerator5 = _source5.GetEnumerator();
                if (_enumerator6 == null)
                    _enumerator6 = _source6.GetEnumerator();
                if (_enumerator7 == null)
                    _enumerator7 = _source7.GetEnumerator();
                if (_enumerator8 == null)
                    _enumerator8 = _source8.GetEnumerator();
                _is_valid_current_object = _enumerator.MoveNext() &&
                                           _enumerator2.MoveNext() &&
                                           _enumerator3.MoveNext() &&
                                           _enumerator4.MoveNext() &&
                                           _enumerator5.MoveNext() &&
                                           _enumerator6.MoveNext() &&
                                           _enumerator7.MoveNext() &&
                                           _enumerator8.MoveNext();
                if (_is_valid_current_object)
                    _current_object = _result_selector(_enumerator.Current, _enumerator2.Current, _enumerator3.Current, _enumerator4.Current, _enumerator5.Current, _enumerator6.Current, _enumerator7.Current, _enumerator8.Current);
                return (_is_valid_current_object);
            }

            public void Reset()
            {
                DisposeImp();
                InitializeImp();
            }

            #endregion
        }

        #endregion

        #region プライベートフィールド

        private static Random _random = new Random();

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 与えられたコレクションから与えられた数の要素を取り出す組み合わせを列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 組み合わせを列挙するコレクションです。
        /// </param>
        /// <param name="n">
        /// sourceから取り出すオブジェクトの数です。
        /// </param>
        /// <returns>列挙された組み合わせのコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<ELEMENT_T>> Combination<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, int n)
        {
            return (source.ToArray().Combination(n));
        }

        /// <summary>
        /// 与えられたコレクションから与えられた数の要素を取り出す組み合わせを列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 組み合わせを列挙するコレクションです。
        /// </param>
        /// <param name="n">
        /// sourceから取り出すオブジェクトの数です。
        /// </param>
        /// <returns>列挙された組み合わせのコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<ELEMENT_T>> Combination<ELEMENT_T>(this ELEMENT_T[] source, int n)
        {
            if (n < 0)
                throw (new ArgumentException());
            if (source.Length < n)
                throw (new ArgumentException());
            if (source.Length == 0 || n == 0)
                return (new IEnumerable<ELEMENT_T>[0]);
            if (source.Length == 1)
                return (new IEnumerable<ELEMENT_T>[] { new ELEMENT_T[] { source[0] } });
            System.Diagnostics.Debug.Assert(source.Length > 1 && n > 0 && source.Length >= n, "source.Length <= 1 || n <= 0 || source.Length < n");
            var result_collection = new List<IEnumerable<ELEMENT_T>>();
            source.Combination(0, n, new ELEMENT_T[0], result_collection);
            return (result_collection);
        }

        /// <summary>
        /// 与えられたコレクションから与えられた数の要素を取り出して並べる順列を列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 順列を列挙するコレクションです。
        /// </param>
        /// <param name="n">
        /// sourceから取り出すオブジェクトの数です。
        /// </param>
        /// <returns>列挙された順列のコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<ELEMENT_T>> Permutation<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, int n)
        {
            return (source.Combination(n).SelectMany(item => item.Permutation()));
        }

        /// <summary>
        /// 与えられたコレクションを取り出して並べられた順列を列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 順列を列挙するコレクションです。
        /// </param>
        /// <returns>
        /// 列挙された順列のコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<ELEMENT_T>> Permutation<ELEMENT_T>(this IEnumerable<ELEMENT_T> source)
        {
            return (source.ToArray().Permutation());
        }

        /// <summary>
        /// 与えられたコレクションを取り出して並べられた順列を列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 順列を列挙するコレクションです。
        /// </param>
        /// <returns>
        /// 列挙された順列のコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<ELEMENT_T>> Permutation<ELEMENT_T>(this ELEMENT_T[] source)
        {
            if (source.Length == 0)
                return (new IEnumerable<ELEMENT_T>[0]);
            if (source.Length == 1)
                return (new IEnumerable<ELEMENT_T>[] { new ELEMENT_T[] { source[0] } });
            var mode = 1;
            if (mode == 1)
            {
                var result_collection = new List<IEnumerable<ELEMENT_T>>();
                source.PermutationImp_r(Enumerable.Range(0, source.Length).ToArray(), source.Length, new ELEMENT_T[0], result_collection);
                return (result_collection);
            }
            else
                return (PermutationImp_nr<ELEMENT_T>(source));
        }

        /// <summary>
        /// 値とその個数を参照可能な要素を持つコレクションから、値が並べられた順列を列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 値の要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 順列を列挙するコレクションです。
        /// </param>
        /// <param name="value_selecter">
        /// ELEMENT_TオブジェクトからVALUE_Tオブジェクトを取得するセレクターです。
        /// </param>
        /// <param name="count_selecter">
        /// ELEMENT_TオブジェクトからVALUE_Tオブジェクトの数を取得するセレクターです。
        /// </param>
        /// <returns>
        /// 列挙された順列のコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<VALUE_T>> Permutation<ELEMENT_T, VALUE_T>(this IEnumerable<ELEMENT_T> source, Func<ELEMENT_T, VALUE_T> value_selecter, Func<ELEMENT_T, int> count_selecter)
        {
            return (source.ToArray().Permutation(value_selecter, count_selecter));
        }

        /// <summary>
        /// 値とその個数を参照可能な要素を持つコレクションから、値が並べられた順列を列挙します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 値の要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 順列を列挙するコレクションです。
        /// </param>
        /// <param name="value_selecter">
        /// ELEMENT_TオブジェクトからVALUE_Tオブジェクトを取得するセレクターです。
        /// </param>
        /// <param name="count_selecter">
        /// ELEMENT_TオブジェクトからVALUE_Tオブジェクトの数を取得するセレクターです。
        /// </param>
        /// <returns>
        /// 列挙された順列のコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<VALUE_T>> Permutation<ELEMENT_T, VALUE_T>(this ELEMENT_T[] source, Func<ELEMENT_T, VALUE_T> value_selecter, Func<ELEMENT_T, int> count_selecter)
        {
            var values = source.Select(item => value_selecter(item)).ToArray();
            var counts = source.Select(item => count_selecter(item)).ToArray();
            if (values.Length != counts.Length)
                throw new ApplicationException();
            if (counts.Any(item => item < 0))
                throw new ArgumentException();
            IEnumerable<VALUE_T> result = new VALUE_T[0];
            return (PermutationImp(result, values, counts));
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T>(this ELEMENT_T[] source, ELEMENT_T value)
            where ELEMENT_T : IEquatable<ELEMENT_T>
        {
            return (source.IndexOf(value, item => item));
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, ELEMENT_T value)
            where ELEMENT_T : IEquatable<ELEMENT_T>
        {
            return (source.IndexOf(value, item => item));
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 値の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <param name="value_selecter">
        /// 比較すべき値を要素から取得するセレクタです。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T, VALUE_T>(this ELEMENT_T[] source, VALUE_T value, Func<ELEMENT_T, VALUE_T> value_selecter)
            where VALUE_T : IEquatable<VALUE_T>
        {
            var found = Enumerable.Range(0, source.Length).Where(index => object.Equals(value_selecter(source[index]), value)).Select(index => new { index }).FirstOrDefault();
            return (found == null ? -1 : found.index);
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 値の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <param name="value_selecter">
        /// 比較すべき値を要素から取得するセレクタです。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T, VALUE_T>(this IEnumerable<ELEMENT_T> source, VALUE_T value, Func<ELEMENT_T, VALUE_T> value_selecter)
            where VALUE_T : IEquatable<VALUE_T>
        {
            var found = source.Select((item, index) => new { index, value = item }).Where(item => object.Equals(value_selecter(item.value), value)).FirstOrDefault();
            return (found == null ? -1 : found.index);
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <param name="start_index">
        /// 与えられたコレクションでの要素の検索開始位置です。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T>(this ELEMENT_T[] source, ELEMENT_T value, int start_index)
            where ELEMENT_T : IEquatable<ELEMENT_T>
        {
            return (source.IndexOf(value, item => item, start_index));
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <param name="start_index">
        /// 与えられたコレクションでの要素の検索開始位置です。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, ELEMENT_T value, int start_index)
            where ELEMENT_T : IEquatable<ELEMENT_T>
        {
            return (source.IndexOf(value, item => item, start_index));
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 値の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <param name="value_selecter">
        /// 比較すべき値を要素から取得するセレクタです。
        /// </param>
        /// <param name="start_index">
        /// 与えられたコレクションでの要素の検索開始位置です。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T, VALUE_T>(this ELEMENT_T[] source, VALUE_T value, Func<ELEMENT_T, VALUE_T> value_selecter, int start_index)
            where VALUE_T : IEquatable<VALUE_T>
        {
            if (start_index < 0 || start_index >= source.Length)
                return (-1);
            var found = Enumerable.Range(start_index, source.Length - start_index).Where(index => object.Equals(value_selecter(source[index]), value)).Select(index => new { index }).FirstOrDefault();
            return (found == null ? -1 : found.index);
        }

        /// <summary>
        /// 与えられたコレクションから、与えられた値に一致する値が一致する要素を探し、そのインデックスを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 値の型です。
        /// </typeparam>
        /// <param name="source">
        /// 要素を探すコレクションです。
        /// </param>
        /// <param name="value">
        /// 探す要素の値です。
        /// </param>
        /// <param name="value_selecter">
        /// 比較すべき値を要素から取得するセレクタです。
        /// </param>
        /// <param name="start_index">
        /// 与えられたコレクションでの要素の検索開始位置です。
        /// </param>
        /// <returns>
        /// 見つかった要素のインデックスです。
        /// 要素が見つからなかった場合は負の値を返します。
        /// </returns>
        public static int IndexOf<ELEMENT_T, VALUE_T>(this IEnumerable<ELEMENT_T> source, VALUE_T value, Func<ELEMENT_T, VALUE_T> value_selecter, int start_index)
            where VALUE_T : IEquatable<VALUE_T>
        {
            if (start_index < 0)
                return (-1);
            var found = source.Select((item, index) => new { index, value = item }).Skip(start_index).Where(item => object.Equals(value_selecter(item.value), value)).FirstOrDefault();
            return (found == null ? -1 : found.index);
        }

        /// <summary>
        /// 与えられたコレクションにおける与えられたインデックスと要素数で示される範囲の要素を配列として返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 配列を取得するコレクションです。
        /// </param>
        /// <param name="start">
        /// 配列を取得する開始位置です。
        /// </param>
        /// <param name="count">
        /// 配列を取得する要素数です。
        /// </param>
        /// <returns>
        /// 取得した配列です。
        /// </returns>
        public static ELEMENT_T[] ToArray<ELEMENT_T>(this ELEMENT_T[] source, int start, int count)
        {
            if (start < 0)
                return (new ELEMENT_T[0]);
            int copy_length = Math.Min(source.Length - start, count);
            var array = new ELEMENT_T[copy_length];
            for (int index = 0; index < copy_length; ++index)
                array[index] = source[start + index];
            return (array);
        }

        /// <summary>
        /// 与えられたコレクションにおける与えられたインデックスと要素数で示される範囲の要素を配列として返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 配列を取得するコレクションです。
        /// </param>
        /// <param name="start">
        /// 配列を取得する開始位置です。
        /// </param>
        /// <param name="count">
        /// 配列を取得する要素数です。
        /// </param>
        /// <returns>
        /// 取得した配列です。
        /// </returns>
        public static ELEMENT_T[] ToArray<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, int start, int count)
        {
            if (start < 0)
                return (new ELEMENT_T[0]);
            return (source.Skip(start).Take(count).ToArray());
        }

        /// <summary>
        /// 与えられたコレクションにおける与えられたインデックスと要素数で示される範囲の要素をリストとして返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// リストを取得するコレクションです。
        /// </param>
        /// <param name="start">
        /// リストを取得する開始位置です。
        /// </param>
        /// <param name="count">
        /// リストを取得する要素数です。
        /// </param>
        /// <returns>
        /// 取得したリストです。
        /// </returns>
        public static List<ELEMENT_T> ToList<ELEMENT_T>(this ELEMENT_T[] source, int start, int count)
        {
            if (start < 0)
                return (new List<ELEMENT_T>());
            int copy_length = Math.Min(source.Length - start, count);
            var list = new List<ELEMENT_T>();
            var limit = start + copy_length;
            for (int index = start; index < limit; ++index)
                list.Add(source[start + index]);
            return (list);
        }

        /// <summary>
        /// 与えられたコレクションにおける与えられたインデックスと要素数で示される範囲の要素をリストとして返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// リストを取得するコレクションです。
        /// </param>
        /// <param name="start">
        /// リストを取得する開始位置です。
        /// </param>
        /// <param name="count">
        /// リストを取得する要素数です。
        /// </param>
        /// <returns>
        /// 取得したリストです。
        /// </returns>
        public static List<ELEMENT_T> ToList<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, int start, int count)
        {
            if (start < 0)
                return (new List<ELEMENT_T>());
            return (source.Skip(start).Take(count).ToList());
        }

        /// <summary>
        /// 与えられたコレクションから与えられた数で「ぶつ切り」にしたコレクションを返します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// ぶつ切りにするコレクションです。
        /// </param>
        /// <param name="chunk_size">
        /// ぶつ切りの最大の大きさです。
        /// </param>
        /// <returns>
        /// ぶつ切りにされたコレクションです。
        /// </returns>
        public static IEnumerable<IEnumerable<ELEMENT_T>> Chunk<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, int chunk_size)
        {
            return (new ChunkEnumerable<ELEMENT_T>(source, chunk_size));
        }

        /// <summary>
        /// 与えられたコレクションから重複する要素を除いたコレクションを返します。
        /// 要素の比較は与えられたセレクタで返されるキー値の比較で行います。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <typeparam name="KEY_T">
        /// 要素の比較のためのキー値の型です。
        /// </typeparam>
        /// <param name="source">
        /// コレクションです。
        /// </param>
        /// <param name="key_selecter">
        /// コレクションの要素から要素同士を比較するためのキー値を取得するセレクタです。
        /// </param>
        /// <returns>
        /// 重複する要素が除かれたコレクションです。
        /// </returns>
        public static IEnumerable<ELEMENT_T> Distinct<ELEMENT_T, KEY_T>(this IEnumerable<ELEMENT_T> source, Func<ELEMENT_T, KEY_T> key_selecter)
        {
            return (source.Distinct(new KeyComparer<ELEMENT_T, KEY_T>(key_selecter)));
        }

        /// <summary>
        /// 与えられたコレクションの任意の要素を取得します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// コレクションです。
        /// </param>
        /// <returns>
        /// 取得された要素です。
        /// 与えれたコレクションが空であった場合は例外が通知されます。
        /// </returns>
        public static ELEMENT_T GetRandomElement<ELEMENT_T>(this IEnumerable<ELEMENT_T> source)
        {
            var souce_array = source.ToArray();
            if (souce_array.Length == 0)
                throw (new InvalidOperationException("sourceに要素が含まれていません。"));
            return (souce_array[_random.Next(souce_array.Length)]);
        }

        /// <summary>
        /// 与えられたコレクションの任意の要素を取得します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// コレクションです。
        /// </param>
        /// <param name="default_value">
        /// sourceが空であった場合に返される既定値です。
        /// </param>
        /// <returns>
        /// 取得された要素です。
        /// 与えられたコレクションが空であった場合は default_value で与えられた値が返ります。
        /// </returns>
        public static ELEMENT_T GetRandomElement<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, ELEMENT_T default_value)
        {
            var souce_array = source.ToArray();
            if (souce_array.Length == 0)
                return (default_value);
            return (souce_array[_random.Next(souce_array.Length)]);
        }

        /// <summary>
        /// 与えられたコレクションにおいて、与えられたセレクタによって得られる値が最小となる要素を取得します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 要素の大小を比較するための型です。
        /// </typeparam>
        /// <param name="source">
        /// コレクションです。
        /// </param>
        /// <param name="value_selecter">
        /// コレクションの要素から要素の大小を比較するための値を取得するためのセレクタです。
        /// </param>
        /// <returns>
        /// セレクタによって得られた値が最小である要素です。
        /// </returns>
        public static ELEMENT_T MinimumElement<ELEMENT_T, VALUE_T>(this IEnumerable<ELEMENT_T> source, Func<ELEMENT_T, VALUE_T> value_selecter)
            where VALUE_T : IComparable
        {
            return (MinMaxImp(source, value_selecter, (x, y) => x.CompareTo(y) < 0).Item);
        }

        /// <summary>
        /// 与えられたコレクションにおいて、与えられたセレクタによって得られる値が最大となる要素を取得します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// コレクションの要素の型です。
        /// </typeparam>
        /// <typeparam name="VALUE_T">
        /// 要素の大小を比較するための型です。
        /// </typeparam>
        /// <param name="source">
        /// コレクションです。
        /// </param>
        /// <param name="value_selecter">
        /// コレクションの要素から要素の大小を比較するための値を取得するためのセレクタです。
        /// </param>
        /// <returns>
        /// セレクタによって得られた値が最大である要素です。
        /// </returns>
        public static ELEMENT_T MaximumElement<ELEMENT_T, VALUE_T>(this IEnumerable<ELEMENT_T> source, Func<ELEMENT_T, VALUE_T> value_selecter)
            where VALUE_T : IComparable
        {
            return (MinMaxImp(source, value_selecter, (x, y) => x.CompareTo(y) > 0).Item);
        }

        /// <summary>
        /// 与えられたシーケンスとキー・値のセレクタから、<see cref="NameValueCollection"/>オブジェクトを構築します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 与えられたシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 与えられたシーケンスです。
        /// </param>
        /// <param name="key_selecter">
        /// 与えられたシーケンスの要素に適用されるキーのセレクタです。
        /// </param>
        /// <param name="value_selecter">
        /// 与えられたシーケンスの要素に適用される値のセレクタです。
        /// </param>
        /// <returns>
        /// 構築された<see cref="NameValueCollection"/>オブジェクトです。
        /// </returns>
        public static NameValueCollection ToNameValueCollection<ELEMENT_T>(this IEnumerable<ELEMENT_T> source, Func<ELEMENT_T, string> key_selecter, Func<ELEMENT_T, string> value_selecter)
        {
            var collection = new NameValueCollection();
            foreach (var element in source)
                collection.Add(key_selecter(element), value_selecter(element));
            return (collection);
        }

        /// <summary>
        /// 3つのシーケンスの対応する要素に対して1つの指定した関数を適用し、結果として1つのシーケンスを生成します。
        /// </summary>
        /// <typeparam name="ELEMENT1_T">
        /// 1番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT2_T">
        /// 2番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT3_T">
        /// 3番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="RESULT_T">
        /// 結果のシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// マージする1番目のシーケンスです。
        /// </param>
        /// <param name="source2">
        /// マージする2番目のシーケンスです。
        /// </param>
        /// <param name="source3">
        /// マージする3番目のシーケンスです。
        /// </param>
        /// <param name="result_selector">
        /// 3つのシーケンスの要素をマージする方法を指定する関数です。
        /// </param>
        /// <returns>
        /// 生成された結果のシーケンスです。
        /// </returns>
        public static IEnumerable<RESULT_T> Zip<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, RESULT_T>(this IEnumerable<ELEMENT1_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, Func<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, RESULT_T> result_selector)
        {
            return (new ZipEnumerable3<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, RESULT_T>(source, source2, source3, result_selector));
        }

        /// <summary>
        /// 4つのシーケンスの対応する要素に対して1つの指定した関数を適用し、結果として1つのシーケンスを生成します。
        /// </summary>
        /// <typeparam name="ELEMENT1_T">
        /// 1番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT2_T">
        /// 2番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT3_T">
        /// 3番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT4_T">
        /// 4番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="RESULT_T">
        /// 結果のシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// マージする1番目のシーケンスです。
        /// </param>
        /// <param name="source2">
        /// マージする2番目のシーケンスです。
        /// </param>
        /// <param name="source3">
        /// マージする3番目のシーケンスです。
        /// </param>
        /// <param name="source4">
        /// マージする4番目のシーケンスです。
        /// </param>
        /// <param name="result_selector">
        /// 4つのシーケンスの要素をマージする方法を指定する関数です。
        /// </param>
        /// <returns>
        /// 生成された結果のシーケンスです。
        /// </returns>
        public static IEnumerable<RESULT_T> Zip<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T>(this IEnumerable<ELEMENT1_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, Func<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T> result_selector)
        {
            return (new ZipEnumerable4<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, RESULT_T>(source, source2, source3, source4, result_selector));
        }

        /// <summary>
        /// 5つのシーケンスの対応する要素に対して1つの指定した関数を適用し、結果として1つのシーケンスを生成します。
        /// </summary>
        /// <typeparam name="ELEMENT1_T">
        /// 1番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT2_T">
        /// 2番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT3_T">
        /// 3番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT4_T">
        /// 4番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT5_T">
        /// 5番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="RESULT_T">
        /// 結果のシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// マージする1番目のシーケンスです。
        /// </param>
        /// <param name="source2">
        /// マージする2番目のシーケンスです。
        /// </param>
        /// <param name="source3">
        /// マージする3番目のシーケンスです。
        /// </param>
        /// <param name="source4">
        /// マージする4番目のシーケンスです。
        /// </param>
        /// <param name="source5">
        /// マージする5番目のシーケンスです。
        /// </param>
        /// <param name="result_selector">
        /// 5つのシーケンスの要素をマージする方法を指定する関数です。
        /// </param>
        /// <returns>
        /// 生成された結果のシーケンスです。
        /// </returns>
        public static IEnumerable<RESULT_T> Zip<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T>(this IEnumerable<ELEMENT1_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, Func<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T> result_selector)
        {
            return (new ZipEnumerable5<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, RESULT_T>(source, source2, source3, source4, source5, result_selector));
        }

        /// <summary>
        /// 6つのシーケンスの対応する要素に対して1つの指定した関数を適用し、結果として1つのシーケンスを生成します。
        /// </summary>
        /// <typeparam name="ELEMENT1_T">
        /// 1番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT2_T">
        /// 2番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT3_T">
        /// 3番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT4_T">
        /// 4番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT5_T">
        /// 5番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT6_T">
        /// 6番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="RESULT_T">
        /// 結果のシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// マージする1番目のシーケンスです。
        /// </param>
        /// <param name="source2">
        /// マージする2番目のシーケンスです。
        /// </param>
        /// <param name="source3">
        /// マージする3番目のシーケンスです。
        /// </param>
        /// <param name="source4">
        /// マージする4番目のシーケンスです。
        /// </param>
        /// <param name="source5">
        /// マージする5番目のシーケンスです。
        /// </param>
        /// <param name="source6">
        /// マージする6番目のシーケンスです。
        /// </param>
        /// <param name="result_selector">
        /// 6つのシーケンスの要素をマージする方法を指定する関数です。
        /// </param>
        /// <returns>
        /// 生成された結果のシーケンスです。
        /// </returns>
        public static IEnumerable<RESULT_T> Zip<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T>(this IEnumerable<ELEMENT1_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, Func<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T> result_selector)
        {
            return (new ZipEnumerable6<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, RESULT_T>(source, source2, source3, source4, source5, source6, result_selector));
        }

        /// <summary>
        /// 7つのシーケンスの対応する要素に対して1つの指定した関数を適用し、結果として1つのシーケンスを生成します。
        /// </summary>
        /// <typeparam name="ELEMENT1_T">
        /// 1番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT2_T">
        /// 2番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT3_T">
        /// 3番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT4_T">
        /// 4番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT5_T">
        /// 5番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT6_T">
        /// 6番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT7_T">
        /// 71番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="RESULT_T">
        /// 結果のシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// マージする1番目のシーケンスです。
        /// </param>
        /// <param name="source2">
        /// マージする2番目のシーケンスです。
        /// </param>
        /// <param name="source3">
        /// マージする3番目のシーケンスです。
        /// </param>
        /// <param name="source4">
        /// マージする4番目のシーケンスです。
        /// </param>
        /// <param name="source5">
        /// マージする5番目のシーケンスです。
        /// </param>
        /// <param name="source6">
        /// マージする6番目のシーケンスです。
        /// </param>
        /// <param name="source7">
        /// マージする7番目のシーケンスです。
        /// </param>
        /// <param name="result_selector">
        /// 7つのシーケンスの要素をマージする方法を指定する関数です。
        /// </param>
        /// <returns>
        /// 生成された結果のシーケンスです。
        /// </returns>
        public static IEnumerable<RESULT_T> Zip<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T>(this IEnumerable<ELEMENT1_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, IEnumerable<ELEMENT7_T> source7, Func<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T> result_selector)
        {
            return (new ZipEnumerable7<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, RESULT_T>(source, source2, source3, source4, source5, source6, source7, result_selector));
        }

        /// <summary>
        /// 8つのシーケンスの対応する要素に対して1つの指定した関数を適用し、結果として1つのシーケンスを生成します。
        /// </summary>
        /// <typeparam name="ELEMENT1_T">
        /// 1番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT2_T">
        /// 2番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT3_T">
        /// 3番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT4_T">
        /// 4番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT5_T">
        /// 5番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT6_T">
        /// 6番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT7_T">
        /// 71番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="ELEMENT8_T">
        /// 8番目の入力シーケンスの要素の型です。
        /// </typeparam>
        /// <typeparam name="RESULT_T">
        /// 結果のシーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// マージする1番目のシーケンスです。
        /// </param>
        /// <param name="source2">
        /// マージする2番目のシーケンスです。
        /// </param>
        /// <param name="source3">
        /// マージする3番目のシーケンスです。
        /// </param>
        /// <param name="source4">
        /// マージする4番目のシーケンスです。
        /// </param>
        /// <param name="source5">
        /// マージする5番目のシーケンスです。
        /// </param>
        /// <param name="source6">
        /// マージする6番目のシーケンスです。
        /// </param>
        /// <param name="source7">
        /// マージする7番目のシーケンスです。
        /// </param>
        /// <param name="source8">
        /// マージする8番目のシーケンスです。
        /// </param>
        /// <param name="result_selector">
        /// 8つのシーケンスの要素をマージする方法を指定する関数です。
        /// </param>
        /// <returns>
        /// 生成された結果のシーケンスです。
        /// </returns>
        public static IEnumerable<RESULT_T> Zip<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T>(this IEnumerable<ELEMENT1_T> source, IEnumerable<ELEMENT2_T> source2, IEnumerable<ELEMENT3_T> source3, IEnumerable<ELEMENT4_T> source4, IEnumerable<ELEMENT5_T> source5, IEnumerable<ELEMENT6_T> source6, IEnumerable<ELEMENT7_T> source7, IEnumerable<ELEMENT8_T> source8, Func<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T> result_selector)
        {
            return (new ZipEnumerable8<ELEMENT1_T, ELEMENT2_T, ELEMENT3_T, ELEMENT4_T, ELEMENT5_T, ELEMENT6_T, ELEMENT7_T, ELEMENT8_T, RESULT_T>(source, source2, source3, source4, source5, source6, source7, source8, result_selector));
        }

        /// <summary>
        /// 与えられたシーケンスが空ではないことを確認します。
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// シーケンスの要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 与えられたシーケンスです。
        /// </param>
        /// <returns>
        /// 与えられたシーケンスをそのまま返します。
        /// </returns>
        public static IEnumerable<ELEMENT_T> ExpectToExist<ELEMENT_T>(this IEnumerable<ELEMENT_T> source)
        {
            if (!source.Any())
                throw new ArgumentException();
            return (source);
        }

        #endregion

        #region プライベートメソッド

        private static void Combination<ELEMENT_T>(this ELEMENT_T[] source, int offset, int n, IEnumerable<ELEMENT_T> result_element, ICollection<IEnumerable<ELEMENT_T>> result_collection)
        {
            if (n < 0)
                throw (new ApplicationException());
            if (n == 0)
            {
                result_collection.Add(result_element);
                return;
            }
            if (offset >= source.Length)
                throw (new ApplicationException());
            foreach (var index in Enumerable.Range(offset, source.Length - offset - n + 1))
            {
                source.Combination(index + 1,
                                   n - 1,
                                   result_element.Concat(new ELEMENT_T[] { source[index] }),
                                   result_collection);
            }
        }

        /// <summary>
        /// 再帰法により順列を計算する実装
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 計算対象のシーケンスです。
        /// </param>
        /// <param name="unused_index_list">
        /// シーケンスを指すインデックスのうち未処理のもののコレクションです。
        /// </param>
        /// <param name="unused_index_list_length">
        /// unused_index_listの長さです。
        /// </param>
        /// <param name="result_element">
        /// 結果の要素です。
        /// </param>
        /// <param name="result_collection">
        /// 結果のコレクションです。
        /// </param>
        /// <remarks>
        /// PermutationImp_nrに比べて計算時間は短いが、得られたシーケンスの列挙に時間がかかり、トータルではPermutationImp_nrより時間がかかる。
        /// </remarks>
        private static void PermutationImp_r<ELEMENT_T>(this ELEMENT_T[] source, int[] unused_index_list, int unused_index_list_length, IEnumerable<ELEMENT_T> result_element, ICollection<IEnumerable<ELEMENT_T>> result_collection)
        {
            if (unused_index_list_length < 0)
                throw (new ApplicationException());
            if (unused_index_list_length == 0)
            {
                result_collection.Add(result_element);
                return;
            }
            foreach (var current_index in unused_index_list)
            {
                source.PermutationImp_r(unused_index_list.Where(index => index != current_index).ToArray(),
                                        unused_index_list_length - 1,
                                        result_element.Concat(new ELEMENT_T[] { source[current_index] }),
                                        result_collection);
            }
        }

        /// <summary>
        /// 階乗進数により順列を計算する実装
        /// </summary>
        /// <typeparam name="ELEMENT_T">
        /// 要素の型です。
        /// </typeparam>
        /// <param name="source">
        /// 計算対象のシーケンスです。
        /// </param>
        /// <returns>
        /// 結果のコレクションです。
        /// </returns>
        /// <remarks>
        /// PermutationImp_rに比べて計算時間は長いが、得られたシーケンスの列挙にあまり時間がかからず、トータルではPermutationImp_rより短時間で済む。
        /// </remarks>
        private static List<IEnumerable<ELEMENT_T>> PermutationImp_nr<ELEMENT_T>(ELEMENT_T[] source)
        {
            var counter = new 階乗進数カウンタ(source.Length);
            var result = new List<IEnumerable<ELEMENT_T>>();
            while (!counter.EndOfCounter)
            {
                var index = Enumerable.Range(0, source.Length).ToList();
                var result_element = new List<ELEMENT_T>();
                foreach (var c in counter.Value)
                {
                    result_element.Add(source[index[c]]);
                    index.RemoveAt(c);
                }
                result.Add(result_element);
                counter.Increment();
            }
            return (result);
        }

        private static IEnumerable<IEnumerable<VALUE_T>> PermutationImp<VALUE_T>(IEnumerable<VALUE_T> result, VALUE_T[] values, int[] counts)
        {
            if (counts.Any(item => item < 0))
                throw new ApplicationException();
            var indexes = counts
                          .Select((count, index) => new { index, count })
                          .Where(item => item.count > 0)
                          .Select(item => item.index);
            if (!indexes.Any())
                return (new[] { result.ToArray() });
            return (indexes.SelectMany(current_index =>
            {
                var value = values[current_index];
                var new_result = result.Concat(new[] { value });
                var new_counts = new int[counts.Length];
                Array.Copy(counts, new_counts, new_counts.Length);
                --new_counts[current_index];
                return (PermutationImp(new_result, values, new_counts));
            }));
        }

        private static MinMaxAccumulator<ELEMENT_T, VALUE_T> MinMaxImp<ELEMENT_T, VALUE_T>(IEnumerable<ELEMENT_T> source, Func<ELEMENT_T, VALUE_T> value_selecter, Func<VALUE_T, VALUE_T, bool> is_replace)
        {
            var r = source.Aggregate((MinMaxAccumulator<ELEMENT_T, VALUE_T>)null,
                                     (acc, item) =>
                                     {
                                         var value = value_selecter(item);
                                         if (acc == null || is_replace(value, acc.Value))
                                             return (new MinMaxAccumulator<ELEMENT_T, VALUE_T>(item, value));
                                         else
                                             return (acc);
                                     });
            if (r == null)
                throw new ArgumentException();
            return (r);
        }

        #endregion
    }
}