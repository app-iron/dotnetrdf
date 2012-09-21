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
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDS.RDF.Configuration;
using VDS.RDF.Parsing;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Test.Parsing
{
    [TestClass]
    public class LoaderTests
    {
        [TestMethod]
        public void ParsingDataUri()
        {
            String rdfFragment = "@prefix : <http://example.org/> . :subject :predicate :object .";
            String rdfBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(rdfFragment));
            String rdfAscii = Uri.EscapeDataString(rdfFragment);
            List<String> uris = new List<string>()
            {
                "data:text/turtle;charset=UTF-8;base64," + rdfBase64,
                "data:text/turtle;base64," + rdfBase64,
                "data:;base64," + rdfBase64,
                "data:text/turtle;charset=UTF-8," + rdfAscii,
                "data:text/tutle," + rdfAscii,
                "data:," + rdfAscii
            };

            foreach (String uri in uris)
            {
                Uri u = new Uri(uri);

                Console.WriteLine("Testing URI " + u.AbsoluteUri);

                Graph g = new Graph();
                UriLoader.Load(g, u);

                Assert.AreEqual(1, g.Triples.Count, "Expected 1 Triple to be produced");

                Console.WriteLine("Triples produced:");
                foreach (Triple t in g.Triples)
                {
                    Console.WriteLine(t.ToString());
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void ParsingDBPedia()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://dbpedia.org/resource/London");
            request.Accept = "application/rdf+xml";
            request.Method = "GET";
            request.Timeout = 45000;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Console.WriteLine("OK");
                    Console.WriteLine("Content Length: " + response.ContentLength);
                    Console.WriteLine("Content Type: " + response.ContentType);
                    Tools.HttpDebugRequest(request);
                    Tools.HttpDebugResponse(response);
                }
            }
            catch (WebException webEx)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(webEx.Message);
                Console.WriteLine(webEx.StackTrace);
                Assert.Fail();
            }

            request = (HttpWebRequest)WebRequest.Create("http://dbpedia.org/data/London");
            request.Accept = "application/rdf+xml";
            request.Method = "GET";
            request.Timeout = 45000;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Console.WriteLine("OK");
                    Console.WriteLine("Content Length: " + response.ContentLength);
                    Console.WriteLine("Content Type: " + response.ContentType);
                    Tools.HttpDebugRequest(request);
                    Tools.HttpDebugResponse(response);
                }
            }
            catch (WebException webEx)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(webEx.Message);
                Console.WriteLine(webEx.StackTrace);
                Assert.Fail();
            }

            try
            {
                Graph g = new Graph();
                Options.HttpDebugging = true;
                UriLoader.Load(g, new Uri("http://dbpedia.org/resource/London"));
                Console.WriteLine("OK");
                Console.WriteLine(g.Triples.Count + " Triples retrieved");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Assert.Fail();
            }
            finally
            {
                Options.HttpDebugging = false;
            }
        }

        [TestMethod]
        public void ParsingUriLoaderWithChunkedData()
        {
            try
            {
                Options.UriLoaderCaching = false;
                Options.HttpDebugging = true;
                Options.UriLoaderTimeout = 90000;
                //Options.HttpFullDebugging = true;

                Graph g = new Graph();
                UriLoader.Load(g, new Uri("http://cheminfov.informatics.indiana.edu:8080/medline/resource/medline/15760907"));

                foreach (Triple t in g.Triples)
                {
                    Console.WriteLine(t.ToString());
                }
            }
            finally
            {
                //Options.HttpFullDebugging = false;
                Options.UriLoaderTimeout = 15000;
                Options.HttpDebugging = false;
                Options.UriLoaderCaching = true;
            }
        }

        [TestMethod]
        public void ParsingUriLoaderDBPedia()
        {
            int defaultTimeout = Options.UriLoaderTimeout;
            try
            {
                Options.HttpDebugging = true;
                Options.UriLoaderCaching = false;
                Options.UriLoaderTimeout = 45000;

                Graph g = new Graph();
                UriLoader.Load(g, new Uri("http://dbpedia.org/resource/Barack_Obama"));
                NTriplesFormatter formatter = new NTriplesFormatter();
                foreach (Triple t in g.Triples)
                {
                    Console.WriteLine(t.ToString(formatter));
                }
                Assert.IsFalse(g.IsEmpty, "Graph should not be empty");
            }
            finally
            {
                Options.HttpDebugging = false;
                Options.UriLoaderCaching = true;
                Options.UriLoaderTimeout = defaultTimeout;
            }
        }

        [TestMethod]
        public void ParsingEmbeddedResourceInDotNetRdf()
        {
            Graph g = new Graph();
            EmbeddedResourceLoader.Load(g, "VDS.RDF.Configuration.configuration.ttl");

            TestTools.ShowGraph(g);

            Assert.IsFalse(g.IsEmpty, "Graph should be non-empty");
        }

        [TestMethod]
        public void ParsingEmbeddedResourceInDotNetRdf2()
        {
            Graph g = new Graph();
            EmbeddedResourceLoader.Load(g, "VDS.RDF.Configuration.configuration.ttl, dotNetRDF");

            TestTools.ShowGraph(g);

            Assert.IsFalse(g.IsEmpty, "Graph should be non-empty");
        }

        [TestMethod]
        public void ParsingEmbeddedResourceInExternalAssembly()
        {
            Graph g = new Graph();
            EmbeddedResourceLoader.Load(g, "VDS.RDF.Test.embedded.ttl, dotNetRDF.Test");

            TestTools.ShowGraph(g);

            Assert.IsFalse(g.IsEmpty, "Graph should be non-empty");
        }

        [TestMethod]
        public void ParsingEmbeddedResourceLoaderGraphIntoTripleStore()
        {
            TripleStore store = new TripleStore();
            store.LoadFromEmbeddedResource("VDS.RDF.Configuration.configuration.ttl");

            Assert.IsTrue(store.Triples.Count() > 0);
            Assert.AreEqual(1, store.Graphs.Count);
        }

        [TestMethod]
        public void ParsingFileLoaderGraphIntoTripleStore()
        {
            Graph g = new Graph();
            g.LoadFromEmbeddedResource("VDS.RDF.Configuration.configuration.ttl");
            g.SaveToFile("fileloader-graph-to-store.ttl");

            TripleStore store = new TripleStore();
            store.LoadFromFile("fileloader-graph-to-store.ttl");

            Assert.IsTrue(store.Triples.Count() > 0);
            Assert.AreEqual(1, store.Graphs.Count);
        }
       
    }
}
