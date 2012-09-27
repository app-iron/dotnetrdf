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

namespace VDS.RDF
{
    /// <summary>
    /// Represents a union of multiple Triple Collections
    /// </summary>
    /// <remarks>
    /// <para>
    /// The union consists of a <em>Base</em> collection which is the collection that Triples can actually be added to and deleted from and any number of additional collections which are read-only as far as the union is concerned (this does not mean they cannot be altered elsewhere by other code)
    /// </para>
    /// </remarks>
    public class UnionTripleCollection 
        : BaseTripleCollection
    {
        private List<BaseTripleCollection> _collections = new List<BaseTripleCollection>();
        private BaseTripleCollection _baseCollection;

        /// <summary>
        /// Creates a new Union Triple Collection which is a union of two collections
        /// </summary>
        /// <param name="baseTriples">Base Triple Collection</param>
        /// <param name="additionalTriples">Additional Triple Collection</param>
        public UnionTripleCollection(BaseTripleCollection baseTriples, BaseTripleCollection additionalTriples)
        {
            if (baseTriples == null) throw new ArgumentNullException("baseTriple");
            if (additionalTriples == null) throw new ArgumentNullException("additionalTriples");
            this._collections.Add(baseTriples);
            this._collections.Add(additionalTriples);
            this._baseCollection = baseTriples;
        }

        /// <summary>
        /// Creates a new Union Triple Collection which is a union of any number of collections
        /// </summary>
        /// <param name="baseTriples">Base Triple Collection</param>
        /// <param name="additionalTriples">Additional Triple Collection(s)</param>
        public UnionTripleCollection(BaseTripleCollection baseTriples, IEnumerable<BaseTripleCollection> additionalTriples)
        {
            if (baseTriples == null) throw new ArgumentNullException("baseTriple");
            this._collections.Add(baseTriples);
            this._collections.AddRange(additionalTriples);
            this._baseCollection = baseTriples;
        }

        /// <summary>
        /// Adds a Triple to the base collection
        /// </summary>
        /// <param name="t">Triple to add</param>
        protected internal override bool Add(Triple t)
        {
            return this._baseCollection.Add(t);
        }

        /// <summary>
        /// Checks whether the union contains this Triple in any of the collections it comprises
        /// </summary>
        /// <param name="t">Triple to test</param>
        /// <returns></returns>
        public override bool Contains(Triple t)
        {
            return this._collections.Any(c => c.Contains(t));
        }

        /// <summary>
        /// Gets the count of Triples in this union
        /// </summary>
        /// <remarks>
        /// The Count is the total number of Triples, this may be different from the number of distinct triples
        /// </remarks>
        public override int Count
        {
            get 
            {
                return this._collections.Sum(c => c.Count);
            }
        }

        /// <summary>
        /// Deletes a Triple from the base collection
        /// </summary>
        /// <param name="t">Triple to delete</param>
        protected internal override bool Delete(Triple t)
        {
            return this._baseCollection.Delete(t);
        }

        /// <summary>
        /// Retrieves a Triple from the union
        /// </summary>
        /// <param name="t">Triple to retrieve</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the Triple is not contained in any of the collections this union comprises</exception>
        public override Triple this[Triple t]
        {
            get 
            {
                foreach (BaseTripleCollection c in this._collections)
                {
                    if (c.Contains(t))
                    {
                        return c[t];
                    }
                }
                throw new KeyNotFoundException("The given Triple does not exist in this Triple Collection");
            }
        }

        /// <summary>
        /// Gets the enumeration of distinct objects of Triples
        /// </summary>
        public override IEnumerable<INode> ObjectNodes
        {
            get 
            {
                return (from c in this._collections
                        from o in c.ObjectNodes
                        select o);
            }
        }

        /// <summary>
        /// Gets the enumeration of distinct predicates of Triples
        /// </summary>
        public override IEnumerable<INode> PredicateNodes
        {
            get 
            {
                return (from c in this._collections
                        from p in c.PredicateNodes
                        select p); 
            }
        }

        /// <summary>
        /// Gets the enumeration of distinct subjects of Triples
        /// </summary>
        public override IEnumerable<INode> SubjectNodes
        {
            get 
            {
                return (from c in this._collections
                        from s in c.SubjectNodes
                        select s); 
            }
        }

        /// <summary>
        /// Disposes of the collection
        /// </summary>
        /// <remarks>
        /// This does nothing since we don't know where and how the collections we are the union of are being used and therefore to dispose of them could have unwanted/unexpected results
        /// </remarks>
        public override void Dispose()
        {
            //Do Nothing
        }

        /// <summary>
        /// Gets the enumeration of Triples in the union
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Triple> GetEnumerator()
        {
            return this._collections.SelectMany(c => c).GetEnumerator();
        }
    }
}
