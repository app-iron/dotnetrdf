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
#if !WINDOWS_PHONE
using System.ComponentModel;
#if SILVERLIGHT
using System.ComponentModel.DataAnnotations;
#endif
#endif
using System.Linq;
using System.Text;

namespace VDS.RDF.Storage.Management.Provisioning.Sesame
{
    /// <summary>
    /// Template for creating Sesame memory stores
    /// </summary>
    /// <remarks>
    /// <para>
    /// This template generates a Sesame repository config graph like the following, depending on exact options the graph may differ:
    /// </para>
    /// <pre>
    /// @prefix rdfs: &lt;http://www.w3.org/2000/01/rdf-schema#&gt;.
    /// @prefix rep: &lt;http://www.openrdf.org/config/repository#&gt;.
    /// @prefix sr: &lt;http://www.openrdf.org/config/repository/sail#&gt;.
    /// @prefix sail: &lt;http://www.openrdf.org/config/sail#&gt;.
    /// @prefix ms: &lt;http://www.openrdf.org/config/sail/memory#&gt;.
    /// 
    /// [] a rep:Repository ;
    /// rep:repositoryID "{this.ID}" ;
    /// rdfs:label "{this.Label}" ;    
    ///    rep:repositoryImpl [
    ///       rep:repositoryType "openrdf:SailRepository" ;
    ///       sr:sailImpl [
    ///          sail:sailType "openrdf:MemoryStore" ;
    ///          ms:persist {this.Persist} ;
    ///          ms:syncDelay {this.SyncDelay}
    ///       ]
    ///   ].
    /// </pre>
    /// <para>
    /// The placeholders of the form <strong>{this.Property}</strong> represent properties of this class whose values will be inserted into the repository config graph and used to create a new store in Sesame.
    /// </para>
    /// </remarks>
    public class SesameMemTemplate
        : BaseSesameTemplate
    {
        /// <summary>
        /// Creates a new memory store template
        /// </summary>
        /// <param name="id">Store ID</param>
        public SesameMemTemplate(String id)
            : base(id, "Sesame Memory", "A Sesame memory store is stored fully in-memory and may be persisted to/from disk") 
        {
            this.Persist = true;
            this.SyncDelay = 0;
        }

        /// <summary>
        /// Gets the template graph used to create the store
        /// </summary>
        /// <returns></returns>
        public override IGraph GetTemplateGraph()
        {
            IGraph g = this.GetBaseTemplateGraph();
            INode impl = g.CreateBlankNode();
            g.Assert(this.ContextNode, g.CreateUriNode("rep:repositoryImpl"), impl);
            g.Assert(impl, g.CreateUriNode("rep:repositoryType"), g.CreateLiteralNode("openrdf:SailRepository"));
            INode sailImpl = g.CreateBlankNode();
            g.Assert(impl, g.CreateUriNode("sr:sailImpl"), sailImpl);

            if (this.DirectTypeHierarchyInferencing)
            {
                INode sailDelegate = g.CreateBlankNode();
                g.Assert(sailImpl, g.CreateUriNode("sail:sailType"), g.CreateLiteralNode("openrdf:DirectTypeHierarchyInferencer"));
                g.Assert(sailImpl, g.CreateUriNode("sail:delegate"), sailDelegate);
                sailImpl = sailDelegate;
            }
            if (this.RdfSchemaInferencing)
            {
                INode sailDelegate = g.CreateBlankNode();
                g.Assert(sailImpl, g.CreateUriNode("sail:sailType"), g.CreateLiteralNode("openrdf:ForwardChainingRDFSInferencer"));
                g.Assert(sailImpl, g.CreateUriNode("sail:delegate"), sailDelegate);
                sailImpl = sailDelegate;
            }
            g.Assert(sailImpl, g.CreateUriNode("sail:sailType"), g.CreateLiteralNode("openrdf:MemoryStore"));
            g.Assert(sailImpl, g.CreateUriNode("ms:persist"), this.Persist.ToLiteral(g));
            g.Assert(sailImpl, g.CreateUriNode("ms:syncDelay"), this.SyncDelay.ToLiteral(g));

            return g;
        }

        /// <summary>
        /// Gets/Sets whether to persist the store
        /// </summary>
#if !WINDOWS_PHONE
        [Category("Sesame Configuration"), Description("Whether the store is persisted"), DefaultValue(true)]
#endif
        public bool Persist
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the sync delay
        /// </summary>
#if !WINDOWS_PHONE
#if !SILVERLIGHT
        [Category("Sesame Configuration"), DisplayName("Sync Delay"), Description("Sets the sync delay for the store"), DefaultValue(0)]
#else
        [Category("Sesame Configuration"), Display(Name = "Sync Delay"), Description("Sets the sync delay for the store"), DefaultValue(0)]
#endif
#endif
        public int SyncDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets whether to enable direct type hierarchy inferencing
        /// </summary>
#if !WINDOWS_PHONE
#if !SILVERLIGHT
        [Category("Sesame Reasoning"), DisplayName("Direct Type Hierarchy Inference"), Description("Enables/Disables Direct Type Hierarchy Inference"), DefaultValue(false)]
#else
        [Category("Sesame Reasoning"), Display(Name = "Direct Type Hierarchy Inference"), Description("Enables/Disables Direct Type Hierarchy Inference"), DefaultValue(false)]
#endif
#endif
        public bool DirectTypeHierarchyInferencing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets whether to enable RDF Schema Inferencing
        /// </summary>
#if !WINDOWS_PHONE
#if !SILVERLIGHT
        [Category("Sesame Reasoning"), DisplayName("RDF Schema Inference"), Description("Enables/Disables RDF Schema inferencing"), DefaultValue(false)]
#else
        [Category("Sesame Reasoning"), Display(Name = "RDF Schema Inference"), Description("Enables/Disables RDF Schema inferencing"), DefaultValue(false)]
#endif
#endif
        public bool RdfSchemaInferencing
        {
            get;
            set;
        }
    }
}
