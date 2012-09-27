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
using VDS.RDF.Query.FullText.Search;
using VDS.RDF.Query.PropertyFunctions;

namespace VDS.RDF.Query.Algebra
{
    /// <summary>
    /// Algebra Operator which provides full text query capabilities for a query
    /// </summary>
    /// <remarks>
    /// The evaluation of this operator simply registers the search provider with the Evaluation Context such that any <see cref="FullTextMatchPropertyFunction"/> instances are honoured
    /// </remarks>
    public class FullTextQuery
        : IUnaryOperator
    {
        private IFullTextSearchProvider _provider;

        public FullTextQuery(IFullTextSearchProvider searchProvider, ISparqlAlgebra algebra)
        {
            this._provider = searchProvider;
            this.InnerAlgebra = algebra;
        }

        public ISparqlAlgebra InnerAlgebra
        {
            get;
            private set;
        }

        public ISparqlAlgebra Transform(Optimisation.IAlgebraOptimiser optimiser)
        {
            return new FullTextQuery(this._provider, optimiser.Optimise(this.InnerAlgebra));
        }

        public BaseMultiset Evaluate(SparqlEvaluationContext context)
        {
            context[FullTextHelper.ContextKey] = this._provider;
            return context.Evaluate(this.InnerAlgebra);
        }

        public IEnumerable<string> Variables
        {
            get 
            {
                return this.InnerAlgebra.Variables;
            }
        }

        public SparqlQuery ToQuery()
        {
            return this.InnerAlgebra.ToQuery();
        }

        public Patterns.GraphPattern ToGraphPattern()
        {
            return this.InnerAlgebra.ToGraphPattern();
        }

        public override string ToString()
        {
            return "FullTextQuery(" + this.InnerAlgebra.ToString() + ")";
        }
    }
}
