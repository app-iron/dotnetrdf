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
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDS.RDF.Configuration;
using VDS.RDF.Parsing;

namespace VDS.RDF.Test.Parsing
{
    [TestClass]
    public class CachingTests
    {
        private static Uri test = new Uri("http://www.dotnetrdf.org/configuration#");

        [TestMethod]
        public void ParsingUriLoaderCache()
        {
            //Load the Graph
            Graph g = new Graph();
            UriLoader.Load(g, test);

            //Then reload the Graph which it should now come from the cache instead
            Graph h = new Graph();
            UriLoader.Load(h, test);

            Assert.AreEqual(g, h);
        }

        [TestMethod]
        public void ParsingUriLoaderCustomCache()
        {
            String original = UriLoader.CacheDirectory;
            try
            {
                UriLoader.CacheDirectory = Environment.CurrentDirectory;

                this.ParsingUriLoaderCache();
            }
            finally
            {
                UriLoader.CacheDirectory = original;
            }
        }

        [TestMethod]
        public void ParsingUriLoaderUriSantisation()
        {
            Uri a = new Uri(ConfigurationLoader.ClassTripleStore);
            Uri b = new Uri(ConfigurationLoader.ClassGraph);

            Console.WriteLine("URI A: " + a.AbsoluteUri + " is equivalent to " + Tools.StripUriFragment(a).AbsoluteUri);
            Console.WriteLine("URI B:" + b.AbsoluteUri + " is equivalent to " + Tools.StripUriFragment(b).AbsoluteUri);

            Assert.AreEqual(Tools.StripUriFragment(a).AbsoluteUri, Tools.StripUriFragment(b).AbsoluteUri, "URIs stripped of their Fragment IDs should have been equal");

            Graph g = new Graph();
            UriLoader.Load(g, a);

            Assert.IsTrue(UriLoader.IsCached(a), "Content should have been cached as a result of loading from the URI");

            Graph h = new Graph();
            UriLoader.Load(h, b);

            Assert.AreEqual(g, h, "Two Graphs should be equal since they come from the same URI");
        }

        [TestMethod]
        public void ParsingUriLoaderResponseUriCaching()
        {
            int defaultTimeout = Options.UriLoaderTimeout;
            try
            {
                Options.UriLoaderTimeout = 45000;
                Uri soton = new Uri("http://dbpedia.org/resource/Southampton");
                Uri sotonPage = new Uri("http://dbpedia.org/page/Southampton.html");
                Uri sotonData = new Uri("http://dbpedia.org/data/Southampton.xml");

                Graph g = new Graph();
                UriLoader.Load(g, soton);

                Assert.IsTrue(UriLoader.IsCached(soton), "Resource URI should have been cached");
                Assert.IsTrue(UriLoader.IsCached(sotonData), "Data URI should have been cached");
                Assert.IsFalse(UriLoader.IsCached(sotonPage), "Page URI should not have been cached");
            }
            finally
            {
                Options.UriLoaderTimeout = defaultTimeout;
            }
        }
    }
}
