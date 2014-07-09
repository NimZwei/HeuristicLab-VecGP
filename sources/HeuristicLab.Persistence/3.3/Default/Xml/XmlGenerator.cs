﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tracing;
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.Persistence.Default.Xml {


  /// <summary>
  /// Main entry point of persistence to XML. Use the static methods to serialize
  /// to a file or to a stream.
  /// </summary>
  public class XmlGenerator : GeneratorBase<string> {

    protected int depth;
    protected int Depth {
      get {
        return depth;
      }
      set {
        depth = value;
        prefix = new string(' ', depth * 2);
      }
    }

    protected string prefix;


    /// <summary>
    /// Initializes a new instance of the <see cref="XmlGenerator"/> class.
    /// </summary>
    public XmlGenerator() {
      Depth = 0;
    }

    protected enum NodeType { Start, End, Inline } ;

    protected static void AddXmlTagContent(StringBuilder sb, string name, Dictionary<string, string> attributes) {
      sb.Append(name);
      foreach (var attribute in attributes) {
        if (attribute.Value != null && !string.IsNullOrEmpty(attribute.Value.ToString())) {
          sb.Append(' ');
          sb.Append(attribute.Key);
          sb.Append("=\"");
          sb.Append(attribute.Value);
          sb.Append('"');
        }
      }
    }

    protected static int AttributeLength(Dictionary<string, string> attributes) {
      return attributes
        .Where(kvp => !string.IsNullOrEmpty(kvp.Key) && !string.IsNullOrEmpty(kvp.Value))
        .Select(kvp => kvp.Key.Length + kvp.Value.Length + 4).Sum();
    }

    protected static void AddXmlStartTag(StringBuilder sb, string name, Dictionary<string, string> attributes) {
      sb.Append('<');
      AddXmlTagContent(sb, name, attributes);
      sb.Append('>');
    }

    protected static void AddXmlInlineTag(StringBuilder sb, string name, Dictionary<string, string> attributes) {
      sb.Append('<');
      AddXmlTagContent(sb, name, attributes);
      sb.Append("/>");
    }

    protected static void AddXmlEndTag(StringBuilder sb, string name) {
      sb.Append("</");
      sb.Append(name);
      sb.Append(">");
    }

    protected string CreateNodeStart(string name, Dictionary<string, string> attributes) {
      StringBuilder sb = new StringBuilder(prefix.Length + name.Length + 4
        + AttributeLength(attributes));
      sb.Append(prefix);
      Depth += 1;
      AddXmlStartTag(sb, name, attributes);
      sb.Append("\r\n");
      return sb.ToString();
    }

    private static Dictionary<string, string> emptyDict = new Dictionary<string, string>();
    protected string CreateNodeStart(string name) {
      return CreateNodeStart(name, emptyDict);
    }

    protected string CreateNodeEnd(string name) {
      Depth -= 1;
      StringBuilder sb = new StringBuilder(prefix.Length + name.Length + 5);
      sb.Append(prefix);
      AddXmlEndTag(sb, name);
      sb.Append("\r\n");
      return sb.ToString();
    }

    protected string CreateNode(string name, Dictionary<string, string> attributes) {
      StringBuilder sb = new StringBuilder(prefix.Length + name.Length + 5
        + AttributeLength(attributes));
      sb.Append(prefix);
      AddXmlInlineTag(sb, name, attributes);
      sb.Append("\r\n");
      return sb.ToString();
    }

    protected string CreateNode(string name, Dictionary<string, string> attributes, string content) {
      StringBuilder sb = new StringBuilder(
        prefix.Length + name.Length + AttributeLength(attributes) + 2
        + content.Length + name.Length + 5);
      sb.Append(prefix);
      AddXmlStartTag(sb, name, attributes);
      sb.Append(content);
      sb.Append("</").Append(name).Append(">\r\n");
      return sb.ToString();
    }

    /// <summary>
    /// Formats the specified begin token.
    /// </summary>
    /// <param name="beginToken">The begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(BeginToken beginToken) {
      var dict = new Dictionary<string, string> {
          {"name", beginToken.Name},
          {"typeId", beginToken.TypeId.ToString()},
          {"id", beginToken.Id.ToString()}};
      AddTypeInfo(beginToken.TypeId, dict);
      return CreateNodeStart(XmlStringConstants.COMPOSITE, dict);

    }

    protected void AddTypeInfo(int typeId, Dictionary<string, string> dict) {
      if (lastTypeToken != null) {
        if (typeId == lastTypeToken.Id) {
          dict.Add("typeName", lastTypeToken.TypeName);
          dict.Add("serializer", lastTypeToken.Serializer);
          lastTypeToken = null;
        } else {
          FlushTypeToken();
        }
      }
    }

    /// <summary>
    /// Formats the specified end token.
    /// </summary>
    /// <param name="endToken">The end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(EndToken endToken) {
      return CreateNodeEnd(XmlStringConstants.COMPOSITE);
    }

    /// <summary>
    /// Formats the specified data token.
    /// </summary>
    /// <param name="dataToken">The data token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(PrimitiveToken dataToken) {
      var dict = new Dictionary<string, string> {
            {"typeId", dataToken.TypeId.ToString()},
            {"name", dataToken.Name},
            {"id", dataToken.Id.ToString()}};
      AddTypeInfo(dataToken.TypeId, dict);
      return CreateNode(XmlStringConstants.PRIMITIVE, dict,
        ((XmlString)dataToken.SerialData).Data);
    }

    /// <summary>
    /// Formats the specified ref token.
    /// </summary>
    /// <param name="refToken">The ref token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(ReferenceToken refToken) {
      return CreateNode(XmlStringConstants.REFERENCE,
        new Dictionary<string, string> {
          {"ref", refToken.Id.ToString()},
          {"name", refToken.Name}});
    }

    /// <summary>
    /// Formats the specified null ref token.
    /// </summary>
    /// <param name="nullRefToken">The null ref token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(NullReferenceToken nullRefToken) {
      return CreateNode(XmlStringConstants.NULL,
        new Dictionary<string, string>{
          {"name", nullRefToken.Name}});
    }

    /// <summary>
    /// Formats the specified meta info begin token.
    /// </summary>
    /// <param name="metaInfoBeginToken">The meta info begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(MetaInfoBeginToken metaInfoBeginToken) {
      return CreateNodeStart(XmlStringConstants.METAINFO);
    }

    /// <summary>
    /// Formats the specified meta info end token.
    /// </summary>
    /// <param name="metaInfoEndToken">The meta info end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(MetaInfoEndToken metaInfoEndToken) {
      return CreateNodeEnd(XmlStringConstants.METAINFO);
    }

    protected TypeToken lastTypeToken;
    /// <summary>
    /// Formats the specified token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(TypeToken token) {
      lastTypeToken = token;
      return "";
    }

    protected string FlushTypeToken() {
      if (lastTypeToken == null)
        return "";
      try {
        return CreateNode(XmlStringConstants.TYPE,
          new Dictionary<string, string> {
          {"id", lastTypeToken.Id.ToString()},
          {"typeName", lastTypeToken.TypeName },
          {"serializer", lastTypeToken.Serializer }});
      }
      finally {
        lastTypeToken = null;
      }
    }

    /// <summary>
    /// Formats the specified type cache.
    /// </summary>
    /// <param name="typeCache">The type cache.</param>
    /// <returns>An enumerable of formatted type cache tags.</returns>
    public IEnumerable<string> Format(List<TypeMapping> typeCache) {
      yield return CreateNodeStart(XmlStringConstants.TYPECACHE);
      foreach (var mapping in typeCache)
        yield return CreateNode(
          XmlStringConstants.TYPE,
          mapping.GetDict());
      yield return CreateNodeEnd(XmlStringConstants.TYPECACHE);
    }

    /// <summary>
    /// Serialize an object into a file.
    /// The XML configuration is obtained from the <c>ConfigurationService</c>.
    /// The file is actually a ZIP file.
    /// Compression level is set to 5 and needed assemblies are not included.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="filename">The filename.</param>
    public static void Serialize(object o, string filename) {
      Serialize(o, filename, ConfigurationService.Instance.GetConfiguration(new XmlFormat()), false, 5);
    }

    /// <summary>
    /// Serialize an object into a file.
    /// The XML configuration is obtained from the <c>ConfigurationService</c>.
    /// Needed assemblies are not included.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="compression">ZIP file compression level</param>
    public static void Serialize(object o, string filename, int compression) {
      Serialize(o, filename, ConfigurationService.Instance.GetConfiguration(new XmlFormat()), false, compression);
    }

    /// <summary>
    /// Serializes the specified object into a file.
    /// Needed assemblies are not included, ZIP compression level is set to 5.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="config">The configuration.</param>
    public static void Serialize(object obj, string filename, Configuration config) {
      Serialize(obj, filename, config, false, 5);
    }

    /// <summary>
    /// Serializes the specified object into a file.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="includeAssemblies">if set to <c>true</c> include needed assemblies.</param>
    /// <param name="compression">The ZIP compression level.</param>
    public static void Serialize(object obj, string filename, Configuration config, bool includeAssemblies, int compression) {
      try {
        string tempfile = Path.GetTempFileName();
        DateTime start = DateTime.Now;
        using (FileStream stream = File.Create(tempfile)) {
          Serializer serializer = new Serializer(obj, config);
          serializer.InterleaveTypeInformation = false;
          XmlGenerator generator = new XmlGenerator();
          using (ZipOutputStream zipStream = new ZipOutputStream(stream)) {
            zipStream.IsStreamOwner = false;
            zipStream.SetLevel(compression);
            zipStream.PutNextEntry(new ZipEntry("data.xml") { DateTime = DateTime.MinValue });
            StreamWriter writer = new StreamWriter(zipStream);
            foreach (ISerializationToken token in serializer) {
              string line = generator.Format(token);
              writer.Write(line);
            }
            writer.Flush();
            zipStream.PutNextEntry(new ZipEntry("typecache.xml") { DateTime = DateTime.MinValue });
            foreach (string line in generator.Format(serializer.TypeCache)) {
              writer.Write(line);
            }
            writer.Flush();
            if (includeAssemblies) {
              foreach (string name in serializer.RequiredFiles) {
                Uri uri = new Uri(name);
                if (!uri.IsFile) {
                  Logger.Warn("cannot read non-local files");
                  continue;
                }
                zipStream.PutNextEntry(new ZipEntry(Path.GetFileName(uri.PathAndQuery)));
                FileStream reader = File.OpenRead(uri.PathAndQuery);
                byte[] buffer = new byte[1024 * 1024];
                while (true) {
                  int bytesRead = reader.Read(buffer, 0, 1024 * 1024);
                  if (bytesRead == 0)
                    break;
                  zipStream.Write(buffer, 0, bytesRead);
                }
                writer.Flush();
              }
            }
          }
        }
        Logger.Info(String.Format("serialization took {0} seconds with compression level {1}",
          (DateTime.Now - start).TotalSeconds, compression));
        File.Copy(tempfile, filename, true);
        File.Delete(tempfile);
      }
      catch (Exception) {
        Logger.Warn("Exception caught, no data has been written.");
        throw;
      }
    }

    /// <summary>
    /// Serializes the specified object into a stream using the <see cref="XmlFormat"/>
    /// obtained from the <see cref="ConfigurationService"/>.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    public static void Serialize(object obj, Stream stream) {
      Serialize(obj, stream, ConfigurationService.Instance.GetConfiguration(new XmlFormat()));
    }


    /// <summary>
    /// Serializes the specified object into a stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="config">The configuration.</param>
    public static void Serialize(object obj, Stream stream, Configuration config) {
      Serialize(obj, stream, config, false);
    }

    /// <summary>
    /// Serializes the specified object into a stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="includeAssemblies">if set to <c>true</c> include need assemblies.</param>
    public static void Serialize(object obj, Stream stream, Configuration config, bool includeAssemblies) {
      try {
        Serializer serializer = new Serializer(obj, config);
        Serialize(stream, serializer);
      } catch (PersistenceException) {
        throw;
      } catch (Exception e) {
        throw new PersistenceException("Unexpected exception during Serialization.", e);
      }
    }

    /// <summary>
    /// Serializes the specified object into a stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="includeAssemblies">if set to <c>true</c> include need assemblies.</param>
    /// <param name="types">The list of all serialized types.</param>
    public static void Serialize(object obj, Stream stream, Configuration config, bool includeAssemblies, out IEnumerable<Type> types) {
      try {
        Serializer serializer = new Serializer(obj, config);
        Serialize(stream, serializer);
        types = serializer.SerializedTypes;
      } catch (PersistenceException) {
        throw;
      } catch (Exception e) {
        throw new PersistenceException("Unexpected exception during Serialization.", e);
      }
    }

    private static void Serialize(Stream stream, Serializer serializer) {
      using (StreamWriter writer = new StreamWriter(new GZipStream(stream, CompressionMode.Compress))) {
        serializer.InterleaveTypeInformation = true;
        XmlGenerator generator = new XmlGenerator();
        foreach (ISerializationToken token in serializer) {
          string line = generator.Format(token);
          writer.Write(line);
        }
        writer.Flush();
      }
    }
  }
}