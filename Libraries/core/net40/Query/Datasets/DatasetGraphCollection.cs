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

namespace VDS.RDF.Query.Datasets
{
    /// <summary>
    /// A Graph Collection which wraps an <see cref="ISparqlDataset">ISparqlDataset</see> implementation so it can be used as if it was a Graph Collection
    /// </summary>
    public class DatasetGraphCollection
        : BaseGraphCollection
    {
        private ISparqlDataset _dataset;

        /// <summary>
        /// Creates a new Dataset Graph collection
        /// </summary>
        /// <param name="dataset">SPARQL Dataset</param>
        public DatasetGraphCollection(ISparqlDataset dataset)
        {
            this._dataset = dataset;
        }

        /// <summary>
        /// Gets whether the Collection contains a Graph with the given URI
        /// </summary>
        /// <param name="graphUri">Graph URI</param>
        /// <returns></returns>
        public override bool Contains(Uri graphUri)
        {
            return this._dataset.HasGraph(graphUri);
        }

        /// <summary>
        /// Adds a Graph to the Collection
        /// </summary>
        /// <param name="g">Graph to add</param>
        /// <param name="mergeIfExists">Whether to merge the given Graph with any existing Graph with the same URI</param>
        /// <exception cref="RdfException">Thrown if a Graph with the given URI already exists and the <paramref name="mergeIfExists">mergeIfExists</paramref> is set to false</exception>
        protected internal override bool Add(IGraph g, bool mergeIfExists)
        {
            if (this.Contains(g.BaseUri))
            {
                if (mergeIfExists)
                {
                    IGraph temp = this._dataset.GetModifiableGraph(g.BaseUri);
                    temp.Merge(g);
                    temp.Dispose();
                    this._dataset.Flush();
                    return true;
                }
                else
                {
                    throw new RdfException("Cannot add this Graph as a Graph with the URI '" + g.BaseUri.ToSafeString() + "' already exists in the Collection and mergeIfExists was set to false");
                }
            }
            else
            {
                //Safe to add a new Graph
                if (this._dataset.AddGraph(g))
                {
                    this._dataset.Flush();
                    this.RaiseGraphAdded(g);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Removes a Graph from the Collection
        /// </summary>
        /// <param name="graphUri">URI of the Graph to removed</param>
        protected internal override bool Remove(Uri graphUri)
        {
            if (this.Contains(graphUri))
            {
                IGraph temp = this._dataset[graphUri];
                bool removed = this._dataset.RemoveGraph(graphUri);
                this._dataset.Flush();
                this.RaiseGraphRemoved(temp);
                temp.Dispose();
                return removed;
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
                return this._dataset.GraphUris.Count(); 
            }
        }

        /// <summary>
        /// Gets the URIs of Graphs in the Collection
        /// </summary>
        public override IEnumerable<Uri> GraphUris
        {
            get 
            {
                return this._dataset.GraphUris;
            }
        }

        /// <summary>
        /// Gets the Graph with the given URI
        /// </summary>
        /// <param name="graphUri">Graph URI</param>
        /// <returns></returns>
        public override IGraph this[Uri graphUri]
        {
            get 
            {
                if (this._dataset.HasGraph(graphUri))
                {
                    return this._dataset[graphUri];
                }
                else
                {
                    throw new RdfException("The Graph with the given URI does not exist in this Graph Collection");
                }
            }
        }

        /// <summary>
        /// Disposes of the Graph Collection
        /// </summary>
        public override void Dispose()
        {
            this._dataset.Flush();
        }

        /// <summary>
        /// Gets the enumeration of Graphs in this Collection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<IGraph> GetEnumerator()
        {
            return this._dataset.Graphs.GetEnumerator();
        }
    }
}
