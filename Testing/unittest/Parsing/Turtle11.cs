﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Test.Parsing
{
    [TestClass]
    public class Turtle11
    {
        private TurtleParser _ttlParser = new TurtleParser(TurtleSyntax.W3C);
        private NTriplesParser _ntParser = new NTriplesParser();
        private int _count, _pass, _fail, _indeterminate;

        private static Uri BaseUri = new Uri("http://www.w3.org/2001/sw/DataAccess/df1/tests/");

        [TestInitialize]
        public void Setup()
        {
            this._count = 0;
            this._pass = 0;
            this._fail = 0;
            this._indeterminate = 0;
        }

        private void RunManifest(String file, bool shouldParse)
        {
            if (!File.Exists(file))
            {
                Assert.Fail("Manifest file " + file + " not found");
            }

            Graph manifest = new Graph();
            manifest.BaseUri = BaseUri;
            try
            {
                this._ttlParser.Load(manifest, file);
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Bad Manifest", ex);
                Assert.Fail("Failed to load Manifest " + file);
            }

            String findTests = @"prefix rdf:    <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
prefix rdfs:	<http://www.w3.org/2000/01/rdf-schema#> 
prefix mf:     <http://www.w3.org/2001/sw/DataAccess/tests/test-manifest#> 
prefix qt:     <http://www.w3.org/2001/sw/DataAccess/tests/test-query#> 
SELECT ?name ?input ?comment ?result
WHERE
{
  ?test mf:action [ qt:data ?input ] .
  OPTIONAL { ?test mf:name ?name }
  OPTIONAL { ?test rdfs:comment ?comment }
  OPTIONAL { ?test mf:result ?result }
}";

            SparqlResultSet tests = manifest.ExecuteQuery(findTests) as SparqlResultSet;
            if (tests == null) Assert.Fail("Failed to find tests in the Manifest");

            foreach (SparqlResult test in tests)
            {
                INode nameNode, inputNode, commentNode, resultNode;
                String name = test.TryGetBoundValue("name", out nameNode) ? nameNode.ToString() : null;
                inputNode = test["input"];
                String input = this.GetFile(inputNode);
                String comment = test.TryGetBoundValue("comment", out commentNode) ? commentNode.ToString() : null;
                String results = test.TryGetBoundValue("result", out resultNode) ? this.GetFile(resultNode) : null;

                this.RunTest(name, comment, input, results, shouldParse);
            }
        }

        private String GetFile(INode n)
        {
            switch (n.NodeType)
            {
                case NodeType.Uri:
                    Uri u = ((IUriNode)n).Uri;
                    if (u.IsFile)
                    {
                        return u.AbsolutePath;
                    }
                    else
                    {
                        String lastSegment = u.Segments[u.Segments.Length - 1];
                        return "turtle11/" + lastSegment;
                    }
                    break;
                default:
                    Assert.Fail("Malformed manifest file, input file must be a  URI");
                    break;
            }
            //Keep compiler happy
            return null;
        }

        private void RunTest(String name, String comment, String file, String resultFile, bool shouldParse)
        {
            Console.WriteLine("### Running Test #" + this._count);
            if (name != null) Console.WriteLine("Test Name " + name);
            if (comment != null) Console.WriteLine(comment);
            Console.WriteLine();
            Console.WriteLine("Input File is " + file);
            if (resultFile != null) Console.WriteLine("Expected Output File is " + resultFile);

            //Check File Exists
            if (!File.Exists(file))
            {
                Console.WriteLine("Input File not found");
                this._fail++;
                return;
            }

            try
            {
                Graph actual = new Graph();
                actual.BaseUri = new Uri(BaseUri, Path.GetFileName(file));
                this._ttlParser.Load(actual, file);

                if (shouldParse)
                {
                    Console.WriteLine("Parsed input in OK");

                    //Validate if necessary
                    if (!File.Exists(resultFile))
                    {
                        Console.WriteLine("Expected Output File not found");
                        this._fail++;
                    }
                    else
                    {
                        Graph expected = new Graph();
                        try
                        {
                            this._ntParser.Load(expected, resultFile);

                            GraphDiffReport diff = expected.Difference(actual);
                            if (diff.AreEqual)
                            {
                                Console.WriteLine("Parsed Graph matches Expected Graph (Test Passed)");
                                this._pass++;
                            }
                            else
                            {
                                Console.WriteLine("Parsed Graph did not match Expected Graph (Test Failed)");
                                this._fail++;
                                TestTools.ShowDifferences(diff);
                            }
                        }
                        catch (RdfParseException)
                        {
                            Console.WriteLine("Expected Output File could not be parsed (Test Indeterminate)");
                            this._indeterminate++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Parsed when failure was expected (Test Failed)");
                    this._fail++;
                }
            }
            catch (RdfParseException parseEx)
            {
                if (shouldParse)
                {
                    Console.WriteLine("Failed when was expected to parse (Test Failed");
                    TestTools.ReportError("Parse Error", parseEx);
                    this._fail++;
                }
                else
                {
                    Console.WriteLine("Failed to parse as expected (Test Passed)");
                    this._pass++;
                }
            }
            Console.WriteLine("### End Test #" + this._count);
            Console.WriteLine();
        }

        [TestMethod]
        public void ParsingTurtleW3CSuite()
        {
            //Run manifests
            this.RunManifest("turtle11/manifest.ttl", true);
            this.RunManifest("turtle11/manifest-bad.ttl", false);            

            Console.WriteLine(this._count + " Tests - " + this._pass + " Passed - " + this._fail + " Failed");
            Console.WriteLine((((double)this._pass / (double)this._count) * 100) + "% Passed");

            if (this._fail > 0) Assert.Fail(this._fail + " Tests failed");
            if (this._indeterminate > 0) Assert.Inconclusive(this._indeterminate + " Tests are indeterminate");
        }

        [TestMethod]
        public void ParsingTurtleW3CBaseTurtleStyle1()
        {
            //Dot required
            String graph = "@base <http://example.org/> .";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.BaseUri);
        }

        [TestMethod,ExpectedException(typeof(RdfParseException))]
        public void ParsingTurtleW3CBaseTurtleStyle2()
        {
            //Missing dot
            String graph = "@base <http://example.org/>";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.BaseUri);
        }

        [TestMethod,ExpectedException(typeof(RdfParseException))]
        public void ParsingTurtleW3CBaseSparqlStyle1()
        {
            //Forbidden dot
            String graph = "BASE <http://example.org/> .";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.BaseUri);
        }

        [TestMethod]
        public void ParsingTurtleW3CBaseSparqlStyle2()
        {
            //No dot required
            String graph = "BASE <http://example.org/>";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.BaseUri);
        }

        [TestMethod]
        public void ParsingTurtleW3CPrefixTurtleStyle1()
        {
            //Dot required
            String graph = "@prefix ex: <http://example.org/> .";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.NamespaceMap.GetNamespaceUri("ex"));
        }

        [TestMethod, ExpectedException(typeof(RdfParseException))]
        public void ParsingTurtleW3CPrefixTurtleStyle2()
        {
            //Missing dot
            String graph = "@prefix ex: <http://example.org/>";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.NamespaceMap.GetNamespaceUri("ex"));
        }

        [TestMethod, ExpectedException(typeof(RdfParseException))]
        public void ParsingTurtleW3CPrefixSparqlStyle1()
        {
            //Forbidden dot
            String graph = "PREFIX ex: <http://example.org/> .";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.NamespaceMap.GetNamespaceUri("ex"));
        }

        [TestMethod]
        public void ParsingTurtleW3CPrefixSparqlStyle2()
        {
            //No dot required
            String graph = "PREFIX ex: <http://example.org/>";
            Graph g = new Graph();
            this._ttlParser.Load(g, new StringReader(graph));

            Assert.AreEqual(new Uri("http://example.org"), g.NamespaceMap.GetNamespaceUri("ex"));
        }
    }
}
