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
using VDS.RDF.Query.Inference.Pellet;
using VDS.RDF.Query.Inference.Pellet.Services;

namespace VDS.RDF.Query
{
    /// <summary>
    /// A SPARQL Query Processor which processes queries by parsing them to the SPARQL Query Service of a Knowledge Base on a Pellet Server
    /// </summary>
    public class PelletQueryProcessor 
        : ISparqlQueryProcessor 
    {
        private Type _svcType = typeof(QueryService);
        private QueryService _svc;

        /// <summary>
        /// Creates a new Pellet Query Processor
        /// </summary>
        /// <param name="server">Pellet Server</param>
        /// <param name="kbName">Knowledge Base Name</param>
        public PelletQueryProcessor(PelletServer server, String kbName)
        {
            if (server.HasKnowledgeBase(kbName))
            {
                KnowledgeBase kb = server.GetKnowledgeBase(kbName);

                //Check SPARQL Query is supported
                if (kb.SupportsService(this._svcType))
                {
                    this._svc = (QueryService)kb.GetService(this._svcType);
                }
                else
                {
                    throw new NotSupportedException("Cannot create a Pellet Query Processor as the selected Knowledge Base does not support the Query Service");
                }
            }
            else
            {
                throw new NotSupportedException("Cannot create a Pellet Query Processor for the Knowledge Base named '" + kbName + "' as this Server does not have the named Knowledge Base");
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Creates a new Pellet Query Processor
        /// </summary>
        /// <param name="serverUri">Pellet Server URI</param>
        /// <param name="kbName">Knowledge Base Name</param>
        public PelletQueryProcessor(Uri serverUri, String kbName)
            : this(new PelletServer(serverUri), kbName) { }

#endif

        /// <summary>
        /// Processes a SPARQL Query
        /// </summary>
        /// <param name="query">SPARQL Query</param>
        /// <returns></returns>
        public object ProcessQuery(SparqlQuery query)
        {
#if !SILVERLIGHT
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                Object temp = this._svc.Query(query.ToString());
                return temp;
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = (DateTime.Now - start);
            }
#else
            throw new NotSupportedException("Synchronous remote query is not supported under Silverlight/WP7 - please use one of the alternative overload of this methods which takes a callback");
#endif
        }

        /// <summary>
        /// Processes a SPARQL Query passing the results to the RDF or Results handler as appropriate
        /// </summary>
        /// <param name="rdfHandler">RDF Handler</param>
        /// <param name="resultsHandler">Results Handler</param>
        /// <param name="query">SPARQL Query</param>
        public void ProcessQuery(IRdfHandler rdfHandler, ISparqlResultsHandler resultsHandler, SparqlQuery query)
        {
#if !SILVERLIGHT
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                this._svc.Query(rdfHandler, resultsHandler, query.ToString());
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = (DateTime.Now - start);
            }
#else
            throw new NotSupportedException("Synchronous remote query is not supported under Silverlight/WP7 - please use one of the alternative overload of this methods which takes a callback");
#endif
        }

        /// <summary>
        /// Processes a SPARQL Query asynchronously invoking the relevant callback when the query completes
        /// </summary>
        /// <param name="query">SPARQL QUery</param>
        /// <param name="rdfCallback">Callback for queries that return a Graph</param>
        /// <param name="resultsCallback">Callback for queries that return a Result Set</param>
        /// <param name="state">State to pass to the callback</param>
        public void ProcessQuery(SparqlQuery query, GraphCallback rdfCallback, SparqlResultsCallback resultsCallback, Object state)
        {
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                this._svc.Query(query.ToString(), rdfCallback, resultsCallback, state);
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = (DateTime.Now - start);
            }
        }

        /// <summary>
        /// Processes a SPARQL Query asynchronously passing the results to the relevant handler and invoking the callback when the query completes
        /// </summary>
        /// <param name="rdfHandler">RDF Handler</param>
        /// <param name="resultsHandler">Results Handler</param>
        /// <param name="query">SPARQL Query</param>
        /// <param name="callback">Callback</param>
        /// <param name="state">State to pass to the callback</param>
        public void ProcessQuery(IRdfHandler rdfHandler, ISparqlResultsHandler resultsHandler, SparqlQuery query, QueryCallback callback, Object state)
        {
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                this._svc.Query(rdfHandler, resultsHandler, query.ToString(), callback, state);
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = (DateTime.Now - start);
            }
        }
    }
}
