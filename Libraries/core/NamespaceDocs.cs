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

namespace VDS.Common
{
    /// <summary>
    /// Top Level Namespace for common data structures that are using throughout the dotNetRDF library but are not necessarily RDF specific
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF
{
    /// <summary>
    /// <para>
    /// Top Level Namespace for the <strong>dotNetRDF Library</strong> which embodies a simple but powerful API for manipulating RDF.   
    /// </para>
    /// <para>
    /// Specific Namespaces within the Hierarchy provide <see cref="VDS.RDF.Parsing">Parsing</see> and <see cref="VDS.RDF.Writing">Serialization</see> functionality along with a host of related classes to support these functions.
    /// </para>
    /// <para>
    /// Support for querying RDF is provided in the <see cref="VDS.RDF.Query">Query</see> namespace which includes SPARQL Query, limited reasoning support in the <see cref="VDS.RDF.Query.Inference">Query.Inference</see> namespace and a Pellet Server client in the <see cref="VDS.RDF.Query.Inference.Pellet">Query.Inference.Pellet</see> namespace.
    /// </para>
    /// <para>
    /// Support for updating RDF based on the SPARQL 1.1 Update and Graph Store HTTP Protocol for RDF Graph Management is provided in the <see cref="VDS.RDF.Update">Update</see> and <see cref="VDS.RDF.Update.Protocol">Update.Protocol</see> namespaces.
    /// </para>
    /// <h3>Third Party Storage</h3>
    /// <para>For communicating with arbitrary Triple Stores we have a dedicated <see cref="VDS.RDF.Storage">Storage</see> namespace.  As of this release we support the following Triple Stores:
    /// <ul>
    ///     <li>AllegroGraph</li>
    ///     <li>Dydra</li>
    ///     <li>4store</li>
    ///     <li>Fuseki</li>
    ///     <li>Joseki</li>
    ///     <li>Any Sesame HTTP Protocol compliant store e.g. Sesame, OWLIM</li>
    ///     <li>Any SPARQL Graph Store HTTP Protocol for RDF Graph Management compliant stores</li>
    ///     <li>Stardog</li>
    ///     <li>Virtuoso</li>
    /// </ul>
    /// </para>
    /// <h4>SQL Storage</h4>
    /// <para>
    /// <strong>Warning:</strong> The SQL Storage feature is now deprecated and no longer included in the release, please transition to one of the aforementioned 3rd party triple stores which are far more performant and all use the same <see cref="IStorageProvider">IStorageProvider</see> interface.  Do not use this support for any new development
    /// </para>
    /// <h3>ASP.Net Integration</h3>
    /// <para>
    /// For those building ASP.Net based websites the <see cref="VDS.RDF.Web">Web</see> namespace is dedicated to providing classes for integrating RDF into ASP.Net applications.
    /// </para>
    /// <h3>Ontology API</h3>
    /// <para>
    /// There is also an <see cref="VDS.RDF.Ontology">Ontology</see> namespace which provides a more resource and ontology centric API for working with RDF than the standard Graph and Triple centric APIs
    /// </para>
    /// <h3>Configuration API</h3>
    /// <para>
    /// We provide a <see cref="Configuration">Configuration</see> API which provides for encoding configuration in RDF Graphs.  This configuration system is used extensively as part of the ASP.Net support as it allows for much more expressive and flexible configurations than were previously possible.  See the <a href="http://www.dotnetrdf.org/content.asp?pageID=Configuration%20API">documentation</a> on the main website for many detailed examples.  This is primarily intended as an easy way to help deploy configurations for ASP.Net applications though you can make use of the API to describe the configuration of various types of objects in other applications, for example we use it in our Store Manager utility to store connection details.
    /// </para>
    /// <h3>Notes</h3>
    /// <para>
    /// dotNetRDF is now in Beta, this means it should be relatively stable for production scenarios but that the API is subject to changes in subsequent releases should we feel it necessary.  As it is a Beta release users should be aware that the software will not be bug free.  While we continually work to improve the quality of this library and to eliminate bugs as we find them we are at the same time attempting to enhance the library by adding more functionality so it is inevitable that some bugs will persist.  Please help us improve this library by emailing us when you find a bug, you can use the <a href="mailto:dotnetrdf-bugs@lists.sourceforge.net">Bug Reports list</a> to report bugs, the <a href="mailto:dotnetrdf-support@lists.sourceforge.net">Support list</a> to ask questions and the <a href="mailto:dotnetrdf-develop@lists.sourceforge.net">Developer list</a> to request new features or discuss development plans (all these are SourceForge mailing lists which require subscription).
    /// </para>
    /// <para>
    /// Be aware that the SPARQL support <em>in particular</em> represents our efforts to match the latest editors drafts of the SPARQL 1.1 specifications.  These specifications are changing all the time and the SPARQL support in this release will not necessarily reflect the very latest features at your time of reading until SPARQL 1.1 becomes fully standardised.
    /// </para>
    /// <h4>Breaking Changes</h4>
    /// <h5>0.7.x vs 0.6.x API</h5>
    /// <para>
    /// The 0.7.x release makes some substantial changes to the API though we have tried to make these backwards compatible as far as possible.  The key breaking changes are the removal of several obsoleted APIs around selection and node collections, rather than providing an explicit node collection graphs now simply allow you to obtain an enumerable over the Nodes of the Graph.
    /// </para>
    /// <para>
    /// The more noticeable but mostly backwards compatible change is the reorganisation of the <see cref="VDS.RDF.Storage"/> namespace, the interfaces in that namespace have been renamed to have more descriptive and consistent naming and we have introduced new interfaces for asynchronous access to storage.  The old interfaces remain as marker interfaces which inherit from the new interfaces so existing code will not need to be changed but will result in compiler warnings reminding you to upgrade to the new interface names.  The following table summarizes the old and new interface names:
    /// </para>
    /// <table class="dtTABLE" cellspacing="0">
    ///     <tr><th>Old Name</th><th>New Name</th></tr>
    ///     <tr><td><see cref="IGenericIOManager"/></td><td><see cref="IStorageProvider"/></td></tr>
    ///     <tr><td><see cref="IQueryableGenericIOManager"/></td><td><see cref="IQueryableStorage"/></td></tr>
    ///     <tr><td><see cref="IUpdateableGenericIOManager"/></td><td><see cref="IUpdateableStorage"/></td></tr>
    ///     <tr><td><see cref="IMultiStoreGenericIOManager"/></td><td><see cref="IStorageServer"/></td></tr>
    /// </table>
    /// <para>
    /// There are also some improvements to the <see cref="IStoreReader"/> and <see cref="IStoreWriter"/> interfaces such that they can now be used to read/write directly from files and streams.  The old <see cref="IStorageParams"/> APIs are marked as obsolete and will be removed in future releases, please update your code to use the newer methods which are more user friendly.
    /// </para>
    /// <h5>0.6.x vs 0.5.x API</h5>
    /// <para>
    /// The 0.6.x release has limited breaking changes and these are primarily in the internals of the SPARQL engine and so should only affect advanced users of the API.  Specifically the <see cref="VDS.RDF.Query.Dataset.ISparqlDataset"/> interface was updated and the <see cref="VDS.RDF.Query.Expressions.ISparqlExpression"/> interface refactored in terms of the new <see cref="IValuedNode"/> interface.  A variety of methods and classes previously marked obsolete are not either removed or marked obsolete unusable.
    /// </para>
    /// <h4>Alternative Builds</h4>
    /// <h5>Mono Build</h5>
    /// <para>
    /// There is no separate build for Mono since dotNetRDF can run directly under Mono.  Note that there may still be some features of .Net we use that Mono does not fully support, see the <a href="http://www.dotnetrdf.org/content.asp?pageID=Mono%20Issues">Mono Issues</a> page for more details.  We recommend Mono 2.10 or higher though the library should run on recent 2.6/2.8 releases.
    /// </para>
    /// <h5>Client Profile Build</h5>
    /// <para>
    /// The Client Profile build omits the reference to <see cref="System.Web">System.Web</see> so lacks the ASP.Net integration and some other features that rely on this dependency but is otherwise a fairly complete build of the library.
    /// </para>
    /// <h5>Silverlight/Windows Phone 7 Build</h5>
    /// <para>
    /// The Silverlight and Windows Phone 7 builds of dotNetRDF (<em>dotNetRDF.Silverlight.dll</em> and <em>dotNetRDF.WindowsPhone.dll</em>) are experimental builds that receive limited internal testing so please be aware that these are not as stable as the standard .Net builds.  These build runs on Silverlight 4/Windows Phone 7 and omits the following features since they can't be supported on these platforms:
    /// </para>
    /// <ul>
    ///     <li>Most of the <see cref="VDS.RDF.Web">Web</see> namespaces</li>
    ///     <li>Does not include parts of the <see cref="VDS.RDF.Storage">Storage</see> namespace that would require synchronous HTTP</li>
    ///     <li>No String normalization support</li>
    ///     <li>No <see cref="VDS.RDF.Parsing.UriLoader">UriLoader</see> caching support</li>
    ///     <li>No multi-threaded support where <see cref="System.Threading.ReaderWriteLockSlim">ReaderWriterLockSlim</see> is used</li>
    ///     <li>Various writers and parsers use streaming rather than DOM based XML parsing</li>
    ///     <li>No support for XSL in TriX files</li>
    ///     <li>Synchronous HTTP Request Features - For most of these there are asynchronous callback driven versions of these features available from the 0.5.0 release onwards</li>
    /// </ul>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Configuration
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration Classes which are used for dynamic loading of Configuration serialized as RDF Graphs.
    /// </para>
    /// <para>
    /// This API which provides for encoding dotNetRDF centric configuration in RDF Graphs though it can be extended to serialize and deserialize arbitrary objects if desired.  This configuration API is used extensively with our ASP.Net support as it allows for highly expressive and flexible configurations.  See the <a href="http://www.dotnetrdf.org/content.asp?pageID=Configuration%20API">documentation</a> on the main website for many detailed examples.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Configuration.Permissions
{
    /// <summary>
    /// <para>
    /// Namespace for classes related to configuring Permissions
    /// </para>
    /// <para>
    /// <strong>Warning:</strong> The API here is experimental and may changed/be removed in future releases
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// Namespace for specialised node implementations and the <see cref="IValuedNode"/> interface, these implementations are primarily used internally in the SPARQL engine.  These all derive from the standard Node implementations so can be used interchangeably with those if desired.
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Ontology
{
    /// <summary>
    /// <para>
    /// The Ontology Namespace is based upon <a href="http://jena.sourceforge.net/ontology/">Jena's Ontology API</a> and is an experimental part of the library.  It allows for a more resource-centric way of manipulating RDF graphs within the dotNetRDF API.
    /// </para>
    /// <para>
    /// The <see cref="OntologyResource">OntologyResource</see> is the base class of resources and allows for the retrieval and manipulation of various common properties of a resource.  More specialised classes like <see cref="OntologyClass">OntologyClass</see> and <see cref="OntologyProperty">OntologyProperty</see> are used to work with classes and properties etc.
    /// </para>
    /// <para>
    /// One key feature of this part of the API is the <see cref="ReasonerGraph">ReasonerGraph</see> which allows you to wrap an existing Graph with a reasoner to get a unified view over the original Triples and materialised inferences without modifying your original Graph.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Parsing
{
    /// <summary>
    /// <para>
    /// Namespace for Parsing Classes and variety of supporting Classes.
    /// </para>
    /// <para>
    /// Classes here are primarily implementations of <see cref="ITokeniser">ITokeniser</see> and <see cref="IRdfReader">IRdfReader</see> with some implementations of <see cref="IStoreReader">IStoreReader</see> and a few other specialised classes.
    /// </para>
    /// <para>
    /// Has child namespaces <see cref="VDS.RDF.Parsing.Events">Events</see> and <see cref="VDS.RDF.Parsing.Tokens">Tokens</see> for supporting Event and Token based Parsing.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Parsing.Contexts
{
    /// <summary>
    /// <para>
    /// Namespace for Parser Context classes, these are classes that are used internally by parsers to store their state.  This allows parsers to be safely used in a multi-threaded scenario so the parsing of one Graph/Store cannot affect the parsing of another.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Parsing.Events
{
    /// <summary>
    /// Namespace for Event classes which are used to support Event Based parsing of RDF syntaxes
    /// </summary>
    class NamespaceDoc
    {
       
    }
}

namespace VDS.RDF.Parsing.Handlers
{
    /// <summary>
    /// <para>
    /// Namespace for RDF and SPARQL Results Handlers
    /// </para>
    /// <para>
    /// Handlers are part of a major parser subsystem rewrite introduced in the 0.4.1 release.  They allow you to parse RDF, RDF Datasets and SPARQL Results in such a way that you can both take arbitrary actions with the data and choose to end parsing early.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Parsing.Tokens
{
    /// <summary>
    /// <para>
    /// Namespace for Token classes which are used to support Token Based parsing of RDF syntaxes
    /// </para>
    /// </summary>
    class NamespaceDoc
    {
        
    }
}

namespace VDS.RDF.Parsing.Validation
{
    /// <summary>
    /// <para>
    /// Namespace for Validator classes that can be used to validate various forms of syntax
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query
{
    /// <summary>
    /// <para>
    /// Namespace for Query Classes which provide Querying capabilities on RDF Graphs
    /// </para>
    /// <para>
    /// Query capabilities are centered around support for the SPARQL standard.  You can execute full SPARQL 1.1 queries over in-memory data or submit queries to remote SPARQL endpoints.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Aggregates
{
    /// <summary>
    /// <para>
    /// Namespace for Aggregate classes which implement Aggregate functions for SPARQL
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Aggregates.Leviathan
{
    /// <summary>
    /// Namespace for aggregates provided by the Leviathan function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Aggregates.Sparql
{
    /// <summary>
    /// Namespace for the built-in SPARQL aggregates
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Aggregates.XPath
{
    /// <summary>
    /// Namespace for aggregates provided by the XPath function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Algebra
{
    /// <summary>
    /// <para>
    /// Contains the classes which model the mapping of SPARQL queries into the SPARQL Algebra.  This namespace is a key component of the new <strong>Leviathan</strong> SPARQL engine introduced in the 0.2.x builds of dotNetRDF
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Construct
{
    /// <summary>
    /// <para>
    /// Namespace for classes used in executing CONSTRUCT queries
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }

}

namespace VDS.RDF.Query.Datasets
{
    /// <summary>
    /// <para>
    /// Namespace for classes used to define a Dataset over which SPARQL Queries and Updates evaluated using the Leviathan engine operate
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Describe
{
    /// <summary>
    /// <para>
    /// Namespace for classes which implement algorithms for executing DESCRIBE queries
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions
{
    /// <summary>
    /// <para>
    /// Namespace containing all the classes related to the execution of expressions in SPARQL queries.  Any valid expression should be able to be modelled and executed using these clases.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Arithmetic
{
    /// <summary>
    /// Namespace containing expression classes pertaining to arithmetic operations
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Comparison
{
    /// <summary>
    /// Namespace containing expression classes pertaining to comparison operations
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Conditional
{
    /// <summary>
    /// Namespace containing expression classes pertaining to conditional operations
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions
{
    /// <summary>
    /// <para>
    /// Namespace containing expression classes which model functions in SPARQL expressions
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Arq
{
    /// <summary>
    /// Namespace containing expression classes which provide the ARQ function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Leviathan
{
    /// <summary>
    /// Namespace containing expression classes which provide the Leviathan function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Leviathan.Hash
{
    /// <summary>
    /// Namespace containing expression classes which provide the hash functions from the Leviathan function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Leviathan.Numeric
{
    /// <summary>
    /// Namespace containing expression classes which provide the numeric functions from the Leviathan function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Leviathan.Numeric.Trigonometry
{
    /// <summary>
    /// Namespace containing expression classes which provide the trigonometric functions from the Leviathan function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Boolean
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions which have boolean results
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Constructor
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions which construct new terms
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.DateTime
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions pertaining to date times
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Hash
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions pertaining to hash algorithms
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Numeric
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in numeric functions
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.Set
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions pertaining to sets (IN and NOT IN)
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.Sparql.String
{
    /// <summary>
    /// Namespace containing expression classes which provide the SPARQL built-in functions pertaining to string manipulation
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.XPath
{
    /// <summary>
    /// Namespace containing expression classes which provide functions from the XPath function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.XPath.Cast
{
    /// <summary>
    /// Namespace containing expression classes which provide cast functions from the XPath function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.XPath.DateTime
{
    /// <summary>
    /// Namespace containing expression classes which provide date time functions from the XPath function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.XPath.Numeric
{
    /// <summary>
    /// Namespace containing expression classes which provide numeric functions from the XPath function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Functions.XPath.String
{
    /// <summary>
    /// Namespace containing expression classes which provide string functions from the XPath function library
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Expressions.Primary
{
    /// <summary>
    /// Namespace containing expression classes representing primary constructs in SPARQL expression trees i.e. constants, modifiers and variables
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Filters
{
    /// <summary>
    /// <para>
    /// Namespace containing classes pertaining to the filtering of the results of SPARQL queries
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Grouping
{
    /// <summary>
    /// <para>
    /// Namespace containing classes used to apply GROUP BY clauses to SPARQL queries
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Inference
{
    /// <summary>
    /// <para>
    /// Namespace for Inference Classes which provide Inferencing capabilities on RDF - these features are currently experimental and may not work as expected.
    /// </para>
    /// <para>
    /// Classes which implement reasoning must implement the <see cref="IInferenceEngine">IInferenceEngine</see> interface, these can then be attached to classes which implement the <see cref="IInferencingTripleStore">IInferencingTripleStore</see> interface or they can be used to apply inference to any <see cref="IGraph">IGraph</see> implementation with the inferred Triples optionally output to a separate Graph.
    /// </para>
    /// <para>
    /// OWL reasoning currently has extremely limited support, we provide a Pellet client in the <see cref="Pellet">Pellet</see> namespace which can be used to connect to a Pellet Server but that currently only provides reasoning on external knowledge bases on the Pellet Server
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Inference.Pellet
{
    /// <summary>
    /// <para>
    /// Namespace which provides a client for interacting with a Pellet Server
    /// </para>
    /// <para>
    /// Due to Pellet Server being a relatively new product it is currently only possible to reason over external knowledge bases on a Pellet Server and not to use Pellet to reason over in-memory data.  As Pellet Server is updated in the future this client will be updated to take advantage of those updates and to eventually provide for in-memory reasoning.  You may also want to consider using the <see cref="StardogConnector"/> which is the triple store from the same people who developed Pellet and which integrates some Pellet capabilities.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Inference.Pellet.Services
{
    /// <summary>
    /// <para>
    /// Namespace which provides classes which represent the Services offered by a Pellet Server knowledge base
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Operators
{
    /// <summary>
    /// <para>
    /// Namespace which provides classes which represent the implementation of various operators in SPARQL.  This allows for some of the basic operators like + and - to be extended to allow functionality beyond the SPARQL specification such as date time arithmetic.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Operators.DateTime
{
    /// <summary>
    /// <para>
    /// Namespace which provides implementations of <see cref="ISparqlOperator"/> which allow for embedding date time arithmetic into SPARQL queries
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Operators.Numeric
{
    /// <summary>
    /// <para>
    /// Namespace which provides implementations of <see cref="ISparqlOperator"/> which provide the default numeric implementations of operators as required by the SPARQL specification
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Optimisation
{
    /// <summary>
    /// <para>
    /// Namespace containing classes that are used in the Optimisation of SPARQL Queries.  Includes the interfaces <see cref="IQueryOptimiser">IQueryOptimiser</see> and <see cref="IAlgebraOptimiser">IAlgebraOptimiser</see> which can be used to implement custom query optimisation. 
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Ordering
{
    /// <summary>
    /// <para>
    /// Namespace containing classes used to order the results of SPARQL queries
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Paths
{
    /// <summary>
    /// <para>
    /// Contains the classes which model property paths in SPARQL, they can be used to both represent and evaluate a property path as part of a SPARQL query.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.Patterns
{
    /// <summary>
    /// Namespace for Pattern Classes that are used in the Graph and Triple matching process for executing SPARQL queries on <see cref="IInMemoryQueryableStore">IInMemoryQueryableStore</see> objects
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Query.PropertyFunctions
{
    /// <summary>
    /// Namespace which provide classes relating to the property function extension point of SPARQL
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Storage
{
    /// <summary>
    /// <para>
    /// Namespace for ctorage classes which provide support for using arbitrary backing Stores
    /// </para>
    /// <para>
    /// Storage is managed via the <see cref="IStorageProvider">IStorageProvider</see> interface, see the <a href="http://www.dotnetrdf.org/content.asp?pageID=Triple%20Store%20Integration">Triple Store Integration</a> documentation on the main website for more detail.
    /// </para>
    /// <para>
    /// Note that this is the new name for the old <see cref="IGenericIOManager"/> interface, the following table summarizes name changes to the interfaces in this API which were made in the 0.7.x releases.  Note that in the 0.7.x initial release the old interface names were preserved as marker interfaces to enable backwards compatibility with existing code.
    /// </para>
    /// <table class="dtTABLE" cellspacing="0">
    ///     <tr><th>Old Name</th><th>New Name</th></tr>
    ///     <tr><td><see cref="IGenericIOManager"/></td><td><see cref="IStorageProvider"/></td></tr>
    ///     <tr><td><see cref="IQueryableGenericIOManager"/></td><td><see cref="IQueryableStorage"/></td></tr>
    ///     <tr><td><see cref="IUpdateableGenericIOManager"/></td><td><see cref="IUpdateableStorage"/></td></tr>
    ///     <tr><td><see cref="IMultiStoreGenericIOManager"/></td><td><see cref="IStorageServer"/></td></tr>
    /// </table>
    /// <h3>Data Provider Libraries</h3>
    /// <para>
    /// From the 0.5.0 release onwards any triple store integration that requires additional dependencies are provided with their own library to reduce dependencies in the Core library and allow that functionality to be optional.  The following stores are currently provided in separate libraries:
    /// </para>
    /// <ul>
    ///     <li>Virtuoso - Virtuoso support can be found in the <strong>dotNetRDF.Data.Virtuoso.dll</strong> library and requires one additional dependency.</li>
    /// </ul>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Storage.Management
{
    /// <summary>
    /// <para>
    /// Namespace for storage classes which provide support for managing servers that provide multiple backing Stores
    /// </para>
    /// <para>
    /// Servers are managed via the <see cref="IStorageServer"/> interface, a server can provide lists of available stores, retrieve a reference to a store, create new stores and delete existing stores.  The exact capabilites may depend on the implementation and may be inspected via the <see cref="IStorageServer.IOBehaviour"/> property.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Storage.Management.Provisioning
{
    /// <summary>
    /// <para>
    /// Namespace for storage classes which provide support for creating new stores in conjunction with a <see cref="IStorageServer"/>
    /// </para>
    /// <para>
    /// In order for an <see cref="IStorageServer"/> to create a new store it requires an instance of the <see cref="IStoreTemplate"/> interface from this namespace.  The basic interface provides only a Store ID, specific implementations may provide many more customizable properties to allow new stores to be created that take advantage of the capabilties of the server the store is being created on.  A <see cref="IStorageServer"/> provides methods to generate the basic templates that it accepts and should be used in preference to creating any of the implementations directly.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Storage.Management.Provisioning.Sesame
{
    /// <summary>
    /// <para>
    /// Namespace containing implementations of <see cref="IStorageTemplate"/> which provide templates for creating new stores on Sesame servers
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Storage.Management.Provisioning.Stardog
{
    /// <summary>
    /// <para>
    /// Namespace containing implementations of <see cref="IStorageTemplate"/> which provide templates for creating new stores on Stardog servers
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Update
{
    /// <summary>
    /// <para>
    /// Namespace for performing updates on Triple Stores using SPARQL Update
    /// </para>
    /// <para>
    /// This is a new part of the API introduced in the 0.3.0 release and adds support for using SPARQL to update Triple Stores.  SPARQL Update is part of the new SPARQL 1.1 standard and provides syntax for inserting, modifying and deleting data as well as managing graphs in a store.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Update.Commands
{
    /// <summary>
    /// <para>
    /// Namespace containing classes which model SPARQL Update Commands.  These can be used both to represent SPARQL Updates and to execute them over in-memory stores.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Update.Protocol
{
    /// <summary>
    /// <para>
    /// Namespaces containing classes which implement the SPARQL Graph Store HTTP Protocol for RDF Graph Management
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web
{
    /// <summary>
    /// <para>
    /// Namespace for Classes designed to aid the deployment of Linked Data, SPARQL Endpoints and other Semantic Web technologies as part of ASP.Net web applications.
    /// </para>
    /// <para>
    /// The ASP.Net support leverages the <see cref="VDS.RDF.Configuration">Configuration API</see> heavily and so only requires only 1 &lt;appSetting&gt; like so:
    /// <code>
    /// &lt;add key="dotNetRDFConfig" value="~/App_Data/config.ttl" /&gt;
    /// </code>
    /// This setting provides a pointer to an RDF configuration graph that uses the <a href="http://www.dotnetrdf.org/configuration#">Configuration Vocabulary</a> to express the configuration of HTTP Handlers for your ASP.Net application.  We also now provide a command line tool <a href="http://www.dotnetrdf.org/content.asp?pageID=rdfWedDeploy">rdfWebDeploy</a> which can be used to automate the testing and deployment of this configuration.  See documentation on the <a href="http://www.dotnetrdf.org/content.asp?pageID=Configuration%20API">Configuration API</a> for more detail.  Individual handler documentation gives basic examples of Handler configurations.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web.Configuration
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration classes which are used to load and store the configuration settings for HTTP Handlers provided as part of the <strong>Web</strong> namespace.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web.Configuration.Protocol
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration classes which are used to load and store the configuration settings for SPARQL Graph Store HTTP Protocol
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web.Configuration.Query
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration classes which are used to load and store the configuration settings for SPARQL Query handlers
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web.Configuration.Resource
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration classes which are used to load and store the configuration settings for handlers which serve resources such as Graphs
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web.Configuration.Server
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration classes which are used to load and store the configuration settings for SPARQL Servers
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Web.Configuration.Update
{
    /// <summary>
    /// <para>
    /// Namespace for Configuration classes which are used to load and store the configuration settings for SPARQL Update handlers
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Writing
{
    /// <summary>
    /// <para>
    /// Namespace for Writing Classes which provide the means to Serialize RDF Graphs as concrete RDF syntaxes or graphical representations.
    /// </para>
    /// <para>
    /// Also contains classes that can be used to save Graphs and Triple Stores to arbitrary database backed storage using classes from the <see cref="VDS.RDF.Storage">Storage</see> namespace.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Writing.Contexts
{
    /// <summary>
    /// <para>
    /// Namespace for Writer Context classes, these are classes that are used internally by writers to store their state.  This allows writers to be safely used in a multi-threaded scenario since the writing of one Graph/Store cannot affect the writing of another.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Writing.Formatting
{
    /// <summary>
    /// <para>
    /// Namespace for Formatter Classes which can be used to format <see cref="VDS.RDF.Triple">Triples</see>, <see cref="VDS.RDF.INode">Nodes</see> and <see cref="System.Uri">URIs</see> among other types.
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

namespace VDS.RDF.Writing.Serialization
{
    /// <summary>
    /// <para>
    /// Namespace for classes related to .Net serialization integration in the library
    /// </para>
    /// </summary>
    class NamespaceDoc
    {

    }
}

