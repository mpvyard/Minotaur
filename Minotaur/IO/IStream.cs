﻿using System;

namespace Minotaur.IO
{
    /// <inheritdoc />
    /// <summary>
    /// This interface describe a generic stream.
    /// </summary>
    public interface IStream : IDisposable
    {
        int Read(byte[] buffer, int offset, int count);

        void Write(byte[] buffer, int offset, int count);

        void Reset();

        void Flush();

        long Seek(long offset);
    }    
}
