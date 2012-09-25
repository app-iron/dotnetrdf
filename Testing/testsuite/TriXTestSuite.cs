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
using System.Linq;
using System.IO;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace dotNetRDFTest
{
    public class TriXTestSuite
    {
        public static void Main(String[] args)
        {
            StreamWriter output = new StreamWriter("TriXTestSuite.txt");
            Console.SetOut(output);

            Console.WriteLine("## TriX Test Suite");
            Console.WriteLine();

            try
            {

                foreach (String file in Directory.GetFiles("trix_tests"))
                {
                    if (Path.GetExtension(file) == ".xml")
                    {
                        Console.WriteLine("## Testing File " + Path.GetFileName(file));

                        try
                        {
                            //Parse in
                            TriXParser parser = new TriXParser();
                            TripleStore store = new TripleStore();
                            parser.Load(store, file);

                            Console.WriteLine("# Parsed OK");
                            Console.WriteLine();
                            foreach (Triple t in store.Triples)
                            {
                                Console.WriteLine(t.ToString() + " from Graph <" + t.GraphUri.ToString() + ">");
                            }
                            Console.WriteLine();

                            //Serialize out
                            Console.WriteLine("# Attempting reserialization");

                            TriXWriter writer = new TriXWriter();
                            writer.Save(store, file + ".out");

                            Console.WriteLine("# Serialized OK");
                            Console.WriteLine();

                            //Now Parse back in
                            TripleStore store2 = new TripleStore();
                            parser.Load(store2, file + ".out");

                            Console.WriteLine("# Parsed back in again");
                            if (store.Graphs.Count == store2.Graphs.Count)
                            {
                                Console.WriteLine("Correct number of Graphs");
                            }
                            else
                            {
                                Console.WriteLine("Incorrect number of Graphs - Expected " + store.Graphs.Count + " - Actual " + store2.Graphs.Count);
                            }
                            if (store.Triples.Count() == store2.Triples.Count())
                            {
                                Console.WriteLine("Correct number of Triples");
                            }
                            else
                            {
                                Console.WriteLine("Incorrect number of Triples - Expected " + store.Triples.Count() + " - Actual " + store2.Triples.Count());
                            }
                        }
                        catch (RdfParseException parseEx)
                        {
                            HandleError("Parser Error", parseEx);
                        }
                        catch (RdfException rdfEx)
                        {
                            HandleError("RDF Error", rdfEx);
                        }
                        catch (Exception ex)
                        {
                            HandleError("Other Error", ex);
                        }
                        finally
                        {
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (RdfParseException parseEx)
            {
                HandleError("Parser Error", parseEx);
            }
            catch (Exception ex)
            {
                HandleError("Other Error", ex);
            }
            finally
            {
                output.Close();
            }
        }

        private static void HandleError(String title, Exception ex)
        {
            Console.WriteLine(title);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                Console.WriteLine();
                Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine(ex.InnerException.StackTrace);
            }
        }
    }
}
