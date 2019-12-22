﻿using System;
using System.Collections.Generic;
using System.IO;
using Minotaur.Core;

namespace Minotaur.IO
{
    public class MinotaurFileStream : IStream
    {
        private readonly IEnumerator<string> _enumerator;
        private IDisposable _fileLock;
        private FileStream _current;
        private long _cumulativeLength;

        public long Position => _cumulativeLength + _current?.Position ?? 0;
        public long Length => _cumulativeLength + _current?.Length ?? 0;

        /// <summary>
        /// Reader Ctor.
        /// </summary>
        public MinotaurFileStream(IEnumerable<string> filePaths)
        {
            _enumerator = filePaths.GetEnumerator();
        }

        /// <summary>
        /// Writer ctor
        /// </summary>
        public MinotaurFileStream(string filePath)
        {
            filePath.GetFolderPath().CreateFolderIfNotExist();
            _fileLock = filePath.AcquireWriteLock();
            _current = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 1);
            //??_current.SetLength(length);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            while (read < count)
            {
                var current = GetCurrentFileStream();
                if (current == null) return read;

                read += _current.Read(buffer, offset, count);
            }

            return read;
        }

        public void Write(byte[] buffer, int offset, int count) 
            => _current.Write(buffer, offset, count);

        public void Reset()
        {
            _cumulativeLength = 0;

            // Reset the writer part
            if (_current != null && _current.CanSeek)
                _current.Seek(0, SeekOrigin.Begin);

            // Reset the reader part
            if (_enumerator != null && _current != null)
            {
                _enumerator.Reset();
                _current = null;
            }
        }

        public void Flush() 
            => _current?.Flush();

        public long Seek(long offset)
        {
            var targetPosition = Position + offset;
            do
            {
                var current = GetCurrentFileStream();
                if (current == null) return Length;

                _current.Seek(0, SeekOrigin.End);
            }
            while (targetPosition < Position);

            _current.Seek(offset, SeekOrigin.Current);
            return Position;
        }

        private FileStream GetCurrentFileStream()
        {
            if (_enumerator != null && (_current == null || _current.Position >= _current.Length))
            {
                _cumulativeLength += _current?.Length ?? 0;
                _current?.Dispose();
                _fileLock?.Dispose();

                while (_enumerator.MoveNext() && _enumerator.Current.FileExists())
                {
                    _fileLock = _enumerator.Current.AcquireReadLock();
                    _current = new FileStream(_enumerator.Current, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
                    return _current;
                }

                return null;
            }

            return _current;
        }

        public void Dispose()
        {
            _current?.Dispose();
            _fileLock?.Dispose();
        }
    }
}
