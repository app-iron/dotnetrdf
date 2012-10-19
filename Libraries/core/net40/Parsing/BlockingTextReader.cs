/*

Copyright dotNetRDF Project 2009-12
dotnetrdf-develop@lists.sf.net

------------------------------------------------------------------------

This file is part of dotNetRDF.

dotNetRDF is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

dotNetRDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with dotNetRDF.  If not, see <http://www.gnu.org/licenses/>.

------------------------------------------------------------------------

dotNetRDF may alternatively be used under the LGPL or MIT License

http://www.gnu.org/licenses/lgpl.html
http://www.opensource.org/licenses/mit-license.php

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VDS.RDF.Parsing
{
    /// <summary>
    /// An extended <see cref="TextReader"/> for use in parsing
    /// </summary>
    public abstract class ParsingTextReader
        : TextReader
    {
        /// <summary>
        /// Gets whether the end of the stream has been reached
        /// </summary>
        public abstract bool EndOfStream
        {
            get;
        }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="input">Text Reader to wrap</param>
        /// <param name="bufferSize">Buffer Size</param>
        /// <remarks>
        /// If the given <see cref="TextReader">TextReader</see> is already a Blocking Text Reader this is a no-op
        /// </remarks>
        public static ParsingTextReader Create(TextReader input, int bufferSize)
        {
            if (input is ParsingTextReader) return (ParsingTextReader)input;
            if (input is StreamReader)
            {
                Stream s = ((StreamReader)input).BaseStream;
                if (!Options.ForceBlockingIO && (s is FileStream || s is MemoryStream))
                {
                    return new NonBlockingTextReader(input, bufferSize);
                }
                else
                {
                    return new BlockingTextReader(input, bufferSize);
                }
            }
            else
            {
                return new BlockingTextReader(input, bufferSize);
            }
        }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="input">Text Reader to wrap</param>
        /// <remarks>
        /// If the given <see cref="TextReader">TextReader</see> is already a Blocking Text Reader this is a no-op
        /// </remarks>
        public static ParsingTextReader Create(TextReader input)
        {
            return Create(input, BlockingTextReader.DefaultBufferSize);
        }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="input">Input Stream</param>
        /// <param name="bufferSize">Buffer Size</param>
        public static ParsingTextReader Create(Stream input, int bufferSize)
        {
            if (!Options.ForceBlockingIO && (input is FileStream || input is MemoryStream))
            {
                return CreateNonBlocking(new StreamReader(input), bufferSize);
            }
            else
            {
                return CreateBlocking(new StreamReader(input), bufferSize);
            }
        }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="input">Input Stream</param>
        public static ParsingTextReader Create(Stream input)
        {
            return Create(input, BlockingTextReader.DefaultBufferSize);
        }

        public static BlockingTextReader CreateBlocking(TextReader input)
        {
            return CreateBlocking(input, BlockingTextReader.DefaultBufferSize);
        }

        public static BlockingTextReader CreateBlocking(TextReader input, int bufferSize)
        {
            if (input is BlockingTextReader) return (BlockingTextReader)input;
            return new BlockingTextReader(input, bufferSize);
        }

        public static NonBlockingTextReader CreateNonBlocking(TextReader input)
        {
            if (input is NonBlockingTextReader) return (NonBlockingTextReader)input;
            return new NonBlockingTextReader(input);
        }

        public static NonBlockingTextReader CreateNonBlocking(TextReader input, int bufferSize)
        {
            if (input is NonBlockingTextReader) return (NonBlockingTextReader)input;
            return new NonBlockingTextReader(input, bufferSize);
        }
    }

    /// <summary>
    /// Abstract class representing a text reader that provides buffering on top of another text reader
    /// </summary>
    public abstract class BufferedTextReader
        : ParsingTextReader
    {
        /// <summary>
        /// Default Buffer Size
        /// </summary>
        public const int DefaultBufferSize = 1024;

        protected char[] _buffer;
        protected int _pos = -1;
        protected int _bufferAmount = -1;
        protected bool _finished = false;
        protected readonly TextReader _reader;

        /// <summary>
        /// Creates a buffered reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="bufferSize"></param>
        protected BufferedTextReader(TextReader reader, int bufferSize)
        {
            if (reader == null) throw new ArgumentNullException("reader", "Cannot read from a null TextReader");
            if (bufferSize < 1) throw new ArgumentException("bufferSize must be >= 1", "bufferSize");
            this._reader = reader;
            this._buffer = new char[bufferSize];
        }

        /// <summary>
        /// Requests that the buffer be filled
        /// </summary>
        protected abstract void FillBuffer();

        /// <summary>
        /// Reads a sequence of characters from the buffer in a blocking way
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="index">Index at which to start writing to the Buffer</param>
        /// <param name="count">Number of characters to read</param>
        /// <returns>Number of characters read</returns>
        public override int ReadBlock(char[] buffer, int index, int count)
        {
            if (count == 0) return 0;
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (index < 0) throw new ArgumentException("index", "Index must be >= 0");
            if (count < 0) throw new ArgumentException("count", "Count must be >= 0");
            if ((buffer.Length - index) < count) throw new ArgumentException("Buffer too small");

            if (this._bufferAmount == -1 || this._pos >= this._bufferAmount)
            {
                if (!this._finished)
                {
                    this.FillBuffer();
                    if (this.EndOfStream) return 0;
                }
                else
                {
                    return 0;
                }
            }

            this._pos = Math.Max(0, this._pos);
            if (count <= this._bufferAmount - this._pos)
            {
                //If we have sufficient things buffered to fufill the request just copy the relevant stuff across
                Array.Copy(this._buffer, this._pos, buffer, index, count);
                this._pos += count;
                return count;
            }
            else
            {
                int copied = 0;
                while (copied < count)
                {
                    int available = this._bufferAmount - this._pos;
                    if (count < copied + available)
                    {
                        //We can finish fufilling this request this round
                        int toCopy = Math.Min(available, count - copied);
                        Array.Copy(this._buffer, this._pos, buffer, index + copied, toCopy);
                        copied += toCopy;
                        this._pos += toCopy;
                        return copied;
                    }
                    else
                    {
                        //Copy everything we currently have available
                        Array.Copy(this._buffer, this._pos, buffer, index + copied, available);
                        copied += available;
                        this._pos = this._bufferAmount;

                        if (!this._finished)
                        {
                            //If we haven't reached the end of the input refill our buffer and continue
                            this.FillBuffer();
                            if (this.EndOfStream) return copied;
                            this._pos = 0;
                        }
                        else
                        {
                            //Otherwise we have reached the end of the input so just return what we've managed to copy
                            return copied;
                        }
                    }
                }
                return copied;
            }
        }

        /// <summary>
        /// Reads a sequence of characters from the buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="index">Index at which to start writing to the Buffer</param>
        /// <param name="count">Number of characters to read</param>
        /// <returns>Number of characters read</returns>
        public override int Read(char[] buffer, int index, int count)
        {
            return this.ReadBlock(buffer, index, count);
        }

        /// <summary>
        /// Reads a single character from the underlying Text Reader
        /// </summary>
        /// <returns>Character read or -1 if at end of input</returns>
        public override int Read()
        {
            if (this._bufferAmount == -1 || this._pos >= this._bufferAmount - 1)
            {
                if (!this._finished)
                {
                    this.FillBuffer();
                    if (this.EndOfStream) return -1;
                }
                else
                {
                    return -1;
                }
            }

            this._pos++;
            return (int)this._buffer[this._pos];
        }

        /// <summary>
        /// Peeks at the next character from the underlying Text Reader
        /// </summary>
        /// <returns>Character peeked or -1 if at end of input</returns>
        public override int Peek()
        {
            if (this._bufferAmount == -1 || this._pos >= this._bufferAmount - 1)
            {
                if (!this._finished)
                {
                    this.FillBuffer();
                    if (this.EndOfStream) return -1;
                }
                else
                {
                    return -1;
                }
            }

            return (int)this._buffer[this._pos + 1];
        }

        /// <summary>
        /// Gets whether the end of the input has been reached
        /// </summary>
        public override bool EndOfStream
        {
            get
            {
                return this._finished && (this._pos >= this._bufferAmount - 1);
            }
        }

        /// <summary>
        /// Closes the reader and the underlying reader
        /// </summary>
        public override void Close()
        {
            this._reader.Close();
        }

        /// <summary>
        /// Disposes of the reader and the underlying reader
        /// </summary>
        /// <param name="disposing">Whether this was called from the Dispose() method</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) GC.SuppressFinalize(this);
            this.Close();
            this._reader.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// The BlockingTextReader is an implementation of a <see cref="BufferedTextReader" /> designed to wrap other readers which may or may not have high latency and thus ensures that premature end of input bug is not experienced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is designed to avoid premature detection of end of input when the input has high latency and the consumer tries to read from the input faster than it can return data.  This derives from <see cref="BufferedTextReader"/> and ensures the buffer is filled by calling the <see cref="TextReader.ReadBlock">ReadBlock()</see> method of the underlying <see cref="TextReader">TextReader</see> thus avoiding the scenario where input appears to end prematurely.
    /// </para>
    /// </remarks>
    public sealed class BlockingTextReader 
        : BufferedTextReader
    {
        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="reader">Text Reader to wrap</param>
        /// <param name="bufferSize">Buffer Size</param>
        internal BlockingTextReader(TextReader reader, int bufferSize)
            : base(reader, bufferSize) { }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="reader">Text Reader to wrap</param>
        internal BlockingTextReader(TextReader reader)
            : this(reader, DefaultBufferSize) { }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="input">Input Stream</param>
        /// <param name="bufferSize">Buffer Size</param>
        internal BlockingTextReader(Stream input, int bufferSize)
            : this(new StreamReader(input), bufferSize) { }

        /// <summary>
        /// Creates a new Blocking Text Reader
        /// </summary>
        /// <param name="input">Input Stream</param>
        internal BlockingTextReader(Stream input)
            : this(new StreamReader(input)) { }

        /// <summary>
        /// Fills the Buffer
        /// </summary>
        protected override void FillBuffer()
        {
            this._pos = -1;
            if (this._finished)
            {
                this._bufferAmount = 0;
            }
            else
            {
                this._bufferAmount = this._reader.ReadBlock(this._buffer, 0, this._buffer.Length);
                if (this._bufferAmount == 0 || this._bufferAmount < this._buffer.Length) this._finished = true;
            }
        }
    }

    /// <summary>
    /// The NonBlockingTextReader is an implementation of a <see cref="BufferedTextReader"/> designed to wrap other readers where latency is known not to be a problem and we don't expect to ever have an empty read occur before the actual end of the stream
    /// </summary>
    /// <remarks>
    /// Currently we only use this for file and network streams, you can force this to never be used with the global static <see cref="Options.ForceBlockingIO"/> option
    /// </remarks>
    public sealed class NonBlockingTextReader
        : BufferedTextReader
    {
        internal NonBlockingTextReader(TextReader input, int bufferSize)
            : base(input, bufferSize) { }

        internal NonBlockingTextReader(TextReader input)
            : this(input, DefaultBufferSize) { }

        internal NonBlockingTextReader(Stream input, int bufferSize)
            : this(new StreamReader(input), bufferSize) { }

        internal NonBlockingTextReader(Stream input)
            : this(new StreamReader(input)) { }

        protected override void FillBuffer()
        {
            this._pos = -1;
            if (this._finished)
            {
                this._bufferAmount = 0;
            }
            else
            {
                this._bufferAmount = this._reader.Read(this._buffer, 0, this._buffer.Length);
                if (this._bufferAmount == 0) this._finished = true;
            }
        }
    }
}
