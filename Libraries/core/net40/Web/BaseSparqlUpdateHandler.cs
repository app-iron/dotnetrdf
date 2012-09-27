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

#if !NO_WEB && !NO_ASP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using VDS.RDF.Web.Configuration.Update;
using VDS.RDF.Parsing;
using VDS.RDF.Update;
using VDS.RDF.Update.Commands;
using VDS.RDF.Writing;

namespace VDS.RDF.Web
{
    /// <summary>
    /// Abstract Base Class for creating SPARQL Update Handler implementations
    /// </summary>
    public abstract class BaseSparqlUpdateHandler
        : IHttpHandler
    {
        /// <summary>
        /// Handler Configuration
        /// </summary>
        protected BaseUpdateHandlerConfiguration _config;

        /// <summary>
        /// Gets that the Handler is reusable
        /// </summary>
        public bool IsReusable
        {
            get 
            {
                return true;
            }
        }

        /// <summary>
        /// Processes SPARQL Update requests
        /// </summary>
        /// <param name="context">HTTP Context</param>
        public void ProcessRequest(HttpContext context)
        {
            this._config = this.LoadConfig(context);
            WebContext webContext = new WebContext(context);

            //Add our Standard Headers
            HandlerHelper.AddStandardHeaders(webContext, this._config);

            if (context.Request.HttpMethod.Equals("OPTIONS"))
            {
                //OPTIONS requests always result in the Service Description document
                IGraph svcDescrip = SparqlServiceDescriber.GetServiceDescription(this._config, UriFactory.Create(context.Request.Url.AbsoluteUri));
                HandlerHelper.SendToClient(webContext, svcDescrip, this._config);
                return;
            }

            //See if there has been an update submitted
            String updateText = null;
            if (context.Request.ContentType != null)
            {
                if (context.Request.ContentType.Equals(MimeTypesHelper.WWWFormURLEncoded))
                {
                    updateText = context.Request.Form["update"];
                }
                else if (context.Request.ContentType.Equals(MimeTypesHelper.SparqlUpdate))
                {
                    updateText = new StreamReader(context.Request.InputStream).ReadToEnd();
                }
            }
            else
            {
                updateText = context.Request.Form["update"];
            }

            //If no Update sent either show Update Form or give a HTTP 400 response
            if (updateText == null || updateText.Equals(String.Empty))
            {
                //If there is no Update we may return the SPARQL Service Description where appropriate
                try
                {
                    //If we might show the Update Form only show the Description if the selected writer is
                    //not a HTML writer
                    MimeTypeDefinition definition = MimeTypesHelper.GetDefinitions(HandlerHelper.GetAcceptTypes(webContext)).FirstOrDefault(d => d.CanWriteRdf);
                    if (definition != null)
                    {
                        IRdfWriter writer = definition.GetRdfWriter();
                        if (!this._config.ShowUpdateForm || !(writer is IHtmlWriter))
                        {
                            //If not a HTML Writer selected OR not showing Update Form then show the Service Description Graph
                            //unless an error occurs creating it
                            IGraph serviceDescrip = SparqlServiceDescriber.GetServiceDescription(this._config, UriFactory.Create(context.Request.Url.AbsoluteUri));
                            context.Response.ContentType = definition.CanonicalMimeType;
                            context.Response.ContentEncoding = definition.Encoding;
                            writer.Save(serviceDescrip, new StreamWriter(context.Response.OutputStream, definition.Encoding));
                            return;
                        }
                    }
                }
                catch
                {
                    //Ignore Exceptions - we'll just show the Query Form or return a 400 Bad Request instead
                }

                //If a Writer can't be selected then we'll either show the Update Form or return a 400 Bad Request
                if (this._config.ShowUpdateForm)
                {
                    this.ShowUpdateForm(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                return;
            }

            //Get Other options associated with this update
            List<String> userDefaultGraphs = new List<String>();
            List<String> userNamedGraphs = new List<String>();

            //Get the USING URIs (if any)
            if (context.Request.QueryString["using-graph-uri"] != null)
            {
                userDefaultGraphs.AddRange(context.Request.QueryString.GetValues("using-graph-uri"));
            }
            else if (context.Request.Form["using-graph-uri"] != null)
            {
                userDefaultGraphs.AddRange(context.Request.Form.GetValues("using-graph-uri"));
            }
            //Get the USING NAMED URIs (if any)
            if (context.Request.QueryString["using-named-graph-uri"] != null)
            {
                userNamedGraphs.AddRange(context.Request.QueryString.GetValues("using-named-graph-uri"));
            }
            else if (context.Request.Form["using-named-graph-uri"] != null)
            {
                userNamedGraphs.AddRange(context.Request.Form.GetValues("using-named-graph-uri"));
            }

            try
            {
                //Now we're going to parse the Updates
                SparqlUpdateParser parser = new SparqlUpdateParser();
                parser.ExpressionFactories = this._config.ExpressionFactories;
                SparqlUpdateCommandSet commands = parser.ParseFromString(updateText);

                //Check whether we need to use authentication
                //If there are no user groups then no authentication is in use so we default to authenticated with no per-action authentication needed
                bool isAuth = true, requireActionAuth = false;
                if (this._config.UserGroups.Any())
                {
                    //If we have user
                    isAuth = HandlerHelper.IsAuthenticated(webContext, this._config.UserGroups);
                    requireActionAuth = true;
                }
                if (!isAuth) return;

                //First check actions to see whether they are all permissible and apply USING/USING NAMED parameters
                foreach (SparqlUpdateCommand cmd in commands.Commands)
                {
                    //Authenticate each action
                    bool actionAuth = true;
                    if (requireActionAuth) actionAuth = HandlerHelper.IsAuthenticated(webContext, this._config.UserGroups, this.GetPermissionAction(cmd));
                    if (!actionAuth)
                    {
                        throw new SparqlUpdatePermissionException("You are not authorised to perform the " + this.GetPermissionAction(cmd) + " action");
                    }

                    //Check whether we need to (and are permitted to) apply USING/USING NAMED parameters
                    if (userDefaultGraphs.Count > 0 || userNamedGraphs.Count > 0)
                    {
                        BaseModificationCommand modify = cmd as BaseModificationCommand;
                        if (modify != null)
                        {
                            if (modify.GraphUri != null || modify.UsingUris.Any() || modify.UsingNamedUris.Any())
                            {
                                //Invalid if a command already has a WITH/USING/USING NAMED
                                throw new SparqlUpdateMalformedException("A command in your update request contains a WITH/USING/USING NAMED clause but you have also specified one/both of the using-graph-uri or using-named-graph-uri parameters which is not permitted by the SPARQL Protocol");
                            }
                            else
                            {
                                //Otherwise go ahead and apply
                                userDefaultGraphs.ForEach(u => modify.AddUsingUri(UriFactory.Create(u)));
                                userNamedGraphs.ForEach(u => modify.AddUsingNamedUri(UriFactory.Create(u)));
                            }
                        }
                    }
                }

                //Then assuming we got here this means all our actions are permitted so now we can process the updates
                this.ProcessUpdates(commands);

                //Flush outstanding changes
                this._config.Processor.Flush();

                //Update the Cache as the request may have changed the endpoint
                this.UpdateConfig(context);
            }
            catch (RdfParseException parseEx)
            {
                HandleErrors(context, "Parsing Error", updateText, parseEx, (int)HttpStatusCode.BadRequest);
            }
            catch (SparqlUpdatePermissionException permEx)
            {
                HandleErrors(context, "Permissions Error", updateText, permEx, (int)HttpStatusCode.Forbidden);
            }
            catch (SparqlUpdateMalformedException malEx)
            {
                HandleErrors(context, "Malformed Update Error", updateText, malEx, (int)HttpStatusCode.BadRequest);
            }
            catch (SparqlUpdateException updateEx)
            {
                HandleErrors(context, "Update Error", updateText, updateEx);
            }
            catch (RdfException rdfEx)
            {
                HandleErrors(context, "RDF Error", updateText, rdfEx);
            }
            catch (Exception ex)
            {
                HandleErrors(context, "Error", updateText, ex);
            }
        }

        /// <summary>
        /// Loads the Handler Configuration
        /// </summary>
        /// <param name="context">HTTP Context</param>
        /// <returns></returns>
        protected abstract BaseUpdateHandlerConfiguration LoadConfig(HttpContext context);

        /// <summary>
        /// Processes SPARQL Updates
        /// </summary>
        /// <param name="cmds">Update Command Set</param>
        /// <remarks>
        /// <para>
        /// Implementations should override this method if their behaviour requires more than just invoking the configured Update processor
        /// </para>
        /// </remarks>
        protected virtual void ProcessUpdates(SparqlUpdateCommandSet cmds)
        {
            this._config.Processor.ProcessCommandSet(cmds);
        }

        /// <summary>
        /// Updates the Handler Configuration
        /// </summary>
        /// <param name="context">HTTP Context</param>
        protected virtual void UpdateConfig(HttpContext context)
        {

        }

        /// <summary>
        /// Generates a SPARQL Update Form
        /// </summary>
        /// <param name="context">HTTP Context</param>
        protected virtual void ShowUpdateForm(HttpContext context)
        {
            //Set Content Type
            context.Response.Clear();
            context.Response.ContentType = "text/html";

            //Get a HTML Text Writer
            HtmlTextWriter output = new HtmlTextWriter(new StreamWriter(context.Response.OutputStream));

            //Page Header
            output.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            output.RenderBeginTag(HtmlTextWriterTag.Html);
            output.RenderBeginTag(HtmlTextWriterTag.Head);
            output.RenderBeginTag(HtmlTextWriterTag.Title);
            output.WriteEncodedText("SPARQL Update Interface");
            output.RenderEndTag();
            //Add Stylesheet
            if (!this._config.Stylesheet.Equals(String.Empty))
            {
                output.AddAttribute(HtmlTextWriterAttribute.Href, this._config.Stylesheet);
                output.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                output.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                output.RenderBeginTag(HtmlTextWriterTag.Link);
                output.RenderEndTag();
            }
            output.RenderEndTag();


            //Header Text
            output.RenderBeginTag(HtmlTextWriterTag.Body);
            output.RenderBeginTag(HtmlTextWriterTag.H3);
            output.WriteEncodedText("SPARQL Update Interface");
            output.RenderEndTag();

            //Query Form
            output.AddAttribute(HtmlTextWriterAttribute.Name, "sparqlUpdate");
            output.AddAttribute("method", "post");
            output.AddAttribute("action", context.Request.Path);
            output.RenderBeginTag(HtmlTextWriterTag.Form);

            if (!this._config.IntroductionText.Equals(String.Empty))
            {
                output.RenderBeginTag(HtmlTextWriterTag.P);
                output.Write(this._config.IntroductionText);
                output.RenderEndTag();
            }

            output.WriteEncodedText("Update");
            output.WriteBreak();
            output.AddAttribute(HtmlTextWriterAttribute.Name, "update");
            output.AddAttribute(HtmlTextWriterAttribute.Rows, "15");
            output.AddAttribute(HtmlTextWriterAttribute.Cols, "100");
            output.RenderBeginTag(HtmlTextWriterTag.Textarea);
            output.WriteEncodedText(this._config.DefaultUpdate);
            output.RenderEndTag();
            output.WriteBreak();

            //output.WriteEncodedText("Default Graph URI: ");
            //output.AddAttribute(HtmlTextWriterAttribute.Name, "default-graph-uri");
            //output.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            //output.AddAttribute(HtmlTextWriterAttribute.Size, "100");
            //output.AddAttribute(HtmlTextWriterAttribute.Value, this._config.DefaultGraphURI);
            //output.RenderBeginTag(HtmlTextWriterTag.Input);
            //output.RenderEndTag();
            //output.WriteBreak();

            output.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
            output.AddAttribute(HtmlTextWriterAttribute.Value, "Perform Update");
            output.RenderBeginTag(HtmlTextWriterTag.Input);
            output.RenderEndTag();

            output.RenderEndTag(); //End Form

            //End of Page
            output.RenderEndTag(); //End Body
            output.RenderEndTag(); //End Html

            output.Flush();
        }

        /// <summary>
        /// Handles errors in processing SPARQL Update Requests
        /// </summary>
        /// <param name="context">Context of the HTTP Request</param>
        /// <param name="title">Error title</param>
        /// <param name="update">SPARQL Update</param>
        /// <param name="ex">Error</param>
        protected virtual void HandleErrors(HttpContext context, String title, String update, Exception ex)
        {
            HandlerHelper.HandleUpdateErrors(new WebContext(context), this._config, title, update, ex);
        }


        /// <summary>
        /// Handles errors in processing SPARQL Update Requests
        /// </summary>
        /// <param name="context">Context of the HTTP Request</param>
        /// <param name="title">Error title</param>
        /// <param name="update">SPARQL Update</param>
        /// <param name="ex">Error</param>
        /// <param name="statusCode">HTTP Status Code</param>
        protected virtual void HandleErrors(HttpContext context, String title, String update, Exception ex, int statusCode)
        {
            HandlerHelper.HandleUpdateErrors(new WebContext(context), this._config, title, update, ex, statusCode);
        }

        /// <summary>
        /// Gets the Permission action for the SPARQL Update Command
        /// </summary>
        /// <param name="cmd">Update Command</param>
        /// <returns></returns>
        private String GetPermissionAction(SparqlUpdateCommand cmd)
        {
            switch (cmd.CommandType)
            {
                case SparqlUpdateCommandType.InsertData:
                    return "INSERT DATA";
                case SparqlUpdateCommandType.DeleteData:
                    return "DELETE DATA";
                case SparqlUpdateCommandType.Insert:
                    return "INSERT";
                case SparqlUpdateCommandType.Delete:
                    return "DELETE";
                case SparqlUpdateCommandType.Modify:
                    return "MODIFY";
                case SparqlUpdateCommandType.Load:
                    return "LOAD";
                case SparqlUpdateCommandType.Clear:
                    return "CLEAR";
                case SparqlUpdateCommandType.Create:
                    return "CREATE";
                case SparqlUpdateCommandType.Drop:
                    return "DROP";
                default:
                    return String.Empty;
            }
        }
    }
}

#endif