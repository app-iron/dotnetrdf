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
using System.ComponentModel;
using System.Linq;
using System.Text;
using VDS.RDF.Configuration;
using VDS.RDF.Storage;

namespace VDS.RDF.Utilities.StoreManager.Connections.BuiltIn
{
    /// <summary>
    /// Definition for connections to Joseki
    /// </summary>
    [Obsolete("The Apache Jena project strongly recommends using Fuseki instead which is it's sucessor, Joseki is no longer supported by Apache Jena", true)]
    public class JosekiConnectionDefinition
        : BaseHttpConnectionDefinition
    {
        /// <summary>
        /// Creates a new Definition
        /// </summary>
        public JosekiConnectionDefinition()
            : base("Joseki", "Connect to a Joseki Server which exposes SPARQL based access to any Jena based stores e.g. SDB and TDB.", typeof(JosekiConnector)) { }

        /// <summary>
        /// Gets/Sets the Server URI
        /// </summary>
        [Connection(DisplayName = "Server URI", IsRequired = true, Type = ConnectionSettingType.String, DisplayOrder = -1, PopulateFrom = ConfigurationLoader.PropertyServer),
DefaultValue("http://localhost:2020")]
        public String Server
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the path to the Query endpoint
        /// </summary>
        [Connection(DisplayName = "Query Path", IsRequired = true, AllowEmptyString = false, PopulateFrom = ConfigurationLoader.PropertyQueryPath),
         DefaultValue("sparql")]
        public String QueryPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the path to the Update endpoint
        /// </summary>
        [Connection(DisplayName = "Update Path", IsRequired = false, AllowEmptyString = true, DisplaySuffix = "(Leave blank for read-only connection)", PopulateFrom = ConfigurationLoader.PropertyUpdatePath),
         DefaultValue("update")]
        public String UpdatePath
        {
            get;
            set;
        }

        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <returns></returns>
        protected override IStorageProvider OpenConnectionInternal()
        {
            if (this.UseProxy)
            {
                return new JosekiConnector(this.Server, this.QueryPath, (String.IsNullOrEmpty(this.UpdatePath) ? null : this.UpdatePath), this.GetProxy());
            }
            else
            {
                return new JosekiConnector(this.Server, this.QueryPath, (String.IsNullOrEmpty(this.UpdatePath) ? null : this.UpdatePath));
            }
        }
    }
}
