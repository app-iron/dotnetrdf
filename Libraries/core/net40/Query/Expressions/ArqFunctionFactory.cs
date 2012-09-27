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
using VDS.RDF.Query.Expressions.Functions.Arq;

namespace VDS.RDF.Query.Expressions
{
    /// <summary>
    /// Expression Factory which generates ARQ Function expressions
    /// </summary>
    /// <remarks>
    /// <para>
    /// Designed to help provide feature parity with the ARQ query engine contained in Jena
    /// </para>
    /// </remarks>
    public class ArqFunctionFactory : ISparqlCustomExpressionFactory
    {
        /// <summary>
        /// ARQ Function Namespace
        /// </summary>
        public const String ArqFunctionsNamespace = "http://jena.hpl.hp.com/ARQ/function#";

        /// <summary>
        /// Constants for ARQ Numeric functions
        /// </summary>
        public const String Max = "max",
                            Min = "min",
                            Pi = "pi",
                            E = "e";

        /// <summary>
        /// Constants for ARQ Graph functions
        /// </summary>
        public const String BNode = "bnode",
                            LocalName = "localname",
                            Namespace = "namespace";

        /// <summary>
        /// Constants for ARQ String functions
        /// </summary>
        public const String Substr = "substr",
                            Substring = "substring",
                            StrJoin = "strjoin";

        /// <summary>
        /// Constants for ARQ Miscellaneous functions
        /// </summary>
        public const String Sha1Sum = "sha1sum",
                            Now = "now";

        /// <summary>
        /// Array of Extension Function URIs
        /// </summary>
        private String[] FunctionUris = {
                                            Max,
                                            Min,
                                            Pi,
                                            E,
                                            BNode,
                                            LocalName,
                                            Namespace,
                                            Substr,
                                            Substring,
                                            StrJoin,
                                            Sha1Sum,
                                            Now
                                        };

        /// <summary>
        /// Tries to create an ARQ Function expression if the function Uri correseponds to a supported ARQ Function
        /// </summary>
        /// <param name="u">Function Uri</param>
        /// <param name="args">Function Arguments</param>
        /// <param name="scalarArgs">Scalar Arguments</param>
        /// <param name="expr">Generated Expression</param>
        /// <returns>Whether an expression was successfully generated</returns>
        public bool TryCreateExpression(Uri u, List<ISparqlExpression> args, Dictionary<String,ISparqlExpression> scalarArgs, out ISparqlExpression expr)
        {
            //If any Scalar Arguments are present then can't possibly be an ARQ Function
            if (scalarArgs.Count > 0)
            {
                expr = null;
                return false;
            }

            String func = u.AbsoluteUri;
            if (func.StartsWith(ArqFunctionFactory.ArqFunctionsNamespace))
            {
                func = func.Substring(ArqFunctionFactory.ArqFunctionsNamespace.Length);
                ISparqlExpression arqFunc = null;

                switch (func)
                {
                    case ArqFunctionFactory.BNode:
                        if (args.Count == 1)
                        {
                            arqFunc = new BNodeFunction(args.First());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ bnode() function");
                        }
                        break;
                    case ArqFunctionFactory.E:
                        if (args.Count == 0)
                        {
                            arqFunc = new EFunction();
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ e() function");
                        }
                        break;
                    case ArqFunctionFactory.LocalName:
                        if (args.Count == 1)
                        {
                            arqFunc = new LocalNameFunction(args.First());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ localname() function");
                        }
                        break;
                    case ArqFunctionFactory.Max:
                        if (args.Count == 2)
                        {
                            arqFunc = new MaxFunction(args.First(), args.Last());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ max() function");
                        }
                        break;
                    case ArqFunctionFactory.Min:
                        if (args.Count == 2)
                        {
                            arqFunc = new MinFunction(args.First(), args.Last());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ min() function");
                        }
                        break;
                    case ArqFunctionFactory.Namespace:
                        if (args.Count == 1)
                        {
                            arqFunc = new NamespaceFunction(args.First());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ namespace() function");
                        }
                        break;
                    case ArqFunctionFactory.Now:
                        if (args.Count == 0)
                        {
                            arqFunc = new NowFunction();
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ now() function");
                        }
                        break;
                    case ArqFunctionFactory.Pi:
                        if (args.Count == 0)
                        {
                            arqFunc = new PiFunction();
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ pi() function");
                        }
                        break;
                    case ArqFunctionFactory.Sha1Sum:
                        if (args.Count == 1)
                        {
                            arqFunc = new Sha1Function(args.First());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ sha1sum() function");
                        }
                        break;
                    case ArqFunctionFactory.StrJoin:
                        if (args.Count >= 2)
                        {
                            arqFunc = new StringJoinFunction(args.First(), args.Skip(1));
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ strjoing() function");
                        }
                        break;
                    case ArqFunctionFactory.Substr:
                    case ArqFunctionFactory.Substring:
                        if (args.Count == 2)
                        {
                            arqFunc = new SubstringFunction(args.First(), args.Last());
                        }
                        else if (args.Count == 3)
                        {
                            arqFunc = new SubstringFunction(args.First(), args[1], args.Last());
                        }
                        else
                        {
                            throw new RdfParseException("Incorrect number of arguments for the ARQ " + func + "() function");
                        }
                        break;
                }

                if (arqFunc != null)
                {
                    expr = arqFunc;
                    return true;
                }
            }
            expr = null;
            return false;  
        }

        /// <summary>
        /// Gets the Extension Function URIs supported by this Factory
        /// </summary>
        public IEnumerable<Uri> AvailableExtensionFunctions
        {
            get
            {
                return (from u in FunctionUris
                        select UriFactory.Create(ArqFunctionsNamespace + u));
            }
        }

        /// <summary>
        /// Gets the Extension Aggregate URIs supported by this Factory
        /// </summary>
        public IEnumerable<Uri> AvailableExtensionAggregates
        {
            get
            {
                return Enumerable.Empty<Uri>();
            }
        }
    }
}
