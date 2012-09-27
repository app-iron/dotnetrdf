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
using VDS.RDF.Parsing;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Expressions.Functions.XPath.String
{
    /// <summary>
    /// Represents the XPath fn:starts-with() function
    /// </summary>
    public class StartsWithFunction
        : BaseBinaryStringFunction
    {
        /// <summary>
        /// Creates a new XPath Starts With function
        /// </summary>
        /// <param name="stringExpr">Expression</param>
        /// <param name="prefixExpr">Prefix Expression</param>
        public StartsWithFunction(ISparqlExpression stringExpr, ISparqlExpression prefixExpr)
            : base(stringExpr, prefixExpr, false, XPathFunctionFactory.AcceptStringArguments) { }

        /// <summary>
        /// Gets the Value of the function as applied to the given String Literal and Argument
        /// </summary>
        /// <param name="stringLit">Simple/String typed Literal</param>
        /// <param name="arg">Argument</param>
        /// <returns></returns>
        public override IValuedNode ValueInternal(ILiteralNode stringLit, ILiteralNode arg)
        {
            if (stringLit.Value.Equals(string.Empty))
            {
                if (arg.Value.Equals(string.Empty))
                {
                    //The Empty String starts with the Empty String
                    return new BooleanNode(null, true);
                }
                else
                {
                    //Empty String doesn't start with a non-empty string
                    return new BooleanNode(null, false);
                }
            }
            else if (arg.Value.Equals(string.Empty))
            {
                //Any non-empty string starts with the empty string
                return new BooleanNode(null, true);
            }
            else
            {
                //Otherwise evalute the StartsWith
                return new BooleanNode(null, stringLit.Value.StartsWith(arg.Value));
            }
        }

        /// <summary>
        /// Gets the String representation of the function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + XPathFunctionFactory.XPathFunctionsNamespace + XPathFunctionFactory.StartsWith + ">(" + this._expr.ToString() + "," + this._arg.ToString() + ")";
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return XPathFunctionFactory.XPathFunctionsNamespace + XPathFunctionFactory.StartsWith;
            }
        }

        /// <summary>
        /// Transforms the Expression using the given Transformer
        /// </summary>
        /// <param name="transformer">Expression Transformer</param>
        /// <returns></returns>
        public override ISparqlExpression Transform(IExpressionTransformer transformer)
        {
            return new StartsWithFunction(transformer.Transform(this._expr), transformer.Transform(this._arg));
        }
    }
}
