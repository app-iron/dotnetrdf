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
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Set
{
    /// <summary>
    /// Abstract base class for SPARQL Functions which operate on Sets
    /// </summary>
    public abstract class BaseSetFunction
        : ISparqlExpression
    {
        /// <summary>
        /// Variable Expression Term that the Set function applies to
        /// </summary>
        protected ISparqlExpression _expr;
        /// <summary>
        /// Set that is used in the function
        /// </summary>
        protected List<ISparqlExpression> _expressions = new List<ISparqlExpression>();

        /// <summary>
        /// Creates a new SPARQL Set function
        /// </summary>
        /// <param name="expr">Expression</param>
        /// <param name="set">Set</param>
        public BaseSetFunction(ISparqlExpression expr, IEnumerable<ISparqlExpression> set)
        {
            this._expr = expr;
            this._expressions.AddRange(set);
        }

        /// <summary>
        /// Gets the value of the function as evaluated for a given Binding in the given Context
        /// </summary>
        /// <param name="context">SPARQL Evaluation Context</param>
        /// <param name="bindingID">Binding ID</param>
        /// <returns></returns>
        public abstract IValuedNode Evaluate(SparqlEvaluationContext context, int bindingID);

        /// <summary>
        /// Gets the Variable the function applies to
        /// </summary>
        public IEnumerable<string> Variables
        {
            get
            {
                return this._expr.Variables.Concat(from e in this._expressions
                                                   from v in e.Variables
                                                   select v);
            }
        }

        /// <summary>
        /// Gets the Type of the Expression
        /// </summary>
        public SparqlExpressionType Type
        {
            get
            {
                return SparqlExpressionType.SetOperator;
            }
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public abstract string Functor
        {
            get;
        }

        /// <summary>
        /// Gets the Arguments of the Exception
        /// </summary>
        public IEnumerable<ISparqlExpression> Arguments
        {
            get
            {
                return this._expr.AsEnumerable<ISparqlExpression>().Concat(this._expressions);
            }
        }

        /// <summary>
        /// Gets whether an expression can safely be evaluated in parallel
        /// </summary>
        public virtual bool CanParallelise
        {
            get
            {
                return this._expr.CanParallelise;
            }
        }

        /// <summary>
        /// Gets the String representation of the Expression
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        /// <summary>
        /// Transforms the Expression using the given Transformer
        /// </summary>
        /// <param name="transformer">Expression Transformer</param>
        /// <returns></returns>
        public abstract ISparqlExpression Transform(IExpressionTransformer transformer);
    }
}
