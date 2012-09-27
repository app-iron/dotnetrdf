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
using VDS.RDF.Query.Patterns;

namespace VDS.RDF.Query.Algebra
{
    /// <summary>
    /// Represents a BGP which is a set of Triple Patterns
    /// </summary>
    public class Bgp : IBgp
    {
        private List<ITriplePattern> _triplePatterns = new List<ITriplePattern>();

        /// <summary>
        /// Creates a new empty BGP
        /// </summary>
        public Bgp()
        {

        }

        /// <summary>
        /// Creates a BGP containing a single Triple Pattern
        /// </summary>
        /// <param name="p">Triple Pattern</param>
        public Bgp(ITriplePattern p)
        {
            this._triplePatterns.Add(p);
        }

        /// <summary>
        /// Creates a BGP containing a set of Triple Patterns
        /// </summary>
        /// <param name="ps">Triple Patterns</param>
        public Bgp(IEnumerable<ITriplePattern> ps)
        {
            this._triplePatterns.AddRange(ps);
        }

        /// <summary>
        /// Gets the number of Triple Patterns in the BGP
        /// </summary>
        public int PatternCount
        {
            get
            {
                return this._triplePatterns.Count;
            }
        }

        /// <summary>
        /// Gets the Triple Patterns in the BGP
        /// </summary>
        public IEnumerable<ITriplePattern> TriplePatterns
        {
            get
            {
                return this._triplePatterns;
            }
        }

        /// <summary>
        /// Evaluates the BGP against the Evaluation Context
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <returns></returns>
        public BaseMultiset Evaluate(SparqlEvaluationContext context)
        {
            if (this._triplePatterns.Count > 0)
            {
                for (int i = 0; i < this._triplePatterns.Count; i++)
                {
                    if (i == 0)
                    {
                        //If the 1st thing in a BGP is a BIND/LET/FILTER the Input becomes the Identity Multiset
                        if (this._triplePatterns[i].PatternType == TriplePatternType.Filter || this._triplePatterns[i].PatternType == TriplePatternType.BindAssignment || this._triplePatterns[i].PatternType == TriplePatternType.LetAssignment)
                        {
                            if (this._triplePatterns[i].PatternType == TriplePatternType.BindAssignment)
                            {
                                if (context.InputMultiset.ContainsVariable(((IAssignmentPattern)this._triplePatterns[i]).VariableName)) throw new RdfQueryException("Cannot use a BIND assigment to BIND to a variable that has previously been declared");
                            }
                            else
                            {
                                context.InputMultiset = new IdentityMultiset();
                            }
                        }
                    }

                    //Create a new Output Multiset
                    context.OutputMultiset = new Multiset();

                    this._triplePatterns[i].Evaluate(context);

                    //If at any point we've got an Empty Multiset as our Output then we terminate BGP execution
                    if (context.OutputMultiset.IsEmpty) break;

                    //Check for Timeout before attempting the Join
                    context.CheckTimeout();

                    //If this isn't the first Pattern we do Join/Product the Output to the Input
                    if (i > 0)
                    {
                        if (context.InputMultiset.IsDisjointWith(context.OutputMultiset))
                        {
                            //Disjoint so do a Product
                            context.OutputMultiset = context.InputMultiset.ProductWithTimeout(context.OutputMultiset, context.RemainingTimeout);
                        }
                        else
                        {
                            //Normal Join
                            context.OutputMultiset = context.InputMultiset.Join(context.OutputMultiset);
                        }
                    }

                    //Then the Input for the next Pattern is the Output from the previous Pattern
                    context.InputMultiset = context.OutputMultiset;
                }

                if (context.TrimTemporaryVariables)
                {
                    //Trim the Multiset - this eliminates any temporary variables
                    context.OutputMultiset.Trim();
                }
            }
            else
            {
                //For an Empty BGP we just return the Identity Multiset
                context.OutputMultiset = new IdentityMultiset();
            }

            //If we've ended with an Empty Multiset then we turn it into the Null Multiset
            //to indicate that this BGP did not match anything
            if (context.OutputMultiset is Multiset && context.OutputMultiset.IsEmpty) context.OutputMultiset = new NullMultiset();

            //Return the Output Multiset
            return context.OutputMultiset;
        }

        /// <summary>
        /// Gets the Variables used in the Algebra
        /// </summary>
        public IEnumerable<String> Variables
        {
            get
            {
                return (from tp in this._triplePatterns
                        from v in tp.Variables
                        select v).Distinct();
            }
        }

        /// <summary>
        /// Gets whether the BGP is the emtpy BGP
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this._triplePatterns.Count == 0);
            }
        }

        /// <summary>
        /// Returns the String representation of the BGP
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this._triplePatterns.Count == 0)
            {
                return "BGP()";
            }
            else if (this._triplePatterns.Count == 1)
            {
                return "BGP(" + this._triplePatterns[0].ToString() + ")";
            }
            else
            {
                return "BGP([" + this._triplePatterns.Count + " Patterns])";
            }
        }

        /// <summary>
        /// Converts the Algebra back to a SPARQL Query
        /// </summary>
        /// <returns></returns>
        public SparqlQuery ToQuery()
        {
            SparqlQuery q = new SparqlQuery();
            q.RootGraphPattern = this.ToGraphPattern();
            q.Optimise();
            return q;
        }

        /// <summary>
        /// Converts the BGP to a Graph Pattern
        /// </summary>
        /// <returns></returns>
        public GraphPattern ToGraphPattern()
        {
            GraphPattern p = new GraphPattern();
            foreach (ITriplePattern tp in this._triplePatterns)
            {
                p.AddTriplePattern(tp);
            }
            return p;
        }
    }
}
