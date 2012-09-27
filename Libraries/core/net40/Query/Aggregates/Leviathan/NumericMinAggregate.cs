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
using VDS.RDF.Query.Expressions;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Expressions.Primary;

namespace VDS.RDF.Query.Aggregates.Leviathan
{
    /// <summary>
    /// Class representing NMIN Aggregate Functions
    /// </summary>
    /// <remarks>
    /// Only operates over numeric data which is typed to one of the supported SPARQL Numeric types (integers, decimals and doubles)
    /// </remarks>
    public class NumericMinAggregate
        : BaseAggregate
    {
        private String _varname;

        /// <summary>
        /// Creates a new NMIN Aggregate
        /// </summary>
        /// <param name="expr">Variable Expression</param>
        public NumericMinAggregate(VariableTerm expr)
            : this(expr, false) { }

        /// <summary>
        /// Creates a new NMIN Aggregate
        /// </summary>
        /// <param name="expr">Expression</param>
        public NumericMinAggregate(ISparqlExpression expr)
            : this(expr, false) { }

        /// <summary>
        /// Creates a new NMIN Aggregate
        /// </summary>
        /// <param name="expr">Variable Expression</param>
        /// <param name="distinct">Whether a DISTINCT modifier applies</param>
        public NumericMinAggregate(VariableTerm expr, bool distinct)
            : base(expr, distinct)
        {
            this._varname = expr.ToString().Substring(1);
        }

        /// <summary>
        /// Creates a new NMIN Aggregate
        /// </summary>
        /// <param name="expr">Expression</param>
        /// <param name="distinct">Whether a DISTINCT modifier applies</param>
        public NumericMinAggregate(ISparqlExpression expr, bool distinct)
            : base(expr, distinct) { }

        /// <summary>
        /// Applies the Numeric Min Aggregate function to the results
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingIDs">Binding IDs over which the Aggregate applies</param>
        /// <returns></returns>
        public override IValuedNode Apply(SparqlEvaluationContext context, IEnumerable<int> bindingIDs)
        {
            if (this._varname != null)
            {
                //Ensured the MINed variable is in the Variables of the Results
                if (!context.Binder.Variables.Contains(this._varname))
                {
                    throw new RdfQueryException("Cannot use the Variable " + this._expr.ToString() + " in a NMIN Aggregate since the Variable does not occur in a Graph Pattern");
                }
            }

            //Prep Variables
            long lngmin = 0;
            decimal decmin = 0.0m;
            float fltmin = 0.0f;
            double dblmin = 0.0d;
            SparqlNumericType mintype = SparqlNumericType.NaN;
            SparqlNumericType numtype;

            foreach (int id in bindingIDs)
            {
                IValuedNode temp;
                try
                {
                    temp = this._expr.Evaluate(context, id);
                    if (temp == null) continue;
                    numtype = temp.NumericType;
                }
                catch
                {
                    continue;
                }

                //Skip if Not a Number
                if (numtype == SparqlNumericType.NaN) continue;

                //Track the Numeric Type
                if ((int)numtype > (int)mintype)
                {
                    if (mintype == SparqlNumericType.NaN)
                    {
                        //Initialise Minimums
                        switch (numtype)
                        {
                            case SparqlNumericType.Integer:
                                lngmin = temp.AsInteger();
                                decmin = temp.AsDecimal();
                                fltmin = temp.AsFloat();
                                dblmin = temp.AsDouble();
                                break;
                            case SparqlNumericType.Decimal:
                                decmin = temp.AsDecimal();
                                fltmin = temp.AsFloat();
                                dblmin = temp.AsDouble();
                                break;
                            case SparqlNumericType.Float:
                                fltmin = temp.AsFloat();
                                dblmin = temp.AsDouble();
                                break;
                            case SparqlNumericType.Double:
                                dblmin = temp.AsDouble();
                                break;
                        }
                        mintype = numtype;
                        continue;
                    }
                    else
                    {
                        mintype = numtype;
                    }
                }

                long lngval;
                decimal decval;
                float fltval;
                double dblval;
                switch (mintype)
                {
                    case SparqlNumericType.Integer:
                        lngval = temp.AsInteger();

                        if (lngval < lngmin)
                        {
                            lngmin = lngval;
                            decmin = temp.AsDecimal();
                            fltmin = temp.AsFloat();
                            dblmin = temp.AsDouble();
                        }
                        break;
                    case SparqlNumericType.Decimal:
                        decval = temp.AsDecimal();

                        if (decval < decmin)
                        {
                            decmin = decval;
                            fltmin = temp.AsFloat();
                            dblmin = temp.AsDouble();
                        }
                        break;
                    case SparqlNumericType.Float:
                        fltval = temp.AsFloat();

                        if (fltval < fltmin)
                        {
                            fltmin = fltval;
                            dblmin = temp.AsDouble();
                        }
                        break;
                    case SparqlNumericType.Double:
                        dblval = temp.AsDouble();

                        if (dblval < dblmin)
                        {
                            dblmin = dblval;
                        }
                        break;
                }
            }

            //Return the Min
            switch (mintype)
            {
                case SparqlNumericType.NaN:
                    //No Numeric Values
                    return null;

                case SparqlNumericType.Integer:
                    //Integer Values
                    return new LongNode(null, lngmin);

                case SparqlNumericType.Decimal:
                    //Decimal Values
                    return new DecimalNode(null, decmin);

                case SparqlNumericType.Float:
                    //Float Values
                    return new FloatNode(null, fltmin);

                case SparqlNumericType.Double:
                    //Double Values
                    return new DoubleNode(null, dblmin);

                default:
                    throw new RdfQueryException("Failed to calculate a valid Minimum");
            }
        }

        /// <summary>
        /// Gets the String representation of the Aggregate
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append('<');
            output.Append(LeviathanFunctionFactory.LeviathanFunctionsNamespace);
            output.Append(LeviathanFunctionFactory.NumericMin);
            output.Append(">(");
            if (this._distinct) output.Append("DISTINCT ");
            output.Append(this._expr.ToString());
            output.Append(')');
            return output.ToString();
        }

        /// <summary>
        /// Gets the Functor of the Aggregate
        /// </summary>
        public override string Functor
        {
            get
            {
                return LeviathanFunctionFactory.LeviathanFunctionsNamespace + LeviathanFunctionFactory.NumericMin;
            }
        }
    }
}
