/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections.Generic;
using System.Linq;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Engine;

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Set
{
    /// <summary>
    /// Abstract base class for SPARQL Functions which operate on Sets
    /// </summary>
    public abstract class BaseSetFunction
        : INAryExpression
    {
        /// <summary>
        /// Variable Expression Term that the Set function applies to
        /// </summary>
        protected IExpression _expr;
        /// <summary>
        /// Set that is used in the function
        /// </summary>
        protected List<IExpression> _expressions = new List<IExpression>();

        /// <summary>
        /// Creates a new SPARQL Set function
        /// </summary>
        /// <param name="expr">Expression</param>
        /// <param name="set">Set</param>
        protected BaseSetFunction(IExpression expr, IEnumerable<IExpression> set)
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
        public abstract IValuedNode Evaluate(ISolution solution, IExpressionContext context);

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
        /// Gets the Functor of the Expression
        /// </summary>
        public abstract string Functor
        {
            get;
        }

        /// <summary>
        /// Gets the Arguments of the Exception
        /// </summary>
        public IEnumerable<IExpression> Arguments
        {
            get
            {
                return this._expr.AsEnumerable<IExpression>().Concat(this._expressions);
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
    }
}
