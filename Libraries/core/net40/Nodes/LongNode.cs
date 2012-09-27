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
    /// A Valued Node with a Long value
    /// </summary>
    public class LongNode
        : NumericNode
    {
        private long _value;

        /// <summary>
        /// Creates a new long valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Long value</param>
        /// <param name="lexicalValue">Lexical Value</param>
        public LongNode(IGraph g, long value, String lexicalValue)
            : this(g, value, lexicalValue, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger)) { }

        /// <summary>
        /// Creates a new long valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Long value</param>
        /// <param name="lexicalValue">Lexical Value</param>
        /// <param name="datatype">Datatype URI</param>
        public LongNode(IGraph g, long value, String lexicalValue, Uri datatype)
            : base(g, lexicalValue, datatype, SparqlNumericType.Integer)
        {
            this._value = value;
        }

        /// <summary>
        /// Creates a new long valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Long value</param>
        public LongNode(IGraph g, long value)
            : this(g, value, value.ToString()) { }

        /// <summary>
        /// Gets the long value
        /// </summary>
        /// <returns></returns>
        public override long AsInteger()
        {
            return this._value;
        }

        /// <summary>
        /// Gets the decimal value of the long
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
                throw new RdfQueryException("Unable to upcast Long to Double");
            }
        }

        /// <summary>
        /// Gets the float value of the long
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
                throw new RdfQueryException("Unable to upcast Long to Float");
            }
        }

        /// <summary>
        /// Gets the double value of the long
        /// </summary>
        /// <returns></returns>
        public override double AsDouble()
        {
            try
            {
                return Convert.ToDouble(this._value);
            }
            catch
            {
                throw new RdfQueryException("Unable to upcast Long to Double");
            }
        }
    }

    /// <summary>
    /// A Valued Node with a unsigned long value
    /// </summary>
    public class UnsignedLongNode
        : NumericNode
    {
        private ulong _value;

        /// <summary>
        /// Creates a new unsigned long valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Unsigned Long value</param>
        /// <param name="lexicalValue">Lexical Value</param>
        public UnsignedLongNode(IGraph g, ulong value, String lexicalValue)
            : this(g, value, lexicalValue, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeUnsignedInt)) { }

        /// <summary>
        /// Creates a new unsigned long valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Unsigned Long value</param>
        /// <param name="lexicalValue">Lexical Value</param>
        /// <param name="datatype">Datatype URI</param>
        public UnsignedLongNode(IGraph g, ulong value, String lexicalValue, Uri datatype)
            : base(g, lexicalValue, datatype, SparqlNumericType.Integer)
        {
            this._value = value;
        }

        /// <summary>
        /// Creates a new usigned long valued node
        /// </summary>
        /// <param name="g">Graph the node belongs to</param>
        /// <param name="value">Unsigned Long value</param>
        public UnsignedLongNode(IGraph g, ulong value)
            : this(g, value, value.ToString()) { }

        /// <summary>
        /// Gets the long value of the ulong
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
                throw new RdfQueryException("Unable to downcast unsigned Long to long");
            }
        }

        /// <summary>
        /// Gets the decimal value of the ulong
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
                throw new RdfQueryException("Unable to upcast Long to Double");
            }
        }

        /// <summary>
        /// Gets the float value of the ulong
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
                throw new RdfQueryException("Unable to upcast Long to Float");
            }
        }

        /// <summary>
        /// Gets the double value of the ulong
        /// </summary>
        /// <returns></returns>
        public override double AsDouble()
        {
            try
            {
                return Convert.ToDouble(this._value);
            }
            catch
            {
                throw new RdfQueryException("Unable to upcast Long to Double");
            }
        }
    }
}
