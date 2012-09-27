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
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Expressions;

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// A Valued Node representing double values
    /// </summary>
    public class DoubleNode
        : NumericNode
    {
        private double _value;

        /// <summary>
        /// Creates a new double valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Double value</param>
        /// <param name="lexicalValue">Lexical value</param>
        public DoubleNode(IGraph g, double value, String lexicalValue)
            : base(g, lexicalValue, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDouble), SparqlNumericType.Double)
        {
            this._value = value;
        }

        /// <summary>
        /// Creates a new double valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Double value</param>
        public DoubleNode(IGraph g, double value)
            : this(g, value, value.ToString()) { }

        /// <summary>
        /// Gets the integer value of the double
        /// </summary>
        /// <returns></returns>
        public override long AsInteger()
        {
            try
            {
                return Convert.ToInt64(this._value);
            }
            catch
            {
                throw new RdfQueryException("Unable to downcast Double to Long");
            }
        }

        /// <summary>
        /// Gets the decimal value of the double
        /// </summary>
        /// <returns></returns>
        public override decimal AsDecimal()
        {
            try
            {
                return Convert.ToDecimal(this._value);
            }
            catch
            {
                throw new RdfQueryException("Unable to cast Double to Decimal");
            }
        }

        /// <summary>
        /// Gets the float value of the double
        /// </summary>
        /// <returns></returns>
        public override float AsFloat()
        {
            try
            {
                return Convert.ToSingle(this._value);
            }
            catch
            {
                throw new RdfQueryException("Unable to downcast Double to Float");
            }
        }

        /// <summary>
        /// Gets the double value
        /// </summary>
        /// <returns></returns>
        public override double AsDouble()
        {
            return this._value;
        }
    }
}
