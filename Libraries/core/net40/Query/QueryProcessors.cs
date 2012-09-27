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
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Storage;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Query
{
    /// <summary>
    /// A SPARQL Query Processor where the query is processed by passing it to the <see cref="INativelyQueryableStore.ExecuteQuery">ExecuteQuery()</see> method of an <see cref="INativelyQueryableStore">INativelyQueryableStore</see>
    /// </summary>
    public class SimpleQueryProcessor 
        : ISparqlQueryProcessor
    {
        private INativelyQueryableStore _store;
        private SparqlFormatter _formatter = new SparqlFormatter();

        /// <summary>
        /// Creates a new Simple Query Processor
        /// </summary>
        /// <param name="store">Triple Store</param>
        public SimpleQueryProcessor(INativelyQueryableStore store)
        {
            this._store = store;
        }

        /// <summary>
        /// Processes a SPARQL Query
        /// </summary>
        /// <param name="query">SPARQL Query</param>
        /// <returns></returns>
        public object ProcessQuery(SparqlQuery query)
        {
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                Object temp = this._store.ExecuteQuery(this._formatter.Format(query));
                return temp;
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
            }
        }

        /// <summary>
        /// Processes a SPARQL Query passing the results to the RDF or Results handler as appropriate
        /// </summary>
        /// <param name="rdfHandler">RDF Handler</param>
        /// <param name="resultsHandler">Results Handler</param>
        /// <param name="query">SPARQL Query</param>
        public void ProcessQuery(IRdfHandler rdfHandler, ISparqlResultsHandler resultsHandler, SparqlQuery query)
        {
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                this._store.ExecuteQuery(rdfHandler, resultsHandler, this._formatter.Format(query));
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
            }
        }

        /// <summary>
        /// Delegate used for asychronous execution
        /// </summary>
        /// <param name="rdfHandler">RDF Handler</param>
        /// <param name="resultsHandler">Results Handler</param>
        /// <param name="query">SPARQL Query</param>
        private delegate void ProcessQueryAsync(IRdfHandler rdfHandler, ISparqlResultsHandler resultsHandler, SparqlQuery query);

        /// <summary>
        /// Processes a SPARQL Query asynchronously invoking the relevant callback when the query completes
        /// </summary>
        /// <param name="query">SPARQL QUery</param>
        /// <param name="rdfCallback">Callback for queries that return a Graph</param>
        /// <param name="resultsCallback">Callback for queries that return a Result Set</param>
        /// <param name="state">State to pass to the callback</param>
        public void ProcessQuery(SparqlQuery query, GraphCallback rdfCallback, SparqlResultsCallback resultsCallback, Object state)
        {
            Graph g = new Graph();
            SparqlResultSet rset = new SparqlResultSet();
            ProcessQueryAsync d = new ProcessQueryAsync(this.ProcessQuery);
            d.BeginInvoke(new GraphHandler(g), new ResultSetHandler(rset), query, r =>
                {
                    d.EndInvoke(r);
                    if (rset.ResultsType != SparqlResultsType.Unknown)
                    {
                        resultsCallback(rset, state);
                    }
                    else
                    {
                        rdfCallback(g, state);
                    }
                }, state);
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
            ProcessQueryAsync d = new ProcessQueryAsync(this.ProcessQuery);
            d.BeginInvoke(rdfHandler, resultsHandler, query, r =>
            {
                d.EndInvoke(r);
                callback(rdfHandler, resultsHandler, state);
            }, state);
        }
    }

    /// <summary>
    /// A SPARQL Query Processor where the query is processed by passing it to the <see cref="IQueryableStorage.Query">Query()</see> method of an <see cref="IQueryableStorage">IQueryableStorage</see>
    /// </summary>
    public class GenericQueryProcessor 
        : ISparqlQueryProcessor
    {
        private IQueryableStorage _manager;
        private SparqlFormatter _formatter = new SparqlFormatter();

        /// <summary>
        /// Creates a new Generic Query Processor
        /// </summary>
        /// <param name="manager">Generic IO Manager</param>
        public GenericQueryProcessor(IQueryableStorage manager)
        {
            this._manager = manager;
        }

        /// <summary>
        /// Processes a SPARQL Query
        /// </summary>
        /// <param name="query">SPARQL Query</param>
        /// <returns></returns>
        public object ProcessQuery(SparqlQuery query)
        {
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                Object temp = this._manager.Query(this._formatter.Format(query));
                return temp;
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
            }
        }

        /// <summary>
        /// Processes a SPARQL Query passing the results to the RDF or Results handler as appropriate
        /// </summary>
        /// <param name="rdfHandler">RDF Handler</param>
        /// <param name="resultsHandler">Results Handler</param>
        /// <param name="query">SPARQL Query</param>
        public void ProcessQuery(IRdfHandler rdfHandler, ISparqlResultsHandler resultsHandler, SparqlQuery query)
        {
            query.QueryExecutionTime = null;
            DateTime start = DateTime.Now;
            try
            {
                this._manager.Query(rdfHandler, resultsHandler, this._formatter.Format(query));
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
            }
        }

        /// <summary>
        /// Delegate used for asychronous execution
        /// </summary>
        /// <param name="rdfHandler">RDF Handler</param>
        /// <param name="resultsHandler">Results Handler</param>
        /// <param name="query">SPARQL Query</param>
        private delegate void ProcessQueryAsync(IRdfHandler rdfHandler, ISparqlResultsHandler resultsHandler, SparqlQuery query);

        /// <summary>
        /// Processes a SPARQL Query asynchronously invoking the relevant callback when the query completes
        /// </summary>
        /// <param name="query">SPARQL QUery</param>
        /// <param name="rdfCallback">Callback for queries that return a Graph</param>
        /// <param name="resultsCallback">Callback for queries that return a Result Set</param>
        /// <param name="state">State to pass to the callback</param>
        public void ProcessQuery(SparqlQuery query, GraphCallback rdfCallback, SparqlResultsCallback resultsCallback, Object state)
        {
            Graph g = new Graph();
            SparqlResultSet rset = new SparqlResultSet();
            ProcessQueryAsync d = new ProcessQueryAsync(this.ProcessQuery);
            d.BeginInvoke(new GraphHandler(g), new ResultSetHandler(rset), query, r =>
            {
                d.EndInvoke(r);
                if (rset.ResultsType != SparqlResultsType.Unknown)
                {
                    resultsCallback(rset, state);
                }
                else
                {
                    rdfCallback(g, state);
                }
            }, state);
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
            ProcessQueryAsync d = new ProcessQueryAsync(this.ProcessQuery);
            d.BeginInvoke(rdfHandler, resultsHandler, query, r =>
            {
                d.EndInvoke(r);
                callback(rdfHandler, resultsHandler, state);
            }, state);
        }
    }

    /// <summary>
    /// A SPARQL Query Processor where the query is processed by passing it to a remote SPARQL endpoint
    /// </summary>
    public class RemoteQueryProcessor 
        : ISparqlQueryProcessor
    {
        private SparqlRemoteEndpoint _endpoint;
        private SparqlFormatter _formatter = new SparqlFormatter();

        /// <summary>
        /// Creates a new Remote Query Processor
        /// </summary>
        /// <param name="endpoint">SPARQL Endpoint</param>
        public RemoteQueryProcessor(SparqlRemoteEndpoint endpoint)
        {
            this._endpoint = endpoint;
        }

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
            Object temp;
            try
            {
                switch (query.QueryType)
                {
                    case SparqlQueryType.Ask:
                    case SparqlQueryType.Select:
                    case SparqlQueryType.SelectAll:
                    case SparqlQueryType.SelectAllDistinct:
                    case SparqlQueryType.SelectAllReduced:
                    case SparqlQueryType.SelectDistinct:
                    case SparqlQueryType.SelectReduced:
                        temp = this._endpoint.QueryWithResultSet(this._formatter.Format(query));
                        break;
                    case SparqlQueryType.Construct:
                    case SparqlQueryType.Describe:
                    case SparqlQueryType.DescribeAll:
                        temp = this._endpoint.QueryWithResultGraph(this._formatter.Format(query));
                        break;
                    default:
                        throw new RdfQueryException("Unable to execute an unknown query type against a Remote Endpoint");
                }
                return temp;
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
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
                switch (query.QueryType)
                {
                    case SparqlQueryType.Ask:
                    case SparqlQueryType.Select:
                    case SparqlQueryType.SelectAll:
                    case SparqlQueryType.SelectAllDistinct:
                    case SparqlQueryType.SelectAllReduced:
                    case SparqlQueryType.SelectDistinct:
                    case SparqlQueryType.SelectReduced:
                        this._endpoint.QueryWithResultSet(resultsHandler, this._formatter.Format(query));
                        break;
                    case SparqlQueryType.Construct:
                    case SparqlQueryType.Describe:
                    case SparqlQueryType.DescribeAll:
                        this._endpoint.QueryWithResultGraph(rdfHandler, this._formatter.Format(query));
                        break;
                    default:
                        throw new RdfQueryException("Unable to execute an unknown query type against a Remote Endpoint");
                }
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
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
                switch (query.QueryType)
                {
                    case SparqlQueryType.Ask:
                    case SparqlQueryType.Select:
                    case SparqlQueryType.SelectAll:
                    case SparqlQueryType.SelectAllDistinct:
                    case SparqlQueryType.SelectAllReduced:
                    case SparqlQueryType.SelectDistinct:
                    case SparqlQueryType.SelectReduced:
                        this._endpoint.QueryWithResultSet(this._formatter.Format(query), resultsCallback, state);
                        break;
                    case SparqlQueryType.Construct:
                    case SparqlQueryType.Describe:
                    case SparqlQueryType.DescribeAll:
                        this._endpoint.QueryWithResultGraph(this._formatter.Format(query), rdfCallback, state);
                        break;
                    default:
                        throw new RdfQueryException("Unable to execute an unknown query type against a Remote Endpoint");
                }
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
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
                switch (query.QueryType)
                {
                    case SparqlQueryType.Ask:
                    case SparqlQueryType.Select:
                    case SparqlQueryType.SelectAll:
                    case SparqlQueryType.SelectAllDistinct:
                    case SparqlQueryType.SelectAllReduced:
                    case SparqlQueryType.SelectDistinct:
                    case SparqlQueryType.SelectReduced:
                        this._endpoint.QueryWithResultSet(resultsHandler, this._formatter.Format(query), callback, state);
                        break;
                    case SparqlQueryType.Construct:
                    case SparqlQueryType.Describe:
                    case SparqlQueryType.DescribeAll:
                        this._endpoint.QueryWithResultGraph(rdfHandler, this._formatter.Format(query), callback, state);
                        break;
                    default:
                        throw new RdfQueryException("Unable to execute an unknown query type against a Remote Endpoint");
                }
            }
            finally
            {
                TimeSpan elapsed = (DateTime.Now - start);
                query.QueryExecutionTime = elapsed;
            }
        }
    }
}
