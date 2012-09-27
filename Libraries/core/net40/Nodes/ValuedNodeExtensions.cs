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
using System.Linq;
using System.Xml;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// Extension Methods related to valued nodes
    /// </summary>
    public static class ValuedNodeExtensions
    {
        /// <summary>
        /// Takes a <see cref="INode">INode</see> and converts it to a <see cref="IValuedNode">IValuedNode</see> if it is not already an instance that implements the interface
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns>Valued Node</returns>
        public static IValuedNode AsValuedNode(this INode n)
        {
            if (n == null) return (IValuedNode)n;
            if (n is IValuedNode) return (IValuedNode)n;

            switch (n.NodeType)
            {
                case NodeType.Blank:
                    IBlankNode b = (IBlankNode)n;
                    return new BlankNode(n.Graph, b.InternalID);
                case NodeType.GraphLiteral:
                    IGraphLiteralNode glit = (IGraphLiteralNode)n;
                    return new GraphLiteralNode(n.Graph, glit.SubGraph);
                case NodeType.Literal:
                    ILiteralNode lit = (ILiteralNode)n;
                    //Decide what kind of valued node to produce based on node datatype
                    if (lit.DataType != null)
                    {
                        String dt = lit.DataType.AbsoluteUri;
                        switch (dt)
                        {
                            case XmlSpecsHelper.XmlSchemaDataTypeBoolean:
                                bool bVal;
                                if (Boolean.TryParse(lit.Value, out bVal))
                                {
                                    return new BooleanNode(n.Graph, bVal);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeByte:
                                //xsd:byte actually maps to SignedByte in .Net
                                sbyte sbVal;
                                if (sbyte.TryParse(lit.Value, out sbVal))
                                {
                                    return new SignedByteNode(n.Graph, sbVal, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeDate:
                                DateTimeOffset date;
                                if (DateTimeOffset.TryParse(lit.Value, out date))
                                {
                                    return new DateNode(n.Graph, date, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeDateTime:
                                DateTimeOffset dateTime;
                                if (DateTimeOffset.TryParse(lit.Value, out dateTime))
                                {
                                    return new DateTimeNode(n.Graph, dateTime, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeDayTimeDuration:
                            case XmlSpecsHelper.XmlSchemaDataTypeDuration:
                                TimeSpan timeSpan;
                                try
                                {
                                    timeSpan = XmlConvert.ToTimeSpan(lit.Value);
                                    return new TimeSpanNode(n.Graph, timeSpan, lit.Value, lit.DataType);
                                }
                                catch
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeDecimal:
                                Decimal dec;
                                if (Decimal.TryParse(lit.Value, out dec))
                                {
                                    return new DecimalNode(n.Graph, dec, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeDouble:
                                Double dbl;
                                if (Double.TryParse(lit.Value, out dbl))
                                {
                                    return new DoubleNode(n.Graph, dbl, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeFloat:
                                Single flt;
                                if (Single.TryParse(lit.Value, out flt))
                                {
                                    return new FloatNode(n.Graph, flt, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeInt:
                            case XmlSpecsHelper.XmlSchemaDataTypeInteger:
                            case XmlSpecsHelper.XmlSchemaDataTypeLong:
                            case XmlSpecsHelper.XmlSchemaDataTypeShort:
                                long lng;
                                if (Int64.TryParse(lit.Value, out lng))
                                {
                                    return new LongNode(n.Graph, lng, lit.Value, lit.DataType);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeNegativeInteger:
                            case XmlSpecsHelper.XmlSchemaDataTypeNonPositiveInteger:
                                //Must be below zero
                                long neglng;
                                if (Int64.TryParse(lit.Value, out neglng) && neglng < 0)
                                {
                                    return new LongNode(n.Graph, neglng, lit.Value, lit.DataType);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeNonNegativeInteger:
                            case XmlSpecsHelper.XmlSchemaDataTypePositiveInteger:
                                //Must be above zero
                                long poslng;
                                if (Int64.TryParse(lit.Value, out poslng) && poslng >= 0)
                                {
                                    return new LongNode(n.Graph, poslng, lit.Value, lit.DataType);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeUnsignedByte:
                                //xsd:unsignedByte actually maps to Byte in .Net
                                byte byVal;
                                if (byte.TryParse(lit.Value, out byVal))
                                {
                                    return new ByteNode(n.Graph, byVal, lit.Value);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            case XmlSpecsHelper.XmlSchemaDataTypeUnsignedInt:
                            case XmlSpecsHelper.XmlSchemaDataTypeUnsignedLong:
                            case XmlSpecsHelper.XmlSchemaDataTypeUnsignedShort:
                                //Must be unsigned
                                ulong ulng;
                                if (UInt64.TryParse(lit.Value, out ulng))
                                {
                                    return new UnsignedLongNode(n.Graph, ulng, lit.Value, lit.DataType);
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                            default:
                                if (SparqlSpecsHelper.IntegerDataTypes.Contains(dt))
                                {
                                    long l;
                                    if (Int64.TryParse(lit.Value, out l))
                                    {
                                        return new LongNode(n.Graph, l, lit.Value, lit.DataType);
                                    }
                                    else
                                    {
                                        return new StringNode(n.Graph, lit.Value, lit.DataType);
                                    }
                                }
                                else
                                {
                                    return new StringNode(n.Graph, lit.Value, lit.DataType);
                                }
                        }
                    }
                    else if (!lit.Language.Equals(String.Empty))
                    {
                        return new StringNode(n.Graph, lit.Value, lit.Language);
                    }
                    else
                    {
                        return new StringNode(n.Graph, lit.Value);
                    }
                case NodeType.Uri:
                    IUriNode u = (IUriNode)n;
                    return new UriNode(n.Graph, u.Uri);
                case NodeType.Variable:
                    IVariableNode v = (IVariableNode)n;
                    return new VariableNode(n.Graph, v.VariableName);
                default:
                    throw new RdfQueryException("Cannot create a valued node for an unknown node type");
            }
        }

        /// <summary>
        /// Tries to get the result of calling <see cref="IValuedNode.AsBoolean()">AsBoolean()</see> on a node throwing an error if the node is null
        /// </summary>
        /// <param name="n">Node</param>
        /// <exception cref="RdfQueryException">Thrown if the input is null of the specific valued node cannot be cast to a boolean</exception>
        /// <returns></returns>
        public static bool AsSafeBoolean(this IValuedNode n)
        {
            if (n == null) throw new RdfQueryException("Cannot cast a null to a boolean");
            return n.AsBoolean();
        }

        //private static bool IsCoerceableDatatype(ILiteralNode lit)
        //{
        //    return lit.DataType == null || !lit.DataType.ToString().Equals(XmlSpecsHelper.XmlSchemaDataTypeString);
        //}

        //public static IValuedNode CoerceToInteger(this IValuedNode n)
        //{
        //    if (n == null) return n;
        //    if (n is LongNode) return n;
        //    if (n.NodeType != NodeType.Literal) return n;
        //    ILiteralNode lit = (ILiteralNode)n;
        //    switch (SparqlSpecsHelper.GetNumericTypeFromDataTypeUri(lit.DataType))
        //    {
        //        case SparqlNumericType.Integer:
        //        case SparqlNumericType.Decimal:
        //        case SparqlNumericType.Double:
        //        case SparqlNumericType.Float:
        //            //No need to coerce as either already an integer or cast will happen automatically if possible
        //            return n;
        //        default:
        //            if (!IsCoerceableDatatype(lit)) return n;
        //            //May coerce if lexical form parses as an integer
        //            long l;
        //            if (Int64.TryParse(lit.Value, out l))
        //            {
        //                return new LongNode(null, l);
        //            }
        //            else
        //            {
        //                return n;
        //            }
        //    }
        //}

        //public static IValuedNode CoerceToDecimal(this IValuedNode n)
        //{
        //    if (n == null) return n;
        //    if (n is DecimalNode) return n;
        //    if (n.NodeType != NodeType.Literal) return n;
        //    ILiteralNode lit = (ILiteralNode)n;
        //    switch (SparqlSpecsHelper.GetNumericTypeFromDataTypeUri(lit.DataType))
        //    {
        //        case SparqlNumericType.Integer:
        //        case SparqlNumericType.Decimal:
        //        case SparqlNumericType.Double:
        //        case SparqlNumericType.Float:
        //            //No need to coerce as either already a decimal or cast will happen automatically if possible
        //            return n;
        //        default:
        //            if (!IsCoerceableDatatype(lit)) return n;
        //            //May coerce if lexical form parses as a decimal
        //            decimal d;
        //            if (Decimal.TryParse(lit.Value, out d))
        //            {
        //                return new DecimalNode(null, d);
        //            }
        //            else
        //            {
        //                return n;
        //            }
        //    }
        //}

        //public static IValuedNode CoerceToDouble(this IValuedNode n)
        //{
        //    if (n == null) return n;
        //    if (n is DoubleNode) return n;
        //    if (n.NodeType != NodeType.Literal) return n;
        //    ILiteralNode lit = (ILiteralNode)n;
        //    switch (SparqlSpecsHelper.GetNumericTypeFromDataTypeUri(lit.DataType))
        //    {
        //        case SparqlNumericType.Integer:
        //        case SparqlNumericType.Decimal:
        //        case SparqlNumericType.Double:
        //        case SparqlNumericType.Float:
        //            //No need to coerce as either already a double or cast will happen automatically if possible
        //            return n;
        //        default:
        //            if (!IsCoerceableDatatype(lit)) return n;
        //            //May coerce if lexical form parses as a double
        //            double d;
        //            if (Double.TryParse(lit.Value, out d))
        //            {
        //                return new DoubleNode(null, d);
        //            }
        //            else
        //            {
        //                return n;
        //            }
        //    }
        //}

        //public static IValuedNode CoerceToFloat(this IValuedNode n)
        //{
        //    if (n == null) return n;
        //    if (n is FloatNode) return n;
        //    if (n.NodeType != NodeType.Literal) return n;
        //    ILiteralNode lit = (ILiteralNode)n;
        //    switch (SparqlSpecsHelper.GetNumericTypeFromDataTypeUri(lit.DataType))
        //    {
        //        case SparqlNumericType.Integer:
        //        case SparqlNumericType.Decimal:
        //        case SparqlNumericType.Double:
        //        case SparqlNumericType.Float:
        //            //No need to coerce as either already a double or cast will happen automatically if possible
        //            return n;
        //        default:
        //            if (!IsCoerceableDatatype(lit)) return n;
        //            //May coerce if lexical form parses as a double
        //            float f;
        //            if (Single.TryParse(lit.Value, out f))
        //            {
        //                return new FloatNode(null, f);
        //            }
        //            else
        //            {
        //                return n;
        //            }
        //    }
        //}

        //public static IValuedNode CoerceToBoolean(this IValuedNode n)
        //{
        //    if (n == null) return n;
        //    if (n is BooleanNode) return n;
        //    if (n.NodeType != NodeType.Literal) return n;
        //    ILiteralNode lit = (ILiteralNode)n;
        //    if (!IsCoerceableDatatype(lit)) return n;
        //    bool b;
        //    if (Boolean.TryParse(lit.Value, out b))
        //    {
        //        return new BooleanNode(null, b);
        //    }
        //    else
        //    {
        //        return n;
        //    }
        //}

        //public static IValuedNode CoerceToDateTime(this IValuedNode n)
        //{
        //    if (n == null) return n;
        //    if (n is DateTimeNode) return n;
        //    if (n.NodeType != NodeType.Literal) return n;
        //    ILiteralNode lit = (ILiteralNode)n;
        //    if (!IsCoerceableDatatype(lit)) return n;
        //    DateTimeOffset dt;
        //    if (DateTimeOffset.TryParse(lit.Value, out dt))
        //    {
        //        return new DateTimeNode(null, dt);
        //    }
        //    else
        //    {
        //        return n;
        //    }
        //}
    }
}
