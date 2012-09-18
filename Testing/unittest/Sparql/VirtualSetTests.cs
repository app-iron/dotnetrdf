using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Query.Datasets;

namespace VDS.RDF.Test.Sparql
{
    [TestClass]
    public class VirtualSetTests
    {
        private SparqlQueryParser _parser = new SparqlQueryParser();

        [TestMethod]
        public void SparqlVirtualSetSimple1()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");
            INode yVal = g.CreateLiteralNode("y");

            Set x = new Set();
            x.Add("x", xVal);
            Set y = new Set();
            y.Add("y", yVal);

            Assert.AreEqual(xVal, x["x"]);
            Assert.AreEqual(yVal, y["y"]);

            VirtualSet z = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z["y"], "Bad value for ?y");
            Assert.AreEqual(2, z.Variables.Count());
        }

        [TestMethod]
        public void SparqlVirtualSetSimple2()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");

            Set x = new Set();
            x.Add("var", xVal);
            Set y = new Set();

            Assert.AreEqual(xVal, x["var"]);
            Assert.IsNull(y["var"]);

            VirtualSet z = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z["var"], "Bad value for ?var");
            Assert.AreEqual(1, z.Variables.Count());

            z = new VirtualSet(y, x);
            Assert.AreEqual(xVal, z["var"], "Bad value for ?var");
            Assert.AreEqual(1, z.Variables.Count());
        }

        [TestMethod]
        public void SparqlVirtualSetSimple3()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");
            INode yVal = g.CreateLiteralNode("y");
            INode zVal = g.CreateLiteralNode("z");

            Set x = new Set();
            x.Add("x", xVal);
            Set y = new Set();
            y.Add("y", yVal);

            Assert.AreEqual(xVal, x["x"]);
            Assert.AreEqual(yVal, y["y"]);

            VirtualSet z1 = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z1["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z1["y"], "Bad value for ?y");
            Assert.AreEqual(2, z1.Variables.Count());

            VirtualSet z2 = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z2["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z2["y"], "Bad value for ?y");
            Assert.AreEqual(2, z2.Variables.Count());

            //Adding things to one VirtualSet should not modify the underlying sets
            z1.Add("z", zVal);

            Assert.AreEqual(xVal, z1["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z1["y"], "Bad value for ?y");
            Assert.AreEqual(zVal, z1["z"], "Bad value for ?z");
            Assert.AreEqual(3, z1.Variables.Count());

            Assert.AreEqual(xVal, z2["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z2["y"], "Bad value for ?y");
            Assert.IsNull(z2["z"]);
            Assert.AreEqual(2, z2.Variables.Count());
        }

        [TestMethod]
        public void SparqlVirtualSetSimple4()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");
            INode yVal = g.CreateLiteralNode("y");

            Set x = new Set();
            x.Add("x", xVal);
            Set y = new Set();
            y.Add("y", yVal);

            Assert.AreEqual(xVal, x["x"]);
            Assert.AreEqual(yVal, y["y"]);

            VirtualSet z1 = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z1["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z1["y"], "Bad value for ?y");
            Assert.AreEqual(2, z1.Variables.Count());

            VirtualSet z2 = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z2["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z2["y"], "Bad value for ?y");
            Assert.AreEqual(2, z2.Variables.Count());

            //Removing things to one VirtualSet should not modify the underlying sets
            z1.Remove("y");

            Assert.AreEqual(xVal, z1["x"], "Bad value for ?x");
            Assert.IsNull(z1["y"]);
            Assert.AreEqual(1, z1.Variables.Count());

            Assert.AreEqual(xVal, z2["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z2["y"], "Bad value for ?y");
            Assert.AreEqual(2, z2.Variables.Count());
        }

        [TestMethod]
        public void SparqlVirtualSetCopy1()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");
            INode yVal = g.CreateLiteralNode("y");

            Set x = new Set();
            x.Add("x", xVal);
            Set y = new Set();
            y.Add("y", yVal);

            Assert.AreEqual(xVal, x["x"]);
            Assert.AreEqual(yVal, y["y"]);

            VirtualSet z = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, z["y"], "Bad value for ?y");
            Assert.AreEqual(2, z.Variables.Count());

            Set zCopy = new Set(z);
            Assert.AreEqual(xVal, zCopy["x"], "Bad value for ?x");
            Assert.AreEqual(yVal, zCopy["y"], "Bad value for ?y");
            Assert.AreEqual(2, zCopy.Variables.Count());
        }

        public void TestNestedVirtualSets(int sets)
        {
            if (sets < 3) throw new ArgumentException("sets must be >= 3");

            Graph g = new Graph();

            //Generate sets
            List<Set> contents = new List<Set>();
            for (int i = 0; i < sets; i++)
            {
                Set s = new Set();
                s.Add("var" + i, i.ToLiteral(g));
                contents.Add(s);
            }

            //Combine them together into a single virtual set
            VirtualSet current = new VirtualSet(contents[0], contents[1]);
            for (int i = 2; i < sets; i++)
            {
                current = new VirtualSet(current, contents[i]);
            }

            //Now check that all the values can be successfully retrieved
            Assert.AreEqual(sets, current.Variables.Count());
            Assert.AreEqual(sets, current.Values.Count());
            for (int i = 0; i < sets; i++)
            {
                Assert.AreEqual(i.ToLiteral(g), current["var" + i], "Bad value for ?var" + i);
            }
        }

        [TestMethod]
        public void SparqlVirtualSetNested1()
        {
            this.TestNestedVirtualSets(3);
        }

        [TestMethod]
        public void SparqlVirtualSetNested2()
        {
            this.TestNestedVirtualSets(5);
        }

        [TestMethod]
        public void SparqlVirtualSetNested3()
        {
            this.TestNestedVirtualSets(10);
        }

        [TestMethod]
        public void SparqlVirtualSetNested4()
        {
            this.TestNestedVirtualSets(50);
        }

        [TestMethod]
        public void SparqlVirtualSetCompatibility1()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");

            Set x = new Set();
            x.Add("x", xVal);
            Set y = new Set();
            y.Add("x", xVal);

            Assert.AreEqual(xVal, x["x"]);
            Assert.AreEqual(xVal, y["x"]);

            VirtualSet z = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z["x"], "Bad value for ?x");
            Assert.AreEqual(1, z.Variables.Count());

            Assert.IsTrue(z.IsCompatibleWith(x, new String[] { "x" }));
            Assert.IsTrue(z.IsCompatibleWith(y, new String[] { "x" }));
        }

        [TestMethod]
        public void SparqlVirtualSetCompatibility2()
        {
            Graph g = new Graph();
            INode xVal = g.CreateLiteralNode("x");

            Set x = new Set();
            x.Add("x", xVal);
            Set y = new Set();
            y.Add("x", xVal);

            Assert.AreEqual(xVal, x["x"]);
            Assert.AreEqual(xVal, y["x"]);

            VirtualSet z = new VirtualSet(x, y);
            Assert.AreEqual(xVal, z["x"], "Bad value for ?x");
            Assert.AreEqual(1, z.Variables.Count());

            Assert.IsTrue(z.IsCompatibleWith(x, new String[] { "x" }));
            Assert.IsTrue(z.IsCompatibleWith(y, new String[] { "x" }));

            VirtualSet z2 = new VirtualSet(x, y);
            Assert.IsTrue(z.IsCompatibleWith(z2, new String[] { "x" }));
        }

        [TestMethod]
        public void SparqlVirtualSetSparql1()
        {
            String query = @"PREFIX :      <http://example/> 
PREFIX xsd:   <http://www.w3.org/2001/XMLSchema#>

SELECT ?v
{
    :x1 ?p ?o
    OPTIONAL { ?o :q ?v }
}";
            SparqlQuery q = this._parser.ParseFromString(query);

            Graph g = new Graph();
            g.LoadFromFile("data-opt.ttl");
            InMemoryDataset ds = new InMemoryDataset(g);

            long current = Options.QueryExecutionTimeout;
            try
            {
                Options.QueryExecutionTimeout = 0;

                LeviathanQueryProcessor processor = new LeviathanQueryProcessor(ds);
                SparqlResultSet results = processor.ProcessQuery(q) as SparqlResultSet;
                if (results == null) Assert.Fail("Did not get a result set as expected");
                Assert.AreEqual(6, results.Count);
                INode temp;
                Assert.AreEqual(4, results.Where(r => r.TryGetBoundValue("v", out temp)).Count());
            }
            finally
            {
                Options.QueryExecutionTimeout = current;
            }
        }
    }
}
