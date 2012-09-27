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
using System.Linq;
using System.Text;
using System.Threading;
using VDS.Common;

namespace VDS.RDF
{
    /// <summary>
    /// Wrapper class for Graph Collections
    /// </summary>
    public class GraphCollection 
        : BaseGraphCollection, IEnumerable<IGraph>
    {
        /// <summary>
        /// Internal Constant used as the Hash Code for the default graph
        /// </summary>
        protected const int DefaultGraphID = 0;

        /// <summary>
        /// Dictionary of Graph Uri Enhanced Hash Codes to Graphs
        /// </summary>
        /// <remarks>See <see cref="Extensions.GetEnhancedHashCode">GetEnhancedHashCode()</see></remarks>
        protected MultiDictionary<Uri, IGraph> _graphs = new MultiDictionary<Uri, IGraph>(u => (u != null ? u.GetEnhancedHashCode() : DefaultGraphID), new UriComparer(), MultiDictionaryMode.AVL);

        /// <summary>
        /// Checks whether the Graph with the given Uri exists in this Graph Collection
        /// </summary>
        /// <param name="graphUri">Graph Uri to test</param>
        /// <returns></returns>
        public override bool Contains(Uri graphUri)
        {
            return this._graphs.ContainsKey(graphUri);
        }

        /// <summary>
        /// Adds a Graph to the Collection
        /// </summary>
        /// <param name="g">Graph to add</param>
        /// <param name="mergeIfExists">Sets whether the Graph should be merged with an existing Graph of the same Uri if present</param>
        /// <exception cref="RdfException">Throws an RDF Exception if the Graph has no Base Uri or if the Graph already exists in the Collection and the <paramref name="mergeIfExists"/> parameter was not set to true</exception>
        protected internal override bool Add(IGraph g, bool mergeIfExists)
        {
            if (this._graphs.ContainsKey(g.BaseUri))
            {
                //Already exists in the Graph Collection
                if (mergeIfExists)
                {
                    //Merge into the existing Graph
                    this._graphs[g.BaseUri].Merge(g);
                    return true;
                }
                else
                {
                    //Not allowed
                    throw new RdfException("The Graph you tried to add already exists in the Graph Collection and the mergeIfExists parameter was set to false");
                }
            }
            else
            {
                //Safe to add a new Graph
                this._graphs.Add(g.BaseUri, g);
                this.RaiseGraphAdded(g);
                return true;
            }
        }

        /// <summary>
        /// Removes a Graph from the Collection
        /// </summary>
        /// <param name="graphUri">Uri of the Graph to remove</param>
        protected internal override bool Remove(Uri graphUri)
        {
            IGraph g;
            if (this._graphs.TryGetValue(graphUri, out g))
            {
                if (this._graphs.Remove(graphUri))
                {
                    this.RaiseGraphRemoved(g);
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of Graphs in the Collection
        /// </summary>
        public override int Count
        {
            get
            {
                return this._graphs.Count;
            }
        }

        /// <summary>
        /// Provides access to the Graph URIs of Graphs in the Collection
        /// </summary>
        public override IEnumerable<Uri> GraphUris
        {
            get
            {
                return this._graphs.Keys;
            }
        }

        /// <summary>
        /// Gets a Graph from the Collection
        /// </summary>
        /// <param name="graphUri">Graph Uri</param>
        /// <returns></returns>
        public override IGraph this[Uri graphUri]
        {
            get 
            {
                IGraph g;
                if (this._graphs.TryGetValue(graphUri, out g))
                {
                    return g;
                }
                else
                {
                    throw new RdfException("The Graph with the given URI does not exist in this Graph Collection");
                }
            }
        }

        /// <summary>
        /// Gets the Enumerator for the Collection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<IGraph> GetEnumerator()
        {
            return this._graphs.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the Enumerator for this Collection
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Disposes of the Graph Collection
        /// </summary>
        /// <remarks>Invokes the <see cref="IGraph.Dipose">Dispose()</see> method of all Graphs contained in the Collection</remarks>
        public override void Dispose()
        {
            this._graphs.Clear();
        }
    }

    /// <summary>
    /// Thread Safe decorator around a Graph collection
    /// </summary>
    /// <remarks>
    /// Dependings on your platform this either provides MRSW concurrency via a <see cref="ReaderWriterLockSlim" /> or exclusive access concurrency via a <see cref="Monitor"/>
    /// </remarks>
    public class ThreadSafeGraphCollection 
        : WrapperGraphCollection
    {
#if !NO_RWLOCK
        private ReaderWriterLockSlim _lockManager = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
#endif

        /// <summary>
        /// Creates a new Thread Safe decorator around the default <see cref="GraphCollection"/>
        /// </summary>
        public ThreadSafeGraphCollection()
            : base(new GraphCollection()) { }

        /// <summary>
        /// Creates a new Thread Safe decorator around the supplied graph collection
        /// </summary>
        /// <param name="graphCollection">Graph Collection</param>
        public ThreadSafeGraphCollection(BaseGraphCollection graphCollection)
            : base(graphCollection) { }

        /// <summary>
        /// Enters the write lock
        /// </summary>
        protected void EnterWriteLock()
        {
#if !NO_RWLOCK
            this._lockManager.EnterWriteLock();
#else
            Monitor.Enter(this._graphs);
#endif
        }

        /// <summary>
        /// Exits the write lock
        /// </summary>
        protected void ExitWriteLock()
        {
#if !NO_RWLOCK
            this._lockManager.ExitWriteLock();
#else
            Monitor.Exit(this._graphs);
#endif
        }

        /// <summary>
        /// Enters the read lock
        /// </summary>
        protected void EnterReadLock()
        {
#if !NO_RWLOCK
            this._lockManager.EnterReadLock();
#else
            Monitor.Enter(this._graphs);
#endif
        }

        /// <summary>
        /// Exits the read lock
        /// </summary>
        protected void ExitReadLock()
        {
#if !NO_RWLOCK
            this._lockManager.ExitReadLock();
#else
            Monitor.Exit(this._graphs);
#endif
        }

        /// <summary>
        /// Checks whether the Graph with the given Uri exists in this Graph Collection
        /// </summary>
        /// <param name="graphUri">Graph Uri to test</param>
        /// <returns></returns>
        public override bool Contains(Uri graphUri)
        {
            bool contains = false;

            try
            {
                this.EnterReadLock();
                contains = this._graphs.Contains(graphUri);
            }
            finally
            {
                this.ExitReadLock();
            }
            return contains;
        }

        /// <summary>
        /// Adds a Graph to the Collection
        /// </summary>
        /// <param name="g">Graph to add</param>
        /// <param name="mergeIfExists">Sets whether the Graph should be merged with an existing Graph of the same Uri if present</param>
        /// <exception cref="RdfException">Throws an RDF Exception if the Graph has no Base Uri or if the Graph already exists in the Collection and the <paramref name="mergeIfExists"/> parameter was not set to true</exception>
        protected internal override bool Add(IGraph g, bool mergeIfExists)
        {
            try
            {
                this.EnterWriteLock();
                return this._graphs.Add(g, mergeIfExists);
            }
            finally
            {
                this.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes a Graph from the Collection
        /// </summary>
        /// <param name="graphUri">Uri of the Graph to remove</param>
        protected internal override bool Remove(Uri graphUri)
        {
            try
            {
                this.EnterWriteLock();
                return this._graphs.Remove(graphUri);
            }
            finally
            {
                this.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets the number of Graphs in the Collection
        /// </summary>
        public override int Count
        {
            get
            {
                int c = 0;
                try
                {
                    this.EnterReadLock();
                    c = this._graphs.Count;
                }
                finally
                {
                    this.ExitReadLock();
                }
                return c;
            }
        }

        /// <summary>
        /// Gets the Enumerator for the Collection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<IGraph> GetEnumerator()
        {
            List<IGraph> graphs = new List<IGraph>();
            try
            {
                this.EnterReadLock();
                graphs = this._graphs.ToList();
            }
            finally
            {
                this.ExitReadLock();
            }
            return graphs.GetEnumerator();
        }

        /// <summary>
        /// Provides access to the Graph URIs of Graphs in the Collection
        /// </summary>
        public override IEnumerable<Uri> GraphUris
        {
            get
            {
                List<Uri> uris = new List<Uri>();
                try
                {
                    this.EnterReadLock();
                    uris = this._graphs.GraphUris.ToList();
                }
                finally
                {
                    this.ExitReadLock();
                }
                return uris;
            }
        }

        /// <summary>
        /// Gets a Graph from the Collection
        /// </summary>
        /// <param name="graphUri">Graph Uri</param>
        /// <returns></returns>
        public override IGraph this[Uri graphUri]
        {
            get
            {
                IGraph g = null;
                try
                {
                    this.EnterReadLock();
                    g = this._graphs[graphUri];
                }
                finally
                {
                    this.ExitReadLock();
                }
                return g;
            }
        }

        /// <summary>
        /// Disposes of the Graph Collection
        /// </summary>
        /// <remarks>Invokes the <see cref="IGraph.Dipose">Dispose()</see> method of all Graphs contained in the Collection</remarks>
        public override void Dispose()
        {
            try
            {
                this.EnterWriteLock();
                this._graphs.Dispose();
            }
            finally
            {
                this.ExitWriteLock();
            }
        }
    }

}
