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
    /// Represents one possible set of values which is a solution to the query
    /// </summary>
    public sealed class Set 
        : BaseSet, IEquatable<Set>
    {
        private Dictionary<String, INode> _values;

        /// <summary>
        /// Creates a new Set
        /// </summary>
        public Set()
        {
            this._values = new Dictionary<string, INode>();
        }

        /// <summary>
        /// Creates a new Set which is the Join of the two Sets
        /// </summary>
        /// <param name="x">A Set</param>
        /// <param name="y">A Set</param>
        public Set(ISet x, ISet y)
        {
            this._values = new Dictionary<string, INode>();
            foreach (String var in x.Variables)
            {
                this._values.Add(var, x[var]);
            }
            foreach (String var in y.Variables)
            {
                if (!this._values.ContainsKey(var))
                {
                    this._values.Add(var, y[var]);
                }
                else if (this._values[var] == null)
                {
                    this._values[var] = y[var];
                }
            }
        }

        /// <summary>
        /// Creates a new Set which is a copy of an existing Set
        /// </summary>
        /// <param name="x">Set to copy</param>
        public Set(ISet x)
        {
            this._values = new Dictionary<string, INode>();
            foreach (String var in x.Variables)
            {
                this._values.Add(var, x[var]);
            }
        }

        /// <summary>
        /// Creates a new Set from a SPARQL Result
        /// </summary>
        /// <param name="result">Result</param>
        public Set(SparqlResult result)
        {
            this._values = new Dictionary<string, INode>();
            foreach (String var in result.Variables)
            {
                this.Add(var, result[var]);
            }
        }

        /// <summary>
        /// Creates a new Set from a Binding Tuple
        /// </summary>
        /// <param name="tuple">Tuple</param>
        public Set(BindingTuple tuple)
        {
            this._values = new Dictionary<string, INode>();
            foreach (KeyValuePair<String, PatternItem> binding in tuple.Values)
            {
                this.Add(binding.Key, tuple[binding.Key]);
            }
        }

        /// <summary>
        /// Retrieves the Value in this set for the given Variable
        /// </summary>
        /// <param name="variable">Variable</param>
        /// <returns>Either a Node or a null</returns>
        public override INode this[String variable]
        {
            get
            {
                if (this._values.ContainsKey(variable))
                {
                    return this._values[variable];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Adds a Value for a Variable to the Set
        /// </summary>
        /// <param name="variable">Variable</param>
        /// <param name="value">Value</param>
        public override void Add(String variable, INode value)
        {
            if (!this._values.ContainsKey(variable))
            {
                this._values.Add(variable, value);
            }
            else
            {
                throw new RdfQueryException("The value of a variable in a Set cannot be changed");
            }
        }

        /// <summary>
        /// Removes a Value for a Variable from the Set
        /// </summary>
        /// <param name="variable">Variable</param>
        public override void Remove(String variable)
        {
            if (this._values.ContainsKey(variable)) this._values.Remove(variable);
        }

        /// <summary>
        /// Checks whether the Set contains a given Variable
        /// </summary>
        /// <param name="variable">Variable</param>
        /// <returns></returns>
        public override bool ContainsVariable(String variable)
        {
            return this._values.ContainsKey(variable);
        }

        /// <summary>
        /// Gets whether the Set is compatible with a given set based on the given variables
        /// </summary>
        /// <param name="s">Set</param>
        /// <param name="vars">Variables</param>
        /// <returns></returns>
        public override bool IsCompatibleWith(ISet s, IEnumerable<string> vars)
        {
            return vars.All(v => this[v] == null || s[v] == null || this[v].Equals(s[v]));
        }

        /// <summary>
        /// Gets whether the Set is minus compatible with a given set based on the given variables
        /// </summary>
        /// <param name="s">Set</param>
        /// <param name="vars">Variables</param>
        /// <returns></returns>
        public override bool IsMinusCompatibleWith(ISet s, IEnumerable<string> vars)
        {
            return vars.Any(v => this[v] != null && this[v].Equals(s[v]));
        }

        /// <summary>
        /// Gets the Variables in the Set
        /// </summary>
        public override IEnumerable<String> Variables
        {
            get
            {
                return (from var in this._values.Keys
                        select var);
            }
        }

        /// <summary>
        /// Gets the Values in the Set
        /// </summary>
        public override IEnumerable<INode> Values
        {
            get
            {
                return (from value in this._values.Values
                        select value);
            }
        }

        /// <summary>
        /// Joins the set to another set
        /// </summary>
        /// <param name="other">Other Set</param>
        /// <returns></returns>
        public override ISet Join(ISet other)
        {
            //return new Set(this, other);
            return new VirtualSet(other, this);
        }

        /// <summary>
        /// Copies the Set
        /// </summary>
        /// <returns></returns>
        public override ISet Copy()
        {
            return new Set(this);
            //return new VirtualSet(this);
        }



        /// <summary>
        /// Gets whether the Set is equal to another set
        /// </summary>
        /// <param name="other">Set to compare with</param>
        /// <returns></returns>
        public bool Equals(Set other)
        {
            if (other == null) return false;
            return this._values.All(pair => other.ContainsVariable(pair.Key) && ((pair.Value == null && other[pair.Key] == null) || pair.Value.Equals(other[pair.Key])));
        }
    }

    /// <summary>
    /// Represents one possible set of values which is a solution to the query where those values are the result of joining one or more possible sets
    /// </summary>
    /// <remarks>
    /// Delays the memory copying and materialisation of the join as late as possible
    /// </remarks>
    public sealed class VirtualSet
        : BaseSet, IEquatable<VirtualSet>
    {
        private List<ISet> _sets = new List<ISet>();
        private bool _materialized = false;
        private Dictionary<String, INode> _values = new Dictionary<string, INode>();

        /// <summary>
        /// Creates a Joined Set
        /// </summary>
        /// <param name="x">Set</param>
        /// <param name="y">Another Set</param>
        public VirtualSet(ISet x, ISet y)
        {
            this._sets.Add(x);
            this._sets.Add(y);
        }

        /// <summary>
        /// Creates a Joined Set
        /// </summary>
        /// <param name="x">Set</param>
        /// <param name="ys">Other Set(s)</param>
        internal VirtualSet(ISet x, IEnumerable<ISet> ys)
        {
            this._sets.Add(x);
            this._sets.AddRange(ys);
        }

        /// <summary>
        /// Creates a Virtual Set which is simply a copy of another set
        /// </summary>
        /// <param name="x">Set</param>
        internal VirtualSet(ISet x)
        {
            this._sets.Add(x);
        }

        /// <summary>
        /// Materialises the virtual set into memory by copying
        /// </summary>
        private void Materialise()
        {
            //Note that we don't clean up the current state of the dictionary as that will already contain
            //an accurate cache of values that have been looked up
            foreach (ISet s in this._sets)
            {
                foreach (String var in s.Variables)
                {
                    INode temp;
                    if (this._values.TryGetValue(var, out temp))
                    {
                        //Only replace a value already copied from another set 
                        //if the value copied so far was a null
                        if (temp == null) this._values[var] = s[var];
                    }
                    else
                    {
                        //Otherwise not yet copied this variable
                        this._values.Add(var, temp);
                    }
                }
            }
            this._sets.Clear();
            this._sets = null;
            this._materialized = true;
        }

        /// <summary>
        /// Adds a Value for a Variable to the Set
        /// </summary>
        /// <param name="variable">Variable</param>
        /// <param name="value">Value</param>
        public override void Add(string variable, INode value)
        {
            //Materialise if necessary
            if (!this._materialized)
            {
                this.Materialise();
            }
            //Add the value
            if (this._values.ContainsKey(variable))
            {
                this._values[variable] = value;
            }
            else
            {
                this._values.Add(variable, value);
            }
        }

        /// <summary>
        /// Checks whether the Set contains a given Variable
        /// </summary>
        /// <param name="variable">Variable</param>
        /// <returns></returns>
        public override bool ContainsVariable(string variable)
        {
            if (this._materialized)
            {
                return this._values.ContainsKey(variable);
            }
            else
            {
                return this._sets.Any(s => s.ContainsVariable(variable));
            }
        }

        /// <summary>
        /// Gets whether the Set is compatible with a given set based on the given variables
        /// </summary>
        /// <param name="s">Set</param>
        /// <param name="vars">Variables</param>
        /// <returns></returns>
        public override bool IsCompatibleWith(ISet s, IEnumerable<string> vars)
        {
            return vars.All(v => this[v] == null || s[v] == null || this[v].Equals(s[v]));
        }

        /// <summary>
        /// Gets whether the Set is minus compatible with a given set based on the given variables
        /// </summary>
        /// <param name="s">Set</param>
        /// <param name="vars">Variables</param>
        /// <returns></returns>
        public override bool IsMinusCompatibleWith(ISet s, IEnumerable<string> vars)
        {
            return vars.Any(v => this[v] != null && this[v].Equals(s[v]));
        }

        /// <summary>
        /// Removes a Value for a Variable from the Set
        /// </summary>
        /// <param name="variable">Variable</param>
        public override void Remove(string variable)
        {
            //Materialise if necessary
            if (!this._materialized)
            {
                this.Materialise();
            }
            //Remove the value
            this._values.Remove(variable);
        }

        /// <summary>
        /// Retrieves the Value in this set for the given Variable
        /// </summary>
        /// <param name="variable">Variable</param>
        /// <returns>Either a Node or a null</returns>
        public override INode this[string variable]
        {
            get
            {
                INode temp = null;

                if (this._values.TryGetValue(variable, out temp))
                {
                    //Use our local dictionary wherever possible
                    return temp;
                }
                else if (this._materialized)
                {
                    //If we've materialized and the variable is not in our dictionary then it doesn't have a value in this
                    //set so return null
                    return null;
                }
                else
                {
                    //Otherwise search the inner sets for a value
                    int i = 0;

                    //Find the first set that has a value for the variable and return it
                    do
                    {
                        temp = this._sets[i][variable];
                        if (temp != null)
                        {
                            //Cache the retrieved value locally so we can re-use it
                            this._values.Add(variable, temp);
                            return temp;
                        }
                        i++;
                    } while (i < this._sets.Count);

                    //Return null if no sets have a value for the variable
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Values in the Set
        /// </summary>
        public override IEnumerable<INode> Values
        {
            get
            {
                return (from v in this.Variables
                        select this[v]);
            }
        }

        /// <summary>
        /// Gets the Variables in the Set
        /// </summary>
        public override IEnumerable<string> Variables
        {
            get
            {
                if (this._materialized)
                {
                    return this._values.Keys;
                }
                else
                {
                    return (from s in this._sets
                            from v in s.Variables
                            select v).Distinct();
                }
            }
        }

        /// <summary>
        /// Joins the set to another set
        /// </summary>
        /// <param name="other">Other Set</param>
        /// <returns></returns>
        public override ISet Join(ISet other)
        {
            return new VirtualSet(other, this);
        }

        /// <summary>
        /// Copies the Set
        /// </summary>
        /// <returns></returns>
        public override ISet Copy()
        {
            return new Set(this);
            //return new VirtualSet(this);
        }

        /// <summary>
        /// Gets the Hash Code of the Set
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Gets the String representation of the Set
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            foreach (String v in this.Variables)
            {
                if (output.Length > 0) output.Append(" , ");
                output.Append("?" + v + " = " + this[v].ToSafeString());
            }
            return output.ToString();
        }

        /// <summary>
        /// Gets whether the Set is equal to another set
        /// </summary>
        /// <param name="other">Set to compare with</param>
        /// <returns></returns>
        public bool Equals(VirtualSet other)
        {
            return this.Equals((ISet)other);
        }
    }
}
