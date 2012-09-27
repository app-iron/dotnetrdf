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
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Expressions
{
    /// <summary>
    /// Numeric Types for Sparql Numeric Expressions
    /// </summary>
    /// <remarks>All Numeric expressions in Sparql are typed as Integer/Decimal/Double</remarks>
    public enum SparqlNumericType : int
    {
        /// <summary>
        /// Not a Number
        /// </summary>
        NaN = -1,
        /// <summary>
        /// An Integer
        /// </summary>
        Integer = 0,
        /// <summary>
        /// A Decimal
        /// </summary>
        Decimal = 1,
        /// <summary>
        /// A Single precision Floating Point
        /// </summary>
        Float = 2,
        /// <summary>
        /// A Double precision Floating Point
        /// </summary>
        Double = 3
    }

    /// <summary>
    /// SPARQL Expression Types
    /// </summary>
    public enum SparqlExpressionType
    {
        /// <summary>
        /// The Expression is a Primary Expression which is a leaf in the expression tree
        /// </summary>
        Primary,
        /// <summary>
        /// The Expression is a Unary Operator which has a single argument
        /// </summary>
        UnaryOperator,
        /// <summary>
        /// The Expression is a Binary Operator which has two arguments
        /// </summary>
        BinaryOperator,
        /// <summary>
        /// The Expression is a Function which has zero/more arguments
        /// </summary>
        Function,
        /// <summary>
        /// The Expression is an Aggregate Function which has one/more arguments
        /// </summary>
        Aggregate,
        /// <summary>
        /// The Expression is a Set Operator where the first argument forms the LHS and all remaining arguments form a set on the RHS
        /// </summary>
        SetOperator,
        /// <summary>
        /// The Expression is a Unary Operator that applies to a Graph Pattern
        /// </summary>
        GraphOperator
    }

    /// <summary>
    /// Interface for SPARQL Expression Terms that can be used in Expression Trees while evaluating Sparql Queries
    /// </summary>
    public interface ISparqlExpression
    {
        /// <summary>
        /// Evalutes a SPARQL Expression for the given binding in a given context
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingID">Binding ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// Newly introduced in Version 0.6.0 to replace the variety of functions that were used previously for numeric vs non-numeric versions to allow our code to be simplified and improve performance
        /// </para>
        /// </remarks>
        IValuedNode Evaluate(SparqlEvaluationContext context, int bindingID);

        /// <summary>
        /// Gets an enumeration of all the Variables used in an expression
        /// </summary>
        IEnumerable<String> Variables
        {
            get;
        }

        /// <summary>
        /// Gets the SPARQL Expression Type
        /// </summary>
        SparqlExpressionType Type
        {
            get;
        }

        /// <summary>
        /// Gets the Function Name or Operator Symbol - function names may be URIs of Keywords or the empty string in the case of primary expressions
        /// </summary>
        String Functor
        {
            get;
        }

        /// <summary>
        /// Gets the Arguments of this Expression
        /// </summary>
        IEnumerable<ISparqlExpression> Arguments
        {
            get;
        }

        /// <summary>
        /// Transforms the arguments of the expression using the given transformer
        /// </summary>
        /// <param name="transformer">Expression Transformer</param>
        /// <returns></returns>
        ISparqlExpression Transform(IExpressionTransformer transformer);

        /// <summary>
        /// Gets whether an expression can safely be evaluated in parallel
        /// </summary>
        bool CanParallelise
        {
            get;
        }
    }
}